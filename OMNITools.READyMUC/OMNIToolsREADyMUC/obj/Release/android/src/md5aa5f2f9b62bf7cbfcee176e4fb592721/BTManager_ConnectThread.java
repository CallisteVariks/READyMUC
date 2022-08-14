package md5aa5f2f9b62bf7cbfcee176e4fb592721;


public class BTManager_ConnectThread
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
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", BTManager_ConnectThread.class, __md_methods);
	}


	public BTManager_ConnectThread ()
	{
		super ();
		if (getClass () == BTManager_ConnectThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
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
