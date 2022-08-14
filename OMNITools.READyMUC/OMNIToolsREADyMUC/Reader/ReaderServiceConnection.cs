using Android.Content;
using Android.OS;
using System;

namespace OMNIToolsREADyMUC.Reader
{
  public class ReaderServiceConnection : Java.Lang.Object, IServiceConnection
  {
    private ReaderService mService;
    public ReaderServiceBinder mBinder { get; private set; }

    /// <summary>
    ///  Called when a connection to the Service has been established
    /// </summary>
    /// <param name="name"> the concrete component name of the service that has been connected </param>
    /// <param name="service"> the IBinder of the Service's communication channel, which you can now make calls on </param>
    public void OnServiceConnected(ComponentName name, IBinder service)
    {
      ReaderServiceBinder serviceBinder = service as ReaderServiceBinder;
      if (serviceBinder != null)
        mBinder = (ReaderServiceBinder)service;
    }

    /// <summary>
    /// Called when a connection to the Service has been lost. This typically happens when the process hosting the service has crashed or been killed
    /// </summary>
    /// <param name="name"> the concrete component name of the service whose connection has been lost </param>
    public void OnServiceDisconnected(ComponentName name)
    {
      mService = null;
      ReaderManager.OnDisconnected();
    }

    /// <summary>
    ///  Called when the binding to this connection is dead
    ///  This may happen, for example, if the application hosting the service it is bound to has been updated
    /// </summary>
    /// <param name="name"> the concrete component name of the service whose connection is dead </param>
    public void OnBindingDied(ComponentName name)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}