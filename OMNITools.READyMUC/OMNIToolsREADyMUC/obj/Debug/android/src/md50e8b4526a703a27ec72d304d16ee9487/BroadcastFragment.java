package md50e8b4526a703a27ec72d304d16ee9487;


public class BroadcastFragment
	extends android.app.Fragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Fragments.BroadcastFragment, OMNIToolsREADyMUC", BroadcastFragment.class, __md_methods);
	}


	public BroadcastFragment ()
	{
		super ();
		if (getClass () == BroadcastFragment.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.BroadcastFragment, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
	}

	public BroadcastFragment (md5483ff621e4fa927fac1e0bc968291b41.MainActivity p0)
	{
		super ();
		if (getClass () == BroadcastFragment.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Fragments.BroadcastFragment, OMNIToolsREADyMUC", "OMNIToolsREADyMUC.MainActivity, OMNIToolsREADyMUC", this, new java.lang.Object[] { p0 });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();

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
