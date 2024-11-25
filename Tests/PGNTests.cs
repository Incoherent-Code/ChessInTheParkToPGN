using ChessInTheParkToPGN;
using static System.Text.Encoding;

namespace Tests {
   [TestClass]
   public class PGNTests {
      public static readonly string expectedSaveResult =
         "[White \"Gary Bowser\"]\n" +
         "[Black \"Nintendo\"]\n" +
         "[Termination \"Nintendo won by Checkmate\"]\n" +
         "\n" +
         "1. e4 g5 2. Nc3 f5 3. Qh5# 0-1";
      [TestMethod]
      public void TestSaveFile() {
         var testPGN = new PGN() {
            metadata = new Dictionary<string, string> {
               {"White", "Gary Bowser" },
               {"Black", "Nintendo" },
               {"Termination", "Nintendo won by Checkmate" }
            },
            moves = ["e4", "g5", "Nc3", "f5", "Qh5#"],
            whiteWon = false
         };
         var testStream = new MemoryStream();
         testPGN.saveToFile(testStream);
         string result = UTF8.GetString(testStream.ToArray());
         Assert.AreEqual(expectedSaveResult, result, true, "Must match PGN Format");
      }
   }
}