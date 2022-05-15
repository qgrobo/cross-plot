# cross-plot

![Preview](https://github.com/qgrobo/cross-plot/blob/main/images/preview.png)

Cross-plot makes a pseudo-3D diagram of two-dimensional dataset. This is a little experiment for testing of idea. The designer idea of data visualization. And I used this project to learn the C# language, it's just my second C# project.

## Packages

I began my learning from System.Drawing.Common package, and later switched to SkiaSharp library.

For example, I used System.Drawing for a shear of layered diagrams.

Shear:
```C#
                Matrix myMatrix = new Matrix();
                myMatrix.Shear(0, 0.724f);
                graphics.MultiplyTransform(myMatrix);
```

But I used SkiaSharp for blur effect and for layer blending.

Blur:
```C#
                paint.ImageFilter = SKImageFilter.CreateBlur(sigma, sigma);
                canvas.DrawBitmap(BitmapExtensions.ToSKBitmap(bitmap), new SKPoint(0, 0), paint);
```

Blend:
```C#
                paint.Color = paint.Color.WithAlpha((byte)(0xFF * Math.Min(1, alpha)));
                canvas.DrawBitmap(BitmapExtensions.ToSKBitmap(bitmap), new SKPoint(0, 0), paint);
```

## Animation

![GIF 480x270 1845KB](https://github.com/qgrobo/cross-plot/blob/main/images/CrossPlot.gif)

Cross-plot makes only separated frames and saves them to `out` folder. Then I make an animated diagram by external tool, ffmpeg. It can make GIF or video file by frames from `out` folder. See the batch files.

make_gif.cmd:
```winbatch
ffmpeg -f image2 -framerate 25 -i ./CrossPlot/out/%%04d.png -s 480x270 ./CrossPlot/out/CrossPlot.gif
```

make_video.cmd:
```winbatch
ffmpeg -framerate 25 -i ./CrossPlot/out/%%04d.png -vcodec libx264 -pix_fmt yuv420p -vf scale=1920x1080 ./CrossPlot/out/CrossPlot.mp4
```

https://user-images.githubusercontent.com/1731533/168454345-0d8f32f1-b88d-4395-bfb3-0d0b34254d0e.mp4
