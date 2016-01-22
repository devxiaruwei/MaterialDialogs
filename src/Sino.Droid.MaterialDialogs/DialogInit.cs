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
using Sino.Droid.MaterialDialogs.Util;
using Android.Graphics.Drawables;
using Sino.Droid.MaterialDialogs.Internal;
using Android.Graphics;
using Android.Text.Method;
using Sino.Droid.MaterialDialogs.Simpelist;
using Sino.Droid.MaterialDialogs.Progress;

namespace Sino.Droid.MaterialDialogs
{
    public class DialogInit
    {
        public static int GetTheme(MaterialDialog.Builder builder)
        {
            bool darkTheme = DialogUtils.ResolveBoolean(builder.Context, Resource.Attribute.sino_droid_md_dark_theme, builder.Theme == Theme.Dark);
            builder.SetTheme(darkTheme ? Theme.Dark : Theme.Light);
            return darkTheme ? Resource.Style.sino_droid_md_Dark : Resource.Style.sino_droid_md_Light;
        }

        public static int GetInflateLayout(MaterialDialog.Builder builder)
        {
            if (builder.CustomView != null)
            {
                return Resource.Layout.sino_droid_md_dialog_custom;
            }
            else if (builder.Items != null && builder.Items.Length > 0 || builder.Adapter != null)
            {
                return Resource.Layout.sino_droid_md_dialog_list;
            }
            else if (builder.Progress > -2)
            {
                return Resource.Layout.sino_droid_md_dialog_progress;
            }
            else if (builder.IndeterminateProgress)
            {
                if (builder.IndeterminateIsHorizontalProgress)
                    return Resource.Layout.sino_droid_md_dialog_progress_indeterminate_horizontal;
                return Resource.Layout.sino_droid_md_dialog_progress_indeterminate;
            }
            else if (builder.InputCallback != null)
            {
                return Resource.Layout.sino_droid_md_dialog_input;
            }
            else
            {
                return Resource.Layout.sino_droid_md_dialog_basic;
            }
        }

