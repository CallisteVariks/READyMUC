package md5d2aa48af6e88aac7ccc3a9586f4c5e4b;


public class ReaderServiceBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("OMNIToolsREADyMUC.Reader.ReaderServiceBinder, OMNIToolsREADyMUC", ReaderServiceBinder.class, __md_methods);
	}


	public ReaderServiceBinder ()
	{
		super ();
		if (getClass () == ReaderServiceBinder.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderServiceBinder, OMNIToolsREADyMUC", "", this, new java.lang.Object[] {  });
	}

	public ReaderServiceBinder (md5d2aa48af6e88aac7ccc3a9586f4c5e4b.ReaderService p0)
	{
		super ();
		if (getClass () == ReaderServiceBinder.class)
			mono.android.TypeManager.Activate ("OMNIToolsREADyMUC.Reader.ReaderServiceBinder, OMNIToolsREADyMUC", "OMNIToolsREADyMUC.Reader.ReaderService, OMNIToolsREADyMUC", this, new java.lang.Object[] { p0 });
	}

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
