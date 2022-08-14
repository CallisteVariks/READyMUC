using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using OMNIToolsREADyMUC.Common.Converters;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.Common.State;
using OMNIToolsREADyMUC.Connection;
using OMNIToolsREADyMUC.Common.Helpers;
using OMNIToolsREADyMUC.KMRF;
using OMNIToolsREADyMUC.KMRF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OMNIToolsREADyMUC.Fragments;

namespace OMNIToolsREADyMUC.Reader
{
  /// <summary>
  /// ReaderManager is a layer on top of the "reader" module
  /// Wraps commands and protocols from the GUI
  /// </summary>
  public class ReaderManager
  {
    // Logging
    private static readonly String TAG = "ReaderManager";

    // Listeners
    public static List<IReaderDeviceConnectionListener> mReaderListeners = new List<IReaderDeviceConnectionListener>();
    public static List<IReaderDeviceDiscoveryListener> mDiscoveryListeners = new List<IReaderDeviceDiscoveryListener>();
    public static List<IDeviceCommandListener> mCmdListeners = new List<IDeviceCommandListener>();

    public static ReaderService mReaderService;
    public static ReaderListener mDataListener = new ReaderListener();
    public byte[] mRDyCDataLinkAddress;
    public static KeepAliveThread mKeepAliveThread;
    public static Context mContext;
    private List<Byte> mKmpBuffer = new List<Byte>();
    public ReaderServiceBinder mReaderServiceBinder;

    /// <summary>
    /// Constructor for the Manager
    /// </summary>
    /// <param name="context"> application context </param>
    public ReaderManager(Context context)
    {
      mContext = context;
    }

    /// <summary>
    /// Initialize the manager
    /// </summary>
    /// <param name="readerService"></param>
    public void Initialize(ReaderService readerService)
    {
      if (readerService == null)
      {
        mReaderServiceBinder = new ReaderServiceBinder(readerService);
        mReaderServiceBinder.Bind(mContext);
        OnConnected();

        mKeepAliveThread = new KeepAliveThread();
      }
      else
      {
        mReaderService = readerService;
        readerService.RegisterListener(mDataListener);
      }
    }

    public void ShutDown()
    {
      if (mReaderService != null)
      {
        mReaderService.DisconnectWithoutReconnectAll();
        mReaderService.UnregisterListener(mDataListener);
        mKeepAliveThread.StopKeepAliveThread();
      }
    }

    /// <summary>
    /// Invoked when service is bound
    /// </summary>
    public void OnConnected()
    {
      mReaderService = mReaderServiceBinder.GetService();
      mReaderServiceBinder.GetService().RegisterListener(mDataListener);

      List<ReaderDevice> list = new List<ReaderDevice>();

      if (RDyCFragment.readerDevicesAdapter != null)
      {
        for (int i = 0; i < RDyCFragment.readerDevicesAdapter.Count; i++)
        {
          ReaderDevice obj = RDyCFragment.devicesList[i];
          list.Add(obj);
        }

        List<ReaderDevice> devices = list;
        if (RDyCFragment.readerDevicesAdapter != null)
        {
          foreach (ReaderDevice device in devices)
            Connect(device);
        }
      }
    }

