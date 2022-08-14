package md5aa5f2f9b62bf7cbfcee176e4fb592721;


public class BTManager_PendingConnectTask
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
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Connection.BTManager+PendingConnectTask, OMNIToolsREADyMUC", BTManager_PendingConnectTask.class, __md_methods);
	}


	public BTManager_PendingConnectTask ()
	{
		super ();
		if (getClass () == BTManager_PendingConnectTask.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Connection.BTManager+PendingConnectTask, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
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
