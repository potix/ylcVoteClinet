using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public class ChoiceAndResult : Choice
    {
        public double Rate { get; set; }
    }

    public partial class ViewWindow : Window
    {

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
                int height = ((liners.Length + 4) * setting.FontSize) + (setting.Padding * 2);
                if (boxHeight < height)
                {
                    boxHeight = height;
                }
                foreach (var liner in liners)
                {
                    int width = (liner.Length * setting.FontSize) + (setting.Padding * 2);
                    if (boxWidth < width)
                    {
                        boxWidth = width;
                    }
                }
            }
            int windowWidth = (boxWidth * maxCols) + (setting.Padding * (maxCols - 1)) + (setting.Padding * 2);
            int windowHeight = (boxHeight * rows) + (setting.Padding * (rows - 1)) + (setting.Padding * 2);
            Width = windowWidth + setting.Padding;
            Height = windowHeight + (setting.Padding * 2);

            if (setting.Results == null)
            {
                _renderChoices(setting, maxCols, boxWidth, boxHeight);
            }
            else
            {
                _renderChoicesAndResults(setting, maxCols, boxWidth, boxHeight);
            }
        }

        public void _renderChoices(Setting setting, int maxCols, int boxWidth, int boxHeight)
        {
            foreach (var choice in setting.Choices)
            {
                int idx = setting.Choices.IndexOf(choice);
                int rowPos = idx / maxCols;
                int colPos = idx % maxCols;
                _renderChoiceBox(setting, maxCols, boxWidth, boxHeight, choice, rowPos, colPos);
                _renderIndexBox(setting, maxCols, boxWidth, boxHeight, choice, idx, rowPos, colPos);
            }
        }

        public void _renderChoicesAndResults(Setting setting, int maxCols, int boxWidth, int boxHeight)
        {
            IEnumerable<ChoiceAndResult> choiceAndResults = setting.Choices.Zip(setting.Results, (choice, result) => new ChoiceAndResult() { Text = choice.Text, Rate = result.Rate });
            Debug.Print(choiceAndResults.Count().ToString());
            int idx = 0;
            foreach (ChoiceAndResult choiceAndResult in choiceAndResults) 
            {
                Debug.Print(choiceAndResult.Text);
                Debug.Print(choiceAndResult.Rate.ToString());
                int rowPos = idx / maxCols;
                int colPos = idx % maxCols;
                _renderChoiceBox(setting, maxCols, boxWidth, boxHeight, choiceAndResult, rowPos, colPos);
                _renderIndexBox(setting, maxCols, boxWidth, boxHeight, choiceAndResult, idx, rowPos, colPos);
                _renderResultBox(setting, maxCols, boxWidth, boxHeight, choiceAndResult, rowPos, colPos);
                idx += 1;
            }
        }

        private void _renderChoiceBox(Setting setting, int maxCols, int boxWidth, int boxHeight, Choice choice, int rowPos, int colPos)
        {
            TextBox textBox = new TextBox();
            textBox.SetBinding(TextBox.TextProperty, "Text");
            textBox.DataContext = choice;
            textBox.FontSize = setting.FontSize;
            textBox.BorderThickness = new Thickness(0);
            Color mColor = Color.FromArgb(0, 0, 0, 0);
            textBox.Background = new SolidColorBrush(mColor);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(setting.BoxForegroundColor);
            mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
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
            border.Margin = new Thickness((boxWidth * colPos) + (setting.Padding * colPos) + setting.Padding, (boxHeight * rowPos) + (setting.Padding * rowPos) + setting.Padding, 0, 0);
            border.Child = textBox;
            ViewGrid.Children.Add(border);
        }

        private void _renderIndexBox(Setting setting, int maxCols, int boxWidth, int boxHeight, Choice choice, int idx, int rowPos, int colPos)
        {
            TextBox textBox = new TextBox();
            textBox.Text = (idx + 1).ToString() + ".";
            textBox.FontSize = setting.FontSize * 1.5;
            textBox.BorderThickness = new Thickness(0);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness((boxWidth * colPos) + (setting.Padding * colPos) + setting.Padding, (boxHeight * rowPos) + (setting.Padding * rowPos) + setting.Padding + 4, 0, 0);
            Color mColor = Color.FromArgb(0, 0, 0, 0);
            textBox.Background = new SolidColorBrush(mColor);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(setting.BoxForegroundColor);
            mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Top;
            textBox.Width = boxWidth;
            textBox.Height = boxHeight;
            ViewGrid.Children.Add(textBox);
        }

        private void _renderResultBox(Setting setting, int maxCols, int boxWidth, int boxHeight, ChoiceAndResult choiceAndResult, int rowPos, int colPos)
        {
            TextBox textBox = new TextBox();
            textBox.Text = choiceAndResult.Rate.ToString() + "%";
            textBox.FontSize = setting.FontSize * 1.5;
            textBox.BorderThickness = new Thickness(0);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness((boxWidth * colPos) + (setting.Padding * colPos) + setting.Padding, (boxHeight * rowPos) + (setting.Padding * rowPos) + setting.Padding - 8, 0, 0);
            Color mColor = Color.FromArgb(0, 0, 0, 0);
            textBox.Background = new SolidColorBrush(mColor);
            System.Drawing.Color dColor = System.Drawing.ColorTranslator.FromHtml(setting.BoxForegroundColor);
            mColor = Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);
            textBox.Foreground = new SolidColorBrush(mColor);
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Bottom;
            textBox.Width = boxWidth;
            textBox.Height = boxHeight;
            ViewGrid.Children.Add(textBox);
        }
    }
}
