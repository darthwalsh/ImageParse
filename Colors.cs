using System.Drawing;

namespace ImageParse
{
    public static class Colors
    {
        public static bool ArgbEquals(this Color @this, Color other) => @this.ToArgb().Equals(other.ToArgb());
    }
}
