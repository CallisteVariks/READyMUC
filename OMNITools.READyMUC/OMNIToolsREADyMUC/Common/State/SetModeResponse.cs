using OMNIToolsREADyMUC.KMRF;
using OMNIToolsREADyMUC.KMRF.Parser;

namespace OMNIToolsREADyMUC.Common.State
{
  public class SetModeResponse : KmRfCommandResponse
  {
    public SetModeResponse(KmRfNetworkTelegram telegram) : base(telegram) { }
  }
}