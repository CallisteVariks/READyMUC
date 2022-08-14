using Android.Content;
using Android.Util;
using Java.Util;
using System;

namespace OMNIToolsREADyMUC.Reader
{
  public class LocaleBroadcastReceiver : BroadcastReceiver
  {
    private static readonly String TAG = "LocaleBroadcastReceiver";

    private readonly ReaderService mReaderService = ReaderServiceBinder.mReaderService;

    public LocaleBroadcastReceiver(ReaderService readerService)
    {
      mReaderService = readerService;
    }

    public override void OnReceive(Context context, Intent intent)
    {
      if (intent.Action.CompareTo(Intent.ActionLocaleChanged) == 0)
        Log.Debug(TAG, "Locale changed to: " + Locale.Default.Country);
    }
  }
}