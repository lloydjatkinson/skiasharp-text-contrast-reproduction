# Text contrast

Windows 10.0.26100.0, 150% scale. Avalonia 12.1.4, SkiaSharp 3.119.4, Edge 157.0.4341.42, Chrome 157.0.8103.50.

The first three lines are 12-pixel (9-point) Segoe UI, the text of a Windows dialog box; the others are "Customer number:" in Segoe UI at each size of the sweep, in device-independent pixels. All are black on #F9F9F9. Ink is the darkness of each pixel from the background towards black, where a black pixel counts 1. Dark pixels are at least 75% of the way to black. The colour of a pixel is its largest channel minus its smallest, from 0 to 255; colour fringes are ink pixels with a colour of more than 24 (ClearType or subpixel anti-aliasing).

| Renderer | Ink relative to Win32, mean of the three 12-pixel lines | Lines with the same pixels as Avalonia |
|---|---|---|
| Win32 | 100.0 % | 0 of 10 |
| Windows Forms | 94.1 % | 0 of 10 |
| Avalonia | 89.8 % | 10 of 10 |
| Edge, default contrast | 99.8 % | 0 of 10 |
| Edge, --text-contrast=0.5 | 90.0 % | 0 of 10 |
| Edge, --text-contrast=1.0 | 99.8 % | 0 of 10 |
| Chrome, default contrast | 99.7 % | 0 of 10 |
| Chrome, --text-contrast=0.5 | 89.8 % | 10 of 10 |
| Chrome, --text-contrast=1.0 | 99.7 % | 0 of 10 |

### Ink relative to Win32

| Size | Win32 | Windows Forms | Avalonia | Edge, default contrast | Edge, --text-contrast=0.5 | Edge, --text-contrast=1.0 | Chrome, default contrast | Chrome, --text-contrast=0.5 | Chrome, --text-contrast=1.0 |
|---|---|---|---|---|---|---|---|---|---|
| 8 px | 100.0 % | 87.4 % | 81.0 % | 93.2 % | 81.2 % | 93.2 % | 93.0 % | 81.0 % | 93.0 % |
| 10 px | 100.0 % | 97.5 % | 92.9 % | 105.1 % | 93.1 % | 105.1 % | 104.9 % | 92.9 % | 104.9 % |
| 12 px | 100.0 % | 93.1 % | 89.5 % | 99.3 % | 89.7 % | 99.3 % | 99.2 % | 89.5 % | 99.2 % |
| 14 px | 100.0 % | 90.5 % | 89.9 % | 102.0 % | 90.1 % | 102.0 % | 101.8 % | 89.9 % | 101.8 % |
| 16 px | 100.0 % | 106.2 % | 90.2 % | 101.3 % | 90.4 % | 101.3 % | 101.1 % | 90.2 % | 101.1 % |
| 20 px | 100.0 % | 97.2 % | 93.6 % | 101.2 % | 93.8 % | 101.2 % | 101.0 % | 93.6 % | 101.0 % |
| 28 px | 100.0 % | 99.3 % | 94.4 % | 100.5 % | 94.5 % | 100.5 % | 100.3 % | 94.4 % | 100.3 % |

### Strongest colour

| Size | Win32 | Windows Forms | Avalonia | Edge, default contrast | Edge, --text-contrast=0.5 | Edge, --text-contrast=1.0 | Chrome, default contrast | Chrome, --text-contrast=0.5 | Chrome, --text-contrast=1.0 |
|---|---|---|---|---|---|---|---|---|---|
| 8 px | 177 | 187 | 193 | 71 | 77 | 71 | 177 | 193 | 177 |
| 10 px | 177 | 187 | 193 | 71 | 77 | 71 | 177 | 193 | 177 |
| 12 px | 177 | 187 | 193 | 71 | 77 | 71 | 177 | 193 | 177 |
| 14 px | 177 | 187 | 193 | 74 | 77 | 74 | 185 | 193 | 185 |
| 16 px | 177 | 187 | 193 | 74 | 77 | 74 | 185 | 193 | 185 |
| 20 px | 177 | 187 | 193 | 74 | 77 | 74 | 185 | 193 | 185 |
| 28 px | 177 | 187 | 193 | 74 | 77 | 74 | 185 | 193 | 185 |

