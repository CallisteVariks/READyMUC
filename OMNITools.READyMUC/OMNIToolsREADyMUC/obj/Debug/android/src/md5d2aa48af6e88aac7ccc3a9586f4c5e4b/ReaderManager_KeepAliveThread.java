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


	public ReaderManager_KeepAliveThread (java.lang.Runnable p0)
	{
		super (p0);
		if (getClass () == ReaderManager_KeepAliveThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", "Java.Lang.IRunnable, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public ReaderManager_KeepAliveThread (java.lang.Runnable p0, java.lang.String p1)
	{
		super (p0, p1);
		if (getClass () == ReaderManager_KeepAliveThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", "Java.Lang.IRunnable, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
	}


	public ReaderManager_KeepAliveThread (java.lang.String p0)
	{
		super (p0);
		if (getClass () == ReaderManager_KeepAliveThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public ReaderManager_KeepAliveThread (java.lang.ThreadGroup p0, java.lang.Runnable p1)
	{
		super (p0, p1);
		if (getClass () == ReaderManager_KeepAliveThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public ReaderManager_KeepAliveThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2)
	{
		super (p0, p1, p2);
		if (getClass () == ReaderManager_KeepAliveThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ReaderManager_KeepAliveThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2, long p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == ReaderManager_KeepAliveThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android:System.String, mscorlib:System.Int64, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public ReaderManager_KeepAliveThread (java.lang.ThreadGroup p0, java.lang.String p1)
	{
		super (p0, p1);
		if (getClass () == ReaderManager_KeepAliveThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderManager+KeepAliveThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
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
