using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using OMNIToolsREADyMUC.Common.Enums;
using OMNIToolsREADyMUC.Common.Helpers;
using OMNIToolsREADyMUC.Connection;
using OMNIToolsREADyMUC.KMRF;
using OMNIToolsREADyMUC.Reader;
using System;
using System.Collections.Generic;

namespace OMNIToolsREADyMUC.Fragments
{
  public class RDyCFragment : Fragment, IReaderDeviceConnectionListener, IReaderDeviceDiscoveryListener
  {
    private static readonly String TAG = "RDyCFragment";
    private readonly String CONNECT_TO_DEVICE_DIALOG_DEVICE_TYPE = "CONNECT_TO_DEVICE_DIALOG_DEVICE_TYPE";
    public static String EXTRA_DEVICE_ADDRESS = "deviceAddress";

    public MainActivity mActivity;
    private BluetoothAdapter mBluetoothAdapter = SettingsFragment.mBluetoothAdapter;
    public static BTManager mBluetoothManager;

    public static TextView mEmptyView;
    private FloatingActionButton mFabScan;

    public static ReaderDevicesAdapter readerDevicesAdapter;
    public static ListView mListView;
    public static List<ReaderDevice> devicesList = new List<ReaderDevice>();
    public static ReaderDevice device;

    public static bool mScanInProgress = false;
    public bool mScanInProgressWhilePaused = false;

    private ProgressDialog mProgressDialog;
    private ConnectDeviceTypeEnum mDeviceType;
    private ReaderDevice mPendingConnectDevice = null;
    private String mConnectToDeviceDialogDeviceType = "";
    private ReaderServiceConnection mReaderServiceConnection;
    public static UpdateUIThread mUpdateUIThread;

    public RDyCFragment() { }

    public RDyCFragment(MainActivity activity)
    {
      mActivity = activity;
      mBluetoothManager = mActivity.GetBTManager();
      mUpdateUIThread = new UpdateUIThread();
    }

    public override void OnStart()
    {
      base.OnStart();

      Intent ServiceIntent = new Intent("Service");
      mReaderServiceConnection = new ReaderServiceConnection();
    //  Context.BindService(ServiceIntent, mReaderServiceConnection, Bind.AutoCreate);
    }

    public override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      SetHasOptionsMenu(true);

      if (savedInstanceState != null)
        mConnectToDeviceDialogDeviceType = savedInstanceState.GetString(CONNECT_TO_DEVICE_DIALOG_DEVICE_TYPE);
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.activity_device_list, container, false);
      mEmptyView = view.FindViewById<TextView>(Resource.Id.Empty);

      // List        
      readerDevicesAdapter = new ReaderDevicesAdapter(mActivity, devicesList);
      mListView = view.FindViewById<ListView>(Resource.Id.readerDevicesListView);
      mListView.Adapter = readerDevicesAdapter;
      mListView.ItemClick += mListView_ItemClick;
      if (!readerDevicesAdapter.IsEmpty)
      {
        readerDevicesAdapter.Clear();
        devicesList.Clear();
      }

      // Initialize FABs
      mFabScan = view.FindViewById<FloatingActionButton>(Resource.Id.fab_scan);

      mFabScan.Click += (o, e) =>
      {
        if (!mBluetoothAdapter.IsEnabled)
          mActivity.ShowAlertDialog();
        else
        {
          devicesList.Clear();
          readerDevicesAdapter.Clear();
          mEmptyView.SetText(Resource.String.scanning);
          mBluetoothAdapter.CancelDiscovery();
          StartScan(true);

          if (!ReaderManager.mKeepAliveThread.IsAlive)
            ReaderManager.mKeepAliveThread.Start();
        }
      };

