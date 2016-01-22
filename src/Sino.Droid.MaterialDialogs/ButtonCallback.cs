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
    public class ButtonCallback
    {
        public Action<MaterialDialog> Any { get; set; }
        public Action<MaterialDialog> Positive { get; set; }
        public Action<MaterialDialog> Negative { get; set; }
        public Action<MaterialDialog> Neutral { get; set; }

        public virtual void OnAny(MaterialDialog dialog)
        {
            if (Any != null)
            {
                Any(dialog);
            }
        }

        public virtual void OnPositive(MaterialDialog dialog)
        {
            if (Positive != null)
            {
                Positive(dialog);
            }
        }

        public virtual void OnNegative(MaterialDialog dialog)
        {
            if (Negative != null)
            {
                Negative(dialog);
            }
        }

        public virtual void OnNeutral(MaterialDialog dialog)
        {
            if (Neutral != null)
            {
                Neutral(dialog);
            }
        }
    }
}