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
    public interface IListCallback
    {
        void OnSelection(MaterialDialog dialog, View itemView, int which, string text);
    }

    public class ListCallback : IListCallback
    {
        public Action<MaterialDialog, View, int, string> Selection { get; set; }

        public void OnSelection(MaterialDialog dialog, View itemView, int which, string text)
        {
            if(Selection != null)
            {
                Selection(dialog, itemView, which, text);
            }
        }
    }
}