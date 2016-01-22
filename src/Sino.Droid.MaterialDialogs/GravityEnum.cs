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

namespace Sino.Droid.MaterialDialogs
{
    public enum GravityEnum
    {
        Start,
        Center,
        End
    }

    public class GravityExt
    {
        private static bool HAS_RTL = Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1;

        public static GravityFlags GetGravity(GravityEnum gravity)
        {
            switch (gravity)
            {
                case GravityEnum.Start:
                    {
                        return HAS_RTL ? GravityFlags.Start : GravityFlags.Left;
                    }
                case GravityEnum.Center:
                    {
                        return GravityFlags.CenterHorizontal;
                    }
                case GravityEnum.End:
                    {
                        return HAS_RTL ? GravityFlags.End : GravityFlags.Right;
                    }
                default:
                    throw new InvalidOperationException("Invalid gravity constant");
            }
        }

        public static TextAlignment GetTextAlignment(GravityEnum gravity)
        {
            switch (gravity)
            {
                case GravityEnum.Center:
                    {
                        return TextAlignment.Center;
                    }
                case GravityEnum.End:
                    {
                        return TextAlignment.ViewEnd;
                    }
                default:
                    return TextAlignment.ViewStart;
            }
        }
    }
}