using Android.Util;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.KMRF;
using System;

namespace OMNIToolsREADyMUC.Common.State
{
  public class StateMBusDataActive : StateBase, IState
  {
    private bool mUseBroadcast;

    public StateMBusDataActive(StateHandler stateHandler)
    {
      mUseBroadcast = false;
      InitState(stateHandler, this);

      // Send command
      SendCommand(GetCommand(), "StartSnoos");
    }

    public StateMBusDataActive(StateHandler stateHandler, bool withBroadcast)
    {
      mUseBroadcast = withBroadcast;
      InitState(stateHandler, this);

      // Send command
      if (mUseBroadcast)
        SendCommand(GetCommandBroadcast(), "StartSnoosWithBroadcast");
      else
        SendCommand(GetCommand(), "StartSnoos");
    }

    public void HandleEvent(EventsEnum @event)
    {
      switch (@event)
      {
        case EventsEnum.StartMbusDataWithBroadcast:
        SendCommand(GetCommandBroadcast(), "StartSnoosWithBroadcast");
        break;

        case EventsEnum.StartMbusData:
        SendCommand(GetCommand(), "StartSnoos");
        break;

        case EventsEnum.StopMBusData:
        new StateMBusDataNotActive(mStateHandler);
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
          // Acknowledge received. Do nothing
          Log.Debug(TAG, "Ack recieved");
        else
          if (!RetrySendCommand(mUseBroadcast ? GetCommandBroadcast() : GetCommand())) // retry
          new StateNotConnected(mStateHandler); // no more retries
      }
    }

    public override StateEnum GetState()
    {
      return StateEnum.MBusDataActive;
    }

    private byte[] GetCommand()
    {
      try
      {
        return new SetModeRequest(ModeEnum.MBUS_MODE_MTO_MODE_C).GetKmRfTelegram().GetData();
      }
      catch (Exception e)
      {
        return new byte[] { 0 }; 
      }
    }

    private byte[] GetCommandBroadcast()
    {
      try
      {
        return new SetModeRequest(ModeEnum.MBUS_MODE_SNOOS_DIRECT_BROADCAST).GetKmRfTelegram().GetData();
      }
      catch (Exception e)
      {
        return new byte[] { 0 }; 
      }
    }

  }
}