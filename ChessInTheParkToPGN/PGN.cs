using static System.Text.Encoding;

namespace ChessInTheParkToPGN {
   public class PGN {
      /// <summary>
      /// Typically contains data like:
      /// Event: What chess event was this
      /// Site: Location (website or location like "Paris FRA")
      /// Date: YYYY.MM.DD
      /// White: While player name
      /// Black: Black player name
      /// </summary>
      public Dictionary<string, string> metadata = new Dictionary<string, string>() {
         { "Site", "discord.com" }
      };
      /// <summary>
      /// Each move that takes place in a chess game
      /// </summary>
      public List<string> moves = [];
      /// <summary>
      /// Whether black or white won
      /// </summary>
      public bool whiteWon = true;

      public void saveToFile(Stream file) {
         foreach (var meta in metadata) {
            var line = $"[{meta.Key} \"{meta.Value}\"]\n";
            file.Write(UTF8.GetBytes(line));
         }
         file.Write(UTF8.GetBytes("\n"));
         for (int i = 0; i < (moves.Count / 2f); i++) {
            string firstMove = moves[i * 2];
            var line = $"{i + 1}. {firstMove} ";
            if (moves.Count >= i * 2 + 2) {
               string secondMove = moves[i * 2 + 1];
               line += $"{secondMove} ";
            }
            file.Write(UTF8.GetBytes(line));
         }
         file.Write(UTF8.GetBytes((whiteWon) ? "1-0" : "0-1"));
      }
   }
}
