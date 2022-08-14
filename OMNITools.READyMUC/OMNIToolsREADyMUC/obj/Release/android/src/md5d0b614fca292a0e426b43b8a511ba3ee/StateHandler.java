package md5d0b614fca292a0e426b43b8a511ba3ee;


public class StateHandler
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		java.lang.Runnable
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_run:()V:GetRunHandler:Java.Lang.IRunnableInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Common.State.StateHandler, OMNIToolsREADyMUC", StateHandler.class, __md_methods);
	}


	public StateHandler ()
	{
		super ();
		if (getClass () == StateHandler.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Common.State.StateHandler, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
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
