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
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Graphics;
using Android.Animation;
using Android.Util;

namespace Sino.Droid.MaterialDialogs.Progress
{
    public class CircularProgressDrawable : Drawable, IAnimatable
    {
        private static IInterpolator ANGLE_INTERPOLATOR = new LinearInterpolator();
        private static IInterpolator SWEEP_INTERPOLATOR = new DecelerateInterpolator();
        private static int ANGLE_ANIMATOR_DURATION = 2000;
        private static int SWEEP_ANIMATOR_DURATION = 600;
        private static int MIN_SWEEP_ANGLE = 30;
        private RectF fBounds = new RectF();

        private ObjectAnimator _objectAnimatorSweep;
        private ObjectAnimator _objectAnimatorAngle;
        private bool _modeAppearing;
        private Paint _paint;
        private float _currentGlobalAngleOffset;
        private float _currentGlobalAnge;
        private float _currentSweepAngle;
        private float _borderWidth;
        private bool _running;

        public CircularProgressDrawable(Color color, float borderWidth)
        {
            _borderWidth = borderWidth;

            _paint = new Paint();
            _paint.AntiAlias = true;
            _paint.SetStyle(Paint.Style.Stroke);
            _paint.StrokeWidth = borderWidth;
            _paint.Color = color;

            SetupAnimations();
        }

        public override void Draw(Canvas canvas)
        {
            float startAngle = _currentGlobalAnge - _currentGlobalAngleOffset;
            float sweepAngle = _currentSweepAngle;
            if (!_modeAppearing)
            {
                startAngle = startAngle + sweepAngle;
                sweepAngle = 360 - sweepAngle - MIN_SWEEP_ANGLE;
            }
            else
            {
                sweepAngle += MIN_SWEEP_ANGLE;
            }
            canvas.DrawArc(fBounds, startAngle, sweepAngle, false, _paint);
        }

        public override int Alpha
        {
            get
            {
                return _paint.Alpha;
            }
            set
            {
                _paint.Alpha = value;
            }
        }

        public override void SetColorFilter(ColorFilter cf)
        {
            _paint.SetColorFilter(cf);
        }

        public override int Opacity
        {
            get
            {
                return (int)Format.Transparent;
            }
        }

        public bool IsRunning
        {
            get
            {
                return _running;
            }
        }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }
            _running = true;
            _objectAnimatorAngle.Start();
            _objectAnimatorSweep.Start();
            InvalidateSelf();
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }
            _running = false;
            _objectAnimatorAngle.Cancel();
            _objectAnimatorSweep.Cancel();
            InvalidateSelf();
        }

        protected override void OnBoundsChange(Rect bounds)
        {
            base.OnBoundsChange(bounds);
            fBounds.Left = bounds.Left + _borderWidth / 2f + 0.5f;
            fBounds.Right = bounds.Right - _borderWidth / 2f - 0.5f;
            fBounds.Top = bounds.Top + _borderWidth / 2f + 0.5f;
            fBounds.Bottom = bounds.Bottom - _borderWidth / 2f - 0.5f;
        }

        public float CurrentGlobalAngle
        {
            get
            {
                return _currentGlobalAnge;
            }
            set
            {
                _currentGlobalAnge = value;
                InvalidateSelf();
            }
        }

        public float CurrentSweepAngle
        {
            get
            {
                return _currentSweepAngle;
            }
            set
            {
                _currentSweepAngle = value;
                InvalidateSelf();
            }
        }

        private void ToggleAppearingMode()
        {
            _modeAppearing = !_modeAppearing;
            if (_modeAppearing)
            {
                _currentGlobalAngleOffset = (_currentGlobalAngleOffset + MIN_SWEEP_ANGLE * 2) % 360;
            }
        }

        private class AngleProperty : Property
        {
            public AngleProperty()
                : base(Java.Lang.Class.FromType(typeof(Java.Lang.Float)), "angle")
            {

            }

            public override Java.Lang.Object Get(Java.Lang.Object @object)
            {
                CircularProgressDrawable obj = (CircularProgressDrawable)@object;
                return obj.CurrentGlobalAngle;
            }

            public override void Set(Java.Lang.Object @object, Java.Lang.Object value)
            {
                CircularProgressDrawable obj = (CircularProgressDrawable)@object;
                obj.CurrentGlobalAngle = (float)value;
            }
        }

        private class SweepProperty : Property
        {
            public SweepProperty()
                : base(Java.Lang.Class.FromType(typeof(Java.Lang.Float)), "arc") { }

            public override Java.Lang.Object Get(Java.Lang.Object @object)
            {
                CircularProgressDrawable obj = (CircularProgressDrawable)@object;
                return obj.CurrentSweepAngle;
            }

            public override void Set(Java.Lang.Object @object, Java.Lang.Object value)
            {
                CircularProgressDrawable obj = (CircularProgressDrawable)@object;
                obj.CurrentSweepAngle = (float)value;
            }
        }

        private Property _angleProperty = new AngleProperty();
        private Property _sweepProperty = new SweepProperty();

        private void SetupAnimations()
        {
            _objectAnimatorAngle = ObjectAnimator.OfFloat(this, _angleProperty, 360f);
            _objectAnimatorAngle.SetInterpolator(ANGLE_INTERPOLATOR);
            _objectAnimatorAngle.SetDuration(ANGLE_ANIMATOR_DURATION);
            _objectAnimatorAngle.RepeatMode = ValueAnimatorRepeatMode.Restart;
            _objectAnimatorAngle.RepeatCount = ValueAnimator.Infinite;

            _objectAnimatorSweep = ObjectAnimator.OfFloat(this, _sweepProperty, 360f - MIN_SWEEP_ANGLE * 2);
            _objectAnimatorSweep.SetInterpolator(SWEEP_INTERPOLATOR);
            _objectAnimatorSweep.SetDuration(SWEEP_ANIMATOR_DURATION);
            _objectAnimatorSweep.RepeatMode = ValueAnimatorRepeatMode.Restart;
            _objectAnimatorSweep.RepeatCount = ValueAnimator.Infinite;
            _objectAnimatorSweep.AnimationRepeat += (e, s) =>
            {
                ToggleAppearingMode();
            };
        }

        public override void SetAlpha(int alpha)
        {
            _paint.Alpha = alpha;
        }
    }
}