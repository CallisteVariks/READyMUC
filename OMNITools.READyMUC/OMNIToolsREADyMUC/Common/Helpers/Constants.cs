namespace OMNIToolsREADyMUC.Common.Helpers
{
  public static class Constants
  {
    /// <summary>
    /// Current state has changed: arg1 = state
    /// </summary>
    public const int MESSAGE_STATE_CHANGED = 1;

    /// <summary>
    /// Data received from device: arg1 = size, obj = (byte[])data
    /// </summary>
    public const int MESSAGE_DATA_RECEIVED = 2;

    /// <summary>
    /// New device discovered: obj = (Device)device
    /// </summary>
    public const int MESSAGE_DEVICE_DISCOVERED = 3;

    /// <summary>
    /// Device discovery process ended
    /// </summary>
    public const int MESSAGE_DEVICE_DISCOVERY_ENDED = 4;

    /// <summary>
    /// Device connected: obj = (Device)device
    /// </summary>
    public const int MESSAGE_DEVICE_CONNECTED = 5;

    /// <summary>
    /// Device connection attempt failed
    /// </summary>
    public const int MESSAGE_CONNECTION_FAILED = 6;

    /// <summary>
    /// Connection to device lost
    /// </summary>
    public const int MESSAGE_CONNECTION_LOST = 7;

    public const int MESSAGE_SHOW_NOTIFICATION_NOT_CONNECTED = 8;

    public const int MESSAGE_SHOW_CONNECTING = 9;

    public const int MESSAGE_SHOW_NOTIFICATION_CONNECTED = 10;

    public const int MESSAGE_RECONNECT = 100;

    /// <summary>
    /// Default state
    /// </summary>
    public const int STATE_NONE = 0;

    /// <summary>
    /// Attempting to connect device
    /// </summary>
    public const int STATE_CONNECTING = 1;

    /// <summary>
    /// Connect to device
    /// </summary>
    public const int STATE_CONNECTED = 2;

  }
}