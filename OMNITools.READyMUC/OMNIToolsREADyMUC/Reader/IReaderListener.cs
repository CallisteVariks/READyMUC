namespace OMNIToolsREADyMUC.Reader
{
  /// <summary>
  /// Interface definition for ReaderService callbacks
  /// </summary>
  public interface IReaderListener
  {
    /// <summary>
    /// Connection status has changed
    /// Reflects the current connection status
    /// This will change when the user successfully connects to device, explicitly calls disconnect or connection is lost due to signal error
    /// </summary>
    /// <param name="device"></param>
    /// <param name="connected"> connection status </param>
    void OnConnectionStatusChanged(ReaderDevice device, bool connected);

    /// <summary>
    /// Connection to current device is lost
    /// This event is signaled when connection to currently connected device is lost, never when user disconnects by explicitly calling disconnect
    /// </summary>
    /// <param name="device"></param>
    void OnConnectionLost(ReaderDevice device);

    /// <summary>
    /// Connection attempted to device failed
    /// This event is signaled when connect attempt to device fails, never when user disconnects by explicitly calling disconnect or an existing connection fails
    /// </summary>
    /// <param name="device"></param>
    void OnConnectFailed(ReaderDevice device);

    /// <summary>
    /// Data received from connected device
    /// </summary>
    /// <param name="device"> the device the data was received from </param>
    /// <param name="data"> buffer with received data </param>
    void OnDataReceived(ReaderDevice device, byte[] data);

    /// <summary>
    /// New device has been found during discovery process
    /// </summary>
    /// <param name="device"> discovered device </param>
    void OnDeviceDiscovered(ReaderDevice device);

    /// <summary>
    /// Discovery process has ended
    /// </summary>
    void OnDiscoveryFinished();

    /// <summary>
    /// A reconnect sequence has been started
    /// </summary>
    /// <param name="device"></param>
    void OnReconnectSequenceStarted(ReaderDevice device);

    /// <summary>
    /// A device has been connected
    /// </summary>
    /// <param name="device"></param>
    void OnConnectionEstablished(ReaderDevice device);
  }
}