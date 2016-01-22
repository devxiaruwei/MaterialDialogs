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
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Sino.Droid.MaterialDialogs.Util;

namespace Sino.Droid.MaterialDialogs.Internal
{
    public class MDTintHelper
    {
        public static void SetTint(RadioButton radioButton, Color color)
        {
            ColorStateList sl = new ColorStateList(new int[][]{
                new int[]{-Android.Resource.Attribute.StateChecked},
                new int[]{Android.Resource.Attribute.StateChecked}}
                , new int[]{
                    DialogUtils.ResolveColor(radioButton.Context,Resource.Attribute.colorControlNormal),color});
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                radioButton.ButtonTintList = sl;
            }
            else
            {
                Drawable d = ContextCompat.GetDrawable(radioButton.Context, Resource.Drawable.abc_btn_radio_material);
                DrawableCompat.SetTintList(d, sl);
                radioButton.SetButtonDrawable(d);
            }
        }

        public static void SetTint(SeekBar seekBar, Color color)
        {
            ColorStateList s1 = ColorStateList.ValueOf(color);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                seekBar.ThumbTintList = s1;
                seekBar.ProgressTintList = s1;
            }
            else if (Build.VERSION.SdkInt > BuildVersionCodes.GingerbreadMr1)
            {
                Drawable progressDrawable = DrawableCompat.Wrap(seekBar.ProgressDrawable);
                seekBar.ProgressDrawable = progressDrawable;
                DrawableCompat.SetTintList(progressDrawable, s1);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
                {
                    Drawable thumbDrawable = DrawableCompat.Wrap(seekBar.Thumb);
                    DrawableCompat.SetTintList(thumbDrawable, s1);
                    seekBar.SetThumb(thumbDrawable);
                }
            }
            else
            {
                PorterDuff.Mode mode = PorterDuff.Mode.SrcIn;
                if (Build.VERSION.SdkInt <= BuildVersionCodes.GingerbreadMr1)
                {
                    mode = PorterDuff.Mode.Multiply;
                }
                if (seekBar.IndeterminateDrawable != null)
                    seekBar.IndeterminateDrawable.SetColorFilter(color, mode);
                if (seekBar.ProgressDrawable != null)
                    seekBar.ProgressDrawable.SetColorFilter(color, mode);
            }
        }

        public static void SetTint(ProgressBar progressBar, Color color)
        {
            SetTint(progressBar, color, false);
        }

        public static void SetTint(ProgressBar progressBar, Color color, bool skipIndeterminate)
        {
            ColorStateList sl = ColorStateList.ValueOf(color);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                progressBar.ProgressTintList = sl;
                progressBar.SecondaryProgressTintList = sl;
                if (!skipIndeterminate)
                    progressBar.IndeterminateTintList = sl;
            }
            else
            {
                PorterDuff.Mode mode = PorterDuff.Mode.SrcIn;
                if (Build.VERSION.SdkInt <= BuildVersionCodes.GingerbreadMr1)
                {
                    mode = PorterDuff.Mode.Multiply;
                }
                if (!skipIndeterminate && progressBar.IndeterminateDrawable != null)
                    progressBar.IndeterminateDrawable.SetColorFilter(color, mode);
                if (progressBar.ProgressDrawable != null)
                    progressBar.ProgressDrawable.SetColorFilter(color, mode);
            }
        }

        private static ColorStateList CreateEditTextColorStateList(Context context, Color color)
        {
            int[][] states = new int[3][];
            int[] colors = new int[3];
            int i = 0;
            states[i] = new int[] { -Android.Resource.Attribute.StateEnabled };
            colors[i] = DialogUtils.ResolveColor(context, Resource.Attribute.colorControlNormal);
            i++;
            states[i] = new int[] { -Android.Resource.Attribute.StatePressed, -Android.Resource.Attribute.StateFocused };
            colors[i] = DialogUtils.ResolveColor(context, Resource.Attribute.colorControlNormal);
            i++;
            states[i] = new int[] { };
            colors[i] = color;
            return new ColorStateList(states, colors);
        }

        public static void SetTint(EditText editText, Color color)
        {
            ColorStateList editTextColorStateList = CreateEditTextColorStateList(editText.Context, color);
            if (editText is AppCompatEditText)
            {
                ((AppCompatEditText)editText).SupportBackgroundTintList = editTextColorStateList;
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                editText.BackgroundTintList = editTextColorStateList;
            }
        }

        public static void SetTint(CheckBox box, Color color)
        {
            ColorStateList sl = new ColorStateList(new int[][]{
                new int[]{-Android.Resource.Attribute.StateChecked},
                new int[]{Android.Resource.Attribute.StateChecked}}
                , new int[]{
                DialogUtils.ResolveColor(box.Context,Resource.Attribute.colorControlNormal),color});
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                box.ButtonTintList = sl;
            }
            else
            {
                Drawable drawable = ContextCompat.GetDrawable(box.Context, Resource.Drawable.abc_btn_check_material);
                DrawableCompat.SetTintList(drawable, sl);
                box.SetButtonDrawable(drawable);
            }
        }
    }
}