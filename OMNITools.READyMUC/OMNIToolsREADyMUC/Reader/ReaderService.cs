using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.Common.Helpers;
using OMNIToolsREADyMUC.Connection;
using OMNIToolsREADyMUC.Fragments;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OMNIToolsREADyMUC.Reader
{
  public class ReaderService : Service, IReaderService
  {
    private static readonly String TAG = "ReaderService";
    private static readonly int MESSAGE_RECONNECT = 100;

    private bool mScanForDeviceInProgress = false;
    private bool mConnectingToDeviceInProgress = false;
    private bool mReconnectModeEnabled = false;
    private bool mAttemptReconnect = true;
    private bool mReconnectSequenceInProgress = false;
    private bool mReconnectOnce = false; // If true it will try to reconnect one

    private int mMaxRetryCount = BluetoothConfig.BT_DEFAULT_CONNECT_RETRIES;
    private int mRetryCount = BluetoothConfig.BT_DEFAULT_CONNECT_RETRIES;

    public ConnectionHandler mHandler;
    public BTManager mManager;
    private ComponentName mName, mNameUnbound;
    private IBinder mService;

    private ReaderServiceBinder mBinder;
    public LocaleBroadcastReceiver mLocaleBroadcastReceiver;
    private readonly List<IReaderListener> mListeners;

    public ReaderService()
    {
      mBinder = new ReaderServiceBinder(this);
      mListeners = new List<IReaderListener>();
    }

    public ReaderService GetService()
    {
      return this;
    }

    public void GetBTManager()
    {
      mManager = MainActivity.mBluetoothManager;
    }

    public void OnServiceConnected(ComponentName name, IBinder service)
    {
      mName = name;
      mService = service;
    }

    public void OnServiceDisconnected(ComponentName name)
    {
      mNameUnbound = name;
    }


    public class ConnectionHandler : Handler
    {
      private readonly WeakReference<ReaderService> mService;

      public ConnectionHandler(ReaderService service)
      {
        mService = new WeakReference<ReaderService>(service);
      }

      public void HandleMessage(Message msg)
      {
        ReaderService service = (ReaderService)mService.ToString();
        if (service == null)
          return;

        var readerDevice = CastToReaderDevice<ReaderDevice>(msg.Obj);
        var dataMessage = CastToDataMessageWrapper<DataMessageWrapper>(msg.Obj);

        switch (msg.What)
        {
          case Constants.MESSAGE_STATE_CHANGED:
          service.NotifyStatusChanged(readerDevice, msg.Arg1 == Constants.STATE_CONNECTED);
          break;

          case Constants.MESSAGE_DATA_RECEIVED:
          DataMessageWrapper dm = dataMessage;
          service.NotifyDataReceived(dm.readerDevice, dm.dataBytes);
          break;

          case Constants.MESSAGE_DEVICE_DISCOVERED:
          service.NotifyDeviceDiscovered(readerDevice);
          break;

          case Constants.MESSAGE_DEVICE_DISCOVERY_ENDED:
          service.NotifyDiscoveryEnded();
          service.mScanForDeviceInProgress = false;
          break;

          case Constants.MESSAGE_DEVICE_CONNECTED:
          service.OnConnectionEstablished(readerDevice);
          break;

          case Constants.MESSAGE_CONNECTION_LOST:
          service.OnConnectionLost(readerDevice);
          break;

          case Constants.MESSAGE_RECONNECT:
          service.OnReconnect(readerDevice);
          break;

          case Constants.MESSAGE_SHOW_NOTIFICATION_NOT_CONNECTED:
          service.mConnectingToDeviceInProgress = false;
          Log.Debug(TAG, "Notify NotConnected");
          break;

          case Constants.MESSAGE_SHOW_CONNECTING:
          service.mConnectingToDeviceInProgress = true;
          Log.Debug(TAG, "Notify Connecting");
          break;

          case Constants.MESSAGE_SHOW_NOTIFICATION_CONNECTED:
          service.mConnectingToDeviceInProgress = false;
          Log.Debug(TAG, "Notify Connected");
          break;
        }
      }

      public static T CastToReaderDevice<T>(Java.Lang.Object obj) where T : ReaderDevice
      {
        var propInfo = obj.GetType().GetProperty("Instance");
        return propInfo == null ? null : propInfo.GetValue(obj, null) as T;
      }

      public static T CastToDataMessageWrapper<T>(Java.Lang.Object obj) where T : DataMessageWrapper
      {
        var propInfo = obj.GetType().GetProperty("Instance");
        return propInfo == null ? null : propInfo.GetValue(obj, null) as T;
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void RegisterListener(IReaderListener listener)
    {
      if (!mListeners.Contains(listener))
        mListeners.Add(listener);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void UnregisterListener(IReaderListener listener)
    {
      if (mListeners.Contains(listener))
        mListeners.Remove(listener);
    }

    public int GetListenerCount()
    {
      return mListeners.Count;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void NotifyDeviceDiscovered(ReaderDevice device)
    {
      foreach (IReaderListener listener in mListeners)
        listener.OnDeviceDiscovered(device);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void NotifyDiscoveryEnded()
    {
      foreach (IReaderListener listener in mListeners)
        listener.OnDiscoveryFinished();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void NotifyStatusChanged(ReaderDevice device, bool connected)
    {
      foreach (IReaderListener listener in mListeners)
        listener.OnConnectionStatusChanged(device, connected);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void NotifyConnectionLost(ReaderDevice device)
    {
      foreach (IReaderListener listener in mListeners)
        listener.OnConnectionLost(device);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void NotifyConnectionEstablished(ReaderDevice dev)
    {
      foreach (IReaderListener listener in mListeners)
        listener.OnConnectionEstablished(dev);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void NotifyDataReceived(ReaderDevice device, byte[] data)
    {
      foreach (IReaderListener listener in mListeners)
        listener.OnDataReceived(device, data);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void NotifyReconnectSequenceStarted(ReaderDevice device)
    {
      foreach (IReaderListener listener in mListeners)
        listener.OnReconnectSequenceStarted(device);
    }

    private void OnConnectionEstablished(ReaderDevice device)
    {
      NotifyConnectionEstablished(device);
      ClearActiveReconnects();
    }

    private void OnConnectionLost(ReaderDevice device) // Called when an active connection fails
    {
      // Reset retry counter before we attempt to reconnect
      // When retry counter reaches zero - attemptReconnect signals connection lost
      mRetryCount = mMaxRetryCount;
      if (mAttemptReconnect)
      {
        // Start reconnect
        mReconnectSequenceInProgress = true;
        AttemptReconnect(device);
      }
      mAttemptReconnect = true;
    }

    private void OnReconnect(ReaderDevice device)
    {
      if (device != null && !device.IsConnected())
        Reconnect(device);
    }

    private void AttemptReconnect(ReaderDevice device)
    {
      if (mReconnectModeEnabled || mReconnectOnce)
      {
        if (device == null)
          mRetryCount = 0;

        if (mRetryCount == mMaxRetryCount) // first try in retry sequence
          NotifyReconnectSequenceStarted(device);

        if (mRetryCount > 0)
        {
          mRetryCount--;
          Log.Debug(TAG, "Attempt reconnect, retries left : " + mRetryCount);
          Message message = mHandler.ObtainMessage(MESSAGE_RECONNECT, device.ToString());
          mHandler.SendMessageDelayed(message, BluetoothConfig.BT_RECONNECT_DELAY_MILLIS);
          return;
        }
        Log.Debug(TAG, "Not more reconnect attempts... ");
        mReconnectOnce = false;
      }
      else
        Log.Debug(TAG, "Reconnect not enabled... ");

      NotifyConnectionLost(device);
    }


    public IConnectionManager GetConnectionManager() => mManager;

    public override void OnDestroy()
    {
      UnregisterReceiver(mLocaleBroadcastReceiver);
      mManager = null;
      mHandler = null;
    }

    public override IBinder OnBind(Intent intent)
    {
      mBinder = new ReaderServiceBinder(this);
      return mBinder;
    }

    public bool ConnectingToDevice()
    {
      return mConnectingToDeviceInProgress;
    }

    public bool ScanningForDevices()
    {
      return mScanForDeviceInProgress;
    }

    public void Write(byte[] Out)
    {
      mManager.Write(Out);
    }

    public void Write(ReaderDevice device, byte[] Out)
    {
      mManager.Write(device, Out);
    }

    public void Write(ConnectDeviceTypeEnum connType, byte[] Out)
    {
      mManager.Write(connType, Out);
    }

    public void Scan()
    {
      mManager = RDyCFragment.mBluetoothManager;
      mScanForDeviceInProgress = true;
      mManager.Scan();
    }

    public void CancelScan()
    {
      GetBTManager();
      mManager.CancelScan();
    }

    private void ClearActiveReconnects()
    {
      mReconnectOnce = false;
      if (mReconnectSequenceInProgress) // Clear reconnects in progress
      {
        mReconnectSequenceInProgress = false;
        mHandler.RemoveMessages(MESSAGE_RECONNECT);
      }
    }

    public bool IsConnected()
    {
      foreach (ReaderDevice device in GetConnectedDevices())
      {
        if (device.IsConnected())
          return true;
      }
      return false;
    }

    public bool IsConnected(ConnectDeviceTypeEnum deviceType)
    {
      foreach (ReaderDevice device in GetConnectedDevices())
      {
        if (device.IsConnected() && device.DeviceType == deviceType)
          return true;
      }
      return false;
    }

    public List<ReaderDevice> GetConnectedDevices()
    {
      return ReaderManager.GetConnectedDevices();
    }

    public int ReconnectMaxRetryCount
    {
      get => mMaxRetryCount;
      set
      {
        mMaxRetryCount = value;
      }
    }

    public void SetReconnectMode(bool active)
    {
      mReconnectModeEnabled = active;
    }

    public void SetReconnectOnce()
    {
      mReconnectOnce = true;
    }

    public void Connect(ReaderDevice device)
    {
      ClearActiveReconnects();
      GetBTManager();
      mManager.Connect(device);
    }

    private void Reconnect(ReaderDevice device) => mManager.Connect(device);

    public void DisconnectAll()
    {
      mManager.DisconnectAll();
    }

    public void Disconnect(ReaderDevice device)
    {
      mManager.Disconnect(device);
    }

    public void DisconnectWithoutReconnectAll()
    {
      mAttemptReconnect = false;
      mManager.DisconnectAll();
    }

    public void DisconnectWithoutReconnect(ReaderDevice device)
    {
      mAttemptReconnect = false;
      mManager.Disconnect(device);
    }
  }
}