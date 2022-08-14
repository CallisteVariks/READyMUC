using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.KMRF;
using OMNIToolsREADyMUC.KMRF.Commands;
using System;

namespace OMNIToolsREADyMUC.Common.State
{
  public class StateFirmwareCheck : StateBase, IState
  {
    public StateFirmwareCheck(StateHandler bcuStateHandler)
    {
      InitState(bcuStateHandler, this);
      SendCommand(GetCommand(), "WhoIs");
    }

    public void HandleEvent(EventsEnum @event)
    {
      switch (@event)
      {
        case EventsEnum.Disconnected:
            new StateNotConnected(mStateHandler);
            break;

        case EventsEnum.TimeOut:
            RetrySendCommand(GetCommand());
            break;

        default:
            NoAction();
            break;
      }
    }

    public void HandleData(KmRfCommandResponse kmRfCommandResponse)
    {
      if (kmRfCommandResponse is WhoIsResponse) // only handle if correct answer
      {
        WhoIsResponse response = (WhoIsResponse)kmRfCommandResponse;
        mStateHandler.ClearTimeOut();

        if (!RetrySendCommand(GetCommand())) // retry
          new StateNotConnected(mStateHandler); // no more retries
      }
    }

    public override StateEnum GetState()
    {
      return StateEnum.FirmwareCheck;
    }

    private byte[] GetCommand()
    {
      try
      {
        return new WhoIsRequest().GetKmRfTelegram().GetData();
      }
      catch (Exception e)
      {
        return new byte[] { 0 }; 
      }
    }

  }
}