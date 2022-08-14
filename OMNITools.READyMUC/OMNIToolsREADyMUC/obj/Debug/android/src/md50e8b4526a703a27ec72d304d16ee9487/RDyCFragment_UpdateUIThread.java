package md50e8b4526a703a27ec72d304d16ee9487;


public class RDyCFragment_UpdateUIThread
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
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", RDyCFragment_UpdateUIThread.class, __md_methods);
	}


	public RDyCFragment_UpdateUIThread ()
	{
		super ();
		if (getClass () == RDyCFragment_UpdateUIThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
	}


	public RDyCFragment_UpdateUIThread (java.lang.Runnable p0)
	{
		super (p0);
		if (getClass () == RDyCFragment_UpdateUIThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", "Java.Lang.IRunnable, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public RDyCFragment_UpdateUIThread (java.lang.Runnable p0, java.lang.String p1)
	{
		super (p0, p1);
		if (getClass () == RDyCFragment_UpdateUIThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", "Java.Lang.IRunnable, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
	}


	public RDyCFragment_UpdateUIThread (java.lang.String p0)
	{
		super (p0);
		if (getClass () == RDyCFragment_UpdateUIThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public RDyCFragment_UpdateUIThread (java.lang.ThreadGroup p0, java.lang.Runnable p1)
	{
		super (p0, p1);
		if (getClass () == RDyCFragment_UpdateUIThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public RDyCFragment_UpdateUIThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2)
	{
		super (p0, p1, p2);
		if (getClass () == RDyCFragment_UpdateUIThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public RDyCFragment_UpdateUIThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2, long p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == RDyCFragment_UpdateUIThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android:System.String, mscorlib:System.Int64, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public RDyCFragment_UpdateUIThread (java.lang.ThreadGroup p0, java.lang.String p1)
	{
		super (p0, p1);
		if (getClass () == RDyCFragment_UpdateUIThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.RDyCFragment+UpdateUIThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
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
