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
    public class DialogBase : Dialog, IDialogInterfaceOnShowListener
    {
        protected MDRootLayout view;
        private IDialogInterfaceOnShowListener _showListener;

        protected DialogBase(Context context, int theme)
            : base(context, theme) { }

        public override View FindViewById(int id)
        {
            return view.FindViewById(id);
        }

        public new T FindViewById<T>(int id) where T : View
        {
            return view.FindViewById<T>(id);
        }

        public override void SetOnShowListener(IDialogInterfaceOnShowListener listener)
        {
            _showListener = listener;
        }

        internal void SetOnShowListenerInternal()
        {
            base.SetOnShowListener(this);
        }

        internal void SetViewInternal(View view)
        {
            base.SetContentView(view);
        }

        public void OnShow(IDialogInterface dialog)
        {
            if (_showListener != null)
            {
                _showListener.OnShow(dialog);
            }
        }

        public override void SetContentView(int layoutResID)
        {
            throw new NotImplementedException("SetContentView() is not supported in MaterialDialog. Specify a custom view in the Builder instead.");
        }

        public override void SetContentView(View view)
        {
            throw new NotImplementedException("SetContentView() is not supported in MaterialDialog. Specify a custom view in the Builder instead.");
        }

        public override void SetContentView(View view, ViewGroup.LayoutParams @params)
        {
            throw new NotImplementedException("SetContentView() is not supported in MaterialDialog. Specify a custom view in the Builder instead.");
        }
    }
}