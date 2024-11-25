using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace ChessInTheParkToPGN {
   public class GifAnalyzer {
      public bool fromBlackPOV = false;
      public string whitePlayer = "Player1";
      public string blackPlayer = "Player2";
      public int moves;

      public List<(int x, int y)[]> differences = [];
      public GifAnalyzer(Stream fileStream) {
         var image = Image.Load<Rgb24>(fileStream);
         if (image.Frames.Count <= 1) {
            throw new Exception("Invalid Image Format!");
         }
         this.moves = image.Frames.Count;
         var Frames = image.Frames;
         for (int i = 1; i < Frames.Count; i++) {
            var lastFrame = new PixelSummary(Frames[i - 1]);
            var currentFrame = new PixelSummary(Frames[i]);
            var diff = currentFrame.Compare(lastFrame);

            //Determine whether or not this is black POV or white POV
            if (i == 1) {
               if (diff.Length != 2) {
                  throw new Exception("Failed to interpret first move. Maybe try a more contrasty chess theme?");
               }
               //If First move is on this side, then this is from black POV
               fromBlackPOV = diff[1].Item1 <= 4;
            }
            differences.Add(diff);
         }
      }
   }
}
