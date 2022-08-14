using Android.Content;
using Android.OS;
using System;

namespace OMNIToolsREADyMUC.Reader
{
  public class ReaderServiceBinder : Binder
  {
    public static ReaderService mReaderService;
    private bool mBound = false;
    private readonly ReaderServiceConnection mConnection = new ReaderServiceConnection();

    public ReaderServiceBinder(ReaderService service)
    {
      mReaderService = service;
    }

    public ReaderService Service { get; private set; }

    /// <summary>
    /// Initiate asynchronous bind of service
    /// </summary>
    /// <param Context="context"></param>
    public void Bind(Context context)
    {
      context.BindService(new Intent(context, typeof(ReaderService)), mConnection, Android.Content.Bind.AutoCreate);
      mBound = true;
    }

    /// <summary>
    /// Unbind previously bound service
    /// </summary>
    /// <param name="context"></param>
    public void Unbind(Context context)
    {
      if (mBound)
      {
        // Detach our existing connection
        context.UnbindService(mConnection);
        mBound = false;
      }
    }

    /// <summary>
    /// Get bound ReaderService
    /// </summary>
    /// <returns> bound ReaderService </returns>
    public ReaderService GetService()
    {
      ReaderService localReaderService = new ReaderService();
      mReaderService = localReaderService;
      return localReaderService.GetService();
    }

    /// <summary>
    /// Invoked when service is bound
    /// Override to handle event
    /// </summary>
    protected virtual void OnConnected()
    {
    }

    /// <summary>
    /// Invoked when service is no longer bound
    /// Override to handle event
    /// </summary>
    protected virtual void OnDisconnected()
    {
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}