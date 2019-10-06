﻿using System.Windows;
using MaterialDesignColors;

namespace MaterialDesignThemes.Wpf
{
    public class BundledTheme : ResourceDictionary
    {
        private BaseTheme? _baseTheme;
        public BaseTheme? BaseTheme
        {
            get { return _baseTheme; }
            set
            {
                if (_baseTheme != value)
                {
                    _baseTheme = value;
                    SetTheme();
                }
            }
        }

        private PrimaryColor? _primaryColor;
        public PrimaryColor? PrimaryColor 
        {
            get { return _primaryColor; }
            set
            {
                if (_primaryColor != value)
                {
                    _primaryColor = value;
                    SetTheme();
                }
            }
        }

        private SecondaryColor? _secondaryColor;
        public SecondaryColor? SecondaryColor
        {
            get { return _secondaryColor; }
            set
            {
                if (_secondaryColor != value)
                {
                    _secondaryColor = value;
                    SetTheme();
                }
            }
        }

        private void SetTheme()
        {
            if (BaseTheme is BaseTheme baseTheme &&
                PrimaryColor is PrimaryColor primaryColor &&
                SecondaryColor is SecondaryColor secondaryColor)
            {
                var theme = Theme.Create(baseTheme.GetBaseTheme(),
                    SwatchHelper.Lookup[(MaterialDesignColor) primaryColor],
                    SwatchHelper.Lookup[(MaterialDesignColor) secondaryColor]);

                ApplyTheme(theme);
            }
        }

        protected virtual void ApplyTheme(ITheme theme)
        {
            this.SetTheme(theme);
        }
    }
}