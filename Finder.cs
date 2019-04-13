using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageParse
{
    public class Finder
    {
        IAsyncBitmap image;
        public Finder(IAsyncBitmap image)
        {
            this.image = image;
        }

        async Task<Point> FindColor(Point p, Color color, Func<Point, Point> nextPoint)
        {
            for (; 0 <= p.Y && p.Y < image.Height && 0 <= p.X && p.X < image.Height; p = nextPoint(p))
            {
                if (await image.GetPixel(p) == color)
                {
                    await Pulse(p);
                    return p;
                }
            }
            throw new InvalidOperationException($"Couldn't find {color}!");
        }

        async Task Pulse(Point center)
        {
            var size = new Size(8, 8);

            var bounding = new Rectangle(center - size, size + size);
            for (var p = new Point(bounding.Left, bounding.Top); bounding.Contains(p); ++p.X) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Left, bounding.Top); bounding.Contains(p); ++p.Y) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Left, bounding.Bottom - 1); bounding.Contains(p); ++p.X) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Right - 1, bounding.Top); bounding.Contains(p); ++p.Y) { await image.GetPixel(p); }
        }
    }
}
