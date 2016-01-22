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
using Sino.Droid.MaterialDialogs.Internal;

namespace Sino.Droid.MaterialDialogs
{
    public class MaterialDialogAdapter : BaseAdapter
    {
        private MaterialDialog dialog;
        private int layout;

        private GravityEnum itemGravity;
        public RadioButton RadioButton { get; set; }
        public bool InitRadio { get; set; }

        public MaterialDialogAdapter(MaterialDialog dialog, int layout)
        {
            this.dialog = dialog;
            this.layout = layout;
            this.itemGravity = dialog.MBuilder.ItemsGravity;
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        public override int Count
        {
            get
            {
                return dialog.MBuilder.Items != null ? dialog.MBuilder.Items.Length : 0;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return dialog.MBuilder.Items[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = LayoutInflater.From(dialog.Context).Inflate(layout, parent, false);
            }

            TextView tv = convertView.FindViewById<TextView>(Resource.Id.title);
            switch (dialog.ListType)
            {
                case ListType.Single:
                    {
                        RadioButton radio = convertView.FindViewById<RadioButton>(Resource.Id.control);
                        bool selected = dialog.MBuilder.SelectedIndex == position;
                        MDTintHelper.SetTint(radio, dialog.MBuilder.WidgetColor);
                        radio.Checked = selected;
                        if (selected && InitRadio)
                            RadioButton = radio;
                    }
                    break;
                case ListType.Multi:
                    {
                        CheckBox checkbox = convertView.FindViewById<CheckBox>(Resource.Id.control);
                        bool selected = dialog.SelectedIndicesList.Contains(position);
                        MDTintHelper.SetTint(checkbox, dialog.MBuilder.WidgetColor);
                        checkbox.Checked = selected;
                    }
                    break;
            }
            tv.Text = dialog.MBuilder.Items[position];
            tv.SetTextColor(dialog.MBuilder.ItemColor);
            dialog.SetTypeface(tv, dialog.MBuilder.RegularFont);

            convertView.Tag = position + ":" + dialog.MBuilder.Items[position];
            SetupGravity((ViewGroup)convertView);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                ViewGroup group = (ViewGroup)convertView;
                if (group.ChildCount == 2)
                {
                    if (group.GetChildAt(0) is CompoundButton)
                        group.GetChildAt(0).Background = null;
                    else if (group.GetChildAt(1) is CompoundButton)
                        group.GetChildAt(1).Background = null;
                }
            }
            return convertView;
        }

        private void SetupGravity(ViewGroup view)
        {
            LinearLayout itemRoot = (LinearLayout)view;
            GravityFlags gravityInt = GravityExt.GetGravity(itemGravity);
            itemRoot.SetGravity(gravityInt | GravityFlags.CenterVertical);

            if (view.ChildCount == 2)
            {
                if (itemGravity == GravityEnum.End && !IsRtl() && view.GetChildAt(0) is CompoundButton)
                {
                    CompoundButton first = (CompoundButton)view.GetChildAt(0);
                    view.RemoveView(first);

                    TextView second = (TextView)view.GetChildAt(0);
                    view.RemoveView(second);
                    second.SetPadding(second.PaddingRight, second.PaddingTop,
                        second.PaddingLeft, second.PaddingBottom);

                    view.AddView(second);
                    view.AddView(first);
                }
                else if (itemGravity == GravityEnum.Start && IsRtl() && view.GetChildAt(1) is CompoundButton)
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
        }

        private bool IsRtl()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr1)
                return false;
            Android.Content.Res.Configuration config = dialog.MBuilder.Context.Resources.Configuration;
            return config.LayoutDirection == LayoutDirection.Rtl;
        }
    }
}