using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Styles.Resources
{
    public partial class TabStyle
    {
        public static double GetSmoothUnderline(DependencyObject obj)
        {
            return (double)obj.GetValue(SmoothUnderlineProperty);
        }

        public static void SetSmoothUnderline(DependencyObject obj, double value)
        {
            obj.SetValue(SmoothUnderlineProperty, value);
        }

        public static readonly DependencyProperty SmoothUnderlineProperty =
            DependencyProperty.RegisterAttached("SmoothUnderline",
                                                typeof(double),
                                                typeof(TabStyle),
                                                new PropertyMetadata(Changing));

        private static void Changing(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rectangle rect = (Rectangle)d;
            DoubleAnimation anim = new((double)e.NewValue, TimeSpan.FromMilliseconds(250));
            rect.BeginAnimation(FrameworkElement.WidthProperty, anim, HandoffBehavior.Compose);
        }

        private void MouseOver(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void contentPresenter_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //ContentPresenter content = (ContentPresenter)sender;

            //DoubleAnimation underlineAnimation = new()
            //{
            //    To = 100,
            //    Duration = TimeSpan.FromMilliseconds(250)
            //};

            //Storyboard.SetTargetName(underlineAnimation, "underline");
            //Storyboard.SetTargetProperty(underlineAnimation, new System.Windows.PropertyPath("Width"));

            //Storyboard storyboard = new();
            //storyboard.Children.Add(underlineAnimation);
            //storyboard.Begin();
        }

        private void templateRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }
    }
}