    /// <summary>
    /// Invoked when service is no longer bound
    /// Override to handle event
    /// </summary>
    public static void OnDisconnected()
    {

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void RegisterConnectionListener(IReaderDeviceConnectionListener listener)
    {

      if (!mReaderListeners.Contains(listener))
        mReaderListeners.Add(listener);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UnregisterConnectionListener(IReaderDeviceConnectionListener listener)
    {
      if (mReaderListeners.Contains(listener))
        mReaderListeners.Remove(listener);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void RegisterCommandListener(IDeviceCommandListener listener)
    {
      if (!mCmdListeners.Contains(listener))
        mCmdListeners.Add(listener);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UnregisterCommandListener(IDeviceCommandListener listener)
    {
      if (mCmdListeners.Contains(listener))
        mCmdListeners.Remove(listener);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void RegisterDeviceDiscoveryListener(IReaderDeviceDiscoveryListener listener)
    {
      if (!mDiscoveryListeners.Contains(listener))
        mDiscoveryListeners.Add(listener);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UnregisterDeviceDiscoveryListener(IReaderDeviceDiscoveryListener listener)
    {
      if (mDiscoveryListeners.Contains(listener))
        mDiscoveryListeners.Remove(listener);
    }



    /// <summary>
    /// Mirror of ReaderService command
    /// </summary>
    /// <param name="Out"></param>
    /// <param name="commandName"></param>
    public void WriteToReader(byte[] Out, String commandName)
    {
      foreach (ReaderDevice device in GetConnectedDevices())
        Write(device, Out, commandName);
    }

    /// <summary>
    /// Mirror of ReaderService command, taking a KmRfCommandRequest object
    /// </summary>
    /// <param name="kmRfRequest"></param>
    /// <returns></returns>
    public bool WriteToReaders(KmRfCommandRequest kmRfRequest)
    {
      foreach (ReaderDevice device in GetConnectedReaderDevices())
      {
        try
        {
          Write(device, kmRfRequest.GetKmRfTelegram().GetData(), typeof(KmRfCommandRequest).Name);
        }
        catch (Exception e)
        {
          Log.Error(TAG, "Error sending command: " + kmRfRequest, e);
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Mirror of ReaderService command, sending a KmRfCommandRequest object to a device
    /// </summary>
    /// <param name="device"></param>
    /// <param name="kmRfRequest"></param>
    /// <returns></returns>
    public bool WriteToReader(ReaderDevice device, KmRfCommandRequest kmRfRequest)
    {
      try
      {
        Write(device, kmRfRequest.GetKmRfTelegram().GetData(), typeof(KmRfCommandRequest).Name);
        return true;
      }
      catch (Exception e)
      {
        Log.Error(TAG, "Error sending command: " + kmRfRequest, e);
        return false;
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Write(ReaderDevice device, byte[] @out, String commandName)
    {
      StaticWrapper.Write(device, @out, commandName);
    }

    public void Scan()
    {
      mReaderService.Scan();
    }

    public void CancelScan()
    {
      mReaderService.CancelScan();
    }

    public void SetReconnectOnce()
    {
      mReaderService.SetReconnectOnce();
    }

    public void EnableReconnect()
    {
      mReaderService.SetReconnectMode(true);
    }

    public void DisableReconnect()
    {
      mReaderService.SetReconnectMode(false);
    }

    public bool IsConnected(ConnectDeviceTypeEnum deviceType)
    {
      if (mReaderService != null && mReaderService.IsConnected(deviceType))
      {
        Log.Debug(TAG, "Connected");
        return true;
      }
      else if (mReaderService != null && mReaderService.IsConnected())
        Log.Debug(TAG, "ReaderManager not connected to the right device type");
      else
        Log.Debug(TAG, "Not connected");
      return false;
    }

    public bool IsConnected()
    {
      return mReaderService != null && mReaderService.IsConnected();
    }

    public static List<ReaderDevice> GetConnectedDevices()
    {
      return StaticWrapper.GetConnectedDevices();
    }

    public List<ReaderDevice> GetConnectedReaderDevices()
    {
      return StaticWrapper.GetConnectedReaderDevices();
    }

    public ReaderDevice GetConnectedDeviceByAddress(String deviceAddress)
    {
      foreach (ReaderDevice device in mReaderService.GetConnectedDevices())
        if (device.Address == deviceAddress)
          return device;
      return null;
    }

    /// <summary>
    /// </summary>
    /// <param name="deviceType"></param>
    /// <returns> true when the reader is connected </returns>
    public bool IsConnectedAndReady(ConnectDeviceTypeEnum deviceType)
    {
      foreach (ReaderDevice device in GetConnectedDevices())
        if (deviceType == ConnectDeviceTypeEnum.RDyC && device.DeviceType == deviceType)
          return device.StateHandler.GetStateFromHandler() == StateEnum.ConnectedAndReady ||
                 device.StateHandler.GetStateFromHandler() == StateEnum.MBusDataActive ||
                 device.StateHandler.GetStateFromHandler() == StateEnum.MBusDataNotActive;
      return false;
    }

    /// <summary>
    /// Check if there is a connection with a converter that supports C2 communication
    /// </summary>
    /// <returns></returns>
    public bool IsConnectedToC2RadioConverter()
    {
      if (mReaderService != null && IsConnectedAndReady(ConnectDeviceTypeEnum.RDyC))
      {
        foreach (ReaderDevice device in GetConnectedReaderDevices())
          if (device.unitType != null && BluetoothConfig.RDyCS_SUPPORTING_C2_RADIO.Contains(device.unitType))
            return true;
      }
      return false;
    }

    /// <summary>
    /// Check if there is a connection with only one converter that supports C2 communication
    /// </summary>
    /// <returns></returns>
    public bool IsConnectedToSingleC2RadioConverter()
    {
      if (IsConnectedToC2RadioConverter() && GetConnectedReaderDevices().Count() == 1)
        return true;
      return false;
    }

    public bool IsConnectedToInstantConnectCompatibleC2Converter()
    {
      bool connectedToInstantConnectCompatibleC2Converter = false;
      if (mReaderService != null && IsConnectedAndReady(ConnectDeviceTypeEnum.RDyC))
      {
        foreach (ReaderDevice device in GetConnectedReaderDevices())
          if (device.unitType != null && BluetoothConfig.RDyCS_SUPPORTING_C2_INSTANT_CONNECT.Contains(device.unitType))
            connectedToInstantConnectCompatibleC2Converter = true;
      }
      return IsConnectedToSingleC2RadioConverter() && connectedToInstantConnectCompatibleC2Converter;
    }

    /// <summary>
    /// </summary>
    /// <returns> true if trying to connect to a device </returns>
    public bool ConnectingToDevice()
    {
      return mReaderService.ConnectingToDevice();
    }

    public void Connect(ReaderDevice device)
    {
      //device.StateHandler = new StateHandler(this, device);
      mReaderService.Connect(device);
    }

    public void Disconnect(ReaderDevice device)
    {
      StaticWrapper.DisconnectForRM(device);
    }

    public void DisconnectWithoutReconnectAll()
    {
      mReaderService.DisconnectWithoutReconnectAll();
    }

    public void DisconnectWithoutReconnect(ReaderDevice device)
    {
      mReaderService.DisconnectWithoutReconnect(device);
    }

    public void DisconnectAll()
    {
      mReaderService.DisconnectAll();
    }

    /// <summary>
    /// Set reader in state where it relays MBus data
    /// </summary>
    public void StartMBusData()
    {
      foreach (ReaderDevice device in GetConnectedReaderDevices())
        device.StateHandler.HandleEvent(EventsEnum.StartMbusData);
    }

    /// <summary>
    /// Set reader in state where it relays MBus data
    /// </summary>
    public void StartMBusDataWithBroadcast()
    {
      foreach (ReaderDevice device in GetConnectedReaderDevices())
      {
        if (BluetoothConfig.DEVICE_TYPE_US_2014.Equals(device.unitType))
          device.StateHandler.HandleEvent(EventsEnum.StartMbusData);
        else
          device.StateHandler.HandleEvent(EventsEnum.StartMbusDataWithBroadcast);
      }
    }

    /// <summary>
    /// Set reader in state where it doesn't relay MBus data
    /// </summary>
    public void StopMBusData()
    {
      foreach (ReaderDevice device in GetConnectedReaderDevices())
        device.StateHandler.HandleEvent(EventsEnum.StopMBusData);
    }

    /// <summary>
    /// Returns true if the reader is currently in a state where mbus data is being relayed
    /// </summary>
    /// <returns> true if MBUS data is being relayed </returns>
    public bool IsRelayingMBusData()
    {
      foreach (ReaderDevice device in GetConnectedReaderDevices())
      {
        if (device.StateHandler.GetStateFromHandler() == StateEnum.MBusDataActive)
          return true;
      }
      return false;
    }

    /// <summary>
    /// This checks if Bluetooth hardware is found and if we are connected to device
    /// If not, appropriate pop-up dialog is shown
    /// </summary>
    /// <param name="cdt"> the device type you want to check </param>
    /// <returns> true if device is connected </returns>
    public bool IsDeviceConnected(ConnectDeviceTypeEnum cdt)
    {
      BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
      if (mBluetoothAdapter == null)
      {
        // Device does not support Bluetooth, show popup informing user of problem
        Log.Debug(TAG, "Bluetooth hardware not detected on device");
      }
      else if (!mBluetoothAdapter.IsEnabled || IsConnected(cdt))
        return false;
      return true;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void GenerateEventConnectedAndReady(ReaderDevice device)
    {
      foreach (IReaderDeviceConnectionListener listener in mReaderListeners)
        listener.OnConnectedAndReady(device);
    }

    /// <summary>
    /// Implements keep alive
    /// Implements one timer that handles KeepAliveMessages and KeepAliveMessagesTimeouts
    /// </summary>
    public class KeepAliveThread : Java.Lang.Thread
    {
      private bool mStopThread = false;
      private byte[] mKeepAliveMessage;

      public KeepAliveThread()
      {
        try
        {
          mKeepAliveMessage = new StatusFrameRequest().GetKmRfTelegram().GetData();
          mStopThread = false;
        }
        catch (Exception e) { }
      }

      public void StopKeepAliveThread()
      {
        mStopThread = true;
      }

      public override void Run()
      {
        Log.Debug(TAG, "Start KeepAliveThread");
        Name = "KeepAliveThread";

        do
        {
          // Only do KeepAlive when connected to a RDyC
          if (BluetoothConfig.USE_RDyC_TIMEOUT)
          {
            List<ReaderDevice> devices = new List<ReaderDevice>();
            devices = StaticWrapper.GetConnectedReaderDevices();

            foreach (ReaderDevice device in devices)
            {
              // Handle Sending KeepAlive messages
              if (device.mWaitingForFrameReceived) // KeepAlive is send but no response yet, the short timeout
                device.mKeepAliveTimeOutInMilliSec = BluetoothConfig.RDyC_KEEPALIVE_TIMEOUT_SHORT;
              else
                device.mKeepAliveTimeOutInMilliSec = BluetoothConfig.RDyC_KEEPALIVE_TIMEOUT_LONG;

              // Check for timeout
              if (SystemClock.ElapsedRealtime() - device.mFrameSendTimeStamp >= device.mKeepAliveTimeOutInMilliSec)
              {
                if (device.mKeepAliveRetryCounter < BluetoothConfig.RDyC_MAX_RETRY)
                {
                  device.mKeepAliveRetryCounter++;
                  device.mWaitingForFrameReceived = true; 
                  StaticWrapper.Write(device, mKeepAliveMessage, "KeepAlive");
                }
                else
                {
                  device.ResetResponseTimeoutTimer();
                  Log.Debug(TAG, device.Name + ": Response timeout and no more retries, disconnect!");
                  StaticWrapper.DisconnectForRM(device); // No more retries
                }
              }
            }
          }

          try
          {
            Sleep(100);
          }
          catch (Exception ignored) { }
        }
        while (!mStopThread);
      }
    }

  }
}