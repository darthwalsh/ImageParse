# ImageParse

Library to help debug image parsing code.

Defines a wrapper around `System.Drawing.Bitmap` like:

    public interface IAsyncBitmap
    {
        int Width { get; }
        int Height { get; }
        void SetPixel(Point p, Color color);
        Task<Color> GetPixel(Point p);
    }

If you write your parsing code asynchronously, then a WinForm GUI can update, showing which 
pixel your code is currently reading!

See example usage in [LifeLog2CSV](https://github.com/darthwalsh/LifeLog2CSV/blob/master/LifeLogGUI/ParserForm.cs) project.
