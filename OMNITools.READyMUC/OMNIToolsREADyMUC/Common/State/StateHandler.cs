using Android.OS;
using Android.Util;
using Java.Lang;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.KMRF;
using OMNIToolsREADyMUC.Reader;
using System;

namespace OMNIToolsREADyMUC.Common.State
{
  public class StateHandler : Java.Lang.Object, IRunnable, IState
  {
    public static readonly System.String TAG = "BcuStateHandler";
    public ReaderManager mReaderManager;
    private static ReaderDevice mDevice;

    IState mState;
    private static readonly Handler mHandler = new Handler();

    public IntPtr Handle => throw new NotImplementedException();


    public void Run()
    {
      Log.Debug(TAG, "TimeOut");
      HandleEvent(EventsEnum.TimeOut);
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public StateHandler(ReaderManager readerManager, ReaderDevice device)
    {
      mReaderManager = readerManager;
      mDevice = device;
      mState = new StateNotConnected(this);
    }

    public void SetNewState(IState newState)
    {
      Log.Debug(TAG, "SetNewState : " + newState.GetState().ToString());
      mState = newState;
    }

    public void StartTimeOut(long timeoutInMilliSeconds)
    {
      mHandler.RemoveCallbacks(this); // remove evt. pending timeout
      mHandler.PostDelayed(this, timeoutInMilliSeconds);
    }

    public void ClearTimeOut()
    {
      mHandler.RemoveCallbacks(this); // remove evt. pending timeout
    }

    public void HandleData(KmRfCommandResponse kmRfCommandResponse)
    {
      mState.HandleData(kmRfCommandResponse);
    }

    public ReaderDevice GetReaderDevice()
    {
      return mDevice;
    }

    StateEnum IState.GetState()
    {
      return mState.GetState();
    }

    public StateEnum GetStateFromHandler()
    {
      return mState.GetState();
    }

    public void HandleEvent(EventsEnum @event)
    {
      Log.Debug(TAG, "HandleEvent : " + @event.ToString());
      mState.HandleEvent(@event);
    }

  }
}
