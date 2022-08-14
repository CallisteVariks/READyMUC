using OMNIToolsREADyMUC.Common.Enums;
using System.Collections.Generic;

namespace OMNIToolsREADyMUC.Reader
{
  /// <summary>
  /// ReaderService interface
  /// </summary>
  public interface IReaderService
  {
    /// <summary>
    /// Returns the devices connected to at the moment
    /// If not connected, return null
    /// </summary>
    /// <returns></returns>
    List<ReaderDevice> GetConnectedDevices();

    /// <summary>
    /// Returns true if a trying to connect to a device is in progress
    /// </summary>
    /// <returns></returns>
    bool ConnectingToDevice();

    /// <summary>
    /// Returns true if a device scan is active
    /// </summary>
    /// <returns></returns>
    bool ScanningForDevices();

    /// <summary>
    /// Initiate device discovery process
    /// </summary>
    void Scan();

    /// <summary>
    /// Cancel any active device discovery process
    /// </summary>
    void CancelScan();

    /// <summary>
    /// Connect to device
    /// </summary>
    /// <param name="device"> ReaderDevice </param>
    void Connect(ReaderDevice device);

    /// <summary>
    /// Disconnect from all devices
    /// </summary>
    void DisconnectAll();

    /// <summary>
    /// Disconnect from any connected device
    /// </summary>
    /// <param name="device"></param>
    void Disconnect(ReaderDevice device);

    /// <summary>
    /// Disconnect from any connected device and do not attempt reconnect
    /// </summary>
    /// <param name="device"></param>
    void DisconnectWithoutReconnect(ReaderDevice device);

    /// <summary>
    /// Returns currently set retry count
    /// Sets the max number of retries when attempting to reconnect to device
    /// </summary>
    /// <returns> max.retry attempts </returns>
    int ReconnectMaxRetryCount { get; set; }

    /// <summary>
    /// Write byte sequence to currently connected devices
    /// </summary>
    /// <param name="out"> byte sequence </param>
    void Write(byte[] @out);

    /// <summary>
    /// Write byte sequence to currently connected devices of the specific type
    /// </summary>
    /// <param name="connType"></param>
    /// <param name="out"> byte sequence </param>
    void Write(ConnectDeviceTypeEnum connType, byte[] @out);

    /// <summary>
    /// Write byte sequence to specified device
    /// </summary>
    /// <param name="device"></param>
    /// <param name="out"> byte sequence </param>
    /// <param name=""></param>
    void Write(ReaderDevice device, byte[] @out);

    /// <summary>
    /// Register listener for receiving events about device status, discovery and incoming data
    /// </summary>
    /// <param name="listener"> event listener (listener object can only be registered once) </param>
    void RegisterListener(IReaderListener listener);

    /// <summary>
    /// Unregister previously registered listener
    /// </summary>
    /// <param name="listener"> event listener </param>
    void UnregisterListener(IReaderListener listener);

    /// <summary>
    /// Get the number of listeners attached
    /// </summary>
    /// <returns></returns>
    int GetListenerCount();

    /// <summary>
    /// Set Reconnect Mode
    /// </summary>
    /// <param name="active"> true = active, reconnect enabled </param>
    void SetReconnectMode(bool active);

    /// <summary>
    /// Set Reconnect Once
    /// Forces a reconnect sequence the next time an active connection is lost
    /// </summary>
    void SetReconnectOnce();

    void DisconnectWithoutReconnectAll();

    bool IsConnected();

    bool IsConnected(ConnectDeviceTypeEnum deviceType);

  }
}