using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Sino.Droid.MaterialDialogs.Simpelist
{
    public class MaterialSimpleListAdapter : ArrayAdapter<MaterialSimpleListItem>
    {
        private MaterialDialog dialog;

        public void SetDialog(MaterialDialog dialog)
        {
            SetDialog(dialog, true);
        }

        public void SetDialog(MaterialDialog dialog, bool notify)
        {
            this.dialog = dialog;
            if (notify)
            {
                NotifyDataSetChanged();
            }
        }

        public MaterialSimpleListAdapter(Context context)
            : base(context, Resource.Layout.sino_droid_md_simplelist_item, Android.Resource.Id.Title)
        {
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = base.GetView(position, convertView, parent);
            if (dialog != null)
            {
                MaterialSimpleListItem item = GetItem(position);
                ImageView ic = view.FindViewById<ImageView>(Android.Resource.Id.Icon);
                if (item.Icon != null)
                {
                    ic.SetImageDrawable(item.Icon);
                }
                else
                {
                    ic.Visibility = ViewStates.Gone;
                }
                TextView tv = view.FindViewById<TextView>(Android.Resource.Id.Title);
                tv.SetTextColor(dialog.MBuilder.ItemColor);
                tv.Text = item.Content;
                dialog.SetTypeface(tv, dialog.MBuilder.RegularFont);
                SetupGravity((ViewGroup)view);
            }
            return view;
        }

        private void SetupGravity(ViewGroup view)
        {
            LinearLayout itemRoot = (LinearLayout)view;
            GravityFlags gravity = GravityExt.GetGravity(dialog.MBuilder.ItemsGravity);
            itemRoot.SetGravity(gravity | GravityFlags.CenterVertical);

            if (view.ChildCount == 2)
            {
                if (dialog.MBuilder.ItemsGravity == GravityEnum.End && !IsRtl() && view.GetChildAt(0) is ImageView)
                {
                    CompoundButton first = (CompoundButton)view.GetChildAt(0);
                    view.RemoveView(first);

                    TextView second = (TextView)view.GetChildAt(0);
                    view.RemoveView(first);
                    second.SetPadding(second.PaddingRight, second.PaddingTop,
                        second.PaddingLeft, second.PaddingBottom);

                    view.AddView(second);
                    view.AddView(first);
                }
            }
            else if (dialog.MBuilder.ItemsGravity == GravityEnum.Start && IsRtl() && view.GetChildAt(1) is ImageView)
            {
                CompoundButton first = (CompoundButton)view.GetChildAt(1);
                view.RemoveView(first);

                TextView second = (TextView)view.GetChildAt(0);
                view.RemoveView(second);
                second.SetPadding(second.PaddingRight, second.PaddingTop,
                    second.PaddingRight, second.PaddingBottom);

                view.AddView(first);
                view.AddView(second);
            }
        }

        private bool IsRtl()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr1)
                return false;
            Android.Content.Res.Configuration config = Context.Resources.Configuration;
            return config.LayoutDirection == LayoutDirection.Rtl;
        }
    }
}