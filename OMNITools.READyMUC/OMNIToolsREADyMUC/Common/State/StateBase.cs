using Android.Util;
using OMNIToolsREADyMUC.Common.Converters;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.Common.Helpers;
using System;

namespace OMNIToolsREADyMUC.Common.State
{
  public abstract class StateBase
  {
    public static readonly String TAG = "BcuState";
    public StateHandler mStateHandler;
    long mRetryCount;
    static readonly int RESPONSE_TIMEOUT = 2500;
    static readonly int GUI_RESPONSE_TIMEOUT = 1000;       // Delay before resending event to GUI/listeners

    public void InitState(StateHandler stateHandler, IState state)
    {
      mStateHandler = stateHandler;
      mStateHandler.SetNewState(state);
      mStateHandler.ClearTimeOut(); // Remove active timeouts;
      mRetryCount = BluetoothConfig.RDyC_MAX_RETRY;
    }

    public void SendCommand(byte[] data, String commandName)
    {
      SendCommand(data, true, commandName);
    }

    public abstract StateEnum GetState();

    public void NoAction()
    {
      Log.Debug(TAG, "No Action. Active state is " + GetState().ToString());
    }

    private void SendCommand(byte[] data, bool resetRetryCount, String commandName)
    {
      if (resetRetryCount)
        mRetryCount = BluetoothConfig.RDyC_MAX_RETRY;

      mStateHandler.StartTimeOut(RESPONSE_TIMEOUT);
      mStateHandler.mReaderManager.Write(mStateHandler.GetReaderDevice(), data, commandName);
      Log.Debug(TAG, "SendCommand : " + HexStringByteConverter.ByteArrayToHexString(data));
    }

    private void DisconnectNoReconnect()
    {
      mStateHandler.mReaderManager.DisconnectWithoutReconnect(mStateHandler.GetReaderDevice());
    }

    private void Disconnect()
    {
      mStateHandler.mReaderManager.Disconnect(mStateHandler.GetReaderDevice());
    }

    public bool RetrySendCommand(byte[] data)
    {
      if (mRetryCount > 0)
      {
        Log.Debug(TAG, "Retry, Retries left : " + mRetryCount);
        SendCommand(data, false, "Retry");
        mRetryCount--;
        return true;
      }
      else
      {
        Log.Debug(TAG, "No more retries");
        mStateHandler.mReaderManager.Disconnect(mStateHandler.GetReaderDevice()); // Keep Alive strategy. No more retries disconnect
        return false;
      }
    }

  }
}