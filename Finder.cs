using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public async Task<Point> FindColor(Point p, Func<Color, bool> foundColor, Func<Point, Point> nextPoint)
        {
            for (; 0 <= p.Y && p.Y < image.Height && 0 <= p.X && p.X < image.Width; p = nextPoint(p))
            {
                if (foundColor(await image.GetPixel(p)))
                {
                    await Pulse(p);
                    return p;
                }
            }
            throw new InvalidOperationException("Couldn't find color!");
        }

        public Task<Point> FindColor(Point p, Color color, Func<Point, Point> nextPoint) =>
            FindColor(p, c => color.ArgbEquals(c), nextPoint);

        public async Task Pulse(Point center)
        {
            var size = new Size(8, 8);

            var bounding = new Rectangle(center - size, size + size);
            for (var p = new Point(bounding.Left, bounding.Top); bounding.Contains(p); ++p.X) { await TryGetPixel(p); }
            for (var p = new Point(bounding.Left, bounding.Top); bounding.Contains(p); ++p.Y) { await TryGetPixel(p); }
            for (var p = new Point(bounding.Left, bounding.Bottom - 1); bounding.Contains(p); ++p.X) { await TryGetPixel(p); }
            for (var p = new Point(bounding.Right - 1, bounding.Top); bounding.Contains(p); ++p.Y) { await TryGetPixel(p); }
        }

        public async Task<Rectangle> FindBoundary(Point point)
        {
            var color = await image.GetPixel(point);

            var toSearch = new Queue<Point>(new[] { point });
            var seen = new HashSet<Point>();
            while (toSearch.Any())
            {
                var next = toSearch.Dequeue();
                if (0 <= next.Y && next.Y < image.Height && 0 <= next.X && next.X < image.Width)
                {
                    // Only check color after checking seen to avoid async noise
                    if (seen.Contains(next) || !color.ArgbEquals(await image.GetPixel(next)))
                    {
                        continue;
                    }
                    seen.Add(next); // Only Add() after checking color to get correct edge semantics
                    toSearch.Enqueue(Dir.Up(next));
                    toSearch.Enqueue(Dir.Down(next));
                    toSearch.Enqueue(Dir.Left(next));
                    toSearch.Enqueue(Dir.Right(next));
                }
            }

            var topLeft = new Point(seen.Min(p => p.X), seen.Min(p => p.Y));
            var bottomRight = new Point(seen.Max(p => p.X) + 1, seen.Max(p => p.Y) + 1);
            return new Rectangle(topLeft, (Size)bottomRight - (Size)topLeft);
        }

        Task TryGetPixel(Point p)
        {
            if (0 <= p.Y && p.Y < image.Height && 0 <= p.X && p.X < image.Width)
            {
                return image.GetPixel(p);
            }
            return Task.CompletedTask;
        }
    }
}
