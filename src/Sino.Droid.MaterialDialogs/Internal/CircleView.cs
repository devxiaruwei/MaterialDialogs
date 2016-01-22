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
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.Support.V4.Content;
using Android.Graphics.Drawables.Shapes;

namespace Sino.Droid.MaterialDialogs.Internal
{
    public class CircleView : FrameLayout
    {
        private int borderWidthSmall;
        private int borderWidthLarge;

        private Paint outerPaint;
        private Paint whitePaint;
        private Paint innerPaint;
        private bool _selected;

        public CircleView(Context context)
            : this(context, null, 0) { }

        public CircleView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0) { }

        public CircleView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            borderWidthSmall = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 3, Resources.DisplayMetrics);
            borderWidthLarge = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 5, Resources.DisplayMetrics);

            whitePaint = new Paint();
            whitePaint.AntiAlias = true;
            whitePaint.Color = Color.White;
            
            innerPaint = new Paint();
            innerPaint.AntiAlias = true;
            
            outerPaint = new Paint();
            outerPaint.AntiAlias = true;

            Update(Color.DarkGray);
            SetWillNotDraw(false);
        }

        private void Update(Color color)
        {
            innerPaint.Color = color;
            outerPaint.Color = color;

            Drawable selector = CreateSelector(color);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                int[][] states = new int[][]{
                    new int[]{
                        Android.Resource.Attribute.StatePressed
                    }
                };
                int[] colors = new int[]{
                    ShiftColorUp(color)
                };
                ColorStateList rippleColors = new ColorStateList(states, colors);
                Foreground = new RippleDrawable(rippleColors, selector, null);
            }
            else
            {
                Foreground = selector;
            }
        }

        public override void SetBackgroundColor(Color color)
        {
            Update(color);
            RequestLayout();
            Invalidate();
        }

        public override void SetBackgroundResource(int resid)
        {
            SetBackgroundColor(Resources.GetColor(resid));
        }

        public override bool Activated
        {
            get
            {
                return base.Activated;
            }
        }

        public override bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                RequestLayout();
                Invalidate();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);

            if (widthMode == MeasureSpecMode.Exactly && heightMode != MeasureSpecMode.Exactly)
            {
                int width = MeasureSpec.GetSize(widthMeasureSpec);
                int height = width;

                if (heightMode == MeasureSpecMode.AtMost)
                {
                    height = Math.Min(height, MeasureSpec.GetSize(heightMeasureSpec));
                }
                SetMeasuredDimension(width, height);
            }
            else
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            int outerRadius = MeasuredWidth / 2;
            if (_selected)
            {
                int whiteRadius = outerRadius - borderWidthLarge;
                int innerRadius = whiteRadius - borderWidthSmall;
                canvas.DrawCircle(MeasuredWidth / 2,
                    MeasuredHeight / 2,
                    outerRadius,
                    outerPaint);

                canvas.DrawCircle(MeasuredWidth / 2,
                    MeasuredHeight / 2,
                    whiteRadius,
                    whitePaint);

                canvas.DrawCircle(MeasuredWidth / 2,
                    MeasuredHeight / 2,
                    innerRadius,
                    innerPaint);
            }
            else
            {
                canvas.DrawCircle(MeasuredWidth / 2,
                    MeasuredHeight / 2,
                    outerRadius,
                    innerPaint);
            }
        }

        private static Color TranslucentColor(Color color)
        {
            float factor = 0.7f;
            int alpha = (int)Math.Round((float)Color.GetAlphaComponent(color) * factor);
            int red = Color.GetRedComponent(color);
            int green = Color.GetGreenComponent(color);
            int blue = Color.GetBlueComponent(color);
            return Color.Argb(alpha, red, green, blue);
        }

        private Drawable CreateSelector(Color color)
        {
            ShapeDrawable darkerCircle = new ShapeDrawable(new OvalShape());
            darkerCircle.Paint.Color = TranslucentColor(ShiftColorUp(color));
            StateListDrawable stateListDrawable = new StateListDrawable();
            stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StatePressed }, darkerCircle);
            return stateListDrawable;
        }

        public static Color ShiftColor(Color color, float by)
        {
            if (by == 1)
                return color;
            float[] hsv = new float[3];
            Color.ColorToHSV(color, hsv);
            hsv[2] *= by;
            return Color.HSVToColor(hsv);
        }

        public static Color ShiftColor(Color color)
        {
            return ShiftColor(color, 0.9f);
        }

        public static Color ShiftColorUp(Color color)
        {
            return ShiftColor(color, 1.1f);
        }
    }
}