using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Util;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.Common.Helpers;
using OMNIToolsREADyMUC.Fragments;
using OMNIToolsREADyMUC.Reader;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OMNIToolsREADyMUC.Connection
{
  /// <summary>
  /// Listens for when the device goes in and out of Bluetooth discoverability mode, and will raise an Event
  /// </summary>
  public class DiscoverableModeReceiver : BroadcastReceiver
  {
    private Activity mActivity;
    public static List<ReaderDevice> listDevices = new List<ReaderDevice>();
    public static ReaderDevice newReaderDevice;

    public DiscoverableModeReceiver(Activity activity)
    {
      mActivity = activity;
    }

    public override void OnReceive(Context context, Intent intent)
    {
      if (!RDyCFragment.mUpdateUIThread.IsAlive)
        RDyCFragment.mUpdateUIThread.Start();

      String action = intent.Action;

      if (action == BluetoothDevice.ActionFound || BluetoothDevice.ActionNameChanged.Equals(action))
      {
        // Get the BluetoothDevice object from the Intent
        BluetoothDevice btDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
        BluetoothClass btClass = btDevice.BluetoothClass;
        String name = btDevice.Name;

        if (name != null)
        {
          if (name.ToLower(CultureInfo.CurrentCulture).Contains("kamstrup") || name.Contains("ready"))
          {
            newReaderDevice = new ReaderDevice(btDevice.Name, btDevice.Address, ConnectDeviceTypeEnum.RDyC);
            RDyCFragment.readerDevicesAdapter.NotifyDataSetChanged();
          }
        }

        String deviceName = intent.GetStringExtra(BluetoothDevice.ExtraName);
        Log.Debug("DiscoveryBroadcastReceiver", "BT Device discovered [" + btDevice.Address + "]: " + deviceName);
      }
      else if (BluetoothAdapter.ActionDiscoveryFinished.Equals(action))
      {
        BTManager.mHandler.ObtainMessage(Constants.MESSAGE_DEVICE_DISCOVERY_ENDED).SendToTarget();
        if (MainActivity.mCurrentFragment == MainActivity.mRDyCFragment && RDyCFragment.devicesList.Count > 0)
          RDyCFragment.mEmptyView.SetText(Resource.String.scan_complete);
        else if(MainActivity.mCurrentFragment == MainActivity.mRDyCFragment)
          RDyCFragment.mEmptyView.SetText(Resource.String.no_devices_found);

        RDyCFragment.mScanInProgress = false;
        RDyCFragment.UpdateUIThread.mUpdateUIThread = false;
      }
    }
  }
}
