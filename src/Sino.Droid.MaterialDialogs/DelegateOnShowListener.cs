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
    /// <summary>
    /// IDialogInterfaceOnShowListener的委托方式
    /// </summary>
    public class DelegateOnShowListener : Java.Lang.Object , IDialogInterfaceOnShowListener
    {
        public Action<IDialogInterface> Show { get; set; }

        public void OnShow(IDialogInterface dialog)
        {
            if (Show != null)
            {
                Show(dialog);
            }
        }
    }
}