using Avalonia.Controls;
using Avalonia.Media;

namespace TextContrast
{
    // The sample lines drawn by Avalonia, with the default text options.
    internal sealed class LabelsWindow : Window
    {
        public LabelsWindow()
        {
            this.Title = "TextContrast: Avalonia";
            this.Width = Samples.Width;
            this.Height = Samples.Height;
            this.CanResize = false;
            this.Background = new SolidColorBrush(Color.FromRgb(Samples.Background, Samples.Background, Samples.Background));
            var canvas = new Canvas();
            foreach (var line in Samples.Lines)
            {
                var text = new TextBlock
                {
                    Text = line.Text,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = line.Size,
                    Foreground = Brushes.Black,
                };
                Canvas.SetLeft(text, Samples.Left);
                Canvas.SetTop(text, line.Top);
                canvas.Children.Add(text);
            }

            this.Content = canvas;
        }
    }
}