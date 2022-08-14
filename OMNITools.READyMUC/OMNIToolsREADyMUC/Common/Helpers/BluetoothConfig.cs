using Java.Util;
using OMNIToolsREADyMUC.Common.Enums;
using System;
using System.Collections.Generic;

namespace OMNIToolsREADyMUC.Common.Helpers
{
  public class BluetoothConfig
  {
    // Bluetooth properties
    public static long BT_RECONNECT_DELAY_MILLIS = 2000;
    public static int BT_DEFAULT_CONNECT_RETRIES = 3;
    public static bool BT_SHOW_UNKNOWN_DEVICES = false;
    public static bool USE_RDyC_TIMEOUT = true;

    public static readonly String DEVICE_TYPE_USB = "8602";
    public static readonly String DEVICE_TYPE_EU_2013 = "8605";
    public static readonly String DEVICE_TYPE_US_2014 = "8606";
    public static readonly String DEVICE_TYPE_EU_2014 = "8607";
    public static readonly String DEVICE_TYPE_AU_2016 = "8608";
    public static readonly String DEVICE_TYPE_IN_2016 = "8609";
    public static readonly String DEVICE_TYPE_EU_2016 = "860A";
    public static readonly String DEVICE_TYPE_EU_2017 = "860C";
    public static readonly String DEVICE_TYPE_US_2017 = "860B";

    public static List<String> RDyCS_SUPPORTING_C2_RADIO = new List<String> { DEVICE_TYPE_EU_2016, DEVICE_TYPE_EU_2017, DEVICE_TYPE_US_2017 };
    public static List<String> RDyCS_SUPPORTING_C2_INSTANT_CONNECT = new List<String> { DEVICE_TYPE_US_2017, DEVICE_TYPE_EU_2017 };
    public static List<String> RDyCS_NOT_SUPPORTING_REPEATERS = new List<String> { DEVICE_TYPE_US_2014, DEVICE_TYPE_US_2017 };
    public static List<String> RDyCS_SUPPORTING_BOOTLOAD_V2 = new List<String> { DEVICE_TYPE_EU_2017, DEVICE_TYPE_US_2017 };

    public static ConnectDeviceTypeEnum DEVICE_TYPE_TEST = ConnectDeviceTypeEnum.RDyC;

    // Temporary set to 0 to be able to pass battery test
    public static int NEEDED_BATTERY_POWER_LEVEL = 30;
    // For test purpose only. if its true uploads will run without waiting for user input
    public static bool USE_CONTINUOUS_UPLOAD = false;
    public static long RDyC_MAX_RETRY = 3;

    public static long RDyC_KEEPALIVE_TIMEOUT_SHORT = 2000; // originally 500
    public static long RDyC_KEEPALIVE_TIMEOUT_LONG = 10000; // originally 2000

    // Bluetooth Service UUID for SerialPortServiceClass
    public static UUID SPP_SERVICE_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

    public static long BT_RECONNECT_DELAY = 5000;
    public static long MAX_TIME_WAIT_FOR_THREADS_STOPPED = 30000;
  }
}