      return view;
    }

    public override void OnSaveInstanceState(Bundle outState)
    {
      base.OnSaveInstanceState(outState);

      outState.PutString(CONNECT_TO_DEVICE_DIALOG_DEVICE_TYPE, mConnectToDeviceDialogDeviceType);
    }

    public override void OnActivityCreated(Bundle savedInstanceState)
    {
      base.OnActivityCreated(savedInstanceState);

      // Which DeviceType are we connecting to?
      Intent i = Activity.Intent;
      String connectDeviceType = i.GetStringExtra(DeviceConfig.CONNECT_DEVICE_TYPE);

      if (connectDeviceType != null && connectDeviceType.Equals(DeviceConfig.CONNECT_DEVICE_TYPE_RDyC))
        SetConnectDeviceType(ConnectDeviceTypeEnum.RDyC);
      else
        SetConnectDeviceType(ConnectDeviceTypeEnum.Unknown);
    }

    public void SetConnectDeviceType(ConnectDeviceTypeEnum cdt)
    {
      if (mDeviceType != cdt)
      {
        readerDevicesAdapter.Clear(); // If ConnectDeviceType changes then clear Devices
        readerDevicesAdapter.NotifyDataSetChanged();
      }

      mDeviceType = cdt;
    }

    public override void OnResume()
    {
      base.OnResume();

      if (mActivity.GetReaderManager() != null)
      {
        mActivity.GetReaderManager().RegisterConnectionListener(this);
        mActivity.GetReaderManager().RegisterDeviceDiscoveryListener(this);
        readerDevicesAdapter.NotifyDataSetChanged();
        Activity.InvalidateOptionsMenu();

        if ((mProgressDialog != null) && (!mActivity.GetReaderManager().ConnectingToDevice()))
        {
          IfDialogActiveThenRemoveIt();
          readerDevicesAdapter.NotifyDataSetChanged();
        }
        else if (mActivity.GetReaderManager().ConnectingToDevice())
        {
          mConnectToDeviceDialogDeviceType = !mConnectToDeviceDialogDeviceType.Equals("") ? mConnectToDeviceDialogDeviceType : GetString(Resource.String.device);
          ShowConnectToDeviceDialog(mConnectToDeviceDialogDeviceType);
        }
      }
    }

    public override void OnPause()
    {
      mBluetoothAdapter.CancelDiscovery();
      mScanInProgressWhilePaused = mScanInProgress;
      mScanInProgress = false;

      if (mActivity.GetReaderManager() != null)
      {
        mActivity.GetReaderManager().UnregisterConnectionListener(this);
        mActivity.GetReaderManager().UnregisterDeviceDiscoveryListener(this);
      }

      IfDialogActiveThenRemoveIt();

      base.OnPause();
    }

    public override void OnDestroy()
    {
      if (mActivity.GetReaderManager() != null)
        mActivity.GetReaderManager().CancelScan();

      base.OnDestroy();
    }

    public void mListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
    {
      ListView listView = sender as ListView;
      device = devicesList[e.Position];

      // Only handle click if for allowed device type
      if (device.DeviceType != ConnectDeviceTypeEnum.Unknown)
      {
        mPendingConnectDevice = null;

        // Cancel scan if in progress
        if (mScanInProgress)
          mActivity.GetReaderManager().CancelScan();

        if (mActivity.GetReaderManager().IsConnected())
        {
          // Check if we are connecting to another device than the currently connected one
          if (device.IsConnected())
            mActivity.GetReaderManager().Disconnect(device);
          else
          {
            // Check if we have reached limit of connected devices, if so show warning
            if (ReaderManager.GetConnectedDevices().Count >= DeviceConfig.MAX_NUMBER_OF_CONNECTIONS)
            {
              ShowToast(Resource.String.bluetooth_max_number_of_connections_reached, false);
              return;
            }
            ConnectToDevice(device);
          }
          return;
        }

        if (!mScanInProgress)
          ConnectToDevice(device);
        else
          mPendingConnectDevice = device;
      }
    }

    public static ReaderDevice GetDeviceFromList => device;

    private void ConnectToDevice(ReaderDevice device)
    {
      if (device != null)
      {
        mActivity.GetReaderManager().Connect(device);
        ShowConnectToDeviceDialog(device);
      }
    }

    private void ShowConnectToDeviceDialog(ReaderDevice device)
    {
      ShowConnectToDeviceDialog(device.GetDeviceTypeString(Context));
    }

    private void ShowConnectToDeviceDialog(String deviceType)
    {
      mConnectToDeviceDialogDeviceType = deviceType;

      mProgressDialog = new ProgressDialog(mActivity);
      mProgressDialog.SetTitle(String.Format(GetString(Resource.String.connecting_to_device) + " " + deviceType));
      mProgressDialog.SetMessage(GetString(Resource.String.please_wait));
      mProgressDialog.SetCancelable(true);
      mProgressDialog.Indeterminate = true;
      mProgressDialog.Show();

      mProgressDialog.DismissEvent += (object sender, EventArgs e) => { mActivity.GetBTManager().DisconnectAll(); ShowToast(Resource.String.connection_cancelled, true); };
    }

    private void AddNewListDevice(ReaderDevice device)
    {
      if (!BluetoothConfig.BT_SHOW_UNKNOWN_DEVICES && device.DeviceType != ConnectDeviceTypeEnum.RDyC)
        return;

      // Filter on request ConnectDeviceType
      if (mDeviceType != ConnectDeviceTypeEnum.Unknown && mDeviceType != device.DeviceType)
        return;

      // Check if it is already added
      int count = mListView.Count;
      for (int i = 0; i < count; i++)
      {
        String address = readerDevicesAdapter[i].Address;
        if (address.Equals(device.Address))
          return;
      }

      // Find insertion point
      for (int i = 0; i < count; i++)
      {
        if (device.GetOrder() < readerDevicesAdapter[i].GetOrder())
        {
          devicesList.Insert(i, device);
          return;
        }
      }
      // Add to end
      devicesList.Add(device);
    }

    private void ReplaceListDevice(ReaderDevice device)
    {
      // Check if it is already added
      int count = readerDevicesAdapter.Count;
      for (int i = 0; i < count; i++)
      {
        String address = readerDevicesAdapter[i].Address;
        if (address.Equals(device.Address))
        {
          devicesList.RemoveAt(i);
          devicesList.Insert(i, device);
          return;
        }
      }
    }

    public void StartScan(bool clearList)
    {
      if (mEmptyView == null)
        mEmptyView = View.FindViewById<TextView>(Resource.Id.Empty);

      ReaderManager rm = mActivity.GetReaderManager();
      if (!mScanInProgress) // Only start scan if it's not already in progress
      {
        if (rm != null && mBluetoothAdapter.IsEnabled && PermissionUtils.RequestAccessFineLocationPermission(mActivity))
        {
          // Update UI
          mEmptyView.SetText(Resource.String.searching_please_wait);
          mActivity.SetProgressBarIndeterminateVisibility(true);
          mActivity.SetTitle(Resource.String.scanning);
          mScanInProgress = true;

          if (readerDevicesAdapter != null)
          {
            if (clearList)
              readerDevicesAdapter.Clear();

            if (rm.IsConnected())
              AddLatestDevice();
          }

          // Request scan from service
          mActivity.InvalidateOptionsMenu();
          //rm.Scan();
          mBluetoothAdapter.StartDiscovery();
        }
      }
    }

    public void StopScan()
    {
      // Cancel scan if it is in progress
      if (mScanInProgress)
      {
        Activity.SetProgressBarIndeterminateVisibility(false);
        mScanInProgress = false;
        mActivity.GetReaderManager().CancelScan();
      }
    }

    private void AddLatestDevice()
    {
      // Add last connected device - if any
      List<ReaderDevice> last = ReaderManager.GetConnectedDevices();
      foreach (ReaderDevice device in last)
        AddNewListDevice(device);

      // Request scan from service
      mActivity.GetReaderManager().Scan();
    }

    private String GetDeviceTypeSelectString(ConnectDeviceTypeEnum deviceType)
    {
      if (deviceType == ConnectDeviceTypeEnum.RDyC)
        return GetString(Resource.String.select_device);
      else
        return GetString(Resource.String.select_device_reader_device);
    }

    private void IfDialogActiveThenRemoveIt()
    {
      DismissDialog(mProgressDialog);
      mProgressDialog = null;
    }

    public static void DismissDialog(ProgressDialog dialog)
    {
      try
      {
        if (dialog != null && dialog.IsShowing)
          dialog.Dismiss();
      }
      catch (Exception e)
      {
        Log.Debug(TAG, "Error cleaning up dialog", e);
      }
    }


    #region IReaderDeviceConnectionListener
    public void OnConnectedAndReady(ReaderDevice device)
    {
      IfDialogActiveThenRemoveIt();

      // Update UI and show connected devices
      String msg = String.Format(GetString(Resource.String.connected_message) + " " + device.GetDeviceTypeString(Context));
      ShowAlertDialog("Connected and ready!", msg, Resource.Drawable.connection_ready);

      // Make sure it is in the list
      AddLatestDevice();
      readerDevicesAdapter.NotifyDataSetChanged();

      // If called from another activity we need to return the result and close
      Activity.SetResult(Result.Ok);
      Activity.Finish();
    }

    public void OnConnectionStatusChanged(ReaderDevice device, bool connected)
    {
      if (connected)
        IfDialogActiveThenRemoveIt();
      else
      {
        // Only to be called called on disconnect
        ReplaceListDevice(device);
        readerDevicesAdapter.NotifyDataSetChanged();

        if (mPendingConnectDevice != null)
        {
          ConnectToDevice(mPendingConnectDevice);
          mPendingConnectDevice = null;
        }
      }
    }

    public void OnConnectFailed(ReaderDevice device)
    {
      IfDialogActiveThenRemoveIt();
      ShowAlertDialog("Connection failed!", "READy MUC App was unable to connect to " + device.Name, Resource.Drawable.connection_failed);
    }

    public void OnConnectionLost(ReaderDevice device)
    {
      ReplaceListDevice(device);
      IfDialogActiveThenRemoveIt();
      ShowAlertDialog("Connection lost!", "READy MUC App was unable to create a connection with " + device.Name, Resource.Drawable.connection_lost);
    }

    /// <summary>
    /// Method used to show an AlertDialog in Fragment
    /// </summary>
    /// <param name="title"> title for AlertDialog </param>
    /// <param name="message"> message displayed </param>
    public void ShowAlertDialog(String title, String message, int icon)
    {
      Handler mainHandler = new Handler(Looper.MainLooper);
      Java.Lang.Runnable runnableAlertDialog = new Java.Lang.Runnable(() =>
      {
        new AlertDialog.Builder(this.Context).SetTitle(title)
                                                 .SetMessage(message)
                                                 .SetNeutralButton("OK", (c, e) => { })
                                                 .SetIcon(icon)
                                                 .Show();
      });
      mainHandler.Post(runnableAlertDialog);
    }

    /// <summary>
    /// Method used to show a Toast in Fragment
    /// </summary>
    /// <param name="text"> the text for the Toast </param>
    /// <param name="IsLengthShort"> true if short, false if long </param>
    public void ShowToast(int text, bool IsLengthShort = false)
    {
      Handler mainHandler = new Handler(Looper.MainLooper);
      Java.Lang.Runnable runnableToast = new Java.Lang.Runnable(() =>
      {
        ToastLength duration = IsLengthShort ? ToastLength.Short : ToastLength.Long;
        Toast.MakeText(this.Context, text, duration).Show();
      });
      mainHandler.Post(runnableToast);
    }

    public void OnMBusDataStarted()
    {
      throw new NotImplementedException();
    }

    public void OnMBusDataStopped()
    {
      throw new NotImplementedException();
    }

    public void OnReconnectSequenceStarted(ReaderDevice device)
    {
      throw new NotImplementedException();
    }

    public void OnKmRfDataReceived(ReaderDevice device, byte[] rawData, KmRfCommandResponse kmRfResponse)
    {
      throw new NotImplementedException();
    }
    #endregion


    #region IReaderDeviceDiscoveryListener
    public void OnDeviceDiscovered(ReaderDevice device)
    {
      AddNewListDevice(device);
    }

    public void OnDiscoveryFinished()
    {
      mScanInProgress = false;
      Activity.SetProgressBarIndeterminateVisibility(false);
      Activity.InvalidateOptionsMenu();

      if (readerDevicesAdapter.IsEmpty)
        mEmptyView.SetText(Resource.String.no_devices_found);
      else
      {
        ConnectToDevice(mPendingConnectDevice);
        mPendingConnectDevice = null;
      }
    }
    #endregion


    /// <summary>
    /// Implements keep alive thread
    /// Used for constantly checking if the DiscoverableModeReceiver has found any devcices that match the requirements (an RDyC)
    /// </summary>
    public class UpdateUIThread : Java.Lang.Thread
    {
      // Logging
      private static readonly String TAG = "UpdateUIThread";
      public static bool mUpdateUIThread = false;

      public UpdateUIThread()
      {
        try
        {
          mUpdateUIThread = false;
        }
        catch (Exception e) { }
      }

      public void StopUpdateUIThread()
      {
        mUpdateUIThread = true;
      }

      public override void Run()
      {
        Log.Debug(TAG, "Start UpdateUIThread");
        Name = "UpdateUIThread";

        do
        {
          // Only do UpdateUI when searching for BT devices
          if (DiscoverableModeReceiver.newReaderDevice != null)
            if (!devicesList.Contains(DiscoverableModeReceiver.newReaderDevice))
              devicesList.Add(DiscoverableModeReceiver.newReaderDevice);
          try
          {
            Sleep(300);
          }
          catch (Exception ignored) { }
        }
        while (!mUpdateUIThread);
      }
    }
  }
}