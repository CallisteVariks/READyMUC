using OMNIToolsREADyMUC.Common.Converters;
using OMNIToolsREADyMUC.KMRF;
using OMNIToolsREADyMUC.KMRF.Parser;
using OMNIToolsREADyMUC.Common.Enums;

namespace OMNIToolsREADyMUC.Common.State
{
  public class SetModeRequest : KmRfCommandRequest
  {
    public readonly ModeEnum mode;

    public SetModeRequest(ModeEnum mode)
    {
      this.mode = mode;
    }

    protected override byte[] buildCommandAplLayer()
    {
      byte[] bytes = new byte[20];

      // APL
      bytes[0] = KmRfCommandConstants.PAYLOAD_CONTROL_REQUEST;

      // CMD
      bytes[1] = (byte)KmRfCommandEnum.READY_CONVERTER_CMD;

      // CTRL / Snoos mode
      bytes[3] = (byte)mode;

      // Reset rest of bytes
      ArraysUtil.Clear(bytes, 4, 16);

      return bytes;
    }
  }
}