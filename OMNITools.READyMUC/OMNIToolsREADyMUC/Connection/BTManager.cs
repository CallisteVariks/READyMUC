using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Java.Lang;
using Java.Lang.Reflect;
using Java.Util;
using Java.Util.Concurrent;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.Common.Helpers;
using OMNIToolsREADyMUC.Fragments;
using OMNIToolsREADyMUC.Reader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace OMNIToolsREADyMUC.Connection
{
  /// <summary>
  /// This class is for settings up and managing Bluetooth connection with other devices. Contains a thread that listens for  incoming connections, a thread for connecting with a device and 
  /// a thread for performing data transmissions when connected
  /// It acts as a wrapper that manages the various threads used to connect, send and receive messages via BT
  /// </summary>
  public class BTManager : IConnectionManager
  {
    public static System.String TAG = "BluetoothManager";
    public static Handler mHandler;
    private static BluetoothAdapter mBluetoothAdapter;

    // Used for measuring time since the last connection because it might deadlock if we connect to soon
    private static long mLastDisconnect = 0;
    private static bool mConnectThreadActive = false;

    private IExecutorService mPendingConnectPool;
    public static Dictionary<ReaderDevice, ConnectedThread> mConnectedThreads = new Dictionary<ReaderDevice, ConnectedThread>();

    public BTManager(Handler handler)
    {
      mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
      mHandler = handler;
      mHandler.ObtainMessage(Constants.MESSAGE_SHOW_NOTIFICATION_NOT_CONNECTED).SendToTarget();
      mPendingConnectPool = Executors.NewSingleThreadExecutor();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void SetState(ReaderDevice device, int state)
    {
      Log.Debug(TAG, "SetState: " + device.State + " -> " + state + " for device: " + device.Name);
      device.State = state;
      mHandler.ObtainMessage(Constants.MESSAGE_STATE_CHANGED, state, -1, device.ToString()).SendToTarget();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public int GetState(ReaderDevice device)
    {
      return device.State;
    }

    public void Scan()
    {
      mBluetoothAdapter.StartDiscovery();
    }

    public void CancelScan()
    {
      mBluetoothAdapter.CancelDiscovery();
    }



    public void Connect(ReaderDevice device)
    {
      BluetoothDevice bluetoothDevice = mBluetoothAdapter.GetRemoteDevice(device.Address);
      device.BluetoothDevice = bluetoothDevice;
      readerDevice = device;
      ConnectInternal(device);
    }

    public static ReaderDevice readerDevice;

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void ConnectInternal(ReaderDevice device)
    {
      Log.Debug(TAG, "Connect: " + device);

      // Check if ExecutorService needs refreshing
      if (mPendingConnectPool.IsShutdown)
        mPendingConnectPool = Executors.NewSingleThreadExecutor();

      // Add a pending connect task
      mPendingConnectPool.Execute(new PendingConnectTask(device));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Connected(BluetoothSocket socket, ReaderDevice device)
    {
      StaticWrapper.Connected(socket, device);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Disconnect(ReaderDevice device)
    {
      StaticWrapper.Disconnect(device);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DisconnectAll()
    {
      Log.Debug(TAG, "DisconnectAll called");

      // Cancel any current pending connect tasks
      mPendingConnectPool.ShutdownNow();

      // Cancel and remove any ConnectedThreads
      foreach (KeyValuePair<ReaderDevice, ConnectedThread> pair in mConnectedThreads)
      {
        pair.Value.Cancel();
        mConnectedThreads.Remove(pair.Key);
        SetState(pair.Key, Constants.STATE_NONE);
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public List<ReaderDevice> GetConnectedDevices()
    {
      return StaticWrapper.GetConnectedDevicesinBM();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public ReaderDevice GetConnectedDeviceByAddress(string deviceAddress)
    {
      return StaticWrapper.GetConnectedDeviceByAddress(deviceAddress);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Write(byte[] buffer)
    {
      foreach (KeyValuePair<ReaderDevice, ConnectedThread> pair in mConnectedThreads)
      {
        // Only write to device if it is connected
        if (pair.Key.IsConnected())
          pair.Value.Write(buffer);
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Write(ReaderDevice device, byte[] buffer)
    {
      if (!device.IsConnected()) return;

      ConnectedThread thread = mConnectedThreads[device];
      if (thread != null)
        thread.Write(buffer);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Write(ConnectDeviceTypeEnum deviceType, byte[] buffer)
    {
      foreach (KeyValuePair<ReaderDevice, ConnectedThread> pair in mConnectedThreads)
      {
        // Only write to device with right connection type
        if (pair.Key.DeviceType == deviceType && pair.Key.IsConnected())
          pair.Value.Write(buffer);
      }
    }

    private void ConnectionFailed(ReaderDevice device)
    {
      StaticWrapper.ConnectionFailed(device);
    }

    private void ConnectionLost(ReaderDevice device)
    {
      StaticWrapper.ConnectionLost(device);
    }



    #region Thread Classes
    /// <summary>
    /// Ensures that the connection is not made before the Bluetooth stack is settled
    /// Creating a delay between disconnect and the next connect
    /// </summary>
    private class PendingConnectTask : Java.Lang.Object, IRunnable
    {
      private readonly ReaderDevice mReaderDevice = readerDevice;
      private long mTimeStamp;
      private bool mWaited = false;
      private bool mPaused = false;

      System.Globalization.Calendar cal = CultureInfo.InvariantCulture.Calendar;
      DateTime dateTime = DateTime.Now;

      int minus = DateTime.Now.Second;

      public PendingConnectTask() : this(RDyCFragment.GetDeviceFromList) { }

      public PendingConnectTask(IntPtr a, JniHandleOwnership b) : base(a, b) { }

      public PendingConnectTask(ReaderDevice device)
      {
        mReaderDevice = device;
      }

      public void Run()
      {
        double milliseconds = cal.GetMilliseconds(dateTime); // seconds

        try
        {
          Log.Debug(TAG, "Pending connect task started");
          mTimeStamp = DateTime.Now.Millisecond;

          // Wait for connect thread and connected thread to stop
          if (mConnectThreadActive)
            Log.Debug(TAG, "Waiting for threads to stop");

          while (mConnectThreadActive && (DateTime.Now.Millisecond - mTimeStamp < BluetoothConfig.MAX_TIME_WAIT_FOR_THREADS_STOPPED))
          {
            try
            {
              Thread.Sleep(100);
              mWaited = true;
            }
            catch (InterruptedException e) { }
          }

          if (mWaited)
            Log.Debug(TAG, "Waited " + (DateTime.Now.Millisecond - mTimeStamp) + " mSec for threads to stop");

          // Now the Connected thread should be stopped
          // Starting the new connection process
          mHandler.ObtainMessage(Constants.MESSAGE_SHOW_CONNECTING).SendToTarget();
          // Make sure we do not reconnect to early                                                                          
          while ((DateTime.Now.Second - minus) != 5)
          {
            try
            {
              Thread.Sleep(100);
              mPaused = true;
            }
            catch (InterruptedException e) { }
          }
          if (mPaused)
            Log.Debug(TAG, "Waited " + (DateTime.Now.Millisecond - mTimeStamp) + " mSec before reconnect");

          if (mReaderDevice != null)
          {
            // Start the thread to connect with the given device
            Log.Debug(TAG, "Start connecting on a pending connect");
            ConnectThread connectThread = new ConnectThread(mReaderDevice);
            connectThread.Start();
            SetState(mReaderDevice, Constants.STATE_CONNECTING);
          }
          else
            // No pending connect, user may have cancelled
            mHandler.ObtainMessage(Constants.MESSAGE_SHOW_NOTIFICATION_NOT_CONNECTED).SendToTarget();
        }
        catch (System.Exception e)
        {
          // Show notification status not connected
          mHandler.ObtainMessage(Constants.MESSAGE_SHOW_NOTIFICATION_NOT_CONNECTED).SendToTarget();
        }
      }

      public void Dispose() => throw new NotImplementedException();
      public IntPtr Handle => (IntPtr)0;
    }


    /// <summary>
    /// Manages connection attempt in separate thread
    /// Employs three different strategies for creating and connecting a valid Bluetooth socket
    /// Uses known workaround for creating valid Bluetooth socket on troublesome devices - createRfCommSocket
    /// </summary>
    private class ConnectThread : Thread
    {
      private readonly ReaderDevice mReaderDevice;
      private BluetoothSocket mBluetoothSocket;
      private bool mStopConnecting = false;

      public ConnectThread(ReaderDevice device)
      {
        mReaderDevice = device;
      }

      /// <summary>
      /// Use for closing the threads socket
      /// </summary>
      private void CloseSocket()
      {
        try
        {
          if (mBluetoothSocket != null)
          {
            mBluetoothSocket.Close();
            mLastDisconnect = DateTime.Now.Millisecond;
          }
        }
        catch (IOException e)
        {
          Log.Error(TAG, "Closing the connect socket failed", e);
        }
      }

      private bool CreateBond(BluetoothDevice bluetoothDevice)
      {
        Log.Debug(TAG, "Calling CreateBond method");
        try
        {
          Class class1 = Class.ForName("android.bluetooth.BluetoothDevice");
          Method createBondMethod = class1.GetMethod("CreateBond");
          bool returnValue = (bool)createBondMethod.Invoke(bluetoothDevice);
          return returnValue;
        }
        catch (System.Exception e)
        {
          Log.Error(TAG, "CreateBond failed", e);
        }
        return false;
      }

      private bool IsDeviceBonded(BluetoothDevice bluetoothDevice)
      {
        Log.Debug(TAG, "Check if device is bonded");
        // Create a connection to the BluetoothSocket
        bool res = bluetoothDevice.BondState == Bond.Bonded;
        if (res)
          Log.Debug(TAG, "Device is bonded");
        else
          Log.Debug(TAG, "Device not bonded");
        return res;
      }

      private void GetDeviceServiceUUIDs(BluetoothDevice bluetoothDevice)
      {
        // Start asynchronous process
        bluetoothDevice.FetchUuidsWithSdp();
        // Wait 10 sec
        Thread.Sleep(10000);
        ParcelUuid[] UUIDs = bluetoothDevice.GetUuids();
        foreach (ParcelUuid UUID in UUIDs)
          Log.Debug(TAG, "UIID : " + UUID.ToString());
      }

      private BluetoothSocket CreateRfcommSocketUsingChannel(int channel)
      {
        Log.Debug(TAG, "Connecting using CreateRfcommSocketUsingChannel(int channel)");
        try
        {
          Method method = mReaderDevice.BluetoothDevice.Class.GetMethod("CreateRfcommSocket", new Class[] { });
          BluetoothSocket bluetoothSocket = (BluetoothSocket)method.Invoke(mReaderDevice.BluetoothDevice, channel);
          return bluetoothSocket;
        }
        catch (NoSuchMethodException e)
        {
          Log.Error(TAG, "CreateRfcommSocketUsingChannel failed!", e);
        }
        catch (IllegalArgumentException e)
        {
          Log.Error(TAG, "CreateRfcommSocketUsingChannel failed!", e);
        }
        catch (IllegalAccessException e)
        {
          Log.Error(TAG, "CreateRfcommSocketUsingChannel failed!", e);
        }
        catch (InvocationTargetException e)
        {
          Log.Error(TAG, "CreateRfcommSocketUsingChannel failed!", e);
        }
        return null;
      }

      private BluetoothSocket CreateInsecureRfcommSocketUsingChannel(int channel)
      {
        Log.Debug(TAG, "Connecting using CreateInsecureRfcommSocketUsingChannel(int channel)");
        try
        {
          Method method = mReaderDevice.BluetoothDevice.Class.GetMethod("CreateInsecureRfcommSocket", new Class[] { });
          BluetoothSocket bluetoothSocket = (BluetoothSocket)method.Invoke(mReaderDevice.BluetoothDevice, channel);
          return bluetoothSocket;
        }
        catch (NoSuchMethodException e)
        {
          Log.Error(TAG, "CreateInsecureRfcommSocketUsingChannel failed!", e);
        }
        catch (IllegalArgumentException e)
        {
          Log.Error(TAG, "CreateInsecureRfcommSocketUsingChannel failed!", e);
        }
        catch (IllegalAccessException e)
        {
          Log.Error(TAG, "CreateInsecureRfcommSocketUsingChannel failed!", e);
        }
        catch (InvocationTargetException e)
        {
          Log.Error(TAG, "CreateInsecureRfcommSocketUsingChannel failed!", e);
        }
        return null;
      }

      private BluetoothSocket CreateRfcommSocketToServiceRecord(UUID uuid)
      {
        Log.Debug(TAG, "Connecting using CreateRfcommSocketToServiceRecord(UUID uuid)");
        try
        {
          BluetoothSocket bluetoothSocket = mReaderDevice.BluetoothDevice.CreateRfcommSocketToServiceRecord(uuid);
          return bluetoothSocket;
        }
        catch (IOException e)
        {
          Log.Error(TAG, "CreateRfcommSocketToServiceRecord failed!", e);
        }
        return null;
      }

      private BluetoothSocket CreateInsecureRfcommSocketToServiceRecord(UUID uuid)
      {
        Log.Debug(TAG, "Connecting using CreateInsecureRfcommSocketToServiceRecord(UUID uuid)");
        try
        {
          BluetoothSocket socket = mReaderDevice.BluetoothDevice.CreateInsecureRfcommSocketToServiceRecord(uuid);
          return socket;
        }
        catch (IOException e)
        {
          Log.Error(TAG, "CreateInsecureRfcommSocketToServiceRecord failed!", e);
        }
        return null;
      }

      private BluetoothSocket ConnectSocket(BluetoothSocket bluetoothSocket)
      {
        if (bluetoothSocket != null)
        {
          try
          {
            bluetoothSocket.Connect();
            return bluetoothSocket;
          }
          catch (System.Exception e)
          {
            try
            {
              CloseSocket();
            }
            catch (System.Exception e2)
            {
              Log.Error(TAG, "Unable to close socket during connection failure!", e2);
            }
          }
        }
        return null;
      }

      public override void Run()
      {
        int pairingDelayCount;
        int MAX_PAIRING_DELAY = 30; // sec

        try
        {
          mConnectThreadActive = true;
          Log.Debug(TAG, "Starting ConnectThread");
          mHandler.ObtainMessage(Constants.MESSAGE_SHOW_CONNECTING).SendToTarget();

          // Always cancel discovery because it will slow down a connection
          mBluetoothAdapter.CancelDiscovery();

          // Attempt different strategies for creating connection based on manufacturer
          ConnectPolicy connectPolicy = new ConnectPolicy(mReaderDevice.DeviceType, mReaderDevice.GetBluetoothDeviceClass());

          // Check if device is already bonded/paired
          bool isBonded = IsDeviceBonded(mReaderDevice.BluetoothDevice);

          // Attempt to programatically create pairing, this is needed on some devices
          if (!isBonded)
          {
            if (connectPolicy.doCreateBond)
            {
              CreateBond(mReaderDevice.BluetoothDevice);

              // If Bluetooth is not finished pairing/bonding when createbond returns, make this wait loop
              pairingDelayCount = 0;
              do
              {
                pairingDelayCount++;
                try
                {
                  Thread.Sleep(500);
                  isBonded = IsDeviceBonded(mReaderDevice.BluetoothDevice);
                }
                catch (InterruptedException e) { }
              }
              while (!isBonded && (MAX_PAIRING_DELAY >= pairingDelayCount));
              if (pairingDelayCount > MAX_PAIRING_DELAY)
                Log.Debug(TAG, "PairingTimeOut");
            }
          }

          // Short delay because we know there is a timing issue, in calling the BT layer to often
          // This is not needed, but just a precaution
          Thread.Sleep(100);

          int MAX_CONNECT_RETRY = 2;
          int connectRetryCount = 0;
          if (connectPolicy.doCreateRfcommSocketUsingChannel)
          {
            connectRetryCount = 1;
            while (((mBluetoothSocket = ConnectUsingCreateRfcommSocketUsingChannel1()) == null) && connectRetryCount < MAX_CONNECT_RETRY)
            {
              connectRetryCount++;
              Sleep(100);
            }
          }
          if (connectPolicy.doCreateRfcommSocketToServiceRecord && mBluetoothSocket == null)
          {
            connectRetryCount = 1;
            while (((mBluetoothSocket = ConnectUsingcreateRfcommSocketToServiceRecord_SPP_SERVICE_UUID()) == null) && connectRetryCount < MAX_CONNECT_RETRY)
            {
              connectRetryCount++;
              Sleep(100);
            }
          }
          if (connectPolicy.doCreateInsecureRfcommSocketUsingChannel && mBluetoothSocket == null)
          {
            connectRetryCount = 1;
            while (((mBluetoothSocket = ConnectUsingCreateInSecureRfCommSocketUsingChannel1()) == null) && connectRetryCount < MAX_CONNECT_RETRY)
            {
              connectRetryCount++;
              Sleep(100);
            }
          }
          if (connectPolicy.doCreateInsecureRfcommSocketToServiceRecord && mBluetoothSocket == null)
          {
            connectRetryCount = 1;
            while (((mBluetoothSocket = ConnectUsingCreateInsecureRfCommSocketToServiceRecord_SPP_SERVICE_UUID()) == null) && connectRetryCount < MAX_CONNECT_RETRY)
            {
              connectRetryCount++;
              Sleep(100);
            }
          }
          if (mBluetoothSocket == null)
          {
            mHandler.ObtainMessage(Constants.MESSAGE_SHOW_NOTIFICATION_NOT_CONNECTED).SendToTarget();
            StaticWrapper.ConnectionFailed(mReaderDevice);
            return;
          }

          if (mStopConnecting && mBluetoothSocket != null)
            // CloseSocket if connection is canceled
            CloseSocket();
          else
          {
            // Wait, then start the connected thread
            try
            {
              Thread.Sleep(1000);
            }
            catch (InterruptedException e) { }

            StaticWrapper.Connected(mBluetoothSocket, mReaderDevice);
          }
        }
        catch (System.Exception e)
        {
          // Show notification status not Connected
          mHandler.ObtainMessage(Constants.MESSAGE_SHOW_NOTIFICATION_NOT_CONNECTED).SendToTarget();
        }
        finally
        {
          mConnectThreadActive = false;
        }
      }

      private void Sleep(int milliSec)
      {
        try
        {
          Thread.Sleep(milliSec);
        }
        catch (InterruptedException e) { }
      }

      private BluetoothSocket ConnectUsingCreateRfcommSocketUsingChannel1()
      {
        Log.Debug(TAG, "Connecting using CreateRfcommSocketUsingChannel1");
        BluetoothSocket bluetoothSocket = CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
        if (bluetoothSocket != null)
          Log.Debug(TAG, "Succes!");
        else
          Log.Debug(TAG, "CreateRfcommSocketUsingChannel1 failed!");
        return bluetoothSocket;
      }

      private BluetoothSocket ConnectUsingCreateInSecureRfCommSocketUsingChannel1()
      {
        Log.Debug(TAG, "Connecting using ConnectUsingCreateInSecureRfCommSocketUsingChannel1");
        BluetoothSocket socket = ConnectSocket(CreateInsecureRfcommSocketUsingChannel(1));
        if (socket != null)
          Log.Debug(TAG, "Succes!");
        else
          Log.Debug(TAG, "ConnectUsingCreateInSecureRfCommSocketUsingChannel1 failed!");
        return socket;
      }

      private BluetoothSocket ConnectUsingcreateRfcommSocketToServiceRecord_SPP_SERVICE_UUID()
      {
        Log.Debug(TAG, "Connecting using ConnectUsingcreateRfcommSocketToServiceRecord_SPP_SERVICE_UUID");
        BluetoothSocket bluetoothSocket = ConnectSocket(CreateRfcommSocketToServiceRecord(BluetoothConfig.SPP_SERVICE_UUID));
        if (bluetoothSocket != null)
          Log.Debug(TAG, "Succes!");
        else
          Log.Debug(TAG, "ConnectUsingcreateRfcommSocketToServiceRecord_SPP_SERVICE_UUID failed!");
        return bluetoothSocket;
      }

      private BluetoothSocket ConnectUsingCreateInsecureRfCommSocketToServiceRecord_SPP_SERVICE_UUID()
      {
        Log.Debug(TAG, "Connecting using ConnectUsingCreateInsecureRfCommSocketToServiceRecord_SPP_SERVICE_UUID");
        BluetoothSocket bluetoothSocket = ConnectSocket(CreateInsecureRfcommSocketToServiceRecord(BluetoothConfig.SPP_SERVICE_UUID));
        if (bluetoothSocket != null)
          Log.Debug(TAG, "Succes!");
        else
          Log.Debug(TAG, "ConnectUsingCreateInsecureRfCommSocketToServiceRecord_SPP_SERVICE_UUID failed!");
        return bluetoothSocket;
      }
    }


    /// <summary>
    /// Manages valid device connection in separate thread
    /// </summary>
    public class ConnectedThread : Thread
    {
      private readonly ReaderDevice mReaderDevice;
      private BluetoothSocket mBluetoothSocket;
      private readonly Stream mInputStream;
      private readonly Stream mOutputStream;

      /// <summary>
      /// Use for closing the threads socket
      /// </summary>
      private void CloseSocket()
      {
        try
        {
          if (mBluetoothSocket != null)
          {
            mBluetoothSocket.Close();
            mBluetoothSocket = null;
            mLastDisconnect = DateTime.Now.Millisecond;
          }
        }
        catch (IOException e)
        {
          Log.Error(TAG, "Closing the connected socket failed!", e);
        }
      }

      public ConnectedThread(ReaderDevice device, BluetoothSocket socket)
      {
        Log.Debug(TAG, "Creating ConnectedThread");
        mReaderDevice = device;
        mBluetoothSocket = socket;
        Stream tmpStreamIn = null;
        Stream tmpStreamOut = null;

        // Get the input and output streams
        try
        {
          tmpStreamIn = socket.InputStream;
          tmpStreamOut = socket.OutputStream;
        }
        catch (IOException e)
        {
          Log.Error(TAG, "Temp sockets not created", e);
        }

        mInputStream = tmpStreamIn;
        mOutputStream = tmpStreamOut;
      }

      public override void Run()
      {
        try
        {
          Log.Debug(TAG, "Start ConnectedThread");
          Name = "ConnectedThread";
          mHandler.ObtainMessage(Constants.MESSAGE_SHOW_NOTIFICATION_CONNECTED, mReaderDevice.ToString()).SendToTarget();

          byte[] buffer = new byte[1024];
          int bytes;

          // Listen to the InputStream while connected
          while (true)
          {
            try
            {
              bytes = mInputStream.Read(buffer, 0, buffer.Length);
              // Notify about the received data
              byte[] data = new byte[bytes];
              System.Array.Copy(buffer, 0, data, 0, bytes);
              mHandler.ObtainMessage(Constants.MESSAGE_DATA_RECEIVED, bytes, -1, new DataMessageWrapper(mReaderDevice, data).ToString()).SendToTarget();
            }
            catch (System.Exception e)
            {
              Log.Debug(TAG, "Socket connection closed");
              StaticWrapper.ConnectionLost(mReaderDevice);
              break;
            }
          }
        }
        finally
        {
          mHandler.ObtainMessage(Constants.MESSAGE_SHOW_NOTIFICATION_NOT_CONNECTED, mReaderDevice.ToString()).SendToTarget();
        }
      }

      public void Write(byte[] buffer)
      {
        try
        {
          mOutputStream.Write(buffer, 0, buffer.Length);
        }
        catch (System.Exception e)
        {
          Log.Error(TAG, "Exception during write", e);
        }
      }

      public void Cancel()
      {
        Log.Debug(TAG, "Closing connected socket");
        CloseSocket();
        Log.Debug(TAG, "Connect socket closed");
      }
      #endregion

    }
  }
}