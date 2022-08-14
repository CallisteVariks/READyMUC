using OMNIToolsREADyMUC.Common.Enums;
using System;
using System.Collections.Generic;

namespace OMNIToolsREADyMUC.Reader
{
  public interface IConnectionManager
  {
    /// <summary>
    /// Connect to device
    /// </summary>
    /// <param name="device"></param>
    void Connect(ReaderDevice device);

    /// <summary>
    /// Get list of connected devices
    /// </summary>
    /// <returns></returns>
    List<ReaderDevice> GetConnectedDevices();

    /// <summary>
    /// Get connected device by address
    /// </summary>
    /// <param name="deviceAddress"></param>
    /// <returns></returns>
    ReaderDevice GetConnectedDeviceByAddress(String deviceAddress);

    /// <summary>
    /// Disconnect from selected device
    /// </summary>
    /// <param name="device"></param>
    void Disconnect(ReaderDevice device);

    /// <summary>
    /// Disconnect from all devices
    /// </summary>
    void DisconnectAll();

    /// <summary>
    /// Write byte sequence to currently connected devices
    /// </summary>
    /// <param name="out"> byte sequence </param>
    void Write(byte[] @out);

    /// <summary>
    /// Write byte sequence to specified device
    /// </summary>
    /// <param name="device"></param>
    /// <param name="out"> byte sequence </param>
    void Write(ReaderDevice device, byte[] @out);

    /// <summary>
    /// Write byte sequence to devices with specified connection type
    /// </summary>
    /// <param name="deviceType"></param>
    /// <param name="out"> byte sequence </param>
    void Write(ConnectDeviceTypeEnum deviceType, byte[] @out);

    /// <summary>
    /// Initiate device discovery process
    /// </summary>
    void Scan();

    /// <summary>
    /// Cancel any active device discovery process
    /// </summary>
    void CancelScan();


  }
}