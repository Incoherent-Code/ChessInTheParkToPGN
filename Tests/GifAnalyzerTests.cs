using ChessInTheParkToPGN;

namespace Tests {
   [TestClass]
   public class GifAnalyzerTests {
      [TestMethod]
      public void GifAnalyzerTest() {
         GifAnalyzer analysis;
         using (var stream = new MemoryStream(Resources.GIF1)) {
            analysis = new GifAnalyzer(stream);
         }
         Assert.IsTrue(analysis.differences.All(x => x.Length == 2 || x.Length == 4), "There shouldn't be any wierd diffs that have an odd number of different squares.");
         //This isnt the most comprehensive test, but I wanted to hurry it up to work on something else
         CollectionAssert.AreEqual(analysis.differences[0], new (int, int)[] { (6, 4), (4, 4) }, "Expected starting result");
         CollectionAssert.AreEqual(analysis.differences[16], new (int, int)[] { (4, 2), (5, 1) }, "Expected ending result");
      }
   }
}
