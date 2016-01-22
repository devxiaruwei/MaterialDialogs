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
    /// ViewTreeObserver.IOnScrollChangedListener的委托形式
    /// </summary>
    public class DelegateScrollChangeListener : Java.Lang.Object, ViewTreeObserver.IOnScrollChangedListener
    {
        public Action ScrollChanged { get; set; }

        public void OnScrollChanged()
        {
            if (ScrollChanged != null)
            {
                ScrollChanged();
            }
        }
    }
}