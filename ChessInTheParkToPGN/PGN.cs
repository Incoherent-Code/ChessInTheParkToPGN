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
         { "Event", "Discord Chess In The Park" },
         { "Site", "discord.com" }
      };
      /// <summary>
      /// Each move that takes place in a chess game
      /// </summary>
      public List<string> moves = [];
      /// <summary>
      /// Whether black or white won, or draw if null
      /// </summary>
      public bool? whiteWon;

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
         if (whiteWon != null)
            file.Write(UTF8.GetBytes((whiteWon == true) ? "1-0" : "0-1"));
         else
            file.Write(UTF8.GetBytes("1/2-1/2"));
      }
   }
}
