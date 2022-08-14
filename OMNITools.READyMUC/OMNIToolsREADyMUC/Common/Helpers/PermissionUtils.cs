using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using System;
using System.Linq;
using AndroidResource = Android.Resource;
using AppResource = OMNIToolsREADyMUC.Resource;

namespace OMNIToolsREADyMUC.Common.Helpers
{
  public static class PermissionUtils
  {
    private static int MY_PERMISSIONS_REQUEST_ACCESS_FINE_LOCATION = 35;
    public const int RC_LOCATION_PERMISSIONS = 1000;
    public static readonly string[] LOCATION_PERMISSIONS = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };

    public static void RequestPermissionsForApp(this Android.App.Fragment frag)
    {
      bool showRequestRationale = ActivityCompat.ShouldShowRequestPermissionRationale(frag.Activity, Manifest.Permission.AccessFineLocation) ||
                                  ActivityCompat.ShouldShowRequestPermissionRationale(frag.Activity, Manifest.Permission.AccessCoarseLocation);

      if (showRequestRationale)
      {
        Android.Views.View rootView = frag.Activity.FindViewById(AndroidResource.Id.Content);
        Snackbar.Make(rootView, AppResource.String.request_location_permissions, Snackbar.LengthIndefinite)
                .SetAction(AppResource.String.ok, v =>
                {
                  frag.RequestPermissions(LOCATION_PERMISSIONS, RC_LOCATION_PERMISSIONS);
                })
                .Show();
      }
      else
        frag.RequestPermissions(LOCATION_PERMISSIONS, RC_LOCATION_PERMISSIONS);
    }

    public static bool AllPermissionsGranted(this Permission[] grantResults)
    {
      if (grantResults.Length < 1)
        return false;

      return !grantResults.Any(result => result == Permission.Denied);
    }

    public static bool HasLocationPermissions(this Context context)
    {
      foreach (String perm in LOCATION_PERMISSIONS)
      {
        if (ContextCompat.CheckSelfPermission(context, perm) != Permission.Granted)
          return false;
      }
      return true;
    }

    public static bool RequestAccessFineLocationPermission(Activity act)
    {
      return RequestPermission(act, Manifest.Permission.AccessFineLocation, MY_PERMISSIONS_REQUEST_ACCESS_FINE_LOCATION);
    }

    private static bool RequestPermission(Activity act, String permission, int requestCode)
    {
      // Check Android 6 permission
      if (ContextCompat.CheckSelfPermission(act, permission) == Permission.Granted)
        return true;
      else
      {
        ActivityCompat.RequestPermissions(act, new String[] { permission }, requestCode);
        return false;
      }
    }



  }
}
