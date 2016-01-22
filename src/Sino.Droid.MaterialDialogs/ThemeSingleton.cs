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
using Android.Graphics;

namespace Sino.Droid.MaterialDialogs
{
    public class ThemeSingleton
    {
        private static ThemeSingleton singleton;

        public static ThemeSingleton Get(bool createIfNull)
        {
            if (singleton == null && createIfNull)
                singleton = new ThemeSingleton();
            return singleton;
        }

        public static ThemeSingleton Get()
        {
            return Get(true);
        }

        public bool darkTheme = false;
        public Color TitleColor { get; set; }
        public Color ContentColor { get; set; }
        public ColorStateList positiveColor = null;
        public ColorStateList neutralColor = null;
        public ColorStateList negativeColor = null;
        public Color WidgetColor { get; set; }
        public Color ItemColor { get; set; }
        public Drawable icon = null;
        public int backgroundColor = 0;
        public int dividerColor = 0;

        public int listSelector = 0;
        public int btnSelectorStacked = 0;
        public int btnSelectorPositive = 0;
        public int btnSelectorNeutral = 0;
        public int btnSelectorNegative = 0;

        public GravityEnum titleGravity = GravityEnum.Start;
        public GravityEnum contentGravity = GravityEnum.Start;
        public GravityEnum btnStackedGravity = GravityEnum.End;
        public GravityEnum itemsGravity = GravityEnum.Start;
        public GravityEnum buttonsGravity = GravityEnum.Start;
    }
}