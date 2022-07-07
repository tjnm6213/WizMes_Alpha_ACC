using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WizMes_Alpha_JA
{
    public class MillerColumns : Control
    {
        static MillerColumns()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MillerColumns), new FrameworkPropertyMetadata(typeof(MillerColumns)));
        }

        public static readonly DependencyProperty ItemsSourceProperty =
    DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MillerColumns), new PropertyMetadata(null));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
    }
}
