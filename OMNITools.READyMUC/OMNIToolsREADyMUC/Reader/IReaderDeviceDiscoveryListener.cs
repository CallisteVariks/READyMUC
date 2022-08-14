namespace OMNIToolsREADyMUC.Reader
{
  /// <summary>
  /// Handles callback for events discovery of bluetooth RDyC devices
  /// Interface definition for RDyCManager callbacks
  /// </summary>
  public interface IReaderDeviceDiscoveryListener
  {
    /// <summary>
    /// New device was found during discovery process
    /// </summary>
    /// <param name="device"> discovered device </param>
    void OnDeviceDiscovered(ReaderDevice device);

    /// <summary>
    /// Discovery process ended
    /// </summary>
    void OnDiscoveryFinished();
  }
}
