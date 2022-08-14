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


	public SettingsFragment_CheckBluetoothThread (java.lang.Runnable p0)
	{
		super (p0);
		if (getClass () == SettingsFragment_CheckBluetoothThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", "Java.Lang.IRunnable, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public SettingsFragment_CheckBluetoothThread (java.lang.Runnable p0, java.lang.String p1)
	{
		super (p0, p1);
		if (getClass () == SettingsFragment_CheckBluetoothThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", "Java.Lang.IRunnable, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
	}


	public SettingsFragment_CheckBluetoothThread (java.lang.String p0)
	{
		super (p0);
		if (getClass () == SettingsFragment_CheckBluetoothThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", "System.String, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public SettingsFragment_CheckBluetoothThread (java.lang.ThreadGroup p0, java.lang.Runnable p1)
	{
		super (p0, p1);
		if (getClass () == SettingsFragment_CheckBluetoothThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public SettingsFragment_CheckBluetoothThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2)
	{
		super (p0, p1, p2);
		if (getClass () == SettingsFragment_CheckBluetoothThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public SettingsFragment_CheckBluetoothThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2, long p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == SettingsFragment_CheckBluetoothThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:Java.Lang.IRunnable, Mono.Android:System.String, mscorlib:System.Int64, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public SettingsFragment_CheckBluetoothThread (java.lang.ThreadGroup p0, java.lang.String p1)
	{
		super (p0, p1);
		if (getClass () == SettingsFragment_CheckBluetoothThread.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.SettingsFragment+CheckBluetoothThread, OMNIToolsREADyMUC", "Java.Lang.ThreadGroup, Mono.Android:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
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
