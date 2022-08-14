using Android.Bluetooth;
using Android.OS;
using Android.Util;
using OMNIToolsREADyMUC.Common.Enums;
using System;

namespace OMNIToolsREADyMUC.Common.Helpers
{
  public class ConnectPolicy
  {
    public readonly bool doCreateBond;
    public readonly bool doCreateInsecureRfcommSocketUsingChannel;
    public readonly bool doCreateInsecureRfcommSocketToServiceRecord;
    public readonly bool doCreateRfcommSocketUsingChannel;
    public readonly bool doCreateRfcommSocketToServiceRecord;

    public ConnectPolicy(ConnectDeviceTypeEnum deviceType, DeviceClass bluetoothDeviceClass)
    {
      String manufacturer = Build.Manufacturer;
      String deviceName = Build.Model;

      //IMPORTANT ! enclose the name in *. ie *name*
      String ConnectPolicy1Devices = "*GT-N7100*,*GT-I9305*,*HTC One X*,*LT25i*,*GT-I9300*,*HTC One S*";

      Log.Debug(Connection.BTManager.TAG, "Device info. Manufacturer: " + manufacturer + ", Model: " + deviceName);

      if (bluetoothDeviceClass == DeviceClass.ComputerDesktop ||
          bluetoothDeviceClass == DeviceClass.ComputerLaptop)
      {
        Log.Debug(Connection.BTManager.TAG, "Using PC connection");
        doCreateBond = true;
        doCreateRfcommSocketUsingChannel = false;
        doCreateInsecureRfcommSocketUsingChannel = false;
        doCreateRfcommSocketToServiceRecord = true;
        doCreateInsecureRfcommSocketToServiceRecord = false;
      }
      else if (ConnectPolicy1Devices.Contains("*" + deviceName + "*"))
      {
        Log.Debug(Connection.BTManager.TAG, "Using ConnectPolicy1");
        doCreateBond = true;
        doCreateRfcommSocketUsingChannel = false;
        doCreateInsecureRfcommSocketUsingChannel = true;
        doCreateRfcommSocketToServiceRecord = false;
        doCreateInsecureRfcommSocketToServiceRecord = false;
      }
      else
      {
        Log.Debug(Connection.BTManager.TAG, "Using Default ConnectPolicy");
        doCreateBond = true;
        doCreateRfcommSocketUsingChannel = true;
        doCreateInsecureRfcommSocketUsingChannel = true;
        doCreateRfcommSocketToServiceRecord = false;
        doCreateInsecureRfcommSocketToServiceRecord = false;
      }
    }
  }
}