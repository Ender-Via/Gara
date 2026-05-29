using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    public partial class SidebarControl : UserControl
    {
        public static readonly DependencyProperty ActiveMenuProperty =
            DependencyProperty.Register("ActiveMenu", typeof(string), typeof(SidebarControl),
                new PropertyMetadata("", OnActiveMenuChanged));

        public string ActiveMenu
        {
            get => (string)GetValue(ActiveMenuProperty);
            set => SetValue(ActiveMenuProperty, value);
        }

        private static void OnActiveMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sidebar = (SidebarControl)d;
            sidebar.UpdateActiveMenu((string)e.NewValue);
        }

        private void UpdateActiveMenu(string activeContent)
        {
            foreach (var rb in FindVisualChildren<RadioButton>(this))
            {
                rb.IsChecked = rb.Content?.ToString() == activeContent;
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;
                foreach (var c in FindVisualChildren<T>(child)) yield return c;
            }
        }

        public SidebarControl()
        {
            InitializeComponent();
        }
    }
}