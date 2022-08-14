package md50e8b4526a703a27ec72d304d16ee9487;


public class SettingsFragment_CheckBluetoothThread
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
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", SettingsFragment_CheckBluetoothThread.class, __md_methods);
	}


	public SettingsFragment_CheckBluetoothThread ()
	{
		super ();
		if (getClass () == SettingsFragment_CheckBluetoothThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
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
