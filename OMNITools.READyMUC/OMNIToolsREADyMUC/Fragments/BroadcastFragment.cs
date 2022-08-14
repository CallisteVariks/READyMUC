using Android.App;
using Android.OS;
using Android.Views;

namespace OMNIToolsREADyMUC.Fragments
{
  public class BroadcastFragment : Fragment
  {
    public MainActivity mActivity;

    public BroadcastFragment(MainActivity activity)
    {
      mActivity = activity;
    }

    public override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      View view = inflater.Inflate(Resource.Layout.broadcast_layout, container, false);
      return view;
    }

    public override void OnResume()
    {
      base.OnResume();
      mActivity.CheckSnackbar();
    }
  }
}