        public static void Init(MaterialDialog dialog)
        {
            MaterialDialog.Builder builder = dialog.MBuilder;

            dialog.SetCancelable(builder.Cancelable);
            dialog.SetCanceledOnTouchOutside(builder.Cancelable);
            if (builder.BackgroundColor == 0)
                builder.BackgroundColor = DialogUtils.ResolveColor(builder.Context, Resource.Attribute.sino_droid_md_background_color);

            if (builder.BackgroundColor != 0)
            {
                GradientDrawable drawable = new GradientDrawable();
                drawable.SetCornerRadius(builder.Context.Resources.GetDimension(Resource.Dimension.sino_droid_md_bg_corner_radius));
                drawable.SetColor(builder.BackgroundColor);
                DialogUtils.SetBackgroundCompat(dialog.GetView(), drawable);
            }

            if (!builder.PositiveColorSet)
                builder.PositiveColor = DialogUtils.ResolveActionTextColorStateList(builder.Context, Resource.Attribute.sino_droid_md_positive_color, builder.PositiveColor);
            if (!builder.NeutralColorSet)
                builder.NeutralColor = DialogUtils.ResolveActionTextColorStateList(builder.Context, Resource.Attribute.sino_droid_md_neutral_color, builder.NeutralColor);
            if (!builder.NegativeColorSet)
                builder.NegativeColor = DialogUtils.ResolveActionTextColorStateList(builder.Context, Resource.Attribute.sino_droid_md_negative_color, builder.NegativeColor);
            if (!builder.WidgetColorSet)
                builder.WidgetColor = DialogUtils.ResolveColor(builder.Context, Resource.Attribute.sino_droid_md_widget_color, builder.WidgetColor);

            if (!builder.TitleColorSet)
            {
                int titleColorFallback = DialogUtils.ResolveColor(dialog.Context, Android.Resource.Attribute.TextColorPrimary);
                builder.TitleColor = DialogUtils.ResolveColor(builder.Context, Resource.Attribute.sino_droid_md_title_color, titleColorFallback);
            }

            if (!builder.ContentColorSet)
            {
                int contentColorFallback = DialogUtils.ResolveColor(dialog.Context, Android.Resource.Attribute.TextColorSecondary);
                builder.ContentColor = DialogUtils.ResolveColor(builder.Context, Resource.Attribute.sino_droid_md_content_color, contentColorFallback);
            }
            if (!builder.ItemColorSet)
                builder.ItemColor = DialogUtils.ResolveColor(builder.Context, Resource.Attribute.sino_droid_md_item_color, builder.ContentColor);

            dialog.Title = dialog.GetView().FindViewById<TextView>(Resource.Id.title);
            dialog.Icon = dialog.GetView().FindViewById<ImageView>(Resource.Id.icon);
            dialog.TitleFrame = dialog.GetView().FindViewById(Resource.Id.titleFrame);
            dialog.Content = dialog.GetView().FindViewById<TextView>(Resource.Id.content);
            dialog.ListView = dialog.GetView().FindViewById<ListView>(Resource.Id.contentListView);

            dialog.PositiveButton = dialog.GetView().FindViewById<MDButton>(Resource.Id.buttonDefaultPositive);
            dialog.NeutralButton = dialog.GetView().FindViewById<MDButton>(Resource.Id.buttonDefaultNeutral);
            dialog.NegativeButton = dialog.GetView().FindViewById<MDButton>(Resource.Id.buttonDefaultNegative);

            if (builder.InputCallback != null && builder.PositiveText == null)
                builder.PositiveText = builder.Context.GetText(Android.Resource.String.Ok);

            dialog.PositiveButton.Visibility = builder.PositiveText != null ? ViewStates.Visible : ViewStates.Gone;
            dialog.NeutralButton.Visibility = builder.NeutralText != null ? ViewStates.Visible : ViewStates.Gone;
            dialog.NegativeButton.Visibility = builder.NegativeText != null ? ViewStates.Visible : ViewStates.Gone;

            if (builder.Icon != null)
            {
                dialog.Icon.Visibility = ViewStates.Visible;
                dialog.Icon.SetImageDrawable(builder.Icon);
            }
            else
            {
                Drawable d = DialogUtils.ResolveDrawable(builder.Context, Resource.Attribute.sino_droid_md_icon);
                if (d != null)
                {
                    dialog.Icon.Visibility = ViewStates.Visible;
                    dialog.Icon.SetImageDrawable(d);
                }
                else
                {
                    dialog.Icon.Visibility = ViewStates.Gone;
                }
            }

            int maxIconSize = builder.MaxIconSize;
            if (maxIconSize == -1)
                maxIconSize = DialogUtils.ResolveDimension(builder.Context, Resource.Attribute.sino_droid_md_icon_max_size);
            if (builder.LimitIconToDefaultSize || DialogUtils.ResolveBoolean(builder.Context, Resource.Attribute.sino_droid_md_icon_limit_icon_to_default_size))
                maxIconSize = builder.Context.Resources.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_icon_max_size);
            if (maxIconSize > -1)
            {
                dialog.Icon.SetAdjustViewBounds(true);
                dialog.Icon.SetMaxHeight(maxIconSize);
                dialog.Icon.SetMaxWidth(maxIconSize);
                dialog.Icon.RequestLayout();
            }

            if (!builder.DividerColorSet)
            {
                int dividerFallback = DialogUtils.ResolveColor(dialog.Context, Resource.Attribute.sino_droid_md_divider);
                builder.DividerColor = DialogUtils.ResolveColor(builder.Context, Resource.Attribute.sino_droid_md_divider_color, dividerFallback);
            }
            dialog.GetView().SetDividerColor(new Color(builder.DividerColor));

