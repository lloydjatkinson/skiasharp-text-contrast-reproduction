#:property TargetFramework=net11.0-windows
#:property UseWindowsForms=true
#:property PublishAot=false

// Prints the ink of each crop of text: the darkness of each pixel from the background towards black, added up, where a background
// pixel counts 0 and a black pixel counts 1. The top-left pixel of each crop is the background, and the first crop is the reference
// for the percentages.
//
// Usage: dotnet run Ink.cs -- win32.png avalonia.png chrome.png

using System;
using System.Drawing;
using System.IO;

var reference = Ink(args[0]);
foreach (var path in args)
{
    var ink = Ink(path);
    Console.WriteLine($"{ink,8:F1}  {ink / reference,7:P1}  {Path.GetFileName(path)}");
}

static double Ink(string path)
{
    using var image = new Bitmap(path);
    var background = Luma(image.GetPixel(0, 0));
    var ink = 0.0;
    for (var y = 0; y < image.Height; y++)
    {
        for (var x = 0; x < image.Width; x++)
        {
            var darkness = (background - Luma(image.GetPixel(x, y))) / background;
            ink += Math.Max(0, darkness);
        }
    }

    return ink;
}

// The brightness of a pixel as stored (gamma-encoded), with the weights of ITU-R BT.709, whose primaries sRGB uses:
// Y' = 0.2126 R' + 0.7152 G' + 0.0722 B'.
static double Luma(Color colour)
{
    return (0.2126 * colour.R) + (0.7152 * colour.G) + (0.0722 * colour.B);
}
