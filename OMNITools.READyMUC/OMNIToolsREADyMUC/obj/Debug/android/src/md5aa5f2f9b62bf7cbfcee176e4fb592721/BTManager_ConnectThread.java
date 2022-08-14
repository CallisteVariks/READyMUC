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


	public BTManager_ConnectThread (java.lang.Runnable p0)
	{
		super (p0);
		if (getClass () == BTManager_ConnectThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", "Java.Lang.IRunnable, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public BTManager_ConnectThread (java.lang.Runnable p0, java.lang.String p1)
	{
		super (p0, p1);
		if (getClass () == BTManager_ConnectThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", "Java.Lang.IRunnable, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
	}


	public BTManager_ConnectThread (java.lang.String p0)
	{
		super (p0);
		if (getClass () == BTManager_ConnectThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public BTManager_ConnectThread (java.lang.ThreadGroup p0, java.lang.Runnable p1)
	{
		super (p0, p1);
		if (getClass () == BTManager_ConnectThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public BTManager_ConnectThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2)
	{
		super (p0, p1, p2);
		if (getClass () == BTManager_ConnectThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public BTManager_ConnectThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2, long p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == BTManager_ConnectThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android:System.String, mscorlib:System.Int64, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public BTManager_ConnectThread (java.lang.ThreadGroup p0, java.lang.String p1)
	{
		super (p0, p1);
		if (getClass () == BTManager_ConnectThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+ConnectThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
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
