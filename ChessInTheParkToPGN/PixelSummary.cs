using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ChessInTheParkToPGN {
   /// <summary>
   /// Summarizes the board by getting the average pixel of each square on the chess board
   /// </summary>
   public class PixelSummary {
      //The gif usually takes on a size of 800 by 450
      //The chess board on the gif takes on a size of exactly 436 x 436
      //The chess board starts at exactly 182, 7
      //Each dark square is 55 by 55, and each light square is 54 by 54.
      private static int offsetX = 182;
      private static int offsetY = 7;
      private static float squareSize = 54.5f;

      /// <summary>
      /// 8 by 8 grid of the average pixel color of that square
      /// </summary>
      public Rgb24[,] summary = new Rgb24[8, 8];

      public PixelSummary(ImageFrame<Rgb24> frame) {
         for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
               var xCord = (int)Math.Ceiling(offsetX + squareSize * x) + 22;
               var yCord = (int)Math.Ceiling(offsetY + squareSize * y) + 22;
               var pixels = getRange(frame, xCord, yCord, 5, 1);
               var averagePixel = averagePixels(pixels);
               summary[x, y] = averagePixel;
            }
         }
      }
      private static readonly double maxDiffForResidualShadow = 0.09;
      /// <summary>
      /// Takes in another pixel summary and find all the cordinates where any color exceeds the maxDifference
      /// </summary>
      /// <param name="other"></param>
      /// <param name="maxDifference">Max difference any one value (R,G, or B) can have</param>
      /// <returns></returns>
      public (int x, int y)[] Compare(PixelSummary other, int maxDifference = 1) {
         List<(int, int)> output = [];
         for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
               var thisAveragePixel = this.summary[x, y];
               var otherAveragePixel = other.summary[x, y];
               if (Math.Abs(thisAveragePixel.R - otherAveragePixel.R) > maxDifference
                  || Math.Abs(thisAveragePixel.G - otherAveragePixel.G) > maxDifference
                  || Math.Abs(thisAveragePixel.B - otherAveragePixel.B) > maxDifference) {
                  double ratioR = (double)otherAveragePixel.R / thisAveragePixel.R;
                  double ratioG = (double)otherAveragePixel.G / thisAveragePixel.G;
                  double ratioB = (double)otherAveragePixel.B / thisAveragePixel.B;
                  //If the square has just lightened, ignore it (Has the same ratio of change)
                  if (Math.Abs(ratioR - ratioG) > maxDiffForResidualShadow
                     || Math.Abs(ratioG - ratioB) > maxDiffForResidualShadow
                     || Math.Abs(ratioB - ratioR) > maxDiffForResidualShadow) {
                     output.Add((x, y));
                  }
               }
            }
         }
         return output.ToArray();
      }

      /// <summary>
      /// Gets a set of pixels from the cordinates in a square given the starting position and size
      /// </summary>
      /// <param name="image"></param>
      /// <param name="size">Lengh and width of the square to search</param>
      /// <param name="spaceBetween">Space between each pixel measured</param>
      /// <returns></returns>
      private Rgb24[] getRange(ImageFrame<Rgb24> image, int startXCord, int startYCord, int size, int spaceBetween = 0) {
         var output = new Rgb24[(int)Math.Pow(size, 2)];
         for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
               var xCord = startXCord + x * (spaceBetween + 1);
               var yCord = startYCord + y * (spaceBetween + 1);
               var pixel = image[xCord, yCord];
               output[x * y + x] = pixel;
            }
         }
         return output;
      }
      public static Rgb24 averagePixels(Rgb24[] pixels) {
         return new Rgb24(
            Average(pixels.Select(x => x.R).ToArray()),
            Average(pixels.Select(x => x.G).ToArray()),
            Average(pixels.Select(x => x.B).ToArray())
         );
      }
      private static byte Average(params byte[] bytes) {
         return (byte)(bytes.Sum((x) => x) / bytes.Length);
      }
   }
}