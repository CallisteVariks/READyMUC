using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.KMRF;
using System;

namespace OMNIToolsREADyMUC.Common.State
{
  public class StateNotConnected : StateBase, IState
  {

    public StateNotConnected(StateHandler stateHandler)
    {
      InitState(stateHandler, this);
    }

    public void HandleEvent(EventsEnum @event)
    {
      switch (@event)
      {
        case EventsEnum.Connect:
          {
            switch (mStateHandler.GetReaderDevice().DeviceType)
            {
              case ConnectDeviceTypeEnum.RDyC:
              new StateFirmwareCheck(mStateHandler);
              break;

              default:
              NoAction();
              break;
            }
            break;
          }
        default:
        NoAction();
        break;
      }
    }

    public void HandleData(KmRfCommandResponse kmRfCommandResponse)
    {
      throw new NotImplementedException();
    }

    public override StateEnum GetState()
    {
      return StateEnum.NotConnected;
    }
  }
}
