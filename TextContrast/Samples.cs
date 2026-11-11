using System;
using System.Collections.Generic;
using System.Linq;

namespace TextContrast
{
    // The lines that every renderer draws, black on #F9F9F9, in device-independent pixels: three lines of 12-pixel (9-point) Segoe UI,
    // the text of a Windows dialog box, then "Customer number:" at each size of the sweep.
    internal static class Samples
    {
        public const int Width = 400;
        public const int Left = 20;
        public const byte Background = 0xF9;
        public const int DialogLines = 3;

        // Each size is a whole number of pixels at 150% as well as at 100%. GDI fonts have whole-pixel heights, so at 9 or 11 pixels
        // (13.5 or 16.5 at 150%) GDI would draw a different size from Skia.
        private static readonly int[] Sizes = [8, 10, 12, 14, 16, 20, 28];

        public static readonly IReadOnlyList<Sample> Lines = Arrange();

        public static readonly int Height = Lines[^1].Top + Gap(Lines[^1].Size);

        private static List<Sample> Arrange()
        {
            string[] dialog = ["Customer number:", "£4,070.00 in 3 unpaid invoices", "The quick brown fox jumps over the lazy dog"];
            var lines = new List<Sample>();
            var top = 20;
            foreach (var (text, size) in dialog.Select(text => (text, 12)).Concat(Sizes.Select(size => ("Customer number:", size))))
            {
                lines.Add(new Sample(text, size, top));
                top += Gap(size);
            }

            return lines;
        }

        private static int Gap(int size)
        {
            return Math.Max(30, size * 2);
        }
    }

    internal sealed record Sample(string Text, int Size, int Top);
}