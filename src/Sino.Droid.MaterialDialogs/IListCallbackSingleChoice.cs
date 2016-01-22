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
    public interface IListCallbackSingleChoice
    {
        bool OnSelection(MaterialDialog dialog, View itemView, int which, string text);
    }

    public class ListCallbackSingleChoice : IListCallbackSingleChoice
    {
        public Func<MaterialDialog, View, int, string, bool> Selection { get; set; }

        public bool OnSelection(MaterialDialog dialog, View itemView, int which, string text)
        {
            if(Selection != null)
            {
                return Selection(dialog, itemView, which, text);
            }
            return false;
        }
    }
}