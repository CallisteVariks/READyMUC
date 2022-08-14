using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace OMNIToolsREADyMUC.Reader
{
  /// <summary>
  /// Own adapter with the device_name view
  /// It acts as a row for the ListView from the activity_device_list layout
  /// </summary>
  public class ReaderDevicesAdapter : BaseAdapter<ReaderDevice>
  {
    public List<ReaderDevice> mItems;
    private Context mContext;
    private ImageView mImageConnected;

    public ReaderDevicesAdapter(Context context, List<ReaderDevice> items) : base()
    {
      mContext = context;
      mItems = items;
    }

    public override long GetItemId(int position) => position;

    public override int Count => mItems.Count;

    public override ReaderDevice this[int position] => mItems[position];


    public override View GetView(int position, View convertView, ViewGroup parent)
    {
      ReaderDevice item = mItems[position];
      View row = convertView;
      if (row == null)
        row = LayoutInflater.From(mContext).Inflate(Resource.Layout.device_name, parent, false);

      row.FindViewById<TextView>(Resource.Id.rdyc_name).Text = item.Name;
      row.FindViewById<TextView>(Resource.Id.rdyc_address).Text = item.Address;

      // Check if the device is connected. If it is, make the checkmark visible. If not, keep it insivible
      bool connected = item.IsConnected();
      mImageConnected = row.FindViewById<ImageView>(Resource.Id.rdyc_connected);
      mImageConnected.SetColorFilter(row.Resources.GetColor(Resource.Color.kamstrup_green));
      mImageConnected.Visibility = connected ? ViewStates.Visible : ViewStates.Invisible;

      return row;
    }

    public override void NotifyDataSetChanged()
    {
      base.NotifyDataSetChanged();
    }

    public void Clear()
    {
      mItems.Clear();
    }
  }
}