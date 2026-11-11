using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.Storage.Xps;

namespace TextContrast
{
    // Captures each window as the screen shows it, finds the sample lines and compares the ink of each line with the line drawn by GDI
    // in the Win32 window. Edge and Chrome draw the same lines with Skia, once at their default text contrast and once at each value of
    // --text-contrast.
    internal static unsafe class Measurement
    {
        private const PRINT_WINDOW_FLAGS RenderFullContent = (PRINT_WINDOW_FLAGS)2;

        private static readonly string?[] Contrasts = [null, "0.5", "1.0"];

        public static async Task RunAsync(Window avalonia, HWND win32, HWND winForms, string output)
        {
            Directory.CreateDirectory(output);
            var report = new StringBuilder();
            try
            {
                await Task.Delay(1500);
                var page = WritePage(output);
                var results = new List<Result>
                {
                    Measure("Win32", win32, output),
                    Measure("Windows Forms", winForms, output),
                    Measure("Avalonia", new HWND(avalonia.TryGetPlatformHandle()!.Handle), output),
                };
                var browsers = Browsers().ToList();
                foreach (var browser in browsers)
                {
                    foreach (var contrast in Contrasts)
                    {
                        results.Add(await MeasureBrowserAsync(browser, contrast, page, output));
                    }
                }

                Report(report, results, win32, browsers);
            }
            catch (Exception exception)
            {
                report.AppendLine(exception.ToString());
            }

            await File.WriteAllTextAsync(Path.Combine(output, "results.md"), report.ToString());
        }

