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
         foreach (string arg in args) {
            if (!File.Exists(arg)) {
               ErrorMessage($"Invalid file Path: {arg}");
               continue;
            }
            try {
               var game = new ChessGame(arg);
               game.saveToFile(Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(arg)) + ".pgn");
            }
            catch (Exception ex) {
               ErrorMessage(ex.Message);
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
