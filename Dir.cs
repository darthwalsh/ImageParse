using System.Drawing;

namespace ImageParse
{
    public static class Dir
    {
        public static Point Up(this Point p) => new Point(p.X, p.Y - 1);
        public static Point Down(this Point p) => new Point(p.X, p.Y + 1);
        public static Point Left(this Point p) => new Point(p.X - 1, p.Y);
        public static Point Right(this Point p) => new Point(p.X + 1, p.Y);
    }
}
