using Android.Bluetooth;
using Android.Util;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.Fragments;
using OMNIToolsREADyMUC.Reader;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static OMNIToolsREADyMUC.Connection.BTManager;

namespace OMNIToolsREADyMUC.Common.Helpers
{
  /// <summary>
  /// Used as a static class, when cannot in other class
  /// Helper class
  /// </summary>
  public static class StaticWrapper
  {
    #region For BluetoothManager
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Disconnect(ReaderDevice device)
    {
      Log.Debug(TAG, "Disconnect called: " + device.Name);

      // If device is in the connections list, cancel and remove it
      if (mConnectedThreads.ContainsKey(device))
      {
        ConnectedThread thread = mConnectedThreads[device];
        thread.Cancel();
        mConnectedThreads.Remove(device);
      }
      SetState(device, Constants.STATE_NONE);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Connected(BluetoothSocket socket, ReaderDevice device)
    {
      Log.Debug(TAG, "Connected:" + device);

      // Start the thread to manage the connection and perform transmissions
      ConnectedThread connectedThread = new ConnectedThread(device, socket);
      connectedThread.Start();
      mConnectedThreads.Add(device, connectedThread);

      // Notify about the connected device
      mHandler.ObtainMessage(Constants.MESSAGE_DEVICE_CONNECTED, device.ToString()).SendToTarget();
      SetState(device, Constants.STATE_CONNECTED);
    }

    public static void ConnectionFailed(ReaderDevice device)
    {
      Log.Debug(TAG, "Connection failed! Calling disconnect");
      Disconnect(device);
      mHandler.ObtainMessage(Constants.MESSAGE_CONNECTION_FAILED, device.ToString()).SendToTarget();

      MainActivity.RDyC.OnConnectFailed(device);
    }

    public static void ConnectionLost(ReaderDevice device)
    {
      Log.Debug(TAG, "Connection lost! Calling disconnect");
      Disconnect(device);
      mHandler.ObtainMessage(Constants.MESSAGE_CONNECTION_LOST, device.ToString()).SendToTarget();
      MainActivity.RDyC.OnConnectionLost(device);
    }
    #endregion




    #region For ReaderManager
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Write(ReaderDevice device, byte[] Out, String commandName)
    {
      if (device == null)
      {
        Log.Error(TAG, "Device was null");
        return;
      }

      device.ResetSendKeepAliveTimer();
      ReaderManager.mReaderService.Write(device, Out);

      foreach (IDeviceCommandListener listenter in ReaderManager.mCmdListeners)
        listenter.OnCommandSent(device, Out, commandName);
    }

    public static void DisconnectForRM(ReaderDevice device)
    {
      ReaderManager.mReaderService.Disconnect(device);
    }

    public static List<ReaderDevice> GetConnectedReaderDevices()
    {
      List<ReaderDevice> result = new List<ReaderDevice>();
      foreach (ReaderDevice device in GetConnectedDevices())
        if (device.DeviceType == ConnectDeviceTypeEnum.RDyC)
          result.Add(device);
      return result;
      // GetConnectedDevices().Where(device => device.DeviceType == ConnectDeviceTypeEnum.BCU).Select(device => device).ToList();
    }

    public static List<ReaderDevice> GetConnectedDevices()
    {
      if (ReaderManager.mReaderService != null)
        return GetConnectedDevicesinBM();
      else
        return new List<ReaderDevice>();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static List<ReaderDevice> GetConnectedDevicesinBM()
    {
      return new List<ReaderDevice>(mConnectedThreads.Keys);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static ReaderDevice GetConnectedDeviceByAddress(string deviceAddress)
    {
      foreach (ReaderDevice readerDevice in mConnectedThreads.Keys)
        if (readerDevice.Address.Equals(deviceAddress))
          return readerDevice;
      return null;
    }
    #endregion
  }
}