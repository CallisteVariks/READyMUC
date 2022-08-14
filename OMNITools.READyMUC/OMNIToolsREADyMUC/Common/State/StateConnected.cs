using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.KMRF;

namespace OMNIToolsREADyMUC.Common.State
{
  public class StateConnected : StateBase, IState
  {
    public StateConnected(StateHandler stateHandler)
    {
      InitState(stateHandler, this);
      mStateHandler.ClearTimeOut();
      mStateHandler.mReaderManager.GenerateEventConnectedAndReady(mStateHandler.GetReaderDevice());
    }

    public void HandleEvent(EventsEnum @event)
    {
      switch (@event)
      {
        case EventsEnum.StartMbusDataWithBroadcast:
        new StateMBusDataActive(mStateHandler, true);
        break;

        case EventsEnum.StartMbusData:
        new StateMBusDataActive(mStateHandler);
        break;

        case EventsEnum.StopMBusData:
        new StateMBusDataNotActive(mStateHandler);
        break;

        case EventsEnum.Disconnected:
        new StateNotConnected(mStateHandler);
        break;

        default:
        NoAction();
        break;
      }
    }

    public void HandleData(KmRfCommandResponse response) { }

    public override StateEnum GetState()
    {
      return StateEnum.ConnectedAndReady;
    }

  }
}