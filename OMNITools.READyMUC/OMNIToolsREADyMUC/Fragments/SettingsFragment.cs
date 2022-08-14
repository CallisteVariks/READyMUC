using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections;

namespace OMNIToolsREADyMUC.Fragments
{
  public partial class SettingsFragment : Fragment, View.IOnClickListener
  {
    public static BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
    public static MainActivity mActivity;
    public static CheckBluetoothThread mCheckBluetoothThread;
    public static Switch mBluetoothOnOffSwitch;

    private Button mShowPairedDevices, mScanReaders, mMakeDiscoverable;
    private ListView mListOfPairedDevices;
    private ArrayList mListOfBondedDevices = new ArrayList();

    public SettingsFragment(MainActivity mainActivity)
    {
      mActivity = mainActivity;
      mCheckBluetoothThread = new CheckBluetoothThread();
    }

    public override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      return inflater.Inflate(Resource.Layout.settings_layout, container, false);
    }

    public override void OnViewCreated(View view, Bundle savedInstanceState)
    {
      mBluetoothOnOffSwitch = view.FindViewById<Switch>(Resource.Id.bluetooth_on_off_switch);
      mShowPairedDevices = view.FindViewById<Button>(Resource.Id.bluetooth_paired_devices_button);
      mScanReaders = view.FindViewById<Button>(Resource.Id.connect_reader);
      mMakeDiscoverable = view.FindViewById<Button>(Resource.Id.discoverable);
      mListOfPairedDevices = view.FindViewById<ListView>(Resource.Id.bluetooth_paired_devices_listView);

      mShowPairedDevices.SetOnClickListener(this);
      mScanReaders.SetOnClickListener(this);
      mMakeDiscoverable.SetOnClickListener(this);

      if (!mBluetoothAdapter.IsEnabled)
        mBluetoothOnOffSwitch.Checked = false;
      else
        mBluetoothOnOffSwitch.Checked = true;

      mBluetoothOnOffSwitch.CheckedChange += (s, b) =>
      {
        bool isChecked = b.IsChecked;
        if (isChecked)
          mBluetoothAdapter.Enable();
        else
          mBluetoothAdapter.Disable();
      };
    }

    /// <summary>
    /// Make sure the Android device has and supports Bluetooth device
    /// </summary>
    public void InitiliazeBluetooth()
    {
      if (mBluetoothAdapter == null)
      {
        new AlertDialog.Builder(Activity).SetTitle("Not compatible")
                                                 .SetMessage("Your phone does not support Bluetooth")
                                                 .SetPositiveButton("Exit", mActivity.ExitApplicationAction)
                                                 .SetIcon(Resource.Drawable.alert_113x113)
                                                 .Show();
      }
    }

    /// <summary>
    /// Display a list of the already paired Bluetooth devices
    /// </summary>
    public void ShowPairedDevices()
    {
      mListOfBondedDevices.Clear();
      foreach (BluetoothDevice bt in mBluetoothAdapter.BondedDevices)
        mListOfBondedDevices.Add(bt.Name + "\r\n" + bt.Address);
      ArrayAdapter adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleListItem1, mListOfBondedDevices);
      mListOfPairedDevices.SetAdapter(adapter);
      adapter.NotifyDataSetChanged();
    }

    /// <summary>
    /// Request permission from the user to make the device discoverable for other Bluetooth devices
    /// </summary>
    public void MakeDiscoverable()
    {
      if (mBluetoothAdapter.ScanMode != ScanMode.ConnectableDiscoverable)
      {
        Intent discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
        discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, 300);
        StartActivity(discoverableIntent);
      }
    }

    public void OnClick(View v)
    {
      switch (v.Id)
      {
        case Resource.Id.bluetooth_paired_devices_button:
        ShowPairedDevices();
        break;

        case Resource.Id.discoverable:
        MakeDiscoverable();
        break;

        case Resource.Id.connect_reader:
        mActivity.ReplaceFragment(MainActivity.mRDyCFragment);
        break;
      }
    }


    /// <summary>
    /// Implements keep alive thread
    /// Used for constantly checking if the user turns on or off the BT from outside the app e.g. the phone bar
    /// </summary>
    public class CheckBluetoothThread : Java.Lang.Thread
    {
      // Logging
      private static readonly String TAG = "CheckBluetoothThread";
      private bool mStopThread = false;

      public CheckBluetoothThread()
      {
        try
        {
          mStopThread = false;
        }
        catch (Exception e) { }
      }

      public void StopKeepAliveThread()
      {
        mStopThread = true;
      }

      public override void Run()
      {
        Log.Debug(TAG, "Start CheckBluetoothThread");
        Name = "KeepAliveThread";

        do
        {
          mActivity.CheckSnackbar();
          try
          {
            Sleep(300);
          }
          catch (Exception ignored) { }
        }
        while (!mStopThread);
      }
    }

  }
}
