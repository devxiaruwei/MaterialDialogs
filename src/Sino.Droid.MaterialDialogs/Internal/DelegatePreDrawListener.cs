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

namespace Sino.Droid.MaterialDialogs.Internal
{
    /// <summary>
    /// ViewTreeObserver.IOnPreDrawListener接口的委托形式
    /// </summary>
    public class DelegatePreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
    {
        public Func<bool> PreDraw { get; set; }

        public bool OnPreDraw()
        {
            if (PreDraw != null)
            {
                return PreDraw();
            }
            return false;
        }
    }
}