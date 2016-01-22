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
using Android.Graphics;
using Android.Content.Res;
using Android.Util;
using Android.Graphics.Drawables;
using Android.Views.InputMethods;

namespace Sino.Droid.MaterialDialogs.Util
{
    public class DialogUtils
    {
        public static Color AdjustAlpha(Color color, float factor)
        {
            int alpha = (int)Math.Round(Color.GetAlphaComponent(color) * factor);
            int red = Color.GetRedComponent(color);
            int green = Color.GetGreenComponent(color);
            int blue = Color.GetBlueComponent(color);
            return Color.Argb(alpha, red, green, blue);
        }

        public static Color ResolveColor(Context context, int attr)
        {
            return ResolveColor(context, attr, 0);
        }

        public static Color ResolveColor(Context context, int attr, int fallback)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(new int[] { attr });
            try
            {
                return a.GetColor(0, fallback);
            }
            finally
            {
                a.Recycle();
            }
        }

        public static ColorStateList ResolveActionTextColorStateList(Context context, int colorAttr, ColorStateList fallback)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(new int[] { colorAttr });
            try
            {
                TypedValue value = a.PeekValue(0);
                if (value == null)
                {
                    return fallback;
                }
                if (value.Type >= DataType.FirstColorInt && value.Type <= DataType.LastColorInt)
                {

                    return GetActionTextStateList(context, new Color(value.Data));
                }
                else
                {
                    ColorStateList stateList = a.GetColorStateList(0);
                    if (stateList != null)
                    {
                        return stateList;
                    }
                    else
                    {
                        return fallback;
                    }
                }
            }
            finally
            {
                a.Recycle();
            }
        }

        public static ColorStateList GetActionTextColorStateList(Context context, int colorId)
        {
            TypedValue value = new TypedValue();
            context.Resources.GetValue(colorId, value, true);
            if (value.Type >= DataType.FirstColorInt && value.Type <= DataType.LastColorInt)
            {
                return GetActionTextStateList(context, new Color(value.Data));
            }
            else
            {
                return context.Resources.GetColorStateList(colorId);
            }
        }

        public static String ResolveString(Context context, int attr)
        {
            TypedValue v = new TypedValue();
            context.Theme.ResolveAttribute(attr, v, true);
            return v.String == null ? null : v.String.ToString();
        }

        private static int GravityEnumToAttrInt(GravityEnum value)
        {
            switch (value)
            {
                case GravityEnum.Center:
                    return 1;
                case GravityEnum.End:
                    return 2;
                default:
                    return 0;
            }
        }

        public static GravityEnum ResolveGravityEnum(Context context, int attr, GravityEnum defaultGravity)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(new int[] { attr });
            try
            {
                switch (a.GetInt(0, GravityEnumToAttrInt(defaultGravity)))
                {
                    case 1:
                        return GravityEnum.Center;
                    case 2:
                        return GravityEnum.End;
                    default:
                        return GravityEnum.Start;
                }
            }
            finally
            {
                a.Recycle();
            }
        }

        public static Drawable ResolveDrawable(Context context, int attr)
        {
            return ResolveDrawable(context, attr, null);
        }

        private static Drawable ResolveDrawable(Context context, int attr, Drawable fallback)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(new int[] { attr });
            try
            {
                Drawable d = a.GetDrawable(0);
                if (d == null && fallback != null)
                    d = fallback;
                return d;
            }
            finally
            {
                a.Recycle();
            }
        }

        public static int ResolveDimension(Context context, int attr)
        {
            return ResolveDimension(context, attr, -1);
        }

        private static int ResolveDimension(Context context, int attr, int fallback)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(new int[] { attr });
            try
            {
                return a.GetDimensionPixelSize(0, fallback);
            }
            finally
            {
                a.Recycle();
            }
        }

        public static bool ResolveBoolean(Context context, int attr, bool fallback)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(new int[] { attr });
            try
            {
                return a.GetBoolean(0, fallback);
            }
            finally
            {
                a.Recycle();
            }
        }

        public static bool ResolveBoolean(Context context, int attr)
        {
            return ResolveBoolean(context, attr, false);
        }

        public static bool IsColorDark(int color)
        {
            double darkness = 1 - (0.299 * Color.GetRedComponent(color) + 0.587 * Color.GetGreenComponent(color) + 0.114 * Color.GetBlueComponent(color)) / 255;
            return darkness >= 0.5;
        }

        public static void SetBackgroundCompat(View view, Drawable d)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
            {
                view.SetBackgroundDrawable(d);
            }
            else
            {
                view.Background = d;
            }
        }

        public static void ShowKeyboard(IDialogInterface di, MaterialDialog.Builder builder)
        {
            MaterialDialog dialog = (MaterialDialog)di;
            if (dialog.GetInputEditText() == null) return;
            dialog.GetInputEditText().Post(() =>
            {
                dialog.GetInputEditText().RequestFocus();
                InputMethodManager imm = (InputMethodManager)builder.Context.GetSystemService(Context.InputMethodService);
                if (imm != null)
                    imm.ShowSoftInput(dialog.GetInputEditText(), ShowFlags.Implicit);
            });
        }

        public static void HideKeyboard(Dialog di, MaterialDialog.Builder builder)
        {
            MaterialDialog dialog = (MaterialDialog)di;
            if (dialog.GetInputEditText() == null) return;
            dialog.GetInputEditText().Post(() =>
            {
                dialog.GetInputEditText().RequestFocus();
                InputMethodManager imm = (InputMethodManager)builder.Context.GetSystemService(Context.InputMethodService);
                if (imm != null)
                    imm.HideSoftInputFromWindow(dialog.GetInputEditText().WindowToken, 0);
            });
        }

        public static ColorStateList GetActionTextStateList(Context context, Color newPrimaryColor)
        {
            Color fallBackButtonColor = DialogUtils.ResolveColor(context,Android.Resource.Attribute.TextColorPrimary);
            if (newPrimaryColor == 0) newPrimaryColor = fallBackButtonColor;
            int[][] states = new int[][]{
                new int[]{-Android.Resource.Attribute.StateEnabled},
                new int[]{}};
            int[] colors = new int[]{
                DialogUtils.AdjustAlpha(newPrimaryColor, 0.4f),
                newPrimaryColor};
            return new ColorStateList(states, colors);
        }
    }
}