using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Windows.Win32.Graphics.Gdi;

namespace TextContrast
{
    // Shows the sample lines drawn by Avalonia, by GDI in a plain Win32 window and by Windows Forms. With --measure [folder] (the Measure launch profile), it also
    // opens them in Edge and Chrome at each text contrast, measures every window, writes Labels.html, results.md and close-ups to the
    // folder, and exits. --cleartype-quality draws the Win32 lines with CLEARTYPE_QUALITY, the font quality that Windows Forms requests.
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = new LabelsWindow { Position = new PixelPoint(20, 20) };
                desktop.MainWindow = window;
                var arguments = desktop.Args ?? [];
                if (Array.IndexOf(arguments, "--cleartype-quality") >= 0)
                {
                    GdiLabelsWindow.Quality = FONT_QUALITY.CLEARTYPE_QUALITY;
                }

                var win32 = GdiLabelsWindow.Create(680, 20);
                var winForms = WinFormsLabelsWindow.Open(1340, 20);
                var index = Array.IndexOf(arguments, "--measure");
                if (index >= 0)
                {
                    var output = index + 1 < arguments.Length ? arguments[index + 1] : DefaultOutput();
                    window.Opened += async (_, _) =>
                    {
                        await Measurement.RunAsync(window, win32, winForms, output);
                        desktop.Shutdown();
                    };
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        // Without a folder, the results go next to TextContrast.slnx, so that a run from Visual Studio, whose working directory is the
        // build output, puts them in the repository.
        private static string DefaultOutput()
        {
            for (var folder = new DirectoryInfo(AppContext.BaseDirectory); folder is not null; folder = folder.Parent)
            {
                if (File.Exists(Path.Combine(folder.FullName, "TextContrast.slnx")))
                {
                    return Path.Combine(folder.FullName, "results");
                }
            }

            return "results";
        }
    }
}