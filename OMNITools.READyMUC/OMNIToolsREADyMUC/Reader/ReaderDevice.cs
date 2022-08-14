using Android.Bluetooth;
using Android.Content;
using Android.OS;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.Common.State;
using OMNIToolsREADyMUC.Common.Helpers;
using System;
using System.Globalization;

namespace OMNIToolsREADyMUC.Reader
{
  public class ReaderDevice
  {
    private String mName;
    private String mAddress;
    private ConnectDeviceTypeEnum mDeviceType;

    [NonSerialized]
    private StateHandler mStateHandler;
    [NonSerialized]
    private int mState = 1;
    [NonSerialized]
    private BluetoothDevice mBluetoothDevice = null;

    [NonSerialized]
    public String unitType;
    [NonSerialized]
    public int firmwareVersion = 0;
    [NonSerialized]
    public int batteryLevel = 0;
    [NonSerialized]
    public bool chargingStatus = false;

    [NonSerialized]
    public long mFrameSendTimeStamp = 0;
    [NonSerialized]
    public bool mWaitingForFrameReceived = false;
    [NonSerialized]
    public long mKeepAliveTimeOutInMilliSec;
    [NonSerialized]
    public long mKeepAliveRetryCounter;

    public ReaderDevice(String name, String address, ConnectDeviceTypeEnum deviceType)
    {
      mName = name;
      mAddress = address;
      mDeviceType = deviceType;
    }

    public String Name
    {
      get
      {
        if (mName == null)
          return mAddress;
        return mName;
      }
      set
      { mName = value; }
    }

    public String Address => mAddress;

    public String GetDeviceTypeString(Context ctx)
    {
      if (mDeviceType == ConnectDeviceTypeEnum.RDyC)
        return ctx.GetString(Resource.String.rdyc_device_name);

      return mDeviceType.ToString();
    }

    public ConnectDeviceTypeEnum DeviceType
    {
      get => mDeviceType;
      set { mDeviceType = value; }
    }

    public BluetoothDevice BluetoothDevice
    {
      get => mBluetoothDevice;
      set { mBluetoothDevice = value; }
    }

    public DeviceClass GetBluetoothDeviceClass()
    {
      if (mBluetoothDevice == null || mBluetoothDevice.BluetoothClass == null)
        return 0;
      return mBluetoothDevice.BluetoothClass.DeviceClass;
    }

    public bool IsConnected() => mState == Constants.STATE_CONNECTED;

    public int State
    {
      get => mState;
      set { mState = value; }
    }

    public StateHandler StateHandler
    {
      get => mStateHandler;
      set
      {
        mStateHandler = value;
      }
    }

    public int GetIconId()
    {
      switch (mDeviceType)
      {
        case ConnectDeviceTypeEnum.RDyC:
        return Resource.Drawable.ic_ready_converter;
        default:
        return Resource.Drawable.ic_bluetooth_device;
      }
    }

    public int GetOrder() => Name.GetHashCode();


    /// <summary>
    /// Resets the SendKeepAliveTimer
    /// Done each time a frame is send to the reader
    /// </summary>
    public void ResetSendKeepAliveTimer()
    {
      mFrameSendTimeStamp = SystemClock.ElapsedRealtime();
    }

    /// <summary>
    /// Reset the ResponseTimeOut timer when a frame is being received EXCEPT if it is a WMbus PushData frame
    /// </summary>
    public void ResetResponseTimeoutTimer()
    {
      mKeepAliveRetryCounter = 0;
      mWaitingForFrameReceived = false;
    }


    #region Helper methods
    public static ConnectDeviceTypeEnum GetConnectDeviceType(String name)
    {
      if (name != null)
      {
        name = name.ToLower(CultureInfo.CurrentCulture);

        if (name.Contains("kamstrup") || name.Contains("ready"))
          return ConnectDeviceTypeEnum.RDyC;
      }

      if (BluetoothConfig.BT_SHOW_UNKNOWN_DEVICES)
        return BluetoothConfig.DEVICE_TYPE_TEST;

      return ConnectDeviceTypeEnum.Unknown;
    }

    public override int GetHashCode()
    {
      int prime = 31;
      int result = 1;
      return result = prime * result + ((mAddress == null) ? 0 : mAddress.GetHashCode());
    }

    public override bool Equals(Object obj)
    {
      if (this == obj)
        return true;
      if (obj == null)
        return false;
      if (GetType() != obj.GetType())
        return false;
      ReaderDevice other = (ReaderDevice)obj;
      if (mAddress == null)
      {
        if (other.mAddress != null)
          return false;
      }
      else if (!mAddress.Equals(other.mAddress))
        return false;
      return true;
    }
    #endregion
  }
}