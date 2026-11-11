using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace TextContrast
{
    // The sample lines drawn by GDI in a plain Win32 window, as the static controls of a dialog box draw them: TextOut with Segoe UI
    // fonts of the default quality, which follows the ClearType setting of Windows.
    internal static unsafe class GdiLabelsWindow
    {
        private const string ClassName = "TextContrastGdiLabels";

        public static FONT_QUALITY Quality { get; set; } = FONT_QUALITY.DEFAULT_QUALITY;

        public static HWND Create(int x, int y)
        {
            var instance = new HINSTANCE(PInvoke.GetModuleHandle(default(PCWSTR)).Value);
            fixed (char* name = ClassName)
            fixed (char* title = "TextContrast: Win32")
            {
                var windowClass = new WNDCLASSEXW
                {
                    cbSize = (uint)sizeof(WNDCLASSEXW),
                    lpfnWndProc = &WindowProcedure,
                    hInstance = instance,
                    hCursor = PInvoke.LoadCursor(default, PInvoke.IDC_ARROW),
                    lpszClassName = name,
                };
                PInvoke.RegisterClassEx(&windowClass);
                var style = WINDOW_STYLE.WS_OVERLAPPED | WINDOW_STYLE.WS_CAPTION | WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_MINIMIZEBOX;
                var window = PInvoke.CreateWindowEx(0, name, title, style, x, y, 0, 0, default, default, instance, null);
                var dpi = PInvoke.GetDpiForWindow(window);
                var bounds = new RECT { right = Scale(Samples.Width, dpi), bottom = Scale(Samples.Height, dpi) };
                PInvoke.AdjustWindowRectExForDpi(&bounds, style, false, 0, dpi);
                PInvoke.SetWindowPos(window, default, 0, 0, bounds.right - bounds.left, bounds.bottom - bounds.top, SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
                PInvoke.ShowWindow(window, SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE);
                return window;
            }
        }

        private static int Scale(int value, uint dpi)
        {
            return PInvoke.MulDiv(value, (int)dpi, 96);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static LRESULT WindowProcedure(HWND window, uint message, WPARAM wParam, LPARAM lParam)
        {
            if (message == PInvoke.WM_PAINT)
            {
                Paint(window);
                return new LRESULT(0);
            }

            return PInvoke.DefWindowProc(window, message, wParam, lParam);
        }

        private static void Paint(HWND window)
        {
            PAINTSTRUCT paint;
            var dc = PInvoke.BeginPaint(window, &paint);
            var dpi = PInvoke.GetDpiForWindow(window);
            RECT client;
            PInvoke.GetClientRect(window, &client);
            var brush = PInvoke.CreateSolidBrush(new COLORREF((uint)(Samples.Background | (Samples.Background << 8) | (Samples.Background << 16))));
            PInvoke.FillRect(dc, &client, brush);
            PInvoke.SetBkMode(dc, BACKGROUND_MODE.TRANSPARENT);
            PInvoke.SetTextColor(dc, new COLORREF(0));
            foreach (var line in Samples.Lines)
            {
                var description = new LOGFONTW { lfHeight = -Scale(line.Size, dpi), lfWeight = 400, lfQuality = Quality };
                "Segoe UI".AsSpan().CopyTo(description.lfFaceName.AsSpan());
                var font = PInvoke.CreateFontIndirect(&description);
                var previous = PInvoke.SelectObject(dc, font);
                fixed (char* text = line.Text)
                {
                    PInvoke.TextOut(dc, Scale(Samples.Left, dpi), Scale(line.Top, dpi), text, line.Text.Length);
                }

                PInvoke.SelectObject(dc, previous);
                PInvoke.DeleteObject(font);
            }

            PInvoke.DeleteObject(brush);
            PInvoke.EndPaint(window, &paint);
        }
    }
}