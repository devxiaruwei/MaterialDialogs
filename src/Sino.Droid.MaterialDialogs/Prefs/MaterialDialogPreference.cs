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
using Android.Preferences;
using Android.Util;

namespace Sino.Droid.MaterialDialogs.Prefs
{
    public class MaterialDialogPreference : DialogPreference
    {
        private Context context;
        private MaterialDialog mDialog;

        public MaterialDialogPreference(Context context)
            : base(context)
        {
            Init(context);
        }

        public MaterialDialogPreference(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(context);
        }

        private void Init(Context context)
        {
            this.context = context;
        }

        public override Dialog Dialog
        {
            get
            {
                return mDialog; ;
            }
        }

        protected override void ShowDialog(Bundle state)
        {
            MaterialDialog.Builder builder = new MaterialDialog.Builder(context)
                .SetTitle(DialogTitle)
                .SetContent(DialogMessage)
                .SetIcon(DialogIcon)
                .SetDismissListener(this)
                .SetCallback(new ButtonCallback
                {
                    Neutral = (x) =>
                    {
                        OnClick(x,(int)DialogButtonType.Neutral);
                    },
                    Negative = (x) =>
                    {
                        OnClick(x, (int)DialogButtonType.Negative);
                    },
                    Positive = (x) =>
                    {
                        OnClick(x, (int)DialogButtonType.Positive);
                    }
                })
                .SetPositiveText(PositiveButtonText)
                .SetNegativeText(NegativeButtonText)
                .SetAutoDismiss(true);

            View contentView = OnCreateDialogView();
            if (contentView != null)
            {
                OnBindDialogView(contentView);
                builder.SetCustomView(contentView, false);
            }
            else
            {
                builder.SetContent(DialogMessage);
            }

            try
            {
                PreferenceManager pm = PreferenceManager;
                Java.Lang.Reflect.Method method = pm.Class.GetDeclaredMethod(
                        "registerOnActivityDestroyListener",
                        Java.Lang.Class.FromType(typeof(PreferenceManager.IOnActivityDestroyListener)));
                method.Accessible = true;
                method.Invoke(pm, this);
            }
            catch (Exception) { }

            mDialog = builder.Build();
            if (state != null)
                mDialog.OnRestoreInstanceState(state);
            mDialog.Show();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            try
            {
                PreferenceManager pm = PreferenceManager;
                Java.Lang.Reflect.Method method = pm.Class.GetDeclaredMethod(
                    "unregisterOnActivityDestroyListener",
                    Java.Lang.Class.FromType(typeof(PreferenceManager.IOnActivityDestroyListener)));
                method.Accessible = true;
                method.Invoke(pm, this);
            }
            catch (Exception) { }
        }

        public override void OnActivityDestroy()
        {
            base.OnActivityDestroy();
            if (mDialog != null && mDialog.IsShowing)
                mDialog.Dismiss();
        }

        protected override IParcelable OnSaveInstanceState()
        {
            IParcelable superState = base.OnSaveInstanceState();
            Dialog dialog = Dialog;
            if (dialog == null || !dialog.IsShowing)
            {
                return superState;
            }

            SavedState myState = new SavedState(superState);
            myState.IsDialogShowing = true;
            myState.DialogBundle = dialog.OnSaveInstanceState();
            return myState;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (state == null || !(state is SavedState))
            {
                base.OnRestoreInstanceState(state);
                return;
            }

            SavedState myState = (SavedState)state;
            base.OnRestoreInstanceState(myState.SuperState);
            if (myState.IsDialogShowing)
            {
                ShowDialog(myState.DialogBundle);
            }
        }
    }
}