        private static IEnumerable<Browser> Browsers()
        {
            Browser[] candidates =
            [
                new("Edge", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge\Application\msedge.exe")),
                new("Chrome", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Google\Chrome\Application\chrome.exe")),
            ];
            return candidates.Where(browser => File.Exists(browser.Path));
        }

        // The sample lines for Edge and Chrome, at the positions, sizes and colours of the other windows. The measurement opens the page
        // with a unique title after the #, so that it can find the window of each run.
        private static string WritePage(string output)
        {
            var culture = CultureInfo.InvariantCulture;
            var background = $"#{Samples.Background:X2}{Samples.Background:X2}{Samples.Background:X2}";
            var page = new StringBuilder();
            page.AppendLine("<!DOCTYPE html>").AppendLine("<html lang=\"en\">").AppendLine("<head>").AppendLine("  <meta charset=\"utf-8\">");
            page.AppendLine("  <title>TextContrast</title>").AppendLine("  <style>");
            page.AppendLine(culture, $"    html, body {{ margin: 0; height: 100%; background: {background}; }}");
            page.AppendLine(culture, $"    p {{ position: absolute; left: {Samples.Left}px; margin: 0; font-family: \"Segoe UI\"; color: #000000; white-space: nowrap; }}");
            page.AppendLine("  </style>");
            page.AppendLine("  <script>document.title = decodeURIComponent(location.hash.slice(1)) || document.title;</script>");
            page.AppendLine("</head>").AppendLine("<body>");
            foreach (var line in Samples.Lines)
            {
                page.AppendLine(culture, $"  <p style=\"top: {line.Top}px; font-size: {line.Size}px\">{WebUtility.HtmlEncode(line.Text)}</p>");
            }

            page.AppendLine("</body>").AppendLine("</html>");
            var path = Path.GetFullPath(Path.Combine(output, "Labels.html"));
            File.WriteAllText(path, page.ToString());
            return path;
        }

        // Each run has its own profile, so that a browser that is already open does not take the page with its own settings. The window
        // is large enough for the lines whether the browser takes its size in physical or in device-independent pixels, and it keeps
        // painting when other windows cover it.
        private static async Task<Result> MeasureBrowserAsync(Browser browser, string? contrast, string page, string output)
        {
            var name = contrast is null ? $"{browser.Name}, default contrast" : $"{browser.Name}, --text-contrast={contrast}";
            var title = "TextContrast " + Guid.NewGuid().ToString("N")[..8];
            var profile = Path.Combine(Path.GetTempPath(), "TextContrast", title.Replace(' ', '-'));
            var start = new ProcessStartInfo(browser.Path);
            start.ArgumentList.Add($"--user-data-dir={profile}");
            start.ArgumentList.Add("--no-first-run");
            start.ArgumentList.Add("--no-default-browser-check");
            start.ArgumentList.Add("--disable-extensions");
            start.ArgumentList.Add("--disable-backgrounding-occluded-windows");
            start.ArgumentList.Add("--disable-features=CalculateNativeWinOcclusion");
            if (browser.Name == "Edge")
            {
                // A new Edge profile can open a prompt to sign in with the Windows account, which dims the page. InPrivate windows are
                // not signed in.
                start.ArgumentList.Add("--inprivate");
            }

            start.ArgumentList.Add("--window-position=20,20");
            start.ArgumentList.Add("--window-size=480,700");
            if (contrast is not null)
            {
                start.ArgumentList.Add($"--text-contrast={contrast}");
            }

            start.ArgumentList.Add($"--app={new Uri(page).AbsoluteUri}#{Uri.EscapeDataString(title)}");
            using var process = Process.Start(start)!;
            try
            {
                var window = default(HWND);
                for (var i = 0; i < 100 && window.IsNull; i++)
                {
                    await Task.Delay(200);
                    window = FindWindow(title);
                }

                if (window.IsNull)
                {
                    throw new TimeoutException($"{browser.Name} did not open a window titled \"{title}\".");
                }

                await Task.Delay(2000);
                return Measure(name, window, output);
            }
            finally
            {
                process.Kill(entireProcessTree: true);
                await Task.Delay(1000);
                try
                {
                    Directory.Delete(profile, recursive: true);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        private static HWND FindWindow(string title)
        {
            var window = PInvoke.FindWindowEx(default, default, default(PCWSTR), default(PCWSTR));
            while (!window.IsNull)
            {
                var length = PInvoke.GetWindowTextLength(window);
                if (length > 0 && PInvoke.IsWindowVisible(window))
                {
                    var buffer = new char[length + 1];
                    fixed (char* text = buffer)
                    {
                        PInvoke.GetWindowText(window, text, buffer.Length);
                    }

                    if (new string(buffer, 0, length).Contains(title, StringComparison.Ordinal))
                    {
                        return window;
                    }
                }

                window = PInvoke.FindWindowEx(default, window, default(PCWSTR), default(PCWSTR));
            }

            return default;
        }

        private static Result Measure(string name, HWND window, string output)
        {
            var image = Capture(window);
            var background = Background(image);
            var file = string.Concat(name.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant));
            Save(image, new Box(0, 0, image.Width, image.Height), 1, Path.Combine(output, $"{file}-window.png"));

            // The page's colour is the most common one in the window. Anything else means that a prompt or another window dimmed or
            // covered the page, and its text cannot be compared.
            if (Math.Abs(background - Samples.Background) > 1)
            {
                return new Result(name, [], $"not measured: the page was dimmed or covered (see {file}-window.png)");
            }

            var lines = new List<Line>();
            var boxes = FindLines(image, background);
            for (var i = 0; i < boxes.Count; i++)
            {
                var box = boxes[i];
                lines.Add(Analyse(image, box, background, Samples.Lines[Samples.Lines.Count - boxes.Count + i].Size));
                Save(image, box, 1, Path.Combine(output, $"{file}-{i + 1}.png"));
                Save(image, box, 8, Path.Combine(output, $"{file}-{i + 1}-x8.png"));
            }

            return new Result(name, lines);
        }

        // PrintWindow with PW_RENDERFULLCONTENT copies what the window presents, including content drawn with DirectComposition. A
        // window that it cannot copy is copied from the screen instead.
        private static Image Capture(HWND window)
        {
            RECT bounds;
            RECT client;
            var origin = default(System.Drawing.Point);
            PInvoke.GetWindowRect(window, &bounds);
            PInvoke.GetClientRect(window, &client);
            PInvoke.ClientToScreen(window, &origin);
            var width = bounds.right - bounds.left;
            var height = bounds.bottom - bounds.top;
            var screen = PInvoke.GetDC(default);
            var memory = PInvoke.CreateCompatibleDC(screen);
            var header = new BITMAPINFO
            {
                bmiHeader = new BITMAPINFOHEADER
                {
                    biSize = (uint)sizeof(BITMAPINFOHEADER),
                    biWidth = width,
                    biHeight = -height,
                    biPlanes = 1,
                    biBitCount = 32,
                },
            };
            void* bits;
            var bitmap = PInvoke.CreateDIBSection(memory, &header, DIB_USAGE.DIB_RGB_COLORS, &bits, default, 0);
            var previous = PInvoke.SelectObject(memory, bitmap);
            try
            {
                PInvoke.PrintWindow(window, memory, RenderFullContent);
                var image = Crop(new ReadOnlySpan<byte>(bits, width * height * 4), width, origin.X - bounds.left, origin.Y - bounds.top, client.right, client.bottom);
                if (image.Pixels.AsSpan().IndexOfAnyExcept(image.Pixels[0]) < 0)
                {
                    PInvoke.BitBlt(memory, 0, 0, width, height, screen, bounds.left, bounds.top, ROP_CODE.SRCCOPY);
                    image = Crop(new ReadOnlySpan<byte>(bits, width * height * 4), width, origin.X - bounds.left, origin.Y - bounds.top, client.right, client.bottom);
                }

                return image;
            }
            finally
            {
                PInvoke.SelectObject(memory, previous);
                PInvoke.DeleteObject(bitmap);
                PInvoke.DeleteDC(memory);
                PInvoke.ReleaseDC(default, screen);
            }
        }

        private static Image Crop(ReadOnlySpan<byte> source, int sourceWidth, int x, int y, int width, int height)
        {
            var pixels = new byte[width * height * 4];
            for (var row = 0; row < height; row++)
            {
                source.Slice((((y + row) * sourceWidth) + x) * 4, width * 4).CopyTo(pixels.AsSpan(row * width * 4));
            }

            return new Image(width, height, pixels);
        }

        // The brightness of a pixel as stored (gamma-encoded), with the weights of ITU-R BT.709, whose primaries sRGB uses:
        // Y' = 0.2126 R' + 0.7152 G' + 0.0722 B'.
        private static double Luma(Image image, int x, int y)
        {
            var i = ((y * image.Width) + x) * 4;
            return (0.2126 * image.Pixels[i + 2]) + (0.7152 * image.Pixels[i + 1]) + (0.0722 * image.Pixels[i]);
        }

        private static double Background(Image image)
        {
            var counts = new Dictionary<int, int>();
            for (var i = 0; i < image.Pixels.Length; i += 4)
            {
                var colour = image.Pixels[i] | (image.Pixels[i + 1] << 8) | (image.Pixels[i + 2] << 16);
                counts[colour] = counts.GetValueOrDefault(colour) + 1;
            }

            var common = counts.MaxBy(pair => pair.Value).Key;
            return (0.2126 * ((common >> 16) & 0xFF)) + (0.7152 * ((common >> 8) & 0xFF)) + (0.0722 * (common & 0xFF));
        }

        // A line is a run of rows with ink, allowing gaps of up to 3 rows (the dot of an i). Runs shorter than 6 rows or taller than 60
        // are frame edges or title bars, and the sample lines are the last ones in the window.
        private static List<Box> FindLines(Image image, double background)
        {
            var ink = new bool[image.Height];
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width && !ink[y]; x++)
                {
                    ink[y] = background - Luma(image, x, y) > 8;
                }
            }

            var runs = new List<(int Top, int Bottom)>();
            var row = 0;
            while (row < image.Height)
            {
                if (!ink[row])
                {
                    row++;
                    continue;
                }

                var top = row;
                var bottom = row;
                while (row < image.Height && (ink[row] || row - bottom <= 3))
                {
                    if (ink[row])
                    {
                        bottom = row;
                    }

                    row++;
                }

                runs.Add((top, bottom));
            }

            var text = runs.Where(run => run.Bottom - run.Top + 1 is >= 6 and <= 60).TakeLast(Samples.Lines.Count);
            var boxes = new List<Box>();
            foreach (var (top, bottom) in text)
            {
                var left = image.Width;
                var right = -1;
                for (var y = top; y <= bottom; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        if (background - Luma(image, x, y) > 8)
                        {
                            left = Math.Min(left, x);
                            right = Math.Max(right, x);
                        }
                    }
                }

                var boxLeft = Math.Max(0, left - 2);
                var boxTop = Math.Max(0, top - 2);
                boxes.Add(new Box(boxLeft, boxTop, Math.Min(image.Width - 1, right + 2) - boxLeft + 1, Math.Min(image.Height - 1, bottom + 2) - boxTop + 1));
            }

            return boxes;
        }

        // Ink is the darkness of each pixel from the background towards black, where a black pixel counts 1. A dark pixel is at least 75%
        // of the way to black. The colour of a pixel is its largest channel minus its smallest, from 0 to 255, and a colour fringe is an
        // ink pixel with a colour of more than 24.
        private static Line Analyse(Image image, Box box, double background, int size)
        {
            var ink = 0.0;
            var dark = 0;
            var inkPixels = 0;
            var fringed = 0;
            var colour = 0;
            var strong = 0;
            var strongest = 0;
            var pixels = new byte[box.Width * box.Height * 4];
            for (var y = box.Y; y < box.Y + box.Height; y++)
            {
                image.Pixels.AsSpan((((y * image.Width) + box.X) * 4), box.Width * 4).CopyTo(pixels.AsSpan((y - box.Y) * box.Width * 4));
                for (var x = box.X; x < box.X + box.Width; x++)
                {
                    var luma = Luma(image, x, y);
                    ink += Math.Max(0, (background - luma) / background);
                    if (luma <= 0.25 * background)
                    {
                        dark++;
                    }

                    var i = ((y * image.Width) + x) * 4;
                    var max = Math.Max(Math.Max(image.Pixels[i], image.Pixels[i + 1]), image.Pixels[i + 2]);
                    var min = Math.Min(Math.Min(image.Pixels[i], image.Pixels[i + 1]), image.Pixels[i + 2]);
                    colour += max - min;
                    strongest = Math.Max(strongest, max - min);
                    if (max - min >= 192)
                    {
                        strong++;
                    }

                    if (background - luma > 8)
                    {
                        inkPixels++;
                        if (max - min > 24)
                        {
                            fringed++;
                        }
                    }
                }
            }

            return new Line(size, box.Width - 4, box.Height - 4, ink, dark, inkPixels, fringed, colour / 255.0, strong, strongest, pixels);
        }

        private static void Save(Image image, Box box, int zoom, string path)
        {
            using var bitmap = new WriteableBitmap(new PixelSize(box.Width * zoom, box.Height * zoom), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
            using (var buffer = bitmap.Lock())
            {
                var row = new byte[box.Width * zoom * 4];
                for (var y = 0; y < box.Height * zoom; y++)
                {
                    for (var x = 0; x < box.Width * zoom; x++)
                    {
                        var source = (((box.Y + (y / zoom)) * image.Width) + box.X + (x / zoom)) * 4;
                        row[x * 4] = image.Pixels[source];
                        row[(x * 4) + 1] = image.Pixels[source + 1];
                        row[(x * 4) + 2] = image.Pixels[source + 2];
                        row[(x * 4) + 3] = 255;
                    }

                    Marshal.Copy(row, 0, buffer.Address + (y * buffer.RowBytes), row.Length);
                }
            }

            bitmap.Save(path, new PngBitmapEncoderOptions());
        }

        private static void Report(StringBuilder report, List<Result> results, HWND win32, List<Browser> browsers)
        {
            var culture = CultureInfo.InvariantCulture;
            var scale = PInvoke.GetDpiForWindow(win32) * 100 / 96;
            report.AppendLine("# Text contrast").AppendLine();
            report.Append(culture, $"Windows {Environment.OSVersion.Version}, {scale}% scale. Avalonia {Version(typeof(Window).Assembly)}, SkiaSharp {Version(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "SkiaSharp"))}");
            foreach (var browser in browsers)
            {
                report.Append(culture, $", {browser.Name} {FileVersionInfo.GetVersionInfo(browser.Path).ProductVersion}");
            }

            report.AppendLine(".").AppendLine();
            report.AppendLine("The first three lines are 12-pixel (9-point) Segoe UI, the text of a Windows dialog box; the others are \"Customer number:\" in Segoe UI at each size of the sweep, in device-independent pixels. All are black on #F9F9F9. Ink is the darkness of each pixel from the background towards black, where a black pixel counts 1. Dark pixels are at least 75% of the way to black. The colour of a pixel is its largest channel minus its smallest, from 0 to 255; colour fringes are ink pixels with a colour of more than 24 (ClearType or subpixel anti-aliasing).").AppendLine();

            var reference = results[0];
            var avalonia = results.First(result => result.Name == "Avalonia");
            report.AppendLine("| Renderer | Ink relative to Win32, mean of the three 12-pixel lines | Lines with the same pixels as Avalonia |").AppendLine("|---|---|---|");
            foreach (var result in results)
            {
                var relative = Complete(result) && Complete(reference)
                    ? Enumerable.Range(0, Samples.DialogLines).Average(i => result.Lines[i].Ink / reference.Lines[i].Ink).ToString("P1", culture)
                    : result.Problem ?? $"{result.Lines.Count} lines found";
                var same = Complete(result) && Complete(avalonia) ? $"{Enumerable.Range(0, Samples.Lines.Count).Count(i => Same(result.Lines[i], avalonia.Lines[i]))} of {Samples.Lines.Count}" : "–";
                report.AppendLine(culture, $"| {result.Name} | {relative} | {same} |");
            }

            SizeTable(report, "Ink relative to Win32", results, (result, i) => (result.Lines[i].Ink / reference.Lines[i].Ink).ToString("P1", culture), Complete(reference));
            SizeTable(report, "Strongest colour", results, (result, i) => result.Lines[i].Strongest.ToString(culture), true);
            SizeTable(report, "Pixels with a colour of at least 192", results, (result, i) => result.Lines[i].Strong.ToString(culture), true);

            report.AppendLine().AppendLine("| Renderer | Line | Font size | Ink box | Ink | Relative to Win32 | Dark pixels | Colour fringes | Total colour ÷ 255 | Colour ≥ 192 | Strongest colour |");
            report.AppendLine("|---|---|---|---|---|---|---|---|---|---|---|");
            foreach (var result in results)
            {
                for (var i = 0; i < result.Lines.Count; i++)
                {
                    var line = result.Lines[i];
                    var relative = Complete(result) && Complete(reference) ? (line.Ink / reference.Lines[i].Ink).ToString("P1", culture) : "–";
                    var fringes = (line.Fringed / (double)Math.Max(1, line.InkPixels)).ToString("P0", culture);
                    report.AppendLine(culture, $"| {result.Name} | {i + 1} | {line.Size} px | {line.Width} × {line.Height} | {line.Ink:F1} | {relative} | {line.Dark} | {fringes} | {line.Colour:F1} | {line.Strong} | {line.Strongest} |");
                }
            }
        }

        // One row for each line of the size sweep and one column for each renderer.
        private static void SizeTable(StringBuilder report, string title, List<Result> results, Func<Result, int, string> cell, bool available)
        {
            var culture = CultureInfo.InvariantCulture;
            report.AppendLine().AppendLine(culture, $"### {title}").AppendLine();
            report.AppendLine(culture, $"| Size | {string.Join(" | ", results.Select(result => result.Name))} |");
            report.AppendLine(string.Concat(Enumerable.Repeat("|---", results.Count + 1)) + "|");
            for (var i = Samples.DialogLines; i < Samples.Lines.Count; i++)
            {
                var index = i;
                var cells = results.Select(result => available && Complete(result) ? cell(result, index) : "–");
                report.AppendLine(culture, $"| {Samples.Lines[i].Size} px | {string.Join(" | ", cells)} |");
            }
        }

        private static bool Complete(Result result)
        {
            return result.Lines.Count == Samples.Lines.Count;
        }

        private static bool Same(Line first, Line second)
        {
            return first.Width == second.Width && first.Height == second.Height && first.Pixels.AsSpan().SequenceEqual(second.Pixels);
        }

        private static string Version(Assembly? assembly)
        {
            var version = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
            var metadata = version.IndexOf('+', StringComparison.Ordinal);
            return metadata < 0 ? version : version[..metadata];
        }

        private sealed record Browser(string Name, string Path);

        private sealed record Box(int X, int Y, int Width, int Height);

        private sealed record Line(int Size, int Width, int Height, double Ink, int Dark, int InkPixels, int Fringed, double Colour, int Strong, int Strongest, byte[] Pixels);

        private sealed record Result(string Name, IReadOnlyList<Line> Lines, string? Problem = null);

        private sealed record Image(int Width, int Height, byte[] Pixels);
    }
}