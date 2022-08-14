package md5d2aa48af6e88aac7ccc3a9586f4c5e4b;


public class ReaderManager_KeepAliveThread
	extends java.lang.Thread
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_run:()V:GetRunHandler\n" +
			"";
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", ReaderManager_KeepAliveThread.class, __md_methods);
	}


	public ReaderManager_KeepAliveThread ()
	{
		super ();
		if (getClass () == ReaderManager_KeepAliveThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
	}


	public void run ()
	{
		n_run ();
	}

	private native void n_run ();

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
