using ChessInTheParkToPGN;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Tests {
   [TestClass]
   public class PixelSummaryTests {
      [TestMethod]
      public void TestNoDifference() {
         var frame17 = Image.Load<Rgb24>(Resources.Frame17);
         var frame17Sum = new PixelSummary(frame17.Frames[0]);
         var diff = frame17Sum.Compare(frame17Sum);
         Assert.IsTrue(diff.Length == 0, "The same frame should find no differences.");
      }
      [TestMethod]
      public void TestDifference() {
         var frame17 = Image.Load<Rgb24>(Resources.Frame17);
         var frame17Sum = new PixelSummary(frame17.Frames[0]);
         var frame18 = Image.Load<Rgb24>(Resources.Frame18);
         var frame18Sum = new PixelSummary(frame18.Frames[0]);
         var diff = frame17Sum.Compare(frame18Sum);
         (int, int)[] expected = [(5, 2), (6, 0)];
         Assert.IsTrue(diff.Intersect(expected).Count() == 2, "Should find these two differences in this frame.");
      }
   }
}
