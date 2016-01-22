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

namespace Sino.Droid.MaterialDialogs.Util
{
    public class TypefaceHelper
    {
        private static Dictionary<String, Typeface> cache = new Dictionary<string, Typeface>();

        public static Typeface Get(Context c, string name)
        {
            lock (cache)
            {
                if (!cache.ContainsKey(name))
                {
                    try
                    {
                        Typeface f = Typeface.CreateFromAsset(c.Assets, String.Format("fonts{0}", name));
                        cache.Add(name, f);
                        return f;
                    }
                    catch (Exception) 
                    {
                        return null;
                    }
                }
                return cache[name];
            }
        }
    }
}