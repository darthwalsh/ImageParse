﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageParse
{
    public class WrappingBitmap : IAsyncBitmap
    {
        public Bitmap Bitmap { get; set; }
        public int Width => Bitmap.Width;
        public int Height => Bitmap.Height;
        public void SetPixel(Point p, Color color) => Bitmap.SetPixel(p.X, p.Y, color);
        public Task<Color> GetPixel(Point p) => Task.FromResult(Bitmap.GetPixel(p.X, p.Y));
    }

    public abstract class DelgatingAsyncBitmap : IAsyncBitmap
    {
        public IAsyncBitmap IAsyncBitmap { private get; set; }
        public int Width => IAsyncBitmap.Width;
        public int Height => IAsyncBitmap.Height;
        public void SetPixel(Point p, Color color) => IAsyncBitmap.SetPixel(p, color);
        public virtual Task<Color> GetPixel(Point p) => IAsyncBitmap.GetPixel(p);
    }

    public class DelayedBitmap : DelgatingAsyncBitmap
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count;

        public int DelayInterval { get; set; }
        public int DelayCount { get; set; } = int.MaxValue;

        public async override Task<Color> GetPixel(Point p)
        {
            ++count;
            if (stopwatch.ElapsedMilliseconds >= DelayInterval || count > DelayCount)
            {
                await Task.Delay(DelayInterval);
                stopwatch.Restart();
                count = 0;
            }
            return await base.GetPixel(p);
        }
    }

    public class InvertingTrackingBitmap : DelgatingAsyncBitmap
    {
        public int KeepCount { get; set; }

        HashSetQueue<Point> touched = new HashSetQueue<Point>();

        public override async Task<Color> GetPixel(Point p)
        {
            var baseColor = await base.GetPixel(p);

            if (touched.Contains(p))
            {
                // color has already been inverted; restore it
                baseColor = baseColor.Invert();
            }
            else
            {
                touched.Enqueue(p);
                base.SetPixel(p, baseColor.Invert());

                if (touched.Count > KeepCount)
                {
                    var toClear = touched.Dequeue();
                    var inverted = await base.GetPixel(toClear);
                    base.SetPixel(toClear, inverted.Invert());
                }
            }

            return baseColor;
        }
    }

    class HashSetQueue<T>
    {
        HashSet<T> hashSet = new HashSet<T>();
        Queue<T> queue = new Queue<T>();

        public int Count => queue.Count;
        public bool Contains(T item) => hashSet.Contains(item);

        public void Enqueue(T item)
        {
            hashSet.Add(item);
            queue.Enqueue(item);
        }

        public T Dequeue()
        {
            T item = queue.Dequeue();
            hashSet.Remove(item);
            return item;
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Inverts colors (i.e. red to cyan) and greys (i.e. light greys to dark greys, and white to black)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Color Invert(this Color c) => Color.FromArgb(c.A, Invert(c.R), Invert(c.G), Invert(c.B));

        static byte Invert(byte b) => unchecked((byte)(b + 128));
    }
}
