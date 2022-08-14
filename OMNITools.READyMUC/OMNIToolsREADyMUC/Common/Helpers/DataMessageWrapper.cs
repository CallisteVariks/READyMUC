using OMNIToolsREADyMUC.Reader;

namespace OMNIToolsREADyMUC.Common.Helpers
{
  public class DataMessageWrapper
  {
    public readonly ReaderDevice readerDevice;
    public readonly byte[] dataBytes;

    public DataMessageWrapper(ReaderDevice readerDevice, byte[] dataBytes)
    {
      this.readerDevice = readerDevice;
      this.dataBytes = dataBytes;
    }
  }
}