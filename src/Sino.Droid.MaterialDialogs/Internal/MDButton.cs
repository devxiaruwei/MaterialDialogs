using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Util;
using Sino.Droid.MaterialDialogs.Util;
using Android.Support.V7.Text;

namespace Sino.Droid.MaterialDialogs.Internal
{
    public class MDButton : TextView
    {
        private bool _stacked = false;
        private GravityEnum _stackedGravity;

        private int _stackedEndPadding;
        private Drawable _stackedBackground;
        private Drawable _defaultBackground;

        public MDButton(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(context, attrs, 0, 0);
        }

        public MDButton(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Init(context, attrs, defStyleAttr, 0);
        }

        public MDButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Init(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
        {
            _stackedEndPadding = context.Resources.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_dialog_frame_margin);
            _stackedGravity = GravityEnum.End;
        }

        public void SetStacked(bool stacked, bool force)
        {
            if (_stacked != stacked || force)
            {
                Gravity = stacked ? GravityFlags.CenterVertical | GravityExt.GetGravity(_stackedGravity) : GravityFlags.Center;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
                {
                    TextAlignment = stacked ? TextAlignment.Gravity : TextAlignment.Center;
                }

                DialogUtils.SetBackgroundCompat(this, stacked ? _stackedBackground : _defaultBackground);

                if (stacked)
                {
                    SetPadding(_stackedEndPadding, PaddingTop, _stackedEndPadding, PaddingBottom);
                }
                _stacked = stacked;
            }
        }

        public void SetStackedGravity(GravityEnum gravity)
        {
            _stackedGravity = gravity;
        }

        public void SetStackedSelector(Drawable d)
        {
            _stackedBackground = d;
            if (_stacked)
                SetStacked(true, true);
        }

        public void SetDefaultSelector(Drawable d)
        {
            _defaultBackground = d;
            if (!_stacked)
            {
                SetStacked(false, true);
            }
        }

        public void SetAllCapsCompat(bool allCaps)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                SetAllCaps(allCaps);
            }
            else
            {
                if (allCaps)
                {
                    TransformationMethod = new AllCapsTransformationMethod(Context);
                }
                else
                {
                    TransformationMethod = null;
                }
            }
        }
    }
}