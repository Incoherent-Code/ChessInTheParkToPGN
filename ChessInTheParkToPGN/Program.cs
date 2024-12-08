using System.Diagnostics;

/* TODO:
 * Add support for En Passant
 * Add support for promotion
 */

namespace ChessInTheParkToPGN {
   internal class Program {
      static void Main(string[] args) {
         if (args.Length == 0) {
            ErrorMessage("Please start the analyzer with a valid file path!");
            Environment.Exit(1);
         }
         for (int i = 0; i < args.Length; i++) {
            var arg = args[i];
            if (!File.Exists(arg)) {
               ErrorMessage($"Invalid file Path: {arg}");
               continue;
            }
            try {
               var game = new ChessGame(arg);
               var fileName = Path.GetFileNameWithoutExtension(arg) + ".pgn";
               var savePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
               game.saveToFile(savePath);
               Console.WriteLine($"Saved {fileName} to \"{savePath}\"");
            }
            catch (Exception ex) {
               ErrorMessage($"Error reading Gif {i + 1}: " + ex.Message);
               if (Debugger.IsAttached)
                  throw;
            }
         }
      }
      public static void ErrorMessage(string message) {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine(message);
         Console.ForegroundColor = ConsoleColor.White;
      }
   }
}
