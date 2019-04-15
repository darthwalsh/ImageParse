using System.Drawing;

namespace ImageParse
{
    public static class Points
    {
        public static Point Average(this Point a, Point b) => new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
    }
}
