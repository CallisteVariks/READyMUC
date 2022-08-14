using System;

namespace OMNIToolsREADyMUC.Reader
{
  public interface IDeviceCommandListener
  {
    /// <summary>
    /// On command data sent to bluetooth device
    /// </summary>
    /// <param name="device"> the origin device </param>
    /// <param name="rawData"> the raw data bytes </param>
    /// <param name="commandName"> name of the command </param>
    void OnCommandSent(ReaderDevice device, byte[] rawData, String commandName);
  }
}