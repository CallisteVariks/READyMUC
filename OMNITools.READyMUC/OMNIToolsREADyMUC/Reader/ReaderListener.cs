using Android.Util;
using OMNIToolsREADyMUC.Common.Converters;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.KMRF;
using OMNIToolsREADyMUC.KMRF.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OMNIToolsREADyMUC.Reader
{
  public class ReaderListener : IReaderListener
  {
    private static readonly String TAG = "ReaderListener";
    private readonly List<IReaderDeviceConnectionListener> mReaderListeners = ReaderManager.mReaderListeners;
    private List<IReaderDeviceDiscoveryListener> mDiscoveryListeners = ReaderManager.mDiscoveryListeners;


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnConnectFailed(ReaderDevice device)
    {
      Log.Debug(TAG, device.Name + ": Connect faile");
      device.StateHandler.HandleEvent(EventsEnum.Disconnected);

      foreach (IReaderDeviceConnectionListener listener in mReaderListeners)
        listener.OnConnectFailed(device);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnConnectionEstablished(ReaderDevice device)
    {
      Log.Debug(TAG, device.Name + ": Connection established");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnConnectionLost(ReaderDevice device)
    {
      Log.Debug(TAG, device.Name + ": Connection lost");
      device.StateHandler.HandleEvent(EventsEnum.Disconnected);

      List<IReaderDeviceConnectionListener> currentListeners = new List<IReaderDeviceConnectionListener>(mReaderListeners); 
      foreach (IReaderDeviceConnectionListener listener in currentListeners)
        listener.OnConnectionLost(device);
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnConnectionStatusChanged(ReaderDevice device, bool connected)
    {
      if (connected)
      {
        Log.Debug(TAG, device.Name + ": Connection status changed, Connected");
        device.StateHandler.HandleEvent(EventsEnum.Connect);
      }
      else
      {
        Log.Debug(TAG, device.Name + ": Connection status changed, Disconnected");
        device.StateHandler.HandleEvent(EventsEnum.Disconnected);
      }

      foreach (var listener in mReaderListeners)
        listener.OnConnectionStatusChanged(device, connected);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnDataReceived(ReaderDevice device, byte[] data)
    {
      try
      {
        switch (device.DeviceType)
        {
          case ConnectDeviceTypeEnum.RDyC:
          List<KmRfCommandResponse> responses = KamstrupRfProtocol.ParseKmRfResponses(data);
          if (responses != null)
          {
            int responseIterator = 0;
            bool hasMultipleResponses = responses.Capacity > 1;
            foreach (KmRfCommandResponse kmRfResponse in responses)
            {
              responseIterator++;

              // Handle timeout
              if (!(kmRfResponse is PushDataResponse))
              {
                if (kmRfResponse is StatusFrameResponse)
                {
                  device.batteryLevel = ((StatusFrameResponse)kmRfResponse).batteryLevel;
                  device.chargingStatus = ((StatusFrameResponse)kmRfResponse).charging;
                }

                // KeepAliveStrategy - a frame has been received that was not push data
                device.ResetResponseTimeoutTimer();
              }

              // Send event to listener
              Log.Debug(TAG, String.Format("%s: onDataReceived: %s (KmRf %s%s)", device.Name, HexStringByteConverter.ByteArrayToHexString(data), typeof(KmRfCommandResponse).Name, hasMultipleResponses ? " multi=" + responseIterator : ""));

              foreach (IReaderDeviceConnectionListener listener in mReaderListeners)
                listener.OnKmRfDataReceived(device, data, kmRfResponse);

              try
              {
                device.StateHandler.HandleData(kmRfResponse);
              }
              catch (Exception e)
              {
                Log.Error(TAG, "Error calling handleData on stateHandler", e);
              }
            }
          }
          else
            Log.Debug(TAG, device.Name + ": onDataReceived: " + HexStringByteConverter.ByteArrayToHexString(data) + "(Unknown protocol type)");
          break;

          default:
          Log.Debug(TAG, device.Name + ": onDataReceived: " + HexStringByteConverter.ByteArrayToHexString(data) + "(Unknown device type)");
          break;
        }
      }
      catch (Exception e)
      {
        Log.Error(TAG, device.Name + ": onDataReceived: " + HexStringByteConverter.ByteArrayToHexString(data) + "(Error in parsing)", e);
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnDeviceDiscovered(ReaderDevice device)
    {
      foreach (IReaderDeviceDiscoveryListener listener in mDiscoveryListeners)
        listener.OnDeviceDiscovered(device);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnDiscoveryFinished()
    {
      foreach (IReaderDeviceDiscoveryListener listener in mDiscoveryListeners)
        listener.OnDiscoveryFinished();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void OnReconnectSequenceStarted(ReaderDevice device)
    {
      Log.Debug(TAG, device.Name + ": Reconnect Sequence started");

      foreach (IReaderDeviceConnectionListener listener in mReaderListeners)
        listener.OnReconnectSequenceStarted(device);
    }
  }
}