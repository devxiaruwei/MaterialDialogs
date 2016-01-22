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

namespace Sino.Droid.MaterialDialogs
{
    public enum ListType
    {
        None,
        Regular,
        Single,
        Multi
    }

    public class ListTypeExt
    {
        public static int GetLayoutForType(ListType type)
        {
            switch (type)
            {
                case ListType.Regular:
                    return Resource.Layout.sino_droid_md_listitem;
                case ListType.Single:
                    return Resource.Layout.sino_droid_md_listitem_singlechoice;
                case ListType.Multi:
                    return Resource.Layout.sino_droid_md_listitem_multichoice;
                default:
                    throw new InvalidOperationException("Not a valid list type");
            }
        }
    }
}