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
