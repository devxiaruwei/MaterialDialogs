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
using Android.Support.V4.Content;

namespace Sino.Droid.MaterialDialogs.Simpelist
{
    public class MaterialSimpleListItem
    {
        private Builder _builder;

        private MaterialSimpleListItem(Builder builder)
        {
            _builder = builder;
        }

        public Drawable Icon
        {
            get
            {
                return _builder._icon;
            }
        }

        public string Content
        {
            get
            {
                return _builder._content;
            }
        }

        public class Builder
        {
            private Context _context;
            internal Drawable _icon;
            internal String _content;

            public Builder(Context context)
            {
                _context = context;
            }

            public Builder Icon(Drawable icon)
            {
                _icon = icon;
                return this;
            }

            public Builder ICon(int iconRes)
            {
                return Icon(ContextCompat.GetDrawable(_context, iconRes));
            }

            public Builder Content(string content)
            {
                _content = content;
                return this;
            }

            public Builder Content(int contentRes)
            {
                return Content(_context.GetString(contentRes));
            }

            public MaterialSimpleListItem Build()
            {
                return new MaterialSimpleListItem(this);
            }
        }

        public override string ToString()
        {
            if (Content != null)
                return Content;
            else
                return "{no content}";
        }
    }
}