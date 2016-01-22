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
using Android.Util;
using Android.Content.Res;
using Android.Webkit;
using Android.Support.V7.Widget;
using Sino.Droid.MaterialDialogs.Util;

namespace Sino.Droid.MaterialDialogs.Internal
{
    public class MDRootLayout : ViewGroup
    {
        private View mTitleBar;
        private View mContent;

        private static int INDEX_NEUTRAL = 0;
        private static int INDEX_NEGATIVE = 1;
        private static int INDEX_POSITIVE = 2;
        private bool mDrawTopDivider = false;
        private bool mDrawBottomDivider = false;
        private MDButton[] mButtons = new MDButton[3];
        private bool mForceStack = false;
        private bool mIsStacked = false;
        private bool mUseFullPadding = true;
        private bool mReducePaddingNoTitleNoButtons;
        private bool mNoTitleNoPadding;

        private int mNoTitlePaddingFull;
        private int mButtonPaddingFull;
        private int mButtonBarHeight;

        private GravityEnum mButtonGravity = GravityEnum.Start;

        private int mButtonHorizontalEdgeMargin;

        private Paint mDividerPaint;

        private ViewTreeObserver.IOnScrollChangedListener mTopOnScrollChangedListener;
        private ViewTreeObserver.IOnScrollChangedListener mBottomOnScrollChangedListener;
        private int mDividerWidth;

        public MDRootLayout(Context context)
            : base(context)
        {
            Init(context, null, 0);
        }

        public MDRootLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(context, attrs, 0);
        }

        public MDRootLayout(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Init(context, attrs, defStyleAttr);
        }

