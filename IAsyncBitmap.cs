using System.Drawing;
using System.Threading.Tasks;

namespace ImageParse
{
    public interface IAsyncBitmap
    {
        int Width { get; }
        int Height { get; }
        void SetPixel(Point p, Color color);
        Task<Color> GetPixel(Point p);
    }
}
