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
using Java.Lang.Reflect;

namespace Sino.Droid.MaterialDialogs.Prefs
{
    public class MaterialMultiSelectListPreference : MultiSelectListPreference
    {
        private Context context;
        private MaterialDialog mDialog;

        public MaterialMultiSelectListPreference(Context context)
            : this(context, null) { }

        public MaterialMultiSelectListPreference(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(context);
        }
        public override void SetEntries(Java.Lang.ICharSequence[] entries)
        {
            base.SetEntries(entries);
            if (mDialog != null)
                mDialog.SetItems(entries.Select(x => x.ToString()).ToArray());
        }

        private void Init(Context context)
        {
            this.context = context;
            if (Build.VERSION.SdkInt <= BuildVersionCodes.GingerbreadMr1)
                WidgetLayoutResource = 0;
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
            List<int> indices = new List<int>();
            foreach (String s in Values)
            {
                int index = FindIndexOfValue(s);
                if (index >= 0)
                    indices.Add(FindIndexOfValue(s));
            }
            MaterialDialog.Builder builder = new MaterialDialog.Builder(context)
                    .SetTitle(DialogTitle)
                    .SetContent(DialogMessage)
                    .SetIcon(DialogIcon)
                    .SetNegativeText(NegativeButtonText)
                    .SetPositiveText(PositiveButtonText)
                    .SetCallback(new ButtonCallback()
                    {
                        Neutral = (x) =>
                        {
                            OnClick(x, (int)DialogButtonType.Neutral);
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
                    .SetItems(GetEntries())
                    .SetItemsCallbackMultiChoice(indices.ToArray(), new ListCallbackMultiChoice
                    {
                        Selection = (dialog, which, text) =>
                        {
                            OnClick(null, (int)DialogButtonType.Positive);
                            dialog.Dismiss();
                            ISet<String> values = new HashSet<String>();
                            foreach (int i in which)
                            {
                                values.Add(GetEntryValues()[i]);
                            }
                            if (CallChangeListener((Java.Lang.Object)values))
                                Values = values;
                            return true;
                        }
                    }).SetDismissListener(this);

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
                Method method = pm.Class.GetDeclaredMethod(
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
                Method method = pm.Class.GetDeclaredMethod(
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