### Pixels with a colour of at least 192

| Size | Win32 | Windows Forms | Avalonia | Edge, default contrast | Edge, --text-contrast=0.5 | Edge, --text-contrast=1.0 | Chrome, default contrast | Chrome, --text-contrast=0.5 | Chrome, --text-contrast=1.0 |
|---|---|---|---|---|---|---|---|---|---|
| 8 px | 0 | 0 | 46 | 0 | 0 | 0 | 0 | 46 | 0 |
| 10 px | 0 | 0 | 58 | 0 | 0 | 0 | 0 | 58 | 0 |
| 12 px | 0 | 0 | 92 | 0 | 0 | 0 | 0 | 92 | 0 |
| 14 px | 0 | 0 | 51 | 0 | 0 | 0 | 0 | 51 | 0 |
| 16 px | 0 | 0 | 54 | 0 | 0 | 0 | 0 | 54 | 0 |
| 20 px | 0 | 0 | 45 | 0 | 0 | 0 | 0 | 45 | 0 |
| 28 px | 0 | 0 | 113 | 0 | 0 | 0 | 0 | 113 | 0 |

| Renderer | Line | Font size | Ink box | Ink | Relative to Win32 | Dark pixels | Colour fringes | Total colour ÷ 255 | Colour ≥ 192 | Strongest colour |
|---|---|---|---|---|---|---|---|---|---|---|
| Win32 | 1 | 12 px | 147 × 14 | 423.4 | 100.0 % | 276 | 86 % | 304.2 | 0 | 177 |
| Win32 | 2 | 12 px | 239 × 18 | 703.7 | 100.0 % | 465 | 88 % | 528.4 | 0 | 177 |
| Win32 | 3 | 12 px | 357 × 18 | 1034.5 | 100.0 % | 692 | 87 % | 764.3 | 0 | 177 |
| Win32 | 4 | 8 px | 99 × 9 | 226.4 | 100.0 % | 180 | 88 % | 172.1 | 0 | 177 |
| Win32 | 5 | 10 px | 121 × 11 | 310.0 | 100.0 % | 200 | 89 % | 249.7 | 0 | 177 |
| Win32 | 6 | 12 px | 147 × 14 | 423.4 | 100.0 % | 276 | 86 % | 304.2 | 0 | 177 |
| Win32 | 7 | 14 px | 170 × 16 | 611.6 | 100.0 % | 427 | 75 % | 351.1 | 0 | 177 |
| Win32 | 8 | 16 px | 197 × 18 | 783.9 | 100.0 % | 613 | 78 % | 409.7 | 0 | 177 |
| Win32 | 9 | 20 px | 246 × 23 | 1212.0 | 100.0 % | 940 | 61 % | 507.1 | 0 | 177 |
| Win32 | 10 | 28 px | 343 × 32 | 2495.0 | 100.0 % | 2180 | 48 % | 746.7 | 0 | 177 |
| Windows Forms | 1 | 12 px | 147 × 14 | 394.4 | 93.1 % | 272 | 86 % | 309.2 | 0 | 187 |
| Windows Forms | 2 | 12 px | 238 × 18 | 667.4 | 94.8 % | 479 | 89 % | 521.5 | 0 | 187 |
| Windows Forms | 3 | 12 px | 357 × 18 | 974.6 | 94.2 % | 698 | 88 % | 761.8 | 0 | 187 |
| Windows Forms | 4 | 8 px | 96 × 9 | 197.9 | 87.4 % | 128 | 87 % | 160.9 | 0 | 187 |
| Windows Forms | 5 | 10 px | 124 × 11 | 302.2 | 97.5 % | 180 | 88 % | 253.8 | 0 | 187 |
| Windows Forms | 6 | 12 px | 147 × 14 | 394.4 | 93.1 % | 272 | 86 % | 309.2 | 0 | 187 |
| Windows Forms | 7 | 14 px | 170 × 16 | 553.6 | 90.5 % | 410 | 77 % | 354.8 | 0 | 187 |
| Windows Forms | 8 | 16 px | 198 × 18 | 832.8 | 106.2 % | 721 | 67 % | 402.1 | 0 | 187 |
| Windows Forms | 9 | 20 px | 245 × 23 | 1177.9 | 97.2 % | 979 | 57 % | 506.3 | 0 | 187 |
| Windows Forms | 10 | 28 px | 343 × 32 | 2477.7 | 99.3 % | 2255 | 43 % | 734.5 | 0 | 187 |
| Avalonia | 1 | 12 px | 147 × 14 | 379.2 | 89.5 % | 242 | 85 % | 304.9 | 92 | 193 |
| Avalonia | 2 | 12 px | 237 × 18 | 631.4 | 89.7 % | 384 | 88 % | 516.3 | 125 | 193 |
| Avalonia | 3 | 12 px | 356 × 18 | 933.0 | 90.2 % | 604 | 88 % | 738.9 | 188 | 193 |
| Avalonia | 4 | 8 px | 98 × 9 | 183.4 | 81.0 % | 87 | 89 % | 165.9 | 46 | 193 |
| Avalonia | 5 | 10 px | 123 × 11 | 288.1 | 92.9 % | 155 | 88 % | 253.9 | 58 | 193 |
| Avalonia | 6 | 12 px | 147 × 14 | 379.2 | 89.5 % | 242 | 85 % | 304.9 | 92 | 193 |
| Avalonia | 7 | 14 px | 171 × 16 | 549.7 | 89.9 % | 346 | 86 % | 372.0 | 51 | 193 |
| Avalonia | 8 | 16 px | 195 × 18 | 706.7 | 90.2 % | 469 | 81 % | 410.2 | 54 | 193 |
| Avalonia | 9 | 20 px | 244 × 23 | 1134.5 | 93.6 % | 887 | 69 % | 499.0 | 45 | 193 |
| Avalonia | 10 | 28 px | 341 × 32 | 2354.5 | 94.4 % | 1948 | 50 % | 742.2 | 113 | 193 |
| Edge, default contrast | 1 | 12 px | 147 × 14 | 420.7 | 99.3 % | 308 | 78 % | 122.3 | 0 | 71 |
| Edge, default contrast | 2 | 12 px | 237 × 18 | 703.7 | 100.0 % | 526 | 76 % | 207.8 | 0 | 71 |
| Edge, default contrast | 3 | 12 px | 356 × 18 | 1035.4 | 100.1 % | 758 | 73 % | 298.7 | 0 | 71 |
| Edge, default contrast | 4 | 8 px | 98 × 9 | 211.1 | 93.2 % | 122 | 83 % | 71.7 | 0 | 71 |
| Edge, default contrast | 5 | 10 px | 123 × 11 | 325.8 | 105.1 % | 240 | 83 % | 104.0 | 0 | 71 |
| Edge, default contrast | 6 | 12 px | 147 × 14 | 420.7 | 99.3 % | 308 | 78 % | 122.3 | 0 | 71 |
| Edge, default contrast | 7 | 14 px | 171 × 16 | 624.0 | 102.0 % | 460 | 67 % | 147.3 | 0 | 74 |
| Edge, default contrast | 8 | 16 px | 195 × 18 | 794.0 | 101.3 % | 611 | 59 % | 162.2 | 0 | 74 |
| Edge, default contrast | 9 | 20 px | 244 × 23 | 1226.4 | 101.2 % | 1022 | 51 % | 200.7 | 0 | 74 |
| Edge, default contrast | 10 | 28 px | 341 × 32 | 2506.6 | 100.5 % | 2194 | 39 % | 297.8 | 0 | 74 |
| Edge, --text-contrast=0.5 | 1 | 12 px | 147 × 14 | 379.7 | 89.7 % | 242 | 84 % | 122.4 | 0 | 77 |
| Edge, --text-contrast=0.5 | 2 | 12 px | 237 × 18 | 632.7 | 89.9 % | 384 | 88 % | 207.4 | 0 | 77 |
| Edge, --text-contrast=0.5 | 3 | 12 px | 356 × 18 | 934.6 | 90.3 % | 604 | 88 % | 296.9 | 0 | 77 |
| Edge, --text-contrast=0.5 | 4 | 8 px | 98 × 9 | 183.9 | 81.2 % | 87 | 87 % | 66.6 | 0 | 77 |
| Edge, --text-contrast=0.5 | 5 | 10 px | 123 × 11 | 288.7 | 93.1 % | 155 | 87 % | 102.0 | 0 | 77 |
| Edge, --text-contrast=0.5 | 6 | 12 px | 147 × 14 | 379.7 | 89.7 % | 242 | 84 % | 122.4 | 0 | 77 |
| Edge, --text-contrast=0.5 | 7 | 14 px | 171 × 16 | 551.1 | 90.1 % | 346 | 75 % | 149.4 | 0 | 77 |
| Edge, --text-contrast=0.5 | 8 | 16 px | 195 × 18 | 708.4 | 90.4 % | 473 | 68 % | 164.7 | 0 | 77 |
| Edge, --text-contrast=0.5 | 9 | 20 px | 244 × 23 | 1136.6 | 93.8 % | 887 | 60 % | 200.6 | 0 | 77 |
| Edge, --text-contrast=0.5 | 10 | 28 px | 341 × 32 | 2356.9 | 94.5 % | 1948 | 42 % | 298.1 | 0 | 77 |
| Edge, --text-contrast=1.0 | 1 | 12 px | 147 × 14 | 420.7 | 99.3 % | 308 | 78 % | 122.3 | 0 | 71 |
| Edge, --text-contrast=1.0 | 2 | 12 px | 237 × 18 | 703.7 | 100.0 % | 526 | 76 % | 207.8 | 0 | 71 |
| Edge, --text-contrast=1.0 | 3 | 12 px | 356 × 18 | 1035.4 | 100.1 % | 758 | 73 % | 298.7 | 0 | 71 |
| Edge, --text-contrast=1.0 | 4 | 8 px | 98 × 9 | 211.1 | 93.2 % | 122 | 83 % | 71.7 | 0 | 71 |
| Edge, --text-contrast=1.0 | 5 | 10 px | 123 × 11 | 325.8 | 105.1 % | 240 | 83 % | 104.0 | 0 | 71 |
| Edge, --text-contrast=1.0 | 6 | 12 px | 147 × 14 | 420.7 | 99.3 % | 308 | 78 % | 122.3 | 0 | 71 |
| Edge, --text-contrast=1.0 | 7 | 14 px | 171 × 16 | 624.0 | 102.0 % | 460 | 67 % | 147.3 | 0 | 74 |
| Edge, --text-contrast=1.0 | 8 | 16 px | 195 × 18 | 794.0 | 101.3 % | 611 | 59 % | 162.2 | 0 | 74 |
| Edge, --text-contrast=1.0 | 9 | 20 px | 244 × 23 | 1226.4 | 101.2 % | 1022 | 51 % | 200.7 | 0 | 74 |
| Edge, --text-contrast=1.0 | 10 | 28 px | 341 × 32 | 2506.6 | 100.5 % | 2194 | 39 % | 297.8 | 0 | 74 |
| Chrome, default contrast | 1 | 12 px | 147 × 14 | 420.0 | 99.2 % | 308 | 85 % | 304.7 | 0 | 177 |
| Chrome, default contrast | 2 | 12 px | 237 × 18 | 702.6 | 99.9 % | 526 | 88 % | 517.4 | 0 | 177 |
| Chrome, default contrast | 3 | 12 px | 356 × 18 | 1034.0 | 99.9 % | 758 | 88 % | 743.7 | 0 | 177 |
| Chrome, default contrast | 4 | 8 px | 98 × 9 | 210.7 | 93.0 % | 122 | 89 % | 178.5 | 0 | 177 |
| Chrome, default contrast | 5 | 10 px | 123 × 11 | 325.2 | 104.9 % | 240 | 88 % | 259.0 | 0 | 177 |
| Chrome, default contrast | 6 | 12 px | 147 × 14 | 420.0 | 99.2 % | 308 | 85 % | 304.7 | 0 | 177 |
| Chrome, default contrast | 7 | 14 px | 171 × 16 | 622.6 | 101.8 % | 460 | 82 % | 366.6 | 0 | 185 |
| Chrome, default contrast | 8 | 16 px | 195 × 18 | 792.6 | 101.1 % | 611 | 75 % | 404.0 | 0 | 185 |
| Chrome, default contrast | 9 | 20 px | 244 × 23 | 1224.6 | 101.0 % | 1021 | 66 % | 499.9 | 0 | 185 |
| Chrome, default contrast | 10 | 28 px | 341 × 32 | 2503.7 | 100.3 % | 2194 | 46 % | 741.4 | 0 | 185 |
| Chrome, --text-contrast=0.5 | 1 | 12 px | 147 × 14 | 379.2 | 89.5 % | 242 | 85 % | 304.9 | 92 | 193 |
| Chrome, --text-contrast=0.5 | 2 | 12 px | 237 × 18 | 631.4 | 89.7 % | 384 | 88 % | 516.3 | 125 | 193 |
| Chrome, --text-contrast=0.5 | 3 | 12 px | 356 × 18 | 933.0 | 90.2 % | 604 | 88 % | 738.9 | 188 | 193 |
| Chrome, --text-contrast=0.5 | 4 | 8 px | 98 × 9 | 183.4 | 81.0 % | 87 | 89 % | 165.9 | 46 | 193 |
| Chrome, --text-contrast=0.5 | 5 | 10 px | 123 × 11 | 288.1 | 92.9 % | 155 | 88 % | 253.9 | 58 | 193 |
| Chrome, --text-contrast=0.5 | 6 | 12 px | 147 × 14 | 379.2 | 89.5 % | 242 | 85 % | 304.9 | 92 | 193 |
| Chrome, --text-contrast=0.5 | 7 | 14 px | 171 × 16 | 549.7 | 89.9 % | 346 | 86 % | 372.0 | 51 | 193 |
| Chrome, --text-contrast=0.5 | 8 | 16 px | 195 × 18 | 706.7 | 90.2 % | 469 | 81 % | 410.2 | 54 | 193 |
| Chrome, --text-contrast=0.5 | 9 | 20 px | 244 × 23 | 1134.5 | 93.6 % | 887 | 69 % | 499.0 | 45 | 193 |
| Chrome, --text-contrast=0.5 | 10 | 28 px | 341 × 32 | 2354.5 | 94.4 % | 1948 | 50 % | 742.2 | 113 | 193 |
| Chrome, --text-contrast=1.0 | 1 | 12 px | 147 × 14 | 420.0 | 99.2 % | 308 | 85 % | 304.7 | 0 | 177 |
| Chrome, --text-contrast=1.0 | 2 | 12 px | 237 × 18 | 702.6 | 99.9 % | 526 | 88 % | 517.4 | 0 | 177 |
| Chrome, --text-contrast=1.0 | 3 | 12 px | 356 × 18 | 1034.0 | 99.9 % | 758 | 88 % | 743.7 | 0 | 177 |
| Chrome, --text-contrast=1.0 | 4 | 8 px | 98 × 9 | 210.7 | 93.0 % | 122 | 89 % | 178.5 | 0 | 177 |
| Chrome, --text-contrast=1.0 | 5 | 10 px | 123 × 11 | 325.2 | 104.9 % | 240 | 88 % | 259.0 | 0 | 177 |
| Chrome, --text-contrast=1.0 | 6 | 12 px | 147 × 14 | 420.0 | 99.2 % | 308 | 85 % | 304.7 | 0 | 177 |
| Chrome, --text-contrast=1.0 | 7 | 14 px | 171 × 16 | 622.6 | 101.8 % | 460 | 82 % | 366.6 | 0 | 185 |
| Chrome, --text-contrast=1.0 | 8 | 16 px | 195 × 18 | 792.6 | 101.1 % | 611 | 75 % | 404.0 | 0 | 185 |
| Chrome, --text-contrast=1.0 | 9 | 20 px | 244 × 23 | 1224.6 | 101.0 % | 1021 | 66 % | 499.9 | 0 | 185 |
| Chrome, --text-contrast=1.0 | 10 | 28 px | 341 × 32 | 2503.7 | 100.3 % | 2194 | 46 % | 741.4 | 0 | 185 |
