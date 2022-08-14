using Android.App;
using Android.Bluetooth;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace OMNIToolsREADyMUC.Fragments
{
  public class HomeFragment : Fragment, View.IOnClickListener
  {
    private BluetoothAdapter mBluetoothAdapter = SettingsFragment.mBluetoothAdapter;
    private static Button mMetersButton, mBroadcastButton, mRdycButton, mAboutButton;
    private readonly MainActivity mActivity;

    public HomeFragment(MainActivity activity)
    {
      mActivity = activity;
    }

    public override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      return inflater.Inflate(Resource.Layout.home_layout, container, false);
    }

    public override void OnViewCreated(View view, Bundle savedInstanceState)
    {
      mMetersButton = view.FindViewById<Button>(Resource.Id.scanMetersButton);
      mBroadcastButton = view.FindViewById<Button>(Resource.Id.broadcastButton);
      mRdycButton = view.FindViewById<Button>(Resource.Id.scanRdycButton);
      mAboutButton = view.FindViewById<Button>(Resource.Id.aboutButton);

      mMetersButton.SetOnClickListener(this);
      mBroadcastButton.SetOnClickListener(this);
      mRdycButton.SetOnClickListener(this);
      mAboutButton.SetOnClickListener(this);

      if (!SettingsFragment.mCheckBluetoothThread.IsAlive)
        SettingsFragment.mCheckBluetoothThread.Start();
    }

    public void OnClick(View v)
    {
      switch (v.Id)
      {
        case Resource.Id.scanMetersButton:
        mActivity.ReplaceFragment(mActivity.mMetersFragment);
        break;

        case Resource.Id.broadcastButton:
        mActivity.ReplaceFragment(mActivity.mBroadcastFragment);
        break;

        case Resource.Id.scanRdycButton:
        mActivity.ReplaceFragment(MainActivity.mRDyCFragment);
        break;

        case Resource.Id.aboutButton:
        mActivity.ReplaceFragment(mActivity.mAboutFragment);
        break;
      }
    }
  }
}