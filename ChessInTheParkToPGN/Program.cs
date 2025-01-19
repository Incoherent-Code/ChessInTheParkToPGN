using System.Diagnostics;

namespace ChessInTheParkToPGN {
   internal class Program {
      private static string helpText = @"ChessInTheParkToPGN Options:
 -h       -  Displays this help message and exits
 -l [arg] -  OCR language (ex: eng) (Corresponding tesseract file must be present in the /tessdata folder
 -l off   -  Disables OCR (May be necessary on UNIX)
 [file]   -  Gif file to analyze and spit out a game pgn. (you can have any amount of these)";

      private static int errorAmount = 0;
      public static List<string> pathsToProcess = [];
      public static bool useTessearct = true;
      public static string tesseractLanguage = "eng";
      static void Main(string[] args) {
         //Handle arguments
         for (int i = 0; i < args.Length; i++) {
            var arg = args[i];
            switch (arg) {
               case "-h":
               case "--help":
                  Console.WriteLine(helpText);
                  Environment.Exit(0);
                  break;
               case "-l":
                  //No next argument
                  if (i + 1 > args.Length)
                     break;
                  var next = args[i + 1];
                  if (next == "off") {
                     useTessearct = false;
                  }
                  else {
                     if (File.Exists(Path.Combine(GifAnalyzer.TessDataPath, $"{next}.traineddata"))) {
                        tesseractLanguage = next;
                     }
                     else {
                        ErrorMessage($"The Trained Data for language \"{next}\" was not found. OCR will be disabled.");
                        useTessearct = false;
                     }
                  }
                  //skip next argument
                  i++;
                  break;
               default:
                  if (!File.Exists(arg)) {
                     ErrorMessage($"Unknown File Path or argument: {arg}");
                     break;
                  }
                  pathsToProcess.Add(arg);
                  break;
            }
         }

         var timer = Stopwatch.StartNew();

         foreach (var path in pathsToProcess) {
            if (!File.Exists(path)) {
               ErrorMessage($"Invalid file Path: {path}");
               continue;
            }
            try {
               var game = new ChessGame(path);
               var fileName = Path.GetFileNameWithoutExtension(path) + ".pgn";
               var savePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
               game.saveToFile(savePath);
               Console.WriteLine($"Saved {fileName} to \"{savePath}\"");
            }
            catch (Exception ex) {
               ErrorMessage($"Error reading Gif {Path.GetFileName(path)}: " + ex.Message);
               errorAmount++;
               if (Debugger.IsAttached)
                  throw;
            }
         }
         timer.Stop();
         Console.WriteLine($"Finished analyzing {pathsToProcess.Count - errorAmount} games in {timer.Elapsed.Seconds} seconds.");
         Environment.Exit(errorAmount);
      }
      public static void ErrorMessage(string message) {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine(message);
         Console.ForegroundColor = ConsoleColor.White;
      }

      public static void WarningMessage(string message) {
         Console.ForegroundColor = ConsoleColor.Yellow;
         Console.WriteLine(message);
         Console.ForegroundColor = ConsoleColor.White;
      }
   }
}
