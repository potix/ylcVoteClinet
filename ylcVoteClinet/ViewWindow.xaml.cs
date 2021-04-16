using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// 

    public class ChoiceAndResult
    {
        public string Choice { get; set; }

        public int Count { get; set; }

        public double Rate { get; set; }
    }

    public partial class ViewWindow : Window
    {
        private readonly int _fontsize = 16;
        private readonly int _padding = 32;
    
        public ViewWindow(Setting setting)
        {
            InitializeComponent();
        }

        public void Render(Setting setting)
        {
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(setting.WindowBackgroundColor);
            Color mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            Background = new SolidColorBrush(mColor);
            int maxCols = 4;
            if (setting.Choices.Count <= 4)
            {
                maxCols = 2;
            }
            else if (setting.Choices.Count <= 9)
            {
                maxCols = 3;
            }
            int boxWidth = 0;
            int boxHeight = 0;
            int rows = ((setting.Choices.Count - 1) / maxCols) + 1;
            foreach (var choiceItem in setting.Choices)
            {
                string[] liners = choiceItem.Text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                int height = ((liners.Length + 4) * _fontsize) + (_padding * 2);
                if (boxHeight < height)
                {
                    boxHeight = height;
                }
                foreach (var liner in liners)
                {
                    int width = (liner.Length * _fontsize) + (_padding * 2);
                    if (boxWidth < width)
                    {
                        boxWidth = width;
                    }
                }
            }
            int windowWidth = (boxWidth * maxCols) + (_padding * (maxCols - 1)) + (_padding * 2);
            int windowHeight = (boxHeight * rows) + (_padding * (rows - 1)) + (_padding * 2);
            Width = windowWidth + _padding;
            Height = windowHeight + (_padding * 2);

            if (setting.Results == null)
            {
                RenderChoices(setting, maxCols, boxWidth, boxHeight);
            } else
            {
                RenderChoicesAndResults(setting, maxCols, boxWidth, boxHeight);
            }

        }

        public void RenderChoices(Setting setting, int maxCols, int boxWidth, int boxHeight)
        {
            foreach (var choice in setting.Choices)
            {
                int idx = setting.Choices.IndexOf(choice);
                int rowPos = idx / maxCols;
                int colPos = idx % maxCols;
                TextBox textBox = new TextBox();
                textBox.SetBinding(TextBox.TextProperty, "Text");
                textBox.DataContext = choice;
                textBox.FontSize = _fontsize;
                textBox.BorderThickness = new Thickness(0);
                Color mColor = Color.FromArgb(0, 0, 0, 0);
                textBox.Background = new SolidColorBrush(mColor);
                System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(setting.BoxForegroundColor);
                mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
                textBox.Foreground = new SolidColorBrush(mColor);
                textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                textBox.VerticalContentAlignment = VerticalAlignment.Top;
                Border border = new Border();
                dColor = System.Drawing.ColorTranslator.FromHtml(setting.BoxBorderColor);
                mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
                border.BorderBrush = new SolidColorBrush(mColor);
                border.BorderThickness = new Thickness(5, 5, 5, 5);
                border.CornerRadius = new CornerRadius(10);
                dColor = System.Drawing.ColorTranslator.FromHtml(setting.BoxBackgroundColor);
                mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
                border.Background = new SolidColorBrush(mColor);
                border.Width = boxWidth;
                border.Height = boxHeight;
                border.HorizontalAlignment = HorizontalAlignment.Left;
                border.VerticalAlignment = VerticalAlignment.Top;
                border.Margin = new Thickness((boxWidth * colPos) + (_padding * colPos) + _padding, (boxHeight * rowPos) + (_padding * rowPos) + _padding, 0, 0);
                border.Child = textBox;
                ViewGrid.Children.Add(border);
            }
        }

        public void RenderChoicesAndResults(Setting setting, int maxCols, int boxWidth, int boxHeight)
        {
            //int renderIdx = 0;


        }


    }
}
