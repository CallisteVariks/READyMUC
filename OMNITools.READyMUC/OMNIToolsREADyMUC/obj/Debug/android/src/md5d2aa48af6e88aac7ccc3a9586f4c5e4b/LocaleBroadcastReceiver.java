package md5d2aa48af6e88aac7ccc3a9586f4c5e4b;


public class LocaleBroadcastReceiver
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Reader.LocaleBroadcastReceiver, OMNIToolsREADyMUC", LocaleBroadcastReceiver.class, __md_methods);
	}


	public LocaleBroadcastReceiver ()
	{
		super ();
		if (getClass () == LocaleBroadcastReceiver.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.LocaleBroadcastReceiver, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
	}

	public LocaleBroadcastReceiver (md5d2aa48af6e88aac7ccc3a9586f4c5e4b.ReaderService p0)
	{
		super ();
		if (getClass () == LocaleBroadcastReceiver.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.LocaleBroadcastReceiver, OMNIToolsREADyMUC", "OMNIToolsREADyMUC.Reader.ReaderService, OMNIToolsREADyMUC", this, new java.lang.Object[] { p0 });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

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
