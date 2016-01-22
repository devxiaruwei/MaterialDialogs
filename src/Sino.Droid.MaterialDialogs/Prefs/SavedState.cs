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

namespace Sino.Droid.MaterialDialogs.Prefs
{
    public class SavedState : Android.Preferences.Preference.BaseSavedState
    {
        internal bool IsDialogShowing;
        internal Bundle DialogBundle;

        public SavedState(Parcel source)
            : base(source)
        {
            IsDialogShowing = source.ReadInt() == 1;
            DialogBundle = source.ReadBundle();
        }

        public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            base.WriteToParcel(dest, flags);
            dest.WriteInt(IsDialogShowing ? 1 : 0);
            dest.WriteBundle(DialogBundle);
        }

        public SavedState(IParcelable superState)
            : base(superState) { }

        public class SavedStateParcelable : Java.Lang.Object, IParcelableCreator
        {

            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new SavedState(source);
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new SavedState[size];
            }
        }

        public SavedStateParcelable Create = new SavedStateParcelable();
    }
}