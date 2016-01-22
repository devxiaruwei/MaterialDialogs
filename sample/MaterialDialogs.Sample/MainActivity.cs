using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Sino.Droid.MaterialDialogs;

namespace MaterialDialogs.Sample
{
    [Activity(Label = "MaterialDialogs.Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            FindViewById<Button>(Resource.Id.Basic).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithBasic)
                    .SetContent(Resource.String.Content)
                    .SetPositiveText(Resource.String.Agree)
                    .SetNegativeText(Resource.String.Cancel)
                    .Show();
            };

            FindViewById<Button>(Resource.Id.DIcon).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithIcon)
                    .SetContent(Resource.String.Content)
                    .SetPositiveText(Resource.String.Agree)
                    .SetIcon(Resource.Drawable.Icon)
                    .Show();
            };

            FindViewById<Button>(Resource.Id.LongBtn).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithLongButton)
                    .SetContent(Resource.String.Content)
                    .SetPositiveText(Resource.String.LongPositive)
                    .SetNegativeText(Resource.String.Cancel)
                    .Show();
            };

            FindViewById<Button>(Resource.Id.ThreeButton).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithThreeButton)
                    .SetContent(Resource.String.Content)
                    .SetPositiveText(Resource.String.Agree)
                    .SetNegativeText(Resource.String.Ignore)
                    .SetNeutralText(Resource.String.Cancel)
                    .Show();
            };

            FindViewById<Button>(Resource.Id.Callback).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithCallback)
                    .SetContent(Resource.String.Content)
                    .SetPositiveText(Resource.String.Agree)
                    .SetNegativeText(Resource.String.Ignore)
                    .SetNeutralText(Resource.String.Cancel)
                    .SetCallback(new ButtonCallback
                    {
                        Positive = (x) =>
                        {
                            Toast.MakeText(this, "点击了确认按钮", ToastLength.Short).Show();
                        },
                        Negative = (x) =>
                        {
                            Toast.MakeText(this, "点击了忽略按钮", ToastLength.Short).Show();
                        },
                        Neutral = (x) =>
                        {
                            Toast.MakeText(this, "点击了取消按钮", ToastLength.Short).Show();
                        }
                    }).Show();
            };

            FindViewById<Button>(Resource.Id.List).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithList)
                    .SetItems(Resource.Array.Items)
                    .SetItemsCallback(new ListCallback
                    {
                        Selection = (dialog,view,which,text) =>
                        {
                            Toast.MakeText(this, String.Format("选中{0}，文字为{1}", which, text), ToastLength.Short).Show();
                        }
                    }).Show();
            };

            FindViewById<Button>(Resource.Id.Single).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithSingle)
                    .SetItems(Resource.Array.Items)
                    .SetItemsCallbackSingleChoice(-1, new ListCallbackSingleChoice
                    {
                        Selection = (dialog,view,which,text) =>
                        {
                            Toast.MakeText(this, String.Format("选中{0}，文字为{1}", which, text), ToastLength.Short).Show();
                            return true;
                        }
                    }).SetPositiveText(Resource.String.Agree).Show();
            };

            FindViewById<Button>(Resource.Id.Multip).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithMultip)
                    .SetItems(Resource.Array.Items)
                    .SetItemsCallbackMultiChoice(null, new ListCallbackMultiChoice
                    {
                        Selection = (dialog, which, text) =>
                        {

                            return true;
                        }
                    }).SetPositiveText(Resource.String.Agree).Show();
            };

            FindViewById<Button>(Resource.Id.Input).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this).SetTitle(Resource.String.WithInput)
                    .SetContent(Resource.String.Content)
                    .SetInputType(Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword)
                    .SetInput("请输入", "", new InputCallback
                    {
                        Input = (x, t) =>
                        {
                            Toast.MakeText(this, t, ToastLength.Short).Show();
                        }
                    }).Show();
            };

            FindViewById<Button>(Resource.Id.Progress).Click += (e, s) =>
            {
                new MaterialDialog.Builder(this)
                .SetTitle(Resource.String.WithProgress)
                .SetContent(Resource.String.Content)
                .SetProgress(true, 0)
                .Show();
            };
        }
    }
}

