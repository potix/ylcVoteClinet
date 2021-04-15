using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ylcVoteClinet
{
    /// <summary>
    /// ViewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ViewWindow : Window
    {
        //private Setting _setting;

        public ViewWindow(Setting setting)
        {
            InitializeComponent();
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(setting.BackgroundColor);
            System.Windows.Media.Color mColor = System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);

            //Label l = new Label();
            //l.Content = "test";
            //ViewGrid.Children.Add(l);

        }



    }
}