        public MDRootLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(context, attrs, defStyleAttr);
        }

        private void Init(Context context, IAttributeSet attrs, int defStyleAttr)
        {
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.MDRootLayout, defStyleAttr, 0);
            mReducePaddingNoTitleNoButtons = a.GetBoolean(Resource.Styleable.MDRootLayout_sino_droid_md_reduce_padding_no_title_no_buttons, true);
            a.Recycle();

            mNoTitlePaddingFull = context.Resources.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_notitle_vertical_padding);
            mButtonPaddingFull = context.Resources.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_button_frame_vertical_padding);

            mButtonHorizontalEdgeMargin = context.Resources.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_button_padding_frame_side);
            mButtonBarHeight = context.Resources.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_button_height);

            mDividerPaint = new Paint();
            mDividerWidth = context.Resources.GetDimensionPixelSize(Resource.Dimension.sino_droid_md_divider_height);
            mDividerPaint.Color = DialogUtils.ResolveColor(context, Resource.Attribute.sino_droid_md_divider_color);
            SetWillNotDraw(false);
        }

        public void NoTitleNoPadding()
        {
            mNoTitleNoPadding = true;
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            for (int i = 0; i < ChildCount; i++)
            {
                View v = GetChildAt(i);
                if (v.Id == Resource.Id.titleFrame)
                {
                    mTitleBar = v;
                }
                else if (v.Id == Resource.Id.buttonDefaultNeutral)
                {
                    mButtons[INDEX_NEUTRAL] = (MDButton)v;
                }
                else if (v.Id == Resource.Id.buttonDefaultNegative)
                {
                    mButtons[INDEX_NEGATIVE] = (MDButton)v;
                }
                else if (v.Id == Resource.Id.buttonDefaultPositive)
                {
                    mButtons[INDEX_POSITIVE] = (MDButton)v;
                }
                else
                {
                    mContent = v;
                }
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);

            mUseFullPadding = true;
            bool hasButtons = false;

            bool stacked;
            if (!mForceStack)
            {
                int buttonsWidth = 0;
                foreach (MDButton button in mButtons)
                {
                    if (button != null && IsVisible(button))
                    {
                        button.SetStacked(false, false);
                        MeasureChild(button, widthMeasureSpec, heightMeasureSpec);
                        buttonsWidth += button.MeasuredWidth;
                        hasButtons = true;
                    }
                }

                int buttonBarPadding = Context.Resources
                        .GetDimensionPixelSize(Resource.Dimension.sino_droid_md_neutral_button_margin);
                int buttonFrameWidth = width - 2 * buttonBarPadding;
                stacked = buttonsWidth > buttonFrameWidth;
            }
            else
            {
                stacked = true;
            }

            int stackedHeight = 0;
            mIsStacked = stacked;
            if (stacked)
            {
                foreach (MDButton button in mButtons)
                {
                    if (button != null && IsVisible(button))
                    {
                        button.SetStacked(true, false);
                        MeasureChild(button, widthMeasureSpec, heightMeasureSpec);
                        stackedHeight += button.MeasuredHeight;
                        hasButtons = true;
                    }
                }
            }

            int availableHeight = height;
            int fullPadding = 0;
            int minPadding = 0;
            if (hasButtons)
            {
                if (mIsStacked)
                {
                    availableHeight -= stackedHeight;
                    fullPadding += 2 * mButtonPaddingFull;
                    minPadding += 2 * mButtonPaddingFull;
                }
                else
                {
                    availableHeight -= mButtonBarHeight;
                    fullPadding += 2 * mButtonPaddingFull;
                }
            }
            else
            {
                fullPadding += 2 * mButtonPaddingFull;
            }

            if (IsVisible(mTitleBar))
            {
                mTitleBar.Measure(MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
                        (int)MeasureSpecMode.Unspecified);
                availableHeight -= mTitleBar.MeasuredHeight;
            }
            else if (!mNoTitleNoPadding)
            {
                fullPadding += mNoTitlePaddingFull;
            }

            if (IsVisible(mContent))
            {
                mContent.Measure(MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
                        MeasureSpec.MakeMeasureSpec(availableHeight - minPadding, MeasureSpecMode.AtMost));

                if (mContent.MeasuredHeight <= availableHeight - fullPadding)
                {
                    if (!mReducePaddingNoTitleNoButtons || IsVisible(mTitleBar) || hasButtons)
                    {
                        mUseFullPadding = true;
                        availableHeight -= mContent.MeasuredHeight + fullPadding;
                    }
                    else
                    {
                        mUseFullPadding = false;
                        availableHeight -= mContent.MeasuredHeight + minPadding;
                    }
                }
                else
                {
                    mUseFullPadding = false;
                    availableHeight = 0;
                }
            }
            SetMeasuredDimension(width, height - availableHeight);
        }

        private static bool IsVisible(View v)
        {
            bool visible = v != null && v.Visibility != ViewStates.Gone;
            if (visible && v is MDButton)
                visible = ((MDButton)v).Text.ToString().Trim().Length > 0;
            return visible;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (mContent != null)
            {
                if (mDrawTopDivider)
                {
                    int y = mContent.Top;
                    canvas.DrawRect(0, y - mDividerWidth, MeasuredWidth, y, mDividerPaint);
                }

                if (mDrawBottomDivider)
                {
                    int y = mContent.Bottom;
                    canvas.DrawRect(0, y, MeasuredWidth, y + mDividerWidth, mDividerPaint);
                }
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            if (IsVisible(mTitleBar))
            {
                int height = mTitleBar.MeasuredHeight;
                mTitleBar.Layout(l, t, r, t + height);
                t += height;
            }
            else if (!mNoTitleNoPadding && mUseFullPadding)
            {
                t += mNoTitlePaddingFull;
            }

            if (IsVisible(mContent))
                mContent.Layout(l, t, r, t + mContent.MeasuredHeight);

            if (mIsStacked)
            {
                b -= mButtonPaddingFull;
                foreach (MDButton mButton in mButtons)
                {
                    if (IsVisible(mButton))
                    {
                        mButton.Layout(l, b - mButton.MeasuredHeight, r, b);
                        b -= mButton.MeasuredHeight;
                    }
                }
            }
            else
            {
                int barTop;
                int barBottom = b;
                if (mUseFullPadding)
                    barBottom -= mButtonPaddingFull;
                barTop = barBottom - mButtonBarHeight;
                int offset = mButtonHorizontalEdgeMargin;

                int neutralLeft = -1;
                int neutralRight = -1;

                if (IsVisible(mButtons[INDEX_POSITIVE]))
                {
                    int bl, br;
                    if (mButtonGravity == GravityEnum.End)
                    {
                        bl = l + offset;
                        br = bl + mButtons[INDEX_POSITIVE].MeasuredWidth;
                    }
                    else
                    {
                        br = r - offset;
                        bl = br - mButtons[INDEX_POSITIVE].MeasuredWidth;
                        neutralRight = bl;
                    }
                    mButtons[INDEX_POSITIVE].Layout(bl, barTop, br, barBottom);
                    offset += mButtons[INDEX_POSITIVE].MeasuredWidth;
                }

                if (IsVisible(mButtons[INDEX_NEGATIVE]))
                {
                    int bl, br;
                    if (mButtonGravity == GravityEnum.End)
                    {
                        bl = l + offset;
                        br = bl + mButtons[INDEX_NEGATIVE].MeasuredWidth;
                    }
                    else if (mButtonGravity == GravityEnum.Start)
                    {
                        br = r - offset;
                        bl = br - mButtons[INDEX_NEGATIVE].MeasuredWidth;
                    }
                    else
                    {
                        bl = l + mButtonHorizontalEdgeMargin;
                        br = bl + mButtons[INDEX_NEGATIVE].MeasuredWidth;
                        neutralLeft = br;
                    }
                    mButtons[INDEX_NEGATIVE].Layout(bl, barTop, br, barBottom);
                }

                if (IsVisible(mButtons[INDEX_NEUTRAL]))
                {
                    int bl, br;
                    if (mButtonGravity == GravityEnum.End)
                    {
                        br = r - mButtonHorizontalEdgeMargin;
                        bl = br - mButtons[INDEX_NEUTRAL].MeasuredWidth;
                    }
                    else if (mButtonGravity == GravityEnum.Start)
                    {
                        bl = l + mButtonHorizontalEdgeMargin;
                        br = bl + mButtons[INDEX_NEUTRAL].MeasuredWidth;
                    }
                    else
                    {
                        if (neutralLeft == -1 && neutralRight != -1)
                        {
                            neutralLeft = neutralRight - mButtons[INDEX_NEUTRAL].MeasuredWidth;
                        }
                        else if (neutralRight == -1 && neutralLeft != -1)
                        {
                            neutralRight = neutralLeft + mButtons[INDEX_NEUTRAL].MeasuredWidth;
                        }
                        else if (neutralRight == -1)
                        {
                            neutralLeft = (r - l) / 2 - mButtons[INDEX_NEUTRAL].MeasuredWidth / 2;
                            neutralRight = neutralLeft + mButtons[INDEX_NEUTRAL].MeasuredWidth;
                        }
                        bl = neutralLeft;
                        br = neutralRight;
                    }

                    mButtons[INDEX_NEUTRAL].Layout(bl, barTop, br, barBottom);
                }
            }

            SetUpDividersVisibility(mContent, true, true);
        }

        public void SetForceStack(bool forceStack)
        {
            mForceStack = forceStack;
            Invalidate();
        }

        public void SetDividerColor(Color color)
        {
            mDividerPaint.Color = color;
            Invalidate();
        }

        public void SetButtonGravity(GravityEnum gravity)
        {
            mButtonGravity = gravity;
        }

        public void SetButtonStackedGravity(GravityEnum gravity)
        {
            foreach (MDButton mButton in mButtons)
            {
                if (mButton != null)
                    mButton.SetStackedGravity(gravity);
            }
        }

        private void SetUpDividersVisibility(View view, bool setForTop, bool setForBottom)
        {
            if (view == null)
                return;
            if (view is ScrollView)
            {
                ScrollView sv = (ScrollView)view;
                if (CanScrollViewScroll(sv))
                {
                    AddScrollListener(sv, setForTop, setForBottom);
                }
                else
                {
                    if (setForTop)
                        mDrawTopDivider = false;
                    if (setForBottom)
                        mDrawBottomDivider = false;
                }
            }
            else if (view is AdapterView)
            {
                AdapterView sv = (AdapterView)view;
                if (CanAdapterViewScroll(sv))
                {
                    AddScrollListener(sv, setForTop, setForBottom);
                }
                else
                {
                    if (setForTop)
                        mDrawTopDivider = false;
                    if (setForBottom)
                        mDrawBottomDivider = false;
                }
            }
            else if (view is WebView)
            {
                DelegatePreDrawListener listner = null;
                listner = new DelegatePreDrawListener
                {
                    PreDraw = () =>
                    {
                        if (view.MeasuredHeight != 0)
                        {
                            if (!CanWebViewScroll((WebView)view))
                            {
                                if (setForTop)
                                    mDrawTopDivider = false;
                                if (setForBottom)
                                    mDrawBottomDivider = false;
                            }
                            else
                            {
                                AddScrollListener((ViewGroup)view, setForTop, setForBottom);
                            }
                            view.ViewTreeObserver.RemoveOnPreDrawListener(listner);
                        }
                        return true;
                    }
                };
                view.ViewTreeObserver.AddOnPreDrawListener(listner);
            }
            else if (view is RecyclerView)
            {
                bool canScroll = CanRecyclerViewScroll((RecyclerView)view);
                if (setForTop)
                    mDrawTopDivider = canScroll;
                if (setForBottom)
                    mDrawBottomDivider = canScroll;
            }
            else if (view is ViewGroup)
            {
                View topView = GetTopView((ViewGroup)view);
                SetUpDividersVisibility(topView, setForTop, setForBottom);
                View bottomView = GetBottomView((ViewGroup)view);
                if (bottomView != topView)
                {
                    SetUpDividersVisibility(bottomView, false, true);
                }
            }
        }

        private void AddScrollListener(ViewGroup vg, bool setForTop, bool setForBottom)
        {
            if ((!setForBottom && mTopOnScrollChangedListener == null
                    || (setForBottom && mBottomOnScrollChangedListener == null)))
            {
                ViewTreeObserver.IOnScrollChangedListener onScrollChangedListener = new DelegateScrollChangeListener
                {
                    ScrollChanged = () =>
                    {
                        bool hasButtons = false;
                        foreach (MDButton button in mButtons)
                        {
                            if (button != null && button.Visibility != ViewStates.Gone)
                            {
                                hasButtons = true;
                                break;
                            }
                        }
                        if (vg is WebView)
                        {
                            InvalidateDividersForWebView((WebView)vg, setForTop, setForBottom, hasButtons);
                        }
                        else
                        {
                            InvalidateDividersForScrollingView(vg, setForTop, setForBottom, hasButtons);
                        }
                        Invalidate();
                    }
                };
                if (!setForBottom)
                {
                    mTopOnScrollChangedListener = onScrollChangedListener;
                    vg.ViewTreeObserver.AddOnScrollChangedListener(mTopOnScrollChangedListener);
                }
                else
                {
                    mBottomOnScrollChangedListener = onScrollChangedListener;
                    vg.ViewTreeObserver.AddOnScrollChangedListener(mBottomOnScrollChangedListener);
                }
                onScrollChangedListener.OnScrollChanged();
            }
        }

        private void InvalidateDividersForScrollingView(ViewGroup view, bool setForTop, bool setForBottom, bool hasButtons)
        {
            if (setForTop && view.ChildCount > 0)
            {
                mDrawTopDivider = mTitleBar != null &&
                        mTitleBar.Visibility != ViewStates.Gone && view.ScrollY + view.PaddingTop > view.GetChildAt(0).Top;
            }
            if (setForBottom && view.ChildCount > 0)
            {
                mDrawBottomDivider = hasButtons &&
                        view.ScrollY + view.Height - view.PaddingBottom < view.GetChildAt(view.ChildCount - 1).Bottom;
            }
        }

        private void InvalidateDividersForWebView(WebView view, bool setForTop, bool setForBottom, bool hasButtons)
        {
            if (setForTop)
            {
                mDrawTopDivider = mTitleBar != null &&
                        mTitleBar.Visibility != ViewStates.Gone && view.ScrollY + view.PaddingTop > 0;
            }
            if (setForBottom)
            {
                mDrawBottomDivider = hasButtons &&
                        view.ScrollY + view.MeasuredHeight - view.PaddingBottom < view.ContentHeight * view.Scale;
            }
        }

        public static bool CanRecyclerViewScroll(RecyclerView view)
        {
            if (view == null || view.GetAdapter() == null || view.GetLayoutManager() == null)
                return false;
            RecyclerView.LayoutManager lm = view.GetLayoutManager();
            int count = view.GetAdapter().ItemCount;
            int lastVisible;

            if (lm is LinearLayoutManager)
            {
                LinearLayoutManager llm = (LinearLayoutManager)lm;
                lastVisible = llm.FindLastVisibleItemPosition();
            }
            else
            {
                throw new InvalidOperationException("Material Dialogs currently only supports LinearLayoutManager. Please report any new layout managers.");
            }

            if (lastVisible == -1)
                return false;

            bool lastItemVisible = lastVisible == count - 1;
            return !lastItemVisible ||
                    (view.ChildCount > 0 && view.GetChildAt(view.ChildCount - 1).Bottom > view.Height - view.PaddingBottom);
        }

        private static bool CanScrollViewScroll(ScrollView sv)
        {
            if (sv.ChildCount == 0)
                return false;
            int childHeight = sv.GetChildAt(0).MeasuredHeight;
            return sv.MeasuredHeight - sv.PaddingTop - sv.PaddingBottom < childHeight;
        }

        private static bool CanWebViewScroll(WebView view)
        {
            return view.MeasuredHeight < view.ContentHeight * view.Scale;
        }

        private static bool CanAdapterViewScroll(AdapterView lv)
        {
            if (lv.LastVisiblePosition == -1)
                return false;
            bool firstItemVisible = lv.FirstVisiblePosition == 0;
            bool lastItemVisible = lv.LastVisiblePosition == lv.Count - 1;
            if (firstItemVisible && lastItemVisible && lv.ChildCount > 0)
            {

                if (lv.GetChildAt(0).Top < lv.PaddingTop)
                    return true;

                return lv.GetChildAt(lv.ChildCount - 1).Bottom >
                        lv.Height - lv.PaddingBottom;
            }
            return true;
        }

        private static View GetBottomView(ViewGroup viewGroup)
        {
            if (viewGroup == null || viewGroup.ChildCount == 0)
                return null;
            View bottomView = null;
            for (int i = viewGroup.ChildCount - 1; i >= 0; i--)
            {
                View child = viewGroup.GetChildAt(i);
                if (child.Visibility == ViewStates.Visible && child.Bottom == viewGroup.MeasuredHeight)
                {
                    bottomView = child;
                    break;
                }
            }
            return bottomView;
        }

        private static View GetTopView(ViewGroup viewGroup)
        {
            if (viewGroup == null || viewGroup.ChildCount == 0)
                return null;
            View topView = null;
            for (int i = viewGroup.ChildCount - 1; i >= 0; i--)
            {
                View child = viewGroup.GetChildAt(i);
                if (child.Visibility == ViewStates.Visible && child.Top == 0)
                {
                    topView = child;
                    break;
                }
            }
            return topView;
        }
    }
}