            if (dialog.Title != null)
            {
                dialog.SetTypeface(dialog.Title, builder.MediumFont);
                dialog.Title.SetTextColor(builder.TitleColor);
                dialog.Title.Gravity = GravityExt.GetGravity(builder.TitleGravity);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
                {
                    dialog.Title.TextAlignment = GravityExt.GetTextAlignment(builder.TitleGravity);
                }

                if (builder.Title == null)
                {
                    dialog.TitleFrame.Visibility = ViewStates.Gone;
                }
                else
                {
                    dialog.Title.Text = builder.Title;
                    dialog.TitleFrame.Visibility = ViewStates.Visible;
                }
            }

            if (dialog.Content != null)
            {
                dialog.Content.MovementMethod = new LinkMovementMethod();
                dialog.SetTypeface(dialog.Content, builder.RegularFont);
                dialog.Content.SetLineSpacing(0f, builder.ContentLineSpacingMultiplier);
                if (builder.PositiveColor == null)
                    dialog.Content.SetLinkTextColor(DialogUtils.ResolveColor(dialog.Context, Android.Resource.Attribute.TextColorPrimary));
                else
                    dialog.Content.SetLinkTextColor(builder.PositiveColor);
                dialog.Content.SetTextColor(builder.ContentColor);
                dialog.Content.Gravity = GravityExt.GetGravity(builder.ContentGravity);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
                {
                    dialog.Content.TextAlignment = GravityExt.GetTextAlignment(builder.ContentGravity);
                }

                if (builder.Content != null)
                {
                    dialog.Content.Text = builder.Content;
                    dialog.Content.Visibility = ViewStates.Visible;
                }
                else
                {
                    dialog.Content.Visibility = ViewStates.Gone;
                }
            }

            dialog.GetView().SetButtonGravity(builder.ButtonsGravity);
            dialog.GetView().SetButtonStackedGravity(builder.BtnStackedGravity);
            dialog.GetView().SetForceStack(builder.ForceStacking);
            bool textAllCaps;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                textAllCaps = DialogUtils.ResolveBoolean(builder.Context, Android.Resource.Attribute.TextAllCaps, true);
                if (textAllCaps)
                    textAllCaps = DialogUtils.ResolveBoolean(builder.Context, Resource.Attribute.textAllCaps, true);
            }
            else
            {
                textAllCaps = DialogUtils.ResolveBoolean(builder.Context, Resource.Attribute.textAllCaps, true);
            }

            MDButton positiveTextView = dialog.PositiveButton;
            dialog.SetTypeface(positiveTextView, builder.MediumFont);
            positiveTextView.SetAllCapsCompat(textAllCaps);
            positiveTextView.Text = builder.PositiveText;
            positiveTextView.SetTextColor(builder.PositiveColor);
            dialog.PositiveButton.SetStackedSelector(dialog.GetButtonSelector(DialogAction.Positive, true));
            dialog.PositiveButton.SetDefaultSelector(dialog.GetButtonSelector(DialogAction.Positive, false));
            dialog.PositiveButton.Tag = (int)DialogAction.Positive;
            dialog.PositiveButton.SetOnClickListener(dialog);
            dialog.PositiveButton.Visibility = ViewStates.Visible;

            MDButton negativeTextView = dialog.NegativeButton;
            dialog.SetTypeface(negativeTextView, builder.MediumFont);
            negativeTextView.SetAllCapsCompat(textAllCaps);
            negativeTextView.Text = builder.NegativeText;
            negativeTextView.SetTextColor(builder.NegativeColor);
            dialog.NegativeButton.SetStackedSelector(dialog.GetButtonSelector(DialogAction.Negative, true));
            dialog.NegativeButton.SetDefaultSelector(dialog.GetButtonSelector(DialogAction.Negative, false));
            dialog.NegativeButton.Tag = (int)DialogAction.Negative;
            dialog.NegativeButton.SetOnClickListener(dialog);
            dialog.NegativeButton.Visibility = ViewStates.Visible;

            MDButton neutralTextView = dialog.NeutralButton;
            dialog.SetTypeface(neutralTextView, builder.MediumFont);
            neutralTextView.SetAllCapsCompat(textAllCaps);
            neutralTextView.Text = builder.NeutralText;
            neutralTextView.SetTextColor(builder.NeutralColor);
            dialog.NeutralButton.SetStackedSelector(dialog.GetButtonSelector(DialogAction.Neutral, true));
            dialog.NeutralButton.SetDefaultSelector(dialog.GetButtonSelector(DialogAction.Neutral, false));
            dialog.NeutralButton.Tag = (int)DialogAction.Neutral;
            dialog.NeutralButton.SetOnClickListener(dialog);
            dialog.NeutralButton.Visibility = ViewStates.Visible;

            if (builder.ListCallbackMultiChoice != null)
                dialog.SelectedIndicesList = new List<int>();
            if (dialog.ListView != null && (builder.Items != null && builder.Items.Length > 0 || builder.Adapter != null))
            {
                dialog.ListView.Selector = dialog.GetListSelector();

                if (builder.Adapter == null)
                {
                    if (builder.ListCallbackSingleChoice != null)
                    {
                        dialog.ListType = ListType.Single;
                    }
                    else if (builder.ListCallbackMultiChoice != null)
                    {
                        dialog.ListType = ListType.Multi;
                        if (builder.SelectedIndices != null)
                            dialog.SelectedIndicesList = builder.SelectedIndices.ToList();
                    }
                    else
                    {
                        dialog.ListType = ListType.Regular;
                    }
                    builder.Adapter = new MaterialDialogAdapter(dialog, ListTypeExt.GetLayoutForType(dialog.ListType));
                }
                else if (builder.Adapter is MaterialSimpleListAdapter)
                {
                    ((MaterialSimpleListAdapter)builder.Adapter).SetDialog(dialog, false);
                }
            }
            SetupProgressDialog(dialog);

            SetupInputDialog(dialog);
            if (builder.CustomView != null)
            {
                dialog.GetView().FindViewById<MDRootLayout>(Resource.Id.root).NoTitleNoPadding();
                FrameLayout frame = dialog.GetView().FindViewById<FrameLayout>(Resource.Id.customViewFrame);
                dialog.CustomViewFrame = frame;
                View innerView = builder.CustomView;
                if (builder.WrapCustomViewInScroll)
                {
                    var r = dialog.Context.Resources;
                    int framePadding = r.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_dialog_frame_margin);
                    ScrollView sv = new ScrollView(dialog.Context);
                    int paddingTop = r.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_content_padding_top);
                    int paddingBottom = r.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_content_padding_bottom);
                    sv.SetClipToPadding(false);
                    if (innerView is EditText)
                    {
                        sv.SetPadding(framePadding, paddingTop, framePadding, paddingBottom);
                    }
                    else
                    {
                        sv.SetPadding(0, paddingTop, 0, paddingBottom);
                        innerView.SetPadding(framePadding, 0, framePadding, 0);
                    }
                    sv.AddView(innerView, new ScrollView.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent));
                    innerView = sv;
                }
                frame.AddView(innerView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent));
            }
            if (builder.ShowListener != null)
                dialog.SetOnShowListener(builder.ShowListener);
            if (builder.CancelListener != null)
                dialog.SetOnCancelListener(builder.CancelListener);
            if (builder.DismissListener != null)
                dialog.SetOnDismissListener(builder.DismissListener);
            if (builder.KeyListener != null)
                dialog.SetOnKeyListener(builder.KeyListener);

            dialog.SetOnShowListenerInternal();

            dialog.InvalidateList();
            dialog.SetViewInternal(dialog.GetView());
            dialog.CheckIfListInitScroll();
        }

        private static void SetupProgressDialog(MaterialDialog dialog)
        {
            MaterialDialog.Builder builder = dialog.MBuilder;
            if (builder.IndeterminateProgress || builder.Progress > -2)
            {
                dialog.Progress = dialog.GetView().FindViewById<ProgressBar>(Android.Resource.Id.Progress);
                if (dialog.Progress == null) return;

                if (builder.IndeterminateProgress && !builder.IndeterminateIsHorizontalProgress &&
                        Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich &&
                        Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    dialog.Progress.IndeterminateDrawable = new CircularProgressDrawable(
                            builder.WidgetColor, builder.Context.Resources.GetDimension(Resource.Dimension.circular_progress_border));
                    MDTintHelper.SetTint(dialog.Progress, builder.WidgetColor, true);
                }
                else
                {
                    MDTintHelper.SetTint(dialog.Progress, builder.WidgetColor);
                }

                if (!builder.IndeterminateProgress || builder.IndeterminateIsHorizontalProgress)
                {
                    dialog.Progress.Indeterminate = builder.IndeterminateIsHorizontalProgress;
                    dialog.Progress.Progress = 0;
                    dialog.Progress.Max = builder.ProgressMax;
                    dialog.ProgressLabel = dialog.GetView().FindViewById<TextView>(Resource.Id.label);
                    if (dialog.ProgressLabel != null)
                    {
                        dialog.ProgressLabel.SetTextColor(builder.ContentColor);
                        dialog.SetTypeface(dialog.ProgressLabel, builder.MediumFont);
                        dialog.ProgressLabel.Text = String.Format(builder.ProgressPercentFormat, 0);
                    }
                    dialog.ProgressMinMax = dialog.GetView().FindViewById<TextView>(Resource.Id.minMax);
                    if (dialog.ProgressMinMax != null)
                    {
                        dialog.ProgressMinMax.SetTextColor(builder.ContentColor);
                        dialog.SetTypeface(dialog.ProgressMinMax, builder.RegularFont);

                        if (builder.ShowMinMax)
                        {
                            dialog.ProgressMinMax.Visibility = ViewStates.Visible;
                            dialog.ProgressMinMax.Text = String.Format(builder.ProgressNumberFormat,
                                    0, builder.ProgressMax);
                            ViewGroup.MarginLayoutParams lp = (ViewGroup.MarginLayoutParams)dialog.Progress.LayoutParameters;
                            lp.LeftMargin = 0;
                            lp.RightMargin = 0;
                        }
                        else
                        {
                            dialog.ProgressMinMax.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        builder.ShowMinMax = false;
                    }
                }
            }
        }

        private static void SetupInputDialog(MaterialDialog dialog)
        {
            MaterialDialog.Builder builder = dialog.MBuilder;
            dialog.Input = dialog.GetView().FindViewById<EditText>(Android.Resource.Id.Input);
            if (dialog.Input == null) return;
            dialog.SetTypeface(dialog.Input, builder.RegularFont);
            if (builder.InputPrefill != null)
                dialog.Input.Text = builder.InputPrefill;
            dialog.SetInternalInputCallback();
            dialog.Input.Hint = builder.InputHint;
            dialog.Input.SetSingleLine();
            dialog.Input.SetTextColor(builder.ContentColor);
            dialog.Input.SetHintTextColor(DialogUtils.AdjustAlpha(builder.ContentColor, 0.3f));
            MDTintHelper.SetTint(dialog.Input, dialog.MBuilder.WidgetColor);

            if (builder.InputType != Android.Text.InputTypes.Null)
            {
                dialog.Input.InputType = builder.InputType;
                if ((builder.InputType & Android.Text.InputTypes.TextVariationPassword) == Android.Text.InputTypes.TextVariationPassword)
                {
                    dialog.Input.TransformationMethod = PasswordTransformationMethod.Instance;
                }
            }

            dialog.InputMinMax = dialog.GetView().FindViewById<TextView>(Resource.Id.minMax);
            if (builder.InputMaxLength > -1)
            {
                dialog.InvalidateInputMinMaxIndicator(dialog.Input.Text.Length,
                        !builder.InputAllowEmpty);
            }
            else
            {
                dialog.InputMinMax.Visibility = ViewStates.Gone;
                dialog.InputMinMax = null;
            }
        }
    }
}