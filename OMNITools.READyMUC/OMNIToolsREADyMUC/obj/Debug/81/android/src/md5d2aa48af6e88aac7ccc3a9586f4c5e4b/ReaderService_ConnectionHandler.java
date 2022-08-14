package md5d2aa48af6e88aac7ccc3a9586f4c5e4b;


public class ReaderService_ConnectionHandler
	extends android.os.Handler
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Reader.ReaderService+ConnectionHandler, OMNIToolsREADyMUC", ReaderService_ConnectionHandler.class, __md_methods);
	}


	public ReaderService_ConnectionHandler ()
	{
		super ();
		if (getClass () == ReaderService_ConnectionHandler.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderService+ConnectionHandler, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
	}


	public ReaderService_ConnectionHandler (android.os.Handler.Callback p0)
	{
		super (p0);
		if (getClass () == ReaderService_ConnectionHandler.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderService+ConnectionHandler, OMNIToolsREADyMUC", "Android.OS.Handler+ICallback, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public ReaderService_ConnectionHandler (android.os.Looper p0)
	{
		super (p0);
		if (getClass () == ReaderService_ConnectionHandler.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderService+ConnectionHandler, OMNIToolsREADyMUC", "Android.OS.Looper, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public ReaderService_ConnectionHandler (android.os.Looper p0, android.os.Handler.Callback p1)
	{
		super (p0, p1);
		if (getClass () == ReaderService_ConnectionHandler.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderService+ConnectionHandler, OMNIToolsREADyMUC", "Android.OS.Looper, Mono.Android:Android.OS.Handler+ICallback, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	public ReaderService_ConnectionHandler (md5d2aa48af6e88aac7ccc3a9586f4c5e4b.ReaderService p0)
	{
		super ();
		if (getClass () == ReaderService_ConnectionHandler.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderService+ConnectionHandler, OMNIToolsREADyMUC", "OMNIToolsREADyMUC.Reader.ReaderService, OMNIToolsREADyMUC", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
