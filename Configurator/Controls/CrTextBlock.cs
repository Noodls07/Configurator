using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Configurator.Controls
{
    static class CrTextBlock
    {
        #region TextBlock
        public static void BuildTextBlock(string name, string displayName)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Name = "txbl_" + name;
            textBlock.Margin = new Thickness(10, 0, 5, 0);
            textBlock.Text = displayName;
            textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.FontSize = 16;
            textBlock.Foreground = Brushes.WhiteSmoke;
            (Application.Current.MainWindow as MainWindow).csAllPropGrid.Children.Add(textBlock);
            (Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(textBlock, 0);
            Grid.SetRow(textBlock, (Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions.Count - 1);
        }
        #endregion
    }
}
