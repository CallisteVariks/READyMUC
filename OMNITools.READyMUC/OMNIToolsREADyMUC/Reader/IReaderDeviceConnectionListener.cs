using OMNIToolsREADyMUC.KMRF;

namespace OMNIToolsREADyMUC.Reader
{
  /// <summary>
  /// Handles callback for events regarding the connection and data
  /// </summary>
  public interface IReaderDeviceConnectionListener
  {
    /// <summary>
    /// RDyC is connected
    /// This event is generated after the RDyC has been connected
    /// </summary>
    /// <param name="device"></param>
    void OnConnectedAndReady(ReaderDevice device);

    /// <summary>
    /// Connection status has changed
    /// Reflects the current connection status
    /// This will change when the user successfully connects to device, explicitly calls disconnect or connection is lost due to signal error
    /// </summary>
    /// <param name="device"></param>
    /// <param name="connected"> Connection status </param>
    void OnConnectionStatusChanged(ReaderDevice device, bool connected);

    /// <summary>
    /// Connection to current device is lost
    /// This event is signaled when connection to currently connected device is lost, never when user disconnects by explicitly calling disconnect
    /// </summary>
    /// <param name="device"> the device that lost connection </param>
    void OnConnectionLost(ReaderDevice device);

    /// <summary>
    /// Connection attempted to device failed
    /// This event is signaled when connect attempt to device fails, never when user disconnects by explicitly calling disconnect or an existing connection fails
    /// </summary>
    /// <param name="device"></param>
    void OnConnectFailed(ReaderDevice device);

    /// <summary>
    /// Mbus data are relayed form the RDyC to Bluetooth
    /// </summary>
    void OnMBusDataStarted();

    /// <summary>
    /// Mbus data are no longer relayed from the RDyC to Bluetooth
    /// </summary>
    void OnMBusDataStopped();

    /// <summary>
    /// A reconnect sequence has been started
    /// </summary>
    /// <param name="device"></param>
    void OnReconnectSequenceStarted(ReaderDevice device);

    /// <summary>
    /// On KmRf data received
    /// </summary>
    /// <param name="device"> the origin device </param>
    /// <param name="rawData"> the raw data bytes </param>
    /// <param name="kmRfResponse"> the parsed KmRf response object </param>
    void OnKmRfDataReceived(ReaderDevice device, byte[] rawData, KmRfCommandResponse kmRfResponse);
  }
}