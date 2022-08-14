using Android.Util;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.KMRF;
using System;

namespace OMNIToolsREADyMUC.Common.State
{
  public class StateMBusDataNotActive : StateBase, IState
  {
    public StateMBusDataNotActive(StateHandler stateHandler)
    {
      InitState(stateHandler, this);

      // Send command
      SendCommand(GetCommand(), "Stop");
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

        case EventsEnum.Disconnected:
        new StateNotConnected(mStateHandler);
        break;

        case EventsEnum.TimeOut:
        RetrySendCommand(GetCommand()); // retry
        break;

        default:
        NoAction();
        break;
      }
    }

    public void HandleData(KmRfCommandResponse response)
    {
      if (response is SetModeResponse) // only handle if correct answer
      {
        mStateHandler.ClearTimeOut();
        if (response.ack)
          Log.Debug(TAG, "Ack recieved");
        else
        {
          if (!RetrySendCommand(GetCommand())) // retry
            new StateNotConnected(mStateHandler); // no more retry's
        }
      }
    }

    public override StateEnum GetState()
    {
      return StateEnum.MBusDataNotActive;
    }

    private byte[] GetCommand()
    {
      try
      {
        return new SetModeRequest(ModeEnum.MBUS_MODE_DONT_LISTEN).GetKmRfTelegram().GetData();
      }
      catch (Exception e)
      {
        return new byte[] { 0 };
      }
    }
  }
}