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
using Android.Util;
using Sino.Droid.MaterialDialogs.Util;
using Android.Support.V7.Widget;
using Sino.Droid.MaterialDialogs.Internal;
using Android.Graphics;
using Android.Preferences;

namespace Sino.Droid.MaterialDialogs.Prefs
{
    public class MaterialEditTextPreference : EditTextPreference
    {
        private Color mColor;
        private MaterialDialog mDialog;
        private EditText mEditText;

        public MaterialEditTextPreference(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            int fallback;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                fallback = DialogUtils.ResolveColor(context, Android.Resource.Attribute.ColorAccent);
            else fallback = 0;
            fallback = DialogUtils.ResolveColor(context, Resource.Attribute.colorAccent, fallback);
            mColor = DialogUtils.ResolveColor(context, Resource.Attribute.sino_droid_md_widget_color, fallback);

            mEditText = new AppCompatEditText(context, attrs);

            mEditText.Id = Android.Resource.Id.Edit;
            mEditText.Enabled = true;
        }

        public MaterialEditTextPreference(Context context)
            : this(context, null) 
        {

        }

        protected override void OnAddEditTextToDialogView(View dialogView, EditText editText)
        {
            ((ViewGroup)dialogView).AddView(editText, new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
        }

        protected override void OnBindDialogView(View view)
        {
            EditText editText = mEditText;
            editText.Text = Text;

            if (editText.Text.Length > 0)
                editText.SetSelection(editText.Length());
            IViewParent oldParent = editText.Parent;
            if (oldParent != view)
            {
                if (oldParent != null)
                    ((ViewGroup)oldParent).RemoveView(editText);
                OnAddEditTextToDialogView(view, editText);
            }
        }

        protected override void OnDialogClosed(bool positiveResult)
        {
            if (positiveResult)
            {
                String value = mEditText.Text;
                if (CallChangeListener(value))
                    Text = value;
            }
        }

        public override EditText EditText
        {
            get
            {
                return mEditText; ;
            }
        }

        public override Dialog Dialog
        {
            get
            {
                return mDialog;
            }
        }

        protected override void ShowDialog(Bundle state)
        {
            var mBuilder = new MaterialDialog.Builder(Context)
                    .SetTitle(DialogTitle)
                    .SetIcon(DialogIcon)
                    .SetPositiveText(PositiveButtonText)
                    .SetNegativeText(NegativeButtonText)
                    .SetDismissListener(this)
                    .SetCallback(new ButtonCallback
                    {
                        Positive = (x) =>
                        {
                            OnClick(x, (int)DialogButtonType.Positive);
                            String value = mEditText.Text;
                            if (CallChangeListener(value) && Persistent)
                                Text = value;
                        },
                        Neutral = (x) =>
                        {
                            OnClick(x, (int)DialogButtonType.Neutral);
                        },
                        Negative = (x) =>
                        {
                            OnClick(x, (int)DialogButtonType.Negative);
                        }
                    })
                    .SetDismissListener(this);

            View layout = LayoutInflater.From(Context).Inflate(Resource.Layout.sino_droid_md_stub_inputpref, null);
            OnBindDialogView(layout);

            MDTintHelper.SetTint(mEditText, mColor);

            TextView message = layout.FindViewById<TextView>(Android.Resource.Id.Message);
            if (DialogMessage != null && DialogMessage.Length > 0)
            {
                message.Visibility = ViewStates.Visible;
                message.Text = DialogMessage;
            }
            else
            {
                message.Visibility = ViewStates.Gone;
            }
            mBuilder.SetCustomView(layout, false);

            try
            {
                var pm = PreferenceManager;
                Java.Lang.Reflect.Method method = pm.Class.GetDeclaredMethod(
                        "registerOnActivityDestroyListener",
                        Java.Lang.Class.FromType(typeof(PreferenceManager.IOnActivityDestroyListener)));
                method.Accessible = true;
                method.Invoke(pm, this);
            }
            catch (Exception) { }

            mDialog = mBuilder.Build();
            if (state != null)
                mDialog.OnRestoreInstanceState(state);
            RequestInputMethod(mDialog);

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

        private void RequestInputMethod(Dialog dialog)
        {
            Window window = dialog.Window;
            window.SetSoftInputMode(SoftInput.StateAlwaysVisible);
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