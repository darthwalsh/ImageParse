using System.Drawing;
using System.Threading.Tasks;
using ImageParse;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class FinderTests
    {
        [TestMethod]
        public async Task FindColorTest()
        {
            var image = new Bitmap(5, 1);
            image.SetPixel(3, 0, Color.FromArgb(255, 0, 0));

            var finder = new Finder(new WrappingBitmap
            {
                Bitmap = image,
            });

            var found = await finder.FindColor(new Point(), Color.Red, Dir.Right);
            Assert.AreEqual(found.X, 3);
        }

        [TestMethod]
        public async Task FindBoundaryTest()
        {
            var text = @"
__________
___****___
__******__
____**____
___****___
_********_
_********_
_**_**_**_
____**____
__________
".Trim().Split("\r\n");
            var image = new Bitmap(text[0].Length, text.Length);
            for (var y = 0; y < text.Length; ++y)
            {
                for (var x = 0; x < text[0].Length; ++x)
                {
                    image.SetPixel(x, y, text[text.Length - 1 - y][x] == '*' ? Color.Red : Color.White);
                }
            }

            var finder = new Finder(new WrappingBitmap
            {
                Bitmap = image,
            });

            var boundary = await finder.FindBoundary(new Point(4, 5));
            Assert.AreEqual(boundary, new Rectangle(1, 1, 8, 8));
        }
    }
}
