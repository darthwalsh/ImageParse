using System.Drawing;

namespace ImageParse
{
    public static class Rects
    {
        public static Point TopLeft(this Rectangle rect) => rect.Location;
        public static Point TopRight(this Rectangle rect) => new Point(rect.Right, rect.Top);
        public static Point BottomLeft(this Rectangle rect) => new Point(rect.Left, rect.Bottom);
        public static Point BottomRight(this Rectangle rect) => new Point(rect.Right, rect.Bottom);
        public static Point Top(this Rectangle rect) => rect.TopLeft().Average(rect.TopRight());
        public static Point Bottom(this Rectangle rect) => rect.BottomLeft().Average(rect.BottomRight());
        public static Point Left(this Rectangle rect) => rect.TopLeft().Average(rect.BottomLeft());
        public static Point Right(this Rectangle rect) => rect.TopRight().Average(rect.BottomRight());
        public static Point Middle(this Rectangle rect) => rect.TopLeft().Average(rect.BottomRight());
    }
}
