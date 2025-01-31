using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Tesseract;


namespace ChessInTheParkToPGN {
   public class GifAnalyzer {
      public bool fromBlackPOV = false;
      public string whitePlayer = "Player1";
      public string blackPlayer = "Player2";
      public string? terminationReason;
      public int moves;
      public static string TessDataPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"); } }

      public List<(int x, int y)[]> differences = [];
      public GifAnalyzer(Stream fileStream) {
         string currentPlayerName;
         string opposingPlayerName;

         var image = Image.Load<Rgb24>(fileStream);
         if (image.Frames.Count <= 1) {
            throw new Exception("Invalid Image Format!");
         }
         this.moves = image.Frames.Count;
         var Frames = image.Frames;

         //Use OCR to get player names
         var opposingStream = new MemoryStream();
         using (var opposingNameImage = image.Frames.CloneFrame(0)) {
            opposingNameImage.Mutate(i => i
               .Crop(new Rectangle(624, 0, 175, 65))
               //More optimal settings for Tesseract
               .Grayscale()
               .Resize(175 * 4, 65 * 4)
            );
            opposingNameImage.SaveAsBmp(opposingStream);
         }

         var currentStream = new MemoryStream();
         using (var currentNameImage = image.Frames.CloneFrame(0)) {
            currentNameImage.Mutate(i => i
               .Crop(new Rectangle(624, 381, 175, 65))
               .Grayscale()
               .Resize(175 * 4, 65 * 4)
            );
            currentNameImage.SaveAsBmp(currentStream);
         }

         //Read termination from text
         var terminationStream = new MemoryStream();
         using (var terminationTextImage = image.Frames.CloneFrame(0)) {
            terminationTextImage.Mutate(i => i
               .Crop(new Rectangle(0, 100, 174, 174))
               //More optimal settings for Tesseract
               .Grayscale()
            //Already at near optimal size
            );
            terminationTextImage.SaveAsBmp(terminationStream);
         }

         if (Program.useTessearct && File.Exists(Path.Combine(TessDataPath, $"{Program.tesseractLanguage}.traineddata"))) {
            using (TesseractEngine TEngine = new TesseractEngine(TessDataPath, Program.tesseractLanguage, EngineMode.Default)) {
               using (var page = TEngine.Process(Pix.LoadFromMemory(opposingStream.ToArray()))) {
                  opposingPlayerName = page.GetText().Trim();
               }
               using (var page = TEngine.Process(Pix.LoadFromMemory(currentStream.ToArray()))) {
                  currentPlayerName = page.GetText().Trim();
               }
               using (var page = TEngine.Process(Pix.LoadFromMemory(terminationStream.ToArray()))) {
                  this.terminationReason = page.GetText().Trim();
               }
            }
         }
         else {
            //Program.ErrorMessage("Could not load the training data for Tesseract. Please ensure that the English tesseract data is placed in /tessdata. Name recognition will not run.");
            currentPlayerName = "Player1";
            opposingPlayerName = "Player2";
         }

         for (int i = 1; i < Frames.Count; i++) {
            var lastFrame = new PixelSummary(Frames[i - 1]);
            var currentFrame = new PixelSummary(Frames[i]);
            var diff = currentFrame.Compare(lastFrame);

            //This is actually the second move of the game, because the first move is depicted in frame 1 with no frame of just the initial setup
            if (i == 1) {
               if (diff.Length != 3) {
                  throw new Exception("Failed to interpret second move. Maybe try a more contrasty chess theme?");
               }
               //If Second move is on this side, then this is from black POV
               fromBlackPOV = diff[1].y >= 4;
               //Add player names
               blackPlayer = fromBlackPOV ? currentPlayerName : opposingPlayerName;
               whitePlayer = fromBlackPOV ? opposingPlayerName : currentPlayerName;
               //Determine the difference for the first move
               var startingPieces = lastFrame.summary.GetColumn(fromBlackPOV ? 0 : 6, fromBlackPOV ? 1 : 7);
               var startingPossibleSpaces = lastFrame.summary.GetColumn(fromBlackPOV ? 2 : 4, fromBlackPOV ? 3 : 5);
               var avgStartingPiecePixel = PixelSummary.averagePixels(startingPieces.Flatten());

               var likelyMovedFromSpot = findFurthestIndex(startingPieces, avgStartingPiecePixel);
               var likelyMovedToSpot = findClosestIndex(startingPossibleSpaces, avgStartingPiecePixel);

               //Add to make the values now consistent again
               likelyMovedFromSpot.y += fromBlackPOV ? 0 : 6;
               likelyMovedToSpot.y += fromBlackPOV ? 2 : 4;

               //Push the assumed first move
               differences.Add([likelyMovedFromSpot, likelyMovedToSpot]);
            }
            differences.Add(diff);
         }
      }
      /// <summary>
      /// Returns the index of the closest match to the pixel.
      /// </summary>
      private (int x, int y) findClosestIndex(Rgb24[,] array, Rgb24 match) {
         var recordIndex = (0, 0);
         int recordNumber = 1000;
         for (int x = 0; x < array.GetLength(0); x++) {
            for (int y = 0; y < array.GetLength(1); y++) {
               var currentPixel = array[x, y];
               var difference = Math.Abs(currentPixel.R - match.R) + Math.Abs(currentPixel.G - match.G) + Math.Abs(currentPixel.B - match.B);
               difference = Math.Abs(difference);
               if (difference < recordNumber) {
                  recordNumber = difference;
                  recordIndex = (x, y);
               }
            }
         }
         return recordIndex;
      }
      /// <summary>
      /// Returns the index of the closest match to the pixel.
      /// </summary>
      private (int x, int y) findFurthestIndex(Rgb24[,] array, Rgb24 match) {
         var recordIndex = (0, 0);
         int recordNumber = 0;
         for (int x = 0; x < array.GetLength(0); x++) {
            for (int y = 0; y < array.GetLength(1); y++) {
               var currentPixel = array[x, y];
               var difference = Math.Abs(currentPixel.R - match.R) + Math.Abs(currentPixel.G - match.G) + Math.Abs(currentPixel.B - match.B);
               difference = Math.Abs(difference);
               if (difference > recordNumber) {
                  recordNumber = difference;
                  recordIndex = (x, y);
               }
            }
         }
         return recordIndex;
      }
   }
}
