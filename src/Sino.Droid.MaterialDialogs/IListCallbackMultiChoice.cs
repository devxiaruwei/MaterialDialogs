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
    public interface IListCallbackMultiChoice
    {
        bool OnSelection(MaterialDialog dialog, int[] which, string[] text);
    }

    public class ListCallbackMultiChoice : IListCallbackMultiChoice
    {
        public Func<MaterialDialog, int[], string[], bool> Selection { get; set; }

        public bool OnSelection(MaterialDialog dialog, int[] which, string[] text)
        {
            if(Selection != null)
            {
                return Selection(dialog, which, text);
            }
            return false;
        }
    }
}