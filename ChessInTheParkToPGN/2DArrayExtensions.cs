namespace ChessInTheParkToPGN {
   public static class _2DArrayExtensions {
      public static T[][] GetArrayArray<T>(this T[,] twoDArray) {
         var output = new T[twoDArray.GetLength(0)][];
         for (var i = 0; i < twoDArray.GetLength(0); i++) {
            var array = new T[twoDArray.GetLength(1)];
            for (var j = 0; j < twoDArray.GetLength(1); j++) {
               array[j] = twoDArray[i, j];
            }
            output[i] = array;
         }
         return output;
      }
      /// <summary>
      /// Returns the 2D array but only within the y cordinate specified.
      /// </summary>
      /// <param name="start">Start y cordinate</param>
      /// <param name="end">The last y to get, or end of array if not specified.</param>
      public static T[,] GetColumn<T>(this T[,] array, int start, int? end = null) {
         end ??= array.GetLength(1) - 1;
         var newLength = end - start + 1;
         var output = new T[array.GetLength(0), (int)newLength];
         for (var i = 0; i < array.GetLength(0); i++) {
            for (var j = 0; j < newLength; j++) {
               output[i, j] = array[i, j + start];
            }
         }
         return output;
      }
      /// <summary>
      /// Flattens a 2d array into a 1d array
      /// </summary>
      public static T[] Flatten<T>(this T[,] array) {
         var output = new T[array.GetLength(0) * array.GetLength(1)];
         for (var x = 0; x < array.GetLength(0); x++) {
            for (var y = 0; y < array.GetLength(1); y++) {
               output[x * array.GetLength(1) + y] = array[x, y];
            }
         }
         return output;
      }

   }
}
