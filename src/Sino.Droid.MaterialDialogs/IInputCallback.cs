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
    public interface IInputCallback
    {
        void OnInput(MaterialDialog dialog, string input);
    }

    public class InputCallback : IInputCallback
    {
        public Action<MaterialDialog, string> Input { get; set; }

        public void OnInput(MaterialDialog dialog, string input)
        {
            if (Input != null)
            {
                Input(dialog, input);
            }
        }
    }
}