using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Styles.Controls
{
    public class DynamicTabControl : TabControl
    {
        #region Dependency Properties
        // Using a DependencyProperty as the backing store for IndicatorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorWidthProperty =
            DependencyProperty.Register("IndicatorWidth", typeof(double), typeof(DynamicTabControl));

        // Using a DependencyProperty as the backing store for IndicatorHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorHeightProperty =
            DependencyProperty.Register("IndicatorHeight", typeof(double), typeof(DynamicTabControl));

        // Using a DependencyProperty as the backing store for IndicatorTranslateX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorTranslateXProperty =
            DependencyProperty.Register("IndicatorTranslateX", typeof(double), typeof(DynamicTabControl));

        // Using a DependencyProperty as the backing store for IndicatorTranslateY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorTranslateYProperty =
            DependencyProperty.Register("IndicatorTranslateY", typeof(double), typeof(DynamicTabControl));

        // Using a DependencyProperty as the backing store for TabsHorizontalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabsHorizontalAlignmentProperty =
            DependencyProperty.Register("TabsHorizontalAlignment", typeof(HorizontalAlignment), typeof(DynamicTabControl));

        // Using a DependencyProperty as the backing store for TabsVerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabsVerticalAlignmentProperty =
            DependencyProperty.Register("TabsVerticalAlignment", typeof(VerticalAlignment), typeof(DynamicTabControl));

        // Using a DependencyProperty as the backing store for IndicatorBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorBrushProperty =
            DependencyProperty.Register("IndicatorBrush", typeof(Brush), typeof(DynamicTabControl));

        public double IndicatorWidth
        {
            get => (double)GetValue(IndicatorWidthProperty);
            set => SetValue(IndicatorWidthProperty, value);
        }

        public double IndicatorHeight
        {
            get => (double)GetValue(IndicatorHeightProperty);
            set => SetValue(IndicatorHeightProperty, value);
        }

        public double IndicatorTranslateX
        {
            get => (double)GetValue(IndicatorTranslateXProperty);
            set => SetValue(IndicatorTranslateXProperty, value);
        }

        public double IndicatorTranslateY
        {
            get => (double)GetValue(IndicatorTranslateYProperty);
            set => SetValue(IndicatorTranslateYProperty, value);
        }

        public HorizontalAlignment TabsHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(TabsHorizontalAlignmentProperty);
            set => SetValue(TabsHorizontalAlignmentProperty, value);
        }

        public VerticalAlignment TabsVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(TabsVerticalAlignmentProperty);
            set => SetValue(TabsVerticalAlignmentProperty, value);
        }

        public Brush IndicatorBrush
        {
            get => (Brush)GetValue(IndicatorBrushProperty);
            set => SetValue(IndicatorBrushProperty, value);
        }
        #endregion

        #region Fields
        private readonly TimeSpan _animDuration;
        private readonly IEasingFunction _easingFunction;
        #endregion

        #region Constructors
        public DynamicTabControl() : base()
        {
            TabStripPlacementProperty.OverrideMetadata(
                typeof(DynamicTabControl),
                new FrameworkPropertyMetadata(OnTabStripPlacementPropertyChanged));

            _animDuration = TimeSpan.FromMilliseconds(250);
            _easingFunction = new CubicEase()
            {
                EasingMode = EasingMode.EaseInOut
            };

            Loaded += (s, e) => Animate();
        }
        #endregion

        #region Methods
        private async void OnTabStripPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Wait for the layout to change to ensure correct calculations

            await Task.Delay(100);

            switch (e.NewValue)
            {
                // https://stackoverflow.com/questions/3733760/unable-to-set-a-property-after-its-animation-in-wpf
                case Dock.Left or Dock.Right:
                    BeginAnimation(IndicatorWidthProperty, null);
                    BeginAnimation(IndicatorTranslateXProperty, null);

                    SetValue(IndicatorWidthProperty, 2.0);
                    SetValue(IndicatorTranslateXProperty, 0.0);
                    break;
                case Dock.Top or Dock.Bottom:
                    BeginAnimation(IndicatorHeightProperty, null);
                    BeginAnimation(IndicatorTranslateYProperty, null);

                    SetValue(IndicatorHeightProperty, 2.0);
                    SetValue(IndicatorTranslateYProperty, 0.0);
                    break;
            }

            Animate();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            Animate();
        }

        private void Animate()
        {
            if (SelectedItem is null) return;

            switch (TabStripPlacement)
            {
                case Dock.Left or Dock.Right:
                    AnimateY();
                    break;
                case Dock.Top or Dock.Bottom:
                    AnimateX();
                    break;
            }
        }

        private void AnimateX()
        {
            TabItem selectedTab = SelectedItem as TabItem;

            // Calculate header width
            double width = selectedTab.ActualWidth - (selectedTab.Padding.Left + selectedTab.Padding.Right);

            if (width < 0) return;

            // Calculate total width of the preceding tabs and the current tab padding
            double translateX = 0.0;

            for (int i = 0; i < SelectedIndex; i++)
            {
                translateX += (Items[i] as TabItem).ActualWidth;
            }

            translateX += selectedTab.Padding.Left;

            DoubleAnimation widthAnim = new(IndicatorWidth, width, _animDuration)
            {
                EasingFunction = _easingFunction
            };

            DoubleAnimation translateXAnim = new(IndicatorTranslateX, translateX, _animDuration)
            {
                EasingFunction = _easingFunction
            };

            // https://stackoverflow.com/questions/3733760/unable-to-set-a-property-after-its-animation-in-wpf
            BeginAnimation(IndicatorWidthProperty, null);
            BeginAnimation(IndicatorTranslateXProperty, null);

            BeginAnimation(IndicatorWidthProperty, widthAnim);
            BeginAnimation(IndicatorTranslateXProperty, translateXAnim);
        }

        private void AnimateY()
        {
            TabItem selectedTab = SelectedItem as TabItem;

            // Calculate header height
            double height = selectedTab.ActualHeight - (selectedTab.Padding.Top + selectedTab.Padding.Bottom);

            if (height < 0) return;

            // Calculate total height of the preceding tabs and the current tab padding
            double translateY = 0.0;

            for (int i = 0; i < SelectedIndex; i++)
            {
                translateY += (Items[i] as TabItem).ActualHeight;
            }

            translateY += selectedTab.Padding.Top;

            DoubleAnimation heightAnim = new(IndicatorHeight, height, _animDuration)
            {
                EasingFunction = _easingFunction
            };

            DoubleAnimation translateYAnim = new(IndicatorTranslateY, translateY, _animDuration)
            {
                EasingFunction = _easingFunction
            };

            // https://stackoverflow.com/questions/3733760/unable-to-set-a-property-after-its-animation-in-wpf
            BeginAnimation(IndicatorHeightProperty, null);
            BeginAnimation(IndicatorTranslateYProperty, null);

            BeginAnimation(IndicatorHeightProperty, heightAnim);
            BeginAnimation(IndicatorTranslateYProperty, translateYAnim);
        }
        #endregion
    }
}
