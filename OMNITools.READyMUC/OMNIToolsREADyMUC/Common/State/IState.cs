using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.KMRF;

namespace OMNIToolsREADyMUC.Common.State
{
  public interface IState
  {
    StateEnum GetState();

    void HandleEvent(EventsEnum @event);

    void HandleData(KmRfCommandResponse kmRfCommandResponse);

  }
}