﻿using static ChessInTheParkToPGN._2DArrayExtensions;

namespace Tests {
   [TestClass]
   public class _2DArrayExtensionTests {
      private int[,] array = new int[3, 3] {
            { 1, 3, 9},
            { 3, 9, 27 },
            { 9, 27, 81}
         };
      [TestMethod]
      public void TestFlatten() {
         var expected = new int[9] { 1, 3, 9, 3, 9, 27, 9, 27, 81 };
         var actual = array.Flatten();
         CollectionAssert.AreEqual(expected, actual, "2D arrays should flatten in a specific manner");
      }
      [TestMethod]
      public void TestGetColumn() {
         var expected = new int[3, 2] {
            { 3, 9 },
            { 9 , 27},
            { 27, 81 }
         };
         var actual = array.GetColumn(1);
         CollectionAssert.AreEqual(expected, actual, "2D array should only return the specific part requested.");
      }
      [TestMethod]
      public void TestFindIndexes() {
         var expected = new List<(int, int)>() {
            (0, 2),
            (1, 1),
            (2, 0)
         };
         var actual = array.FindIndexes((x, _) => x == 9);
         CollectionAssert.AreEqual(expected, actual, "Predicate should only return coordinates where 9 is present.");
      }
   }
}
