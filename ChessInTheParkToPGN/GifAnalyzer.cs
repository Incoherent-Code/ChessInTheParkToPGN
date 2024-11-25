using SixLabors.ImageSharp;

namespace ChessInTheParkToPGN {
   public class GifAnalyzer {
      public ImageFrameCollection Frames;
      public GifAnalyzer(Stream fileStream) {
         var image = Image.Load(fileStream);
         if (image.Frames.Count <= 1) {
            throw new Exception("Invalid Image Format!");
         }
         Frames = image.Frames;

      }
   }
}
