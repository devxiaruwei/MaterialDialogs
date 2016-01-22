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
using Android.Graphics;
using Android.Graphics.Drawables;
using Sino.Droid.MaterialDialogs.Util;
using Android.Support.V4.Content.Res;
using Sino.Droid.MaterialDialogs.Internal;

namespace Sino.Droid.MaterialDialogs
{
    public class MaterialDialog : DialogBase, View.IOnClickListener, AdapterView.IOnItemClickListener
    {
        internal Builder MBuilder { get; set; }

        public FrameLayout CustomViewFrame { get; set; }

        internal ProgressBar Progress { get; set; }
        internal TextView ProgressLabel { get; set; }
        internal TextView ProgressMinMax { get; set; }
        internal EditText Input { get; set; }
        internal TextView InputMinMax { get; set; }
        public MDButton PositiveButton { get; set; }
        public MDButton NeutralButton { get; set; }
        public MDButton NegativeButton { get; set; }

        internal ListType ListType { get; set; }
        public List<int> SelectedIndicesList { get; set; }

        public TextView Title { get; set; }
        public ImageView Icon { get; set; }
        public View TitleFrame { get; set; }
        public TextView Content { get; set; }
        public ListView ListView { get; set; }

        public MaterialDialog(Builder builder)
            : base(builder.Context, DialogInit.GetTheme(builder))
        {
            mHandler = new Handler();
            MBuilder = builder;
            LayoutInflater inflater = LayoutInflater.From(builder.Context);
            view = (MDRootLayout)inflater.Inflate(DialogInit.GetInflateLayout(builder), null);
            DialogInit.Init(this);

            if (builder.Context.Resources.GetBoolean(Resource.Boolean.sino_droid_md_is_tablet))
            {
                WindowManagerLayoutParams lp = new WindowManagerLayoutParams();
                lp.CopyFrom(Window.Attributes);
                lp.Width = builder.Context.Resources.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_default_dialog_width);
                Window.Attributes = lp;
            }
        }

        public void SetTypeface(TextView target, Typeface t)
        {
            if (t == null) return;
            PaintFlags flags = target.PaintFlags | PaintFlags.SubpixelText;
            target.PaintFlags = flags;
            target.Typeface = t;
        }

        internal void CheckIfListInitScroll()
        {
            if (ListView == null)
                return;
            EventHandler eh = null;
            eh = (e, s) =>
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
                {
                    ListView.ViewTreeObserver.GlobalLayout -= eh;
                }
                else
                {
                    ListView.ViewTreeObserver.GlobalLayout -= eh;
                }

                if (ListType == ListType.Single || ListType == ListType.Multi)
                {
                    int selectedIndex;
                    if (ListType == ListType.Single)
                    {
                        if (MBuilder.SelectedIndex < 0)
                            return;
                        selectedIndex = MBuilder.SelectedIndex;
                    }
                    else
                    {
                        if (MBuilder.SelectedIndices == null || MBuilder.SelectedIndices.Length == 0)
                            return;
                        var indicesList = MBuilder.SelectedIndices.OrderBy(x => x).ToList();
                        selectedIndex = indicesList[0];
                    }

                    if (ListView.LastVisiblePosition < selectedIndex)
                    {
                        int totalVisibl = ListView.LastVisiblePosition - ListView.FirstVisiblePosition;
                        int scrollIndex = selectedIndex - (totalVisibl / 2);
                        if (scrollIndex < 0) scrollIndex = 0;
                        int fScrollIndex = scrollIndex;
                        ListView.Post(() =>
                        {
                            ListView.RequestFocus();
                            ListView.SetSelection(fScrollIndex);
                        });
                    }
                }
            };
            ListView.ViewTreeObserver.GlobalLayout += eh;
        }

        internal void InvalidateList()
        {
            if (ListView == null)
                return;
            else if ((MBuilder.Items == null || MBuilder.Items.Length == 0) && MBuilder.Adapter == null)
                return;

            ListView.Adapter = MBuilder.Adapter;
            if (ListType != MaterialDialogs.ListType.None || MBuilder.ListCallbackCustom != null)
                ListView.OnItemClickListener = this;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (MBuilder.ListCallbackCustom != null)
            {
                string text = null;
                if (view is TextView)
                {
                    text = ((TextView)view).Text;
                }
                MBuilder.ListCallbackCustom.OnSelection(this, view, position, text);
            }
            else if (ListType == MaterialDialogs.ListType.None || ListType == ListType.Regular)
            {
                if (MBuilder.AutoDismiss)
                {
                    Dismiss();
                }
                MBuilder.ListCallback.OnSelection(this, view, position, MBuilder.Items[position]);
            }
            else
            {
                if (ListType == ListType.Multi)
                {
                    bool shouldBeChecked = !SelectedIndicesList.Contains(position);
                    CheckBox cb = view.FindViewById<CheckBox>(Resource.Id.control);
                    if (shouldBeChecked)
                    {
                        SelectedIndicesList.Add(position);
                        if (MBuilder.AlwaysCallMultiChoiceCallback)
                        {
                            if (SendMultichoiceCallback())
                            {
                                cb.Checked = true;
                            }
                            else
                            {
                                SelectedIndicesList.Remove(position);
                            }
                        }
                        else
                        {
                            cb.Checked = true;
                        }
                    }
                    else
                    {
                        SelectedIndicesList.Remove(position);
                        cb.Checked = false;
                        if (MBuilder.AlwaysCallMultiChoiceCallback)
                        {
                            SendMultichoiceCallback();
                        }
                    }
                }
                else if (ListType == ListType.Single)
                {
                    bool allowSelection = true;
                    MaterialDialogAdapter adapter = (MaterialDialogAdapter)MBuilder.Adapter;
                    RadioButton radio = view.FindViewById<RadioButton>(Resource.Id.control);
                    if (MBuilder.AutoDismiss && MBuilder.PositiveText == null)
                    {
                        Dismiss();
                        allowSelection = false;
                        MBuilder.SelectedIndex = position;
                        SendSingleChoiceCallback(view);
                    }
                    else if (MBuilder.AlwaysCallSingleChoiceCallback)
                    {
                        int oldSelected = MBuilder.SelectedIndex;
                        MBuilder.SelectedIndex = position;
                        allowSelection = SendSingleChoiceCallback(view);
                        MBuilder.SelectedIndex = oldSelected;
                    }

                    if (allowSelection && MBuilder.SelectedIndex != position)
                    {
                        MBuilder.SelectedIndex = position;
                        if (adapter.RadioButton == null)
                        {
                            adapter.InitRadio = true;
                            adapter.NotifyDataSetChanged();
                        }
                        if (adapter.RadioButton != null)
                        {
                            adapter.RadioButton.Checked = false;
                        }
                        radio.Checked = true;
                        adapter.RadioButton = radio;
                    }
                }
            }
        }

        internal Drawable GetListSelector()
        {
            if (MBuilder.ListSelector != 0)
            {
                return ResourcesCompat.GetDrawable(MBuilder.Context.Resources, MBuilder.ListSelector, null);
            }
            Drawable d = DialogUtils.ResolveDrawable(MBuilder.Context, Resource.Attribute.sino_droid_md_list_selector);
            if (d != null)
                return d;
            return DialogUtils.ResolveDrawable(Context, Resource.Attribute.sino_droid_md_list_selector);
        }

        public Drawable GetButtonSelector(DialogAction which, bool isStacked)
        {
            if (isStacked)
            {
                if (MBuilder.BtnSelectorStacked != 0)
                {
                    return ResourcesCompat.GetDrawable(MBuilder.Context.Resources, MBuilder.BtnSelectorStacked, null);
                }
                Drawable d = DialogUtils.ResolveDrawable(MBuilder.Context, Resource.Attribute.sino_droid_md_btn_stacked_selector);
                if (d != null)
                    return d;
                return DialogUtils.ResolveDrawable(Context, Resource.Attribute.sino_droid_md_btn_stacked_selector);
            }
            else
            {
                switch (which)
                {
                    default:
                        {
                            if (MBuilder.BtnSelectorPositive != 0)
                            {
                                return ResourcesCompat.GetDrawable(MBuilder.Context.Resources, MBuilder.BtnSelectorPositive, null);
                            }
                            Drawable d = DialogUtils.ResolveDrawable(MBuilder.Context, Resource.Attribute.sino_droid_md_btn_positive_selector);
                            if (d != null)
                                return d;
                            return DialogUtils.ResolveDrawable(Context, Resource.Attribute.sino_droid_md_btn_positive_selector);
                        }
                    case DialogAction.Neutral:
                        {
                            if (MBuilder.BtnSelectorNeutral != 0)
                                return ResourcesCompat.GetDrawable(MBuilder.Context.Resources, MBuilder.BtnSelectorNeutral, null);
                            Drawable d = DialogUtils.ResolveDrawable(MBuilder.Context, Resource.Attribute.sino_droid_md_btn_neutral_selector);
                            if (d != null) return d;
                            return DialogUtils.ResolveDrawable(Context, Resource.Attribute.sino_droid_md_btn_neutral_selector);
                        }
                    case DialogAction.Negative:
                        {
                            if (MBuilder.BtnSelectorNegative != 0)
                                return ResourcesCompat.GetDrawable(MBuilder.Context.Resources, MBuilder.BtnSelectorNegative, null);
                            Drawable d = DialogUtils.ResolveDrawable(MBuilder.Context, Resource.Attribute.sino_droid_md_btn_negative_selector);
                            if (d != null) return d;
                            return DialogUtils.ResolveDrawable(Context, Resource.Attribute.sino_droid_md_btn_negative_selector);
                        }
                }
            }
        }

        private bool SendSingleChoiceCallback(View v)
        {
            string text = null;
            if (MBuilder.SelectedIndex >= 0)
            {
                text = MBuilder.Items[MBuilder.SelectedIndex];
            }
            return MBuilder.ListCallbackSingleChoice.OnSelection(this, v, MBuilder.SelectedIndex, text);
        }

        private bool SendMultichoiceCallback()
        {
            SelectedIndicesList = SelectedIndicesList.OrderBy(x => x).ToList();
            List<string> selectedTitles = new List<string>();
            foreach (int i in SelectedIndicesList)
            {
                selectedTitles.Add(MBuilder.Items[i]);
            }
            return MBuilder.ListCallbackMultiChoice.OnSelection(this,
                    SelectedIndicesList.ToArray(),
                    selectedTitles.ToArray());
        }

        public void OnClick(View v)
        {
            DialogAction tag = (DialogAction)(int)v.Tag;
            switch (tag)
            {
                case DialogAction.Positive:
                    {
                        if (MBuilder.Callback != null)
                        {
                            MBuilder.Callback.OnAny(this);
                            MBuilder.Callback.OnPositive(this);
                        }
                        if (MBuilder.ListCallbackSingleChoice != null)
                        {
                            SendSingleChoiceCallback(v);
                        }
                        if (MBuilder.ListCallbackMultiChoice != null)
                        {
                            SendMultichoiceCallback();
                        }
                        if (MBuilder.InputCallback != null && Input != null && !MBuilder.AlwaysCallInputCallback)
                        {
                            MBuilder.InputCallback.OnInput(this, Input.Text);
                        }
                        if (MBuilder.AutoDismiss)
                            Dismiss();
                    }
                    break;
                case DialogAction.Negative:
                    {
                        if (MBuilder.Callback != null)
                        {
                            MBuilder.Callback.OnAny(this);
                            MBuilder.Callback.OnNegative(this);
                        }
                        if (MBuilder.AutoDismiss)
                            Dismiss();
                    }
                    break;
                case DialogAction.Neutral:
                    {
                        if (MBuilder.Callback != null)
                        {
                            MBuilder.Callback.OnAny(this);
                            MBuilder.Callback.OnNeutral(this);
                        }
                        if (MBuilder.AutoDismiss)
                            Dismiss();
                    }
                    break;
            }
        }

        public View GetActionButton(DialogAction which)
        {
            switch (which)
            {
                default:
                    return view.FindViewById(Resource.Id.buttonDefaultPositive);
                case DialogAction.Neutral:
                    return view.FindViewById(Resource.Id.buttonDefaultNeutral);
                case DialogAction.Negative:
                    return view.FindViewById(Resource.Id.buttonDefaultNegative);
            }
        }

        public MDRootLayout GetView()
        {
            return view;
        }

        public ListView GetListView()
        {
            return ListView;
        }

        public EditText GetInputEditText()
        {
            return Input;
        }

        public TextView GetContentView()
        {
            return Content;
        }

        public View GetCustomView()
        {
            return MBuilder.CustomView;
        }

        public void SetActionButton(DialogAction which, string title)
        {
            switch (which)
            {
                default:
                    {
                        MBuilder.PositiveText = title;
                        PositiveButton.Text = title;
                        PositiveButton.Visibility = title == null ? ViewStates.Gone : ViewStates.Visible;
                    }
                    break;
                case DialogAction.Neutral:
                    {
                        MBuilder.NeutralText = title;
                        NeutralButton.Text = title;
                        NeutralButton.Visibility = title == null ? ViewStates.Gone : ViewStates.Visible;
                    }
                    break;
                case DialogAction.Negative:
                    {
                        MBuilder.NegativeText = title;
                        NegativeButton.Text = title;
                        NegativeButton.Visibility = title == null ? ViewStates.Gone : ViewStates.Visible;
                    }
                    break;
            }
        }

        public void SetActionButton(DialogAction which, int titleRes)
        {
            SetActionButton(which, Context.GetText(titleRes));
        }

        public bool HasActionButtons
        {
            get
            {
                return NumberOfActionButtons > 0;
            }
        }

        public int NumberOfActionButtons
        {
            get
            {
                int number = 0;
                if (MBuilder.PositiveText != null && PositiveButton.Visibility == ViewStates.Visible)
                    number++;
                if (MBuilder.NeutralText != null && NeutralButton.Visibility == ViewStates.Visible)
                    number++;
                if (MBuilder.NegativeText != null && NegativeButton.Visibility == ViewStates.Visible)
                    number++;
                return number;
            }
        }

        public override void SetTitle(Java.Lang.ICharSequence title)
        {
            Title.Text = title.ToString();
        }

        public override void SetTitle(int titleId)
        {
            SetTitle(MBuilder.Context.GetString(titleId));
        }

        public void SetTitle(int newTitleRes, params object[] formatArgs)
        {
            SetTitle(String.Format(MBuilder.Context.GetString(newTitleRes), formatArgs));
        }

        public void SetIcon(int resid)
        {
            Icon.SetImageResource(resid);
            Icon.Visibility = resid != 0 ? ViewStates.Visible : ViewStates.Gone;
        }

        public void SetIcon(Drawable d)
        {
            Icon.SetImageDrawable(d);
            Icon.Visibility = d != null ? ViewStates.Visible : ViewStates.Gone;
        }

        public void setIconAttribute(int attrId)
        {
            Drawable d = DialogUtils.ResolveDrawable(MBuilder.Context, attrId);
            SetIcon(d);
        }

        public void SetContent(string newContent)
        {
            Content.Text = newContent;
            Content.Visibility = String.IsNullOrEmpty(newContent) ? ViewStates.Gone : ViewStates.Visible;
        }

        public void setContent(int newContentRes)
        {
            SetContent(MBuilder.Context.GetString(newContentRes));
        }

        public void setContent(int newContentRes, params object[] formatArgs)
        {
            SetContent(String.Format(MBuilder.Context.GetString(newContentRes), formatArgs));
        }

        public void SetMessage(string message)
        {
            SetContent(message);
        }

        public void SetItems(string[] items)
        {
            if (MBuilder.Adapter == null)
                throw new InvalidOperationException("This MaterialDialog instance does not yet have an adapter set to it. You cannot use setItems().");
            MBuilder.Items = items;
            if (MBuilder.Adapter is MaterialDialogAdapter)
            {
                MBuilder.Adapter = new MaterialDialogAdapter(this, ListTypeExt.GetLayoutForType(ListType));
            }
            else
            {
                throw new InvalidOperationException("When using a custom adapter, setItems() cannot be used. Set items through the adapter instead.");
            }
            ListView.Adapter = MBuilder.Adapter;
        }

        public int GetCurrentProgress()
        {
            if (Progress == null) return -1;
            return Progress.Progress;
        }

        public ProgressBar GetProgressBar()
        {
            return Progress;
        }

        public void IncrementProgress(int by)
        {
            SetProgress(GetCurrentProgress() + by);
        }

        private Handler mHandler;

        public void SetProgress(int progress)
        {
            if (MBuilder.Progress <= -2)
                throw new InvalidOperationException("Cannot use setProgress() on this dialog.");
            Progress.Progress = progress;
            mHandler.Post(() =>
            {
                if (ProgressLabel != null)
                {
                    ProgressLabel.Text = String.Format(MBuilder.ProgressPercentFormat,
                            (float)GetCurrentProgress() / (float)GetMaxProgress());
                }
                if (ProgressMinMax != null)
                {
                    ProgressMinMax.Text = String.Format(MBuilder.ProgressNumberFormat,
                            GetCurrentProgress(), GetMaxProgress());
                }
            }
            );
        }

        public void SetMaxProgress(int max)
        {
            if (MBuilder.Progress <= -2)
                throw new InvalidOperationException("Cannot use setMaxProgress() on this dialog.");
            Progress.Max = max;
        }

        public bool IsIndeterminateProgress()
        {
            return MBuilder.IndeterminateProgress;
        }

        public int GetMaxProgress()
        {
            if (Progress == null) return -1;
            return Progress.Max;
        }

        public void SetProgressPercentFormat(string format)
        {
            MBuilder.ProgressPercentFormat = format;
            SetProgress(GetCurrentProgress());
        }

        public void SetProgressNumberFormat(String format)
        {
            MBuilder.ProgressNumberFormat = format;
            SetProgress(GetCurrentProgress());
        }

        public bool IsCancelled()
        {
            return !IsShowing;
        }

        public int GetSelectedIndex()
        {
            if (MBuilder.ListCallbackSingleChoice != null)
            {
                return MBuilder.SelectedIndex;
            }
            else
            {
                return -1;
            }
        }

        public int[] GetSelectedIndices()
        {
            if (MBuilder.ListCallbackMultiChoice != null)
            {
                return SelectedIndicesList.ToArray();
            }
            else
            {
                return null;
            }
        }

        public void SetSelectedIndex(int index)
        {
            MBuilder.SelectedIndex = index;
            if (MBuilder.Adapter != null && MBuilder.Adapter is MaterialDialogAdapter)
            {
                ((MaterialDialogAdapter)MBuilder.Adapter).NotifyDataSetChanged();
            }
            else
            {
                throw new InvalidOperationException("You can only use setSelectedIndex() with the default adapter implementation.");
            }
        }

        public void SetSelectedIndices(int[] indices)
        {
            MBuilder.SelectedIndices = indices;
            SelectedIndicesList = indices.ToList();
            if (MBuilder.Adapter != null && MBuilder.Adapter is MaterialDialogAdapter)
            {
                ((MaterialDialogAdapter)MBuilder.Adapter).NotifyDataSetChanged();
            }
            else
            {
                throw new InvalidOperationException("You can only use setSelectedIndices() with the default adapter implementation.");
            }
        }

        public void ClearSelectedIndices()
        {
            if (SelectedIndicesList == null)
                throw new InvalidOperationException("You can only use clearSelectedIndicies() with multi choice list dialogs.");
            MBuilder.SelectedIndices = null;
            SelectedIndicesList.Clear();
            if (MBuilder.Adapter != null && MBuilder.Adapter is MaterialDialogAdapter)
            {
                ((MaterialDialogAdapter)MBuilder.Adapter).NotifyDataSetChanged();
            }
            else
            {
                throw new InvalidOperationException("You can only use clearSelectedIndicies() with the default adapter implementation.");
            }
        }

        public new void OnShow(IDialogInterface dialog)
        {
            if (Input != null)
            {
                DialogUtils.ShowKeyboard(this, MBuilder);
                if (Input.Text.Length > 0)
                    Input.SetSelection(Input.Text.Length);
            }
            base.OnShow(dialog);
        }

        internal void SetInternalInputCallback()
        {
            if (Input == null) return;
            Input.TextChanged += (e, s) =>
            {
                int length = s.Text.Count();
                bool emptyDisabled = false;
                if (!MBuilder.InputAllowEmpty)
                {
                    emptyDisabled = length == 0;
                    View positiveAb = GetActionButton(DialogAction.Positive);
                    positiveAb.Enabled = !emptyDisabled;
                }
                InvalidateInputMinMaxIndicator(length, emptyDisabled);
                if (MBuilder.AlwaysCallInputCallback)
                    MBuilder.InputCallback.OnInput(this, s.Text.ToString());
            };
        }

        internal void InvalidateInputMinMaxIndicator(int currentLength, bool emptyDisabled)
        {
            if (InputMinMax != null)
            {
                InputMinMax.Text = currentLength + "/" + MBuilder.InputMaxLength;
                bool isDisabled = (emptyDisabled && currentLength == 0) || currentLength > MBuilder.InputMaxLength;
                int colorText = isDisabled ? MBuilder.InputMaxLengthErrorColor : MBuilder.ContentColor;
                int colorWidget = isDisabled ? MBuilder.InputMaxLengthErrorColor : MBuilder.WidgetColor;
                InputMinMax.SetTextColor(new Color(colorText));
                MDTintHelper.SetTint(Input, new Color(colorWidget));
                View positiveAb = GetActionButton(DialogAction.Positive);
                positiveAb.Enabled = !isDisabled;
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (Input != null)
            {
                DialogUtils.HideKeyboard(this, MBuilder);
            }
        }

        public class Builder
        {
            public Context Context { get; set; }
            public string Title { get; set; }
            public GravityEnum TitleGravity { get; set; }
            public GravityEnum ContentGravity { get; set; }
            public GravityEnum BtnStackedGravity { get; set; }
            public GravityEnum ItemsGravity { get; set; }
            public GravityEnum ButtonsGravity { get; set; }
            public Color TitleColor { get; set; }
            public Color ContentColor { get; set; }
            public string Content { get; set; }
            public string[] Items { get; set; }
            public string PositiveText { get; set; }
            public string NeutralText { get; set; }
            public string NegativeText { get; set; }
            public View CustomView { get; set; }
            public Color WidgetColor { get; set; }
            public ColorStateList PositiveColor { get; set; }
            public ColorStateList NegativeColor { get; set; }
            public ColorStateList NeutralColor { get; set; }
            public ButtonCallback Callback { get; set; }
            public IListCallback ListCallback { get; set; }
            public IListCallbackSingleChoice ListCallbackSingleChoice { get; set; }
            public IListCallbackMultiChoice ListCallbackMultiChoice { get; set; }
            public IListCallback ListCallbackCustom { get; set; }
            public bool AlwaysCallMultiChoiceCallback { get; set; }
            public bool AlwaysCallSingleChoiceCallback { get; set; }
            public Theme Theme { get; set; }
            public bool Cancelable { get; set; }
            public float ContentLineSpacingMultiplier { get; set; }
            public int SelectedIndex { get; set; }
            public int[] SelectedIndices { get; set; }
            public bool AutoDismiss { get; set; }
            public Typeface RegularFont { get; set; }
            public Typeface MediumFont { get; set; }
            public Drawable Icon { get; set; }
            public bool LimitIconToDefaultSize { get; set; }
            public int MaxIconSize { get; set; }
            public IListAdapter Adapter { get; set; }
            public IDialogInterfaceOnDismissListener DismissListener { get; set; }
            public IDialogInterfaceOnCancelListener CancelListener { get; set; }
            public IDialogInterfaceOnKeyListener KeyListener { get; set; }
            public IDialogInterfaceOnShowListener ShowListener { get; set; }
            public bool ForceStacking { get; set; }
            public bool WrapCustomViewInScroll { get; set; }
            public int DividerColor { get; set; }
            public int BackgroundColor { get; set; }
            public Color ItemColor { get; set; }
            public bool IndeterminateProgress { get; set; }
            public bool ShowMinMax { get; set; }
            public int Progress { get; set; }
            public int ProgressMax { get; set; }
            public string InputPrefill { get; set; }
            public string InputHint { get; set; }
            public IInputCallback InputCallback { get; set; }
            public bool InputAllowEmpty { get; set; }
            public Android.Text.InputTypes InputType { get; set; }
            public bool AlwaysCallInputCallback { get; set; }
            public int InputMaxLength { get; set; }
            public int InputMaxLengthErrorColor { get; set; }

            public String ProgressNumberFormat { get; set; }
            public string ProgressPercentFormat { get; set; }
            public bool IndeterminateIsHorizontalProgress { get; set; }

            public bool TitleColorSet { get; set; }
            public bool ContentColorSet { get; set; }
            public bool ItemColorSet { get; set; }
            public bool PositiveColorSet { get; set; }
            public bool NeutralColorSet { get; set; }
            public bool NegativeColorSet { get; set; }
            public bool WidgetColorSet { get; set; }
            public bool DividerColorSet { get; set; }

            public int ListSelector { get; set; }
            public int BtnSelectorStacked { get; set; }
            public int BtnSelectorPositive { get; set; }
            public int BtnSelectorNeutral { get; set; }
            public int BtnSelectorNegative { get; set; }

            public Builder(Context context)
            {
                TitleGravity = GravityEnum.Start;
                ContentGravity = GravityEnum.Start;
                BtnStackedGravity = GravityEnum.End;
                ItemsGravity = GravityEnum.Start;
                ButtonsGravity = GravityEnum.Start;
                Theme = MaterialDialogs.Theme.Light;
                Cancelable = true;
                ContentLineSpacingMultiplier = 1.2f;
                Context = context;
                SelectedIndex = -1;
                AutoDismiss = true;
                MaxIconSize = -1;
                Progress = -2;
                InputMaxLength = -1;

                int materialBlue = context.Resources.GetColor(Resource.Color.sino_droid_md_material_blue_600);

                WidgetColor = DialogUtils.ResolveColor(context, Resource.Attribute.colorAccent, materialBlue);
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    this.WidgetColor = DialogUtils.ResolveColor(context, Android.Resource.Attribute.ColorAccent, WidgetColor);
                }

                PositiveColor = DialogUtils.GetActionTextStateList(context, WidgetColor);
                NegativeColor = DialogUtils.GetActionTextStateList(context, WidgetColor);
                NeutralColor = DialogUtils.GetActionTextStateList(context, WidgetColor);

                ProgressPercentFormat = "{0}%";
                ProgressNumberFormat = "{0}/{1}";

                int primaryTextColor = DialogUtils.ResolveColor(context, Android.Resource.Attribute.TextColorPrimary);
                Theme = DialogUtils.IsColorDark(primaryTextColor) ? Theme.Light : Theme.Dark;

                CheckSingleton();

                TitleGravity = DialogUtils.ResolveGravityEnum(context, Resource.Attribute.sino_droid_md_title_gravity, TitleGravity);
                ContentGravity = DialogUtils.ResolveGravityEnum(context, Resource.Attribute.sino_droid_md_content_gravity, ContentGravity);
                BtnStackedGravity = DialogUtils.ResolveGravityEnum(context, Resource.Attribute.sino_droid_md_btnstacked_gravity, BtnStackedGravity);
                ItemsGravity = DialogUtils.ResolveGravityEnum(context, Resource.Attribute.sino_droid_md_items_gravity, ItemsGravity);
                ButtonsGravity = DialogUtils.ResolveGravityEnum(context, Resource.Attribute.sino_droid_md_buttons_gravity, ButtonsGravity);

                String mediumFont = DialogUtils.ResolveString(context, Resource.Attribute.sino_droid_md_medium_font);
                String regularFont = DialogUtils.ResolveString(context, Resource.Attribute.sino_droid_md_regular_font);
                SetTypeface(mediumFont, regularFont);

                if (MediumFont == null)
                {
                    try
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                            MediumFont = Typeface.Create("sans-serif-medium", TypefaceStyle.Normal);
                        else
                            MediumFont = Typeface.Create("sans-serif", TypefaceStyle.Bold);
                    }
                    catch (Exception) { }
                }
                if (RegularFont == null)
                {
                    try
                    {
                        RegularFont = Typeface.Create("sans-serif", TypefaceStyle.Normal);
                    }
                    catch (Exception) { }
                }
            }

            private void CheckSingleton()
            {
                if (ThemeSingleton.Get(false) == null) return;
                ThemeSingleton s = ThemeSingleton.Get();
                if (s.darkTheme)
                    Theme = Theme.Dark;
                if (s.TitleColor != null)
                    TitleColor = s.TitleColor;
                if (s.ContentColor != null)
                    ContentColor = s.ContentColor;
                if (s.positiveColor != null)
                    PositiveColor = s.positiveColor;
                if (s.neutralColor != null)
                    NeutralColor = s.neutralColor;
                if (s.negativeColor != null)
                    NegativeColor = s.negativeColor;
                if (s.ItemColor != null)
                    ItemColor = s.ItemColor;
                if (s.icon != null)
                    Icon = s.icon;
                if (s.backgroundColor != 0)
                    BackgroundColor = s.backgroundColor;
                if (s.dividerColor != 0)
                    DividerColor = s.dividerColor;
                if (s.btnSelectorStacked != 0)
                    BtnSelectorStacked = s.btnSelectorStacked;
                if (s.listSelector != 0)
                    ListSelector = s.listSelector;
                if (s.btnSelectorPositive != 0)
                    BtnSelectorPositive = s.btnSelectorPositive;
                if (s.btnSelectorNeutral != 0)
                    BtnSelectorNeutral = s.btnSelectorNeutral;
                if (s.btnSelectorNegative != 0)
                    BtnSelectorNegative = s.btnSelectorNegative;
                if (s.WidgetColor != null)
                    WidgetColor = s.WidgetColor;
                this.TitleGravity = s.titleGravity;
                this.ContentGravity = s.contentGravity;
                this.BtnStackedGravity = s.btnStackedGravity;
                this.ItemsGravity = s.itemsGravity;
                this.ButtonsGravity = s.buttonsGravity;
            }

            public Builder SetTitle(int titleRes)
            {
                SetTitle(this.Context.GetText(titleRes));
                return this;
            }

            public Builder SetTitle(string title)
            {
                this.Title = title;
                return this;
            }

            public Builder SetTitleGravity(GravityEnum gravity)
            {
                this.TitleGravity = gravity;
                return this;
            }

            public Builder SetTitleColor(Color color)
            {
                this.TitleColor = color;
                this.TitleColorSet = true;
                return this;
            }

            public Builder SetTitleColorRes(int colorRes)
            {
                SetTitleColor(this.Context.Resources.GetColor(colorRes));
                return this;
            }

            public Builder SetTitleColorAttr(int colorAttr)
            {
                SetTitleColor(DialogUtils.ResolveColor(this.Context, colorAttr));
                return this;
            }

            public Builder SetTypeface(Typeface medium, Typeface regular)
            {
                this.MediumFont = medium;
                this.RegularFont = regular;
                return this;
            }

            public Builder SetTypeface(String medium, String regular)
            {
                if (medium != null)
                {
                    this.MediumFont = TypefaceHelper.Get(this.Context, medium);
                    if (this.MediumFont == null)
                        throw new InvalidOperationException("No font asset found for " + medium);
                }
                if (regular != null)
                {
                    this.RegularFont = TypefaceHelper.Get(this.Context, regular);
                    if (this.RegularFont == null)
                        throw new InvalidOperationException("No font asset found for " + regular);
                }
                return this;
            }

            public Builder SetIcon(Drawable icon)
            {
                this.Icon = icon;
                return this;
            }

            public Builder SetIcon(int icon)
            {
                this.Icon = Context.Resources.GetDrawable(icon);
                return this;
            }

            public Builder SetIconAttr(int iconAttr)
            {
                this.Icon = DialogUtils.ResolveDrawable(Context, iconAttr);
                return this;
            }

            public Builder SetContent(int contentRes)
            {
                SetContent(this.Context.GetText(contentRes));
                return this;
            }

            public Builder SetContent(string content)
            {
                if (this.CustomView != null)
                    throw new InvalidOperationException("You cannot set content() when you're using a custom view.");
                this.Content = content;
                return this;
            }

            public Builder SetContent(int contentRes, params Java.Lang.Object[] formatArgs)
            {
                SetContent(this.Context.GetString(contentRes, formatArgs));
                return this;
            }

            public Builder SetContentColor(Color color)
            {
                this.ContentColor = color;
                this.ContentColorSet = true;
                return this;
            }

            public Builder SetContentColorRes(int colorRes)
            {
                SetContentColor(this.Context.Resources.GetColor(colorRes));
                return this;
            }

            public Builder SetContentColorAttr(int colorAttr)
            {
                SetContentColor(DialogUtils.ResolveColor(this.Context, colorAttr));
                return this;
            }

            public Builder SetContentGravity(GravityEnum gravity)
            {
                this.ContentGravity = gravity;
                return this;
            }

            public Builder SetContentLineSpacing(float multiplier)
            {
                this.ContentLineSpacingMultiplier = multiplier;
                return this;
            }

            public Builder SetItems(int itemsRes)
            {
                SetItems(this.Context.Resources.GetTextArray(itemsRes));
                return this;
            }

            public Builder SetItems(string[] items)
            {
                if (this.CustomView != null)
                    throw new InvalidOperationException("You cannot set items() when you're using a custom view.");
                this.Items = items;
                return this;
            }

            public Builder SetItemsCallback(ListCallback callback)
            {
                return this.SetItemsCallback((IListCallback)callback);
            }

            public Builder SetItemsCallback(IListCallback callback)
            {
                this.ListCallback = callback;
                this.ListCallbackSingleChoice = null;
                this.ListCallbackMultiChoice = null;
                return this;
            }

            public Builder SetItemColor(Color color)
            {
                this.ItemColor = color;
                this.ItemColorSet = true;
                return this;
            }

            public Builder SetItemColorRes(int colorRes)
            {
                return SetItemColor(this.Context.Resources.GetColor(colorRes));
            }

            public Builder SetItemColorAttr(int colorAttr)
            {
                return SetItemColor(DialogUtils.ResolveColor(this.Context, colorAttr));
            }

            public Builder SetItemsGravity(GravityEnum gravity)
            {
                this.ItemsGravity = gravity;
                return this;
            }

            public Builder SetButtonsGravity(GravityEnum gravity)
            {
                this.ButtonsGravity = gravity;
                return this;
            }

            public Builder SetItemsCallbackSingleChoice(int selectedIndex, ListCallbackSingleChoice callback)
            {
                return SetItemsCallbackSingleChoice(selectedIndex, (IListCallbackSingleChoice)callback);
            }

            public Builder SetItemsCallbackSingleChoice(int selectedIndex, IListCallbackSingleChoice callback)
            {
                SelectedIndex = selectedIndex;
                ListCallback = null;
                ListCallbackSingleChoice = callback;
                ListCallbackMultiChoice = null;
                return this;
            }

            public Builder SetAlwaysCallSingleChoiceCallback()
            {
                AlwaysCallSingleChoiceCallback = true;
                return this;
            }

            public Builder SetItemsCallbackMultiChoice(int[] selectedIndices, ListCallbackMultiChoice callback)
            {
                return SetItemsCallbackMultiChoice(selectedIndices, (IListCallbackMultiChoice)callback);
            }

            public Builder SetItemsCallbackMultiChoice(int[] selectedIndices, IListCallbackMultiChoice callback)
            {
                SelectedIndices = selectedIndices;
                ListCallback = null;
                ListCallbackSingleChoice = null;
                ListCallbackMultiChoice = callback;
                return this;
            }

            public Builder SetAlwaysCallMultiChoiceCallback()
            {
                AlwaysCallMultiChoiceCallback = true;
                return this;
            }

            public Builder SetPositiveText(int postiveRes)
            {
                SetPositiveText(Context.GetText(postiveRes));
                return this;
            }

            public Builder SetPositiveText(string message)
            {
                PositiveText = message;
                return this;
            }

            public Builder SetPositiveColor(Color color)
            {
                return SetPositiveColor(DialogUtils.GetActionTextStateList(Context, color));
            }

            public Builder SetPositiveColorRes(int colorRes)
            {
                return SetPositiveColor(DialogUtils.GetActionTextColorStateList(Context, colorRes));
            }

            public Builder SetPositiveColorAttr(int colorAttr)
            {
                return SetPositiveColor(DialogUtils.ResolveActionTextColorStateList(Context, colorAttr, null));
            }

            public Builder SetPositiveColor(ColorStateList colorStateList)
            {
                PositiveColor = colorStateList;
                PositiveColorSet = true;
                return this;
            }

            public Builder SetNeutralText(int neutralRes)
            {
                return SetNeutralText(Context.GetText(neutralRes));
            }

            public Builder SetNeutralText(string message)
            {
                NeutralText = message;
                return this;
            }

            public Builder SetNegativeColor(Color color)
            {
                return SetNegativeColor(DialogUtils.GetActionTextStateList(Context, color));
            }

            public Builder SetNegativeColorRes(int colorRes)
            {
                return SetNegativeColor(DialogUtils.GetActionTextColorStateList(Context, colorRes));
            }

            public Builder SetNegativeColorAttr(int colorAttr)
            {
                return SetNegativeColor(DialogUtils.ResolveActionTextColorStateList(Context, colorAttr, null));
            }

            public Builder SetNegativeColor(ColorStateList colorStateList)
            {
                NegativeColor = colorStateList;
                NegativeColorSet = true;
                return this;
            }

            public Builder SetNegativeText(int negativeRes)
            {
                return SetNegativeText(Context.GetText(negativeRes));
            }

            public Builder SetNegativeText(string message)
            {
                NegativeText = message;
                return this;
            }

            public Builder SetNeutralColor(Color color)
            {
                return SetNeutralColor(DialogUtils.GetActionTextStateList(Context, color));
            }

            public Builder SetNeutralColorRes(int colorRes)
            {
                return SetNeutralColor(DialogUtils.GetActionTextColorStateList(Context, colorRes));
            }

            public Builder SetNeutralColorAttr(int colorAttr)
            {
                return SetNeutralColor(DialogUtils.ResolveActionTextColorStateList(Context, colorAttr, null));
            }

            public Builder SetNeutralColor(ColorStateList colorStateList)
            {
                NeutralColor = colorStateList;
                NeutralColorSet = true;
                return this;
            }

            public Builder SetListSelector(int selectorRes)
            {
                ListSelector = selectorRes;
                return this;
            }

            public Builder SetBtnSelectorStacked(int selectorRes)
            {
                BtnSelectorStacked = selectorRes;
                return this;
            }

            public Builder SetBtnSelector(int selectorRes)
            {
                BtnSelectorPositive = selectorRes;
                BtnSelectorNeutral = selectorRes;
                BtnSelectorNegative = selectorRes;
                return this;
            }

            public Builder SetBtnSelector(int selectorRes, DialogAction which)
            {
                switch (which)
                {
                    default:
                        this.BtnSelectorPositive = selectorRes;
                        break;
                    case DialogAction.Neutral:
                        this.BtnSelectorNeutral = selectorRes;
                        break;
                    case DialogAction.Negative:
                        this.BtnSelectorNegative = selectorRes;
                        break;
                }
                return this;
            }

            public Builder SetBtnStackedGravity(GravityEnum gravity)
            {
                this.BtnStackedGravity = gravity;
                return this;
            }

            public Builder SetCustomView(int layoutRes, bool wrapInScrollView)
            {
                LayoutInflater li = LayoutInflater.From(Context);
                return SetCustomView(li.Inflate(layoutRes, null), wrapInScrollView);
            }

            public Builder SetCustomView(View view, bool wrapInScrollView)
            {
                if (this.Content != null)
                    throw new InvalidOperationException("You cannot use customView() when you have content set.");
                else if (this.Items != null)
                    throw new InvalidOperationException("You cannot use customView() when you have items set.");
                else if (this.InputCallback != null)
                    throw new InvalidOperationException("You cannot use customView() with an input dialog");
                else if (this.Progress > -2 || this.IndeterminateProgress)
                    throw new InvalidOperationException("You cannot use customView() with a progress dialog");
                if (view.Parent != null && view.Parent is ViewGroup)
                    ((ViewGroup)view.Parent).RemoveView(view);
                this.CustomView = view;
                this.WrapCustomViewInScroll = wrapInScrollView;
                return this;
            }

            public Builder SetProgress(bool indeterminate, int max)
            {
                if (this.CustomView != null)
                    throw new InvalidOperationException("You cannot set progress() when you're using a custom view.");
                if (indeterminate)
                {
                    this.IndeterminateProgress = true;
                    this.Progress = -2;
                }
                else
                {
                    this.IndeterminateProgress = false;
                    this.Progress = -1;
                    this.ProgressMax = max;
                }
                return this;
            }

            public Builder SetProgress(bool indeterminate, int max, bool showMinMax)
            {
                this.ShowMinMax = showMinMax;
                return SetProgress(indeterminate, max);
            }

            public Builder SetProgressNumberFormat(String format)
            {
                this.ProgressNumberFormat = format;
                return this;
            }

            public Builder SetProgressPercentFormat(string format)
            {
                this.ProgressPercentFormat = format;
                return this;
            }


            public Builder SetProgressIndeterminateStyle(bool horizontal)
            {
                this.IndeterminateIsHorizontalProgress = horizontal;
                return this;
            }

            public Builder SetWidgetColor(Color color)
            {
                this.WidgetColor = color;
                this.WidgetColorSet = true;
                return this;
            }

            public Builder SetWidgetColorRes(int colorRes)
            {
                return SetWidgetColor(Context.Resources.GetColor(colorRes));
            }

            public Builder SetWidgetColorAttr(int colorAttr)
            {
                return SetWidgetColorRes(DialogUtils.ResolveColor(Context, colorAttr));
            }

            public Builder SetDividerColor(int color)
            {
                this.DividerColor = color;
                this.DividerColorSet = true;
                return this;
            }

            public Builder SetDividerColorRes(int colorRes)
            {
                return SetDividerColor(Context.Resources.GetColor(colorRes));
            }

            public Builder SetDividerColorAttr(int colorAttr)
            {
                return SetDividerColor(DialogUtils.ResolveColor(Context, colorAttr));
            }

            public Builder SetBackgroundColor(int color)
            {
                this.BackgroundColor = color;
                return this;
            }

            public Builder SetBackgroundColorRes(int colorRes)
            {
                return SetBackgroundColor(Context.Resources.GetColor(colorRes));
            }

            public Builder SetBackgroundColorAttr(int colorAttr)
            {
                return SetBackgroundColor(DialogUtils.ResolveColor(Context, colorAttr));
            }

            public Builder SetCallback(ButtonCallback callback)
            {
                this.Callback = callback;
                return this;
            }

            public Builder SetTheme(Theme theme)
            {
                this.Theme = theme;
                return this;
            }

            public Builder SetCancelable(bool cancelable)
            {
                this.Cancelable = cancelable;
                return this;
            }

            public Builder SetAutoDismiss(bool dismiss)
            {
                this.AutoDismiss = dismiss;
                return this;
            }

            public Builder SetAdapter(IListAdapter adapter, IListCallback callback)
            {
                if (this.CustomView != null)
                    throw new InvalidOperationException("You cannot set adapter() when you're using a custom view.");
                this.Adapter = adapter;
                this.ListCallbackCustom = callback;
                return this;
            }

            public Builder SLimitIconToDefaultSize()
            {
                this.LimitIconToDefaultSize = true;
                return this;
            }

            public Builder SetMaxIconSize(int maxIconSize)
            {
                this.MaxIconSize = maxIconSize;
                return this;
            }

            public Builder SetMaxIconSizeRes(int maxIconSizeRes)
            {
                return SetMaxIconSize((int)Context.Resources.GetDimension(maxIconSizeRes));
            }

            public Builder SetShowListener(IDialogInterfaceOnShowListener listener)
            {
                this.ShowListener = listener;
                return this;
            }

            public Builder SetDismissListener(IDialogInterfaceOnDismissListener listener)
            {
                this.DismissListener = listener;
                return this;
            }

            public Builder SetCancelListener(IDialogInterfaceOnCancelListener listener)
            {
                this.CancelListener = listener;
                return this;
            }

            public Builder SetKeyListener(IDialogInterfaceOnKeyListener listener)
            {
                this.KeyListener = listener;
                return this;
            }

            public Builder SetForceStacking(bool stacked)
            {
                this.ForceStacking = stacked;
                return this;
            }

            public Builder SetInput(string hint, string prefill, bool allowEmptyInput, InputCallback callback)
            {
                return SetInput(hint, prefill, allowEmptyInput, (IInputCallback)callback);
            }

            public Builder SetInput(string hint, string prefill, bool allowEmptyInput, IInputCallback callback)
            {
                if (this.CustomView != null)
                    throw new InvalidOperationException("You cannot set content() when you're using a custom view.");
                this.InputCallback = callback;
                this.InputHint = hint;
                this.InputPrefill = prefill;
                this.InputAllowEmpty = allowEmptyInput;
                return this;
            }

            public Builder SetInput(string hint, string prefill, InputCallback callback)
            {
                return SetInput(hint, prefill, (IInputCallback)callback);
            }

            public Builder SetInput(string hint, string prefill, IInputCallback callback)
            {
                return SetInput(hint, prefill, true, callback);
            }

            public Builder SetInput(int hint, int prefill, bool allowEmptyInput, InputCallback callback)
            {
                return SetInput(hint, prefill, allowEmptyInput, (IInputCallback)callback);
            }

            public Builder SetInput(int hint, int prefill, bool allowEmptyInput, IInputCallback callback)
            {
                return SetInput(hint == 0 ? null : Context.GetText(hint), prefill == 0 ? null : Context.GetText(prefill), allowEmptyInput, callback);
            }

            public Builder SetInput(int hint, int prefill, InputCallback callback)
            {
                return SetInput(hint, prefill, (IInputCallback)callback);
            }

            public Builder SetInput(int hint, int prefill, IInputCallback callback)
            {
                return SetInput(hint, prefill, true, callback);
            }

            public Builder SetInputType(Android.Text.InputTypes type)
            {
                this.InputType = type;
                return this;
            }

            public Builder SetInputMaxLength(int maxLength)
            {
                return SetInputMaxLength(maxLength, 0);
            }

            public Builder SetInputMaxLength(int maxLength, int errorColor)
            {
                if (maxLength < 1)
                    throw new InvalidOperationException("Max length for input dialogs cannot be less than 1.");
                this.InputMaxLength = maxLength;
                if (errorColor == 0)
                {
                    InputMaxLengthErrorColor = Context.Resources.GetColor(Resource.Color.sino_droid_md_edittext_error);
                }
                else
                {
                    this.InputMaxLengthErrorColor = errorColor;
                }
                return this;
            }

            public Builder SetInputMaxLengthRes(int maxLength, int errorColor)
            {
                return SetInputMaxLength(maxLength, Context.Resources.GetColor(errorColor));
            }

            public Builder SetAlwaysCallInputCallback()
            {
                this.AlwaysCallInputCallback = true;
                return this;
            }

            public MaterialDialog Build()
            {
                return new MaterialDialog(this);
            }

            public MaterialDialog Show()
            {
                MaterialDialog dialog = Build();
                dialog.Show();
                return dialog;
            }
        }
    }
}