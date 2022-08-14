using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using OMNIToolsREADyMUC.Connection;
using OMNIToolsREADyMUC.Fragments;
using OMNIToolsREADyMUC.Reader;
using System.Collections.Generic;
using static OMNIToolsREADyMUC.Reader.ReaderService;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace OMNIToolsREADyMUC
{
  [Activity(Label = "@string/title_app_name", MainLauncher = true, Icon = "@drawable/READyMUC_icon_green", Theme = "@style/MyTheme",
    ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
  public class MainActivity : AppCompatActivity
  {
    public CoordinatorLayout mRootLayout;
    private SupportToolbar mToolbar;
    private View mNavHeader;
    private DrawerLayout mDrawerLayout;
    private ActionBarDrawerToggle mDrawerToggle;
    private NavigationView mNavigationView;
    private ImageView mUserImage, mBackgroundNavDrawer;
    private TextView mUserName, mUserWebsite;
    private static TextView mActionText;
    public static Fragment mCurrentFragment = new Fragment();
    public Fragment mHomeFragment, mMetersFragment, mAboutFragment, mSettingsFragment, mBroadcastFragment;
    public static Fragment mRDyCFragment;
    public static RDyCFragment RDyC;
    private Stack<Fragment> mStackFragment;
    private FragmentTransaction mFragmentTransaction;
    public Snackbar mSnackbar;

    // Bluetooth
    public BluetoothAdapter mBluetoothAdapter;
    private SettingsFragment mSettings;
    private RDyCFragment mRrdyc;
    private const int REQUEST_ENABLE_BT = 2;
    public static BTManager mBluetoothManager;
    public ReaderManager mReaderManager;
    private DiscoverableModeReceiver mReceiver;

    protected override void OnDestroy()
    {
      base.OnDestroy();
      SettingsFragment.mCheckBluetoothThread.Stop();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      // Set our view from the "main" layout resource
      SetContentView(Resource.Layout.main_layout);
      mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

      mStackFragment = new Stack<Fragment>();
      mSettings = new SettingsFragment(this);
      mSettings.InitiliazeBluetooth();
      InitializeFragments();
      InitializeComponents();


      if (mReceiver == null)
      {
        mReceiver = new DiscoverableModeReceiver(this);

        // Register for broadcasts when a device is discovered
        IntentFilter filter = new IntentFilter(BluetoothDevice.ActionFound);
        RegisterReceiver(mReceiver, filter);

        // Register for broadcasts when discovery has finished
        filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
        RegisterReceiver(mReceiver, filter);
      }


      mBluetoothAdapter = SettingsFragment.mBluetoothAdapter;
      mRrdyc = new RDyCFragment(this);
      mReaderManager = new ReaderManager(Application.Context);
      InitializeManager();

      ReaderService service = mReaderManager.mReaderServiceBinder.GetService();
      ConnectionHandler handler = new ConnectionHandler(service);
      service.mHandler = handler;
      service.mLocaleBroadcastReceiver = new LocaleBroadcastReceiver(service);

      RegisterReceiver(service.mLocaleBroadcastReceiver, new IntentFilter(Intent.ActionLocaleChanged));

      mBluetoothManager = new BTManager(handler);

      service.mManager = mBluetoothManager;
    }

    public void Stop(Context context)
    {
      mBluetoothManager.DisconnectAll();
      if (mReceiver != null)
      {
        UnregisterReceiver(mReceiver);
        mReceiver = null;
      }
    }

    public void InitializeManager()
    {
      mReaderManager.Initialize(null);
    }

    public ReaderManager GetReaderManager()
    {
      return mReaderManager;
    }

    public BTManager GetBTManager()
    {
      return mBluetoothManager;
    }

    public void ResetReaderManager()
    {
      // Disconnect Bluetooth and unregister readerservice listener
      mReaderManager.ShutDown();
    }

    private void InitializeFragments()
    {
      mHomeFragment = new HomeFragment(this);
      mMetersFragment = new MetersFragment(this);
      mAboutFragment = new AboutFragment(this);
      mSettingsFragment = new SettingsFragment(this);
      mBroadcastFragment = new BroadcastFragment(this);
      mRDyCFragment = new RDyCFragment(this);
      RDyC = new RDyCFragment(this);
    }

    private void InitializeComponents()
    {
      // Set toolbar
      mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
      SetSupportActionBar(mToolbar);
      SupportActionBar.SetTitle(Resource.String.title_app_name);
      SupportActionBar.SetSubtitle(Resource.String.subtitle_app_name);
      SupportActionBar.SetIcon(Resource.Drawable.READyMUC_icon_white_32x41);
      SupportActionBar.SetDisplayHomeAsUpEnabled(true);
      SupportActionBar.SetDisplayShowHomeEnabled(true);
      SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

      // Attach item selected handler to navigation view
      mNavigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
      mNavigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

      // Create ActionBarDrawerToggel button and add it to the toolbar
      mDrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, mToolbar, Resource.String.open_drawer, Resource.String.close_drawer);
      mDrawerLayout.SetDrawerListener(mDrawerToggle);
      mDrawerToggle.SyncState();

      // Load default home screen
      mFragmentTransaction = FragmentManager.BeginTransaction();
      mFragmentTransaction.Add(Resource.Id.MainFrameLayout, mHomeFragment, "Home");
      mFragmentTransaction.Commit();
      mCurrentFragment = mHomeFragment;

      // Navigation Header
      mNavHeader = mNavigationView.GetHeaderView(0);
      mUserName = FindViewById<TextView>(Resource.Id.user_name);
      mUserWebsite = FindViewById<TextView>(Resource.Id.website);
      mUserImage = FindViewById<ImageView>(Resource.Id.user_image);
      mBackgroundNavDrawer = FindViewById<ImageView>(Resource.Id.header_image);

      // Create snackbar for Bluetooth enabling
      mRootLayout = FindViewById<CoordinatorLayout>(Resource.Id.root_layout);
      mActionText = FindViewById<TextView>(Resource.Id.action_text);
      mSnackbar = Snackbar.Make(mRootLayout, "This application requires to enable bluetooth to connect with READy Converter", Snackbar.LengthIndefinite)
                         .SetAction("Enable", v => RequestEnableBluetooth());
      View snackbarView = mSnackbar.View;
      TextView textView = (TextView)snackbarView.FindViewById(Resource.Id.snackbar_text);
      textView.SetMaxLines(5);
    }


    #region Bluetooth
    /// <summary>
    /// Make sure the user knows the app needs to have Bluetooth
    /// If the user dismissed the snackbar, an AlertDialog will be prompted
    /// </summary>
    public void CheckSnackbar()
    {
      if (!mBluetoothAdapter.IsEnabled)
        mSnackbar.Show();
      else if (mSnackbar != null && mSnackbar.IsShown && mBluetoothAdapter.IsEnabled)
        mSnackbar.Dismiss();
      else
        return;
    }

    /// <summary>
    /// An AlertDialog that is used across the fragments, whether the user dismissed the Snackbar or selected "No" when requesting to enable Bluetooth
    /// </summary>
    public void ShowAlertDialog()
    {
      new Android.App.AlertDialog.Builder(this).SetTitle("This app requires Bluetooth")
                                               .SetMessage("To procced using this application, you need to enable Bluetooth")
                                               .SetPositiveButton("Enable", RequestEnableBluetoothAction)
                                               .SetNegativeButton("Exit", ExitApplicationAction)
                                               .SetIcon(Resource.Drawable.alert_113x113)
                                               .Show();
    }

    /// <summary>
    /// This method is used as an Action for the AlertDialog
    /// If the user presses the exit button, the application will be terminated
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void ExitApplicationAction(object sender, DialogClickEventArgs e)
    {
      RunOnUiThread(() => { Process.KillProcess(Process.MyPid()); });
    }

    /// <summary>
    /// Ask the user for the permission of enabling Bluetooth
    /// </summary>
    public void RequestEnableBluetooth()
    {
      if (!SettingsFragment.mBluetoothAdapter.IsEnabled)
      {
        Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
        base.StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
      }
    }

    /// <summary>
    /// Ask the user for the permission of enabling Bluetooth
    /// This method is used as an Action for the AlertDialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void RequestEnableBluetoothAction(object sender, DialogClickEventArgs e)
    {
      if (!SettingsFragment.mBluetoothAdapter.IsEnabled)
      {
        Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
        base.StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
      }
    }

    /// <summary>
    /// When requesting to enable Bluetooth, checks is the user selected "No"
    /// If "No", then the AlertDialog will be prompted
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="resultCode"></param>
    /// <param name="data"></param>
    protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
    {
      switch (requestCode)
      {
        case (REQUEST_ENABLE_BT):
        if (resultCode == Result.Canceled)
          ShowAlertDialog();

        if (resultCode == Result.Ok)
        {
          // To turn off the switch in the settings fragment 
          if (mCurrentFragment == mSettingsFragment)
            SettingsFragment.mBluetoothOnOffSwitch.Checked = true;
        }
        break;

        default:
        if (mCurrentFragment == mRDyCFragment)
          RDyCFragment.mEmptyView.SetText(Resource.String.enable_bluetooth);
        break;
      }
    }
    #endregion


    #region Fragments
    // Define action for navigation menu selection
    private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
    {
      switch (e.MenuItem.ItemId)
      {
        case Resource.Id.nav_home:
        ReplaceFragment(mHomeFragment);
        break;

        case Resource.Id.nav_meters:
        ReplaceFragment(mMetersFragment);
        break;

        case Resource.Id.nav_settings:
        ReplaceFragment(mSettingsFragment);
        break;

        case Resource.Id.nav_broadcast:
        ReplaceFragment(mBroadcastFragment);
        break;

        case Resource.Id.nav_about:
        ReplaceFragment(mAboutFragment);
        break;
      }
      // Close drawer
      mDrawerLayout.CloseDrawers();
    }

    public void ShowFragment(Fragment fragment)
    {
      if (mCurrentFragment == fragment && fragment.IsVisible)
        return;

      FragmentTransaction trans = FragmentManager.BeginTransaction();
      trans.Hide(mCurrentFragment);
      trans.Show(fragment);
      trans.AddToBackStack(null);
      trans.Commit();

      mStackFragment.Push(mCurrentFragment);
      mCurrentFragment = fragment;
    }

    public void ReplaceFragment(Fragment fragment)
    {
      if (fragment.IsVisible)
        return;

      FragmentTransaction trans = FragmentManager.BeginTransaction();
      trans.Replace(Resource.Id.MainFrameLayout, fragment);
      trans.AddToBackStack(null);
      trans.Commit();

      mCurrentFragment = fragment;
    }

    /// <summary>
    /// If the current fragment is not the HomeFragment, replace it and show home
    /// If the current fragment is the HomeFragment, exit
    /// </summary>
    public override void OnBackPressed()
    {
      if (mCurrentFragment == mHomeFragment)
        RunOnUiThread(() => { Process.KillProcess(Process.MyPid()); });
      else
        ReplaceFragment(mHomeFragment);
    }
    #endregion


    #region Toolbar Menu
    public override bool OnCreateOptionsMenu(IMenu menu)
    {
      MenuInflater.Inflate(Resource.Menu.toolbar_menu, menu);
      return base.OnCreateOptionsMenu(menu);
    }

    public override bool OnOptionsItemSelected(IMenuItem item)
    {
      switch (item.ItemId)
      {
        case Android.Resource.Id.Home:
        mDrawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
        mDrawerToggle.OnOptionsItemSelected(item);
        return true;

        case Resource.Id.action_search:
        return true;

        case Resource.Id.action_setupFrequency:
        return true;
      }
      return base.OnOptionsItemSelected(item);
    }

    public override bool OnPrepareOptionsMenu(IMenu menu)
    {
      if (mCurrentFragment == mRrdyc)
      {
        IMenuItem scan = menu.FindItem(Resource.Id.start_scan_menu);
        IMenuItem progress = menu.FindItem(Resource.Id.miActionProgress);

        scan.SetVisible(!RDyCFragment.mScanInProgress);
        progress.SetVisible(!RDyCFragment.mScanInProgress);
      }
      return base.OnPrepareOptionsMenu(menu);
    }
    #endregion
  }
}

