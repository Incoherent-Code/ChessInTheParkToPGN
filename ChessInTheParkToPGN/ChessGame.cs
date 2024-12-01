namespace ChessInTheParkToPGN {
   public class ChessGame {
      /// <summary>
      /// Keeps track of the chess game
      /// Do note, the x and y axis are inverted between gif analyzer and this
      /// So using it would be state[y, x]
      /// </summary>
      private (Piece, bool isBlack)[,] state = new (Piece, bool isBlack)[8, 8] {
         {(Piece.Rook, true), (Piece.Knight, true), (Piece.Bishop, true), (Piece.Queen, true), (Piece.King, true), (Piece.Bishop, true), (Piece.Knight, true), (Piece.Rook, true)},
         {(Piece.Pawn, true), (Piece.Pawn, true), (Piece.Pawn, true), (Piece.Pawn, true), (Piece.Pawn, true), (Piece.Pawn, true), (Piece.Pawn, true), (Piece.Pawn, true)},
         {(Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false)},
         {(Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false)},
         {(Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false)},
         {(Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false), (Piece.None, false)},
         {(Piece.Pawn, false), (Piece.Pawn, false), (Piece.Pawn, false), (Piece.Pawn, false), (Piece.Pawn, false), (Piece.Pawn, false), (Piece.Pawn, false), (Piece.Pawn, false)},
         {(Piece.Rook, false), (Piece.Knight, false), (Piece.Bishop, false), (Piece.Queen, false), (Piece.King, false), (Piece.Bishop, false), (Piece.Knight, false), (Piece.Rook, false)}
      };
      private static readonly (Piece, bool isBlack) EmptySpot = (Piece.None, false);

      public PGN result = new PGN();
      public GifAnalyzer analyzer;

      public static (int x, int y)[] whiteLongCastleDiff = [(0, 7), (2, 7), (3, 7), (4, 7)];
      public static (int x, int y)[] whiteShortCastleDiff = [(4, 7), (5, 7), (6, 7), (7, 7)];
      public static (int x, int y)[] blackLongCastleDiff = [(0, 0), (2, 0), (3, 0), (4, 0)];
      public static (int x, int y)[] blackShortCastleDiff = [(4, 0), (5, 0), (6, 0), (7, 0)];
      public ChessGame(string filePath) {
         using (var file = File.OpenRead(filePath)) {
            analyzer = new GifAnalyzer(file);
         }
         result.metadata.Add("White", analyzer.whitePlayer);
         result.metadata.Add("Black", analyzer.blackPlayer);
         for (int i = 0; i < analyzer.moves; i++) {
            var blackIsMoving = i % 2 == 1;
            var diff = analyzer.differences[i];
            //If not moving just one piece, maybe they are castling
            if (diff.Length != 2) {
               if (!blackIsMoving && SameDiffrence(diff, whiteLongCastleDiff)) {
                  result.moves.Add("0-0-0");
                  state[7, 0] = EmptySpot;
                  state[7, 2] = (Piece.King, false);
                  state[7, 3] = (Piece.Rook, false);
                  state[7, 4] = EmptySpot;
                  continue;
               }
               else if (!blackIsMoving && SameDiffrence(diff, whiteShortCastleDiff)) {
                  result.moves.Add("0-0");
                  state[7, 4] = EmptySpot;
                  state[7, 5] = (Piece.Rook, false);
                  state[7, 6] = (Piece.King, false);
                  state[7, 7] = EmptySpot;
                  continue;
               }
               else if (blackIsMoving && SameDiffrence(diff, blackLongCastleDiff)) {
                  result.moves.Add("0-0-0");
                  state[0, 0] = EmptySpot;
                  state[0, 2] = (Piece.King, true);
                  state[0, 3] = (Piece.Rook, true);
                  state[0, 4] = EmptySpot;
                  continue;
               }
               else if (blackIsMoving && SameDiffrence(diff, blackShortCastleDiff)) {
                  result.moves.Add("0-0");
                  state[0, 4] = EmptySpot;
                  state[0, 5] = (Piece.Rook, true);
                  state[0, 6] = (Piece.King, true);
                  state[0, 7] = EmptySpot;
                  continue;
               }
               else {
                  throw new Exception("Could not identify move " + i);
               }
            }
            var pieceInFirstSpot = state[diff[0].y, diff[0].x];
            var oldSpot = (pieceInFirstSpot.Item1 != Piece.None && pieceInFirstSpot.isBlack == blackIsMoving) ? diff[0] : diff[1];
            var newSpot = (pieceInFirstSpot.Item1 != Piece.None && pieceInFirstSpot.isBlack == blackIsMoving) ? diff[1] : diff[0];
            var pieceMoving = state[oldSpot.y, oldSpot.x];
            var pieceLandedon = state[newSpot.y, newSpot.x];

            //Build algebraic chess notation
            string algNotation = (pieceMoving.Item1 != Piece.Pawn) ? ((char)pieceMoving.Item1).ToString() : "";
            //TODO: Ambiguous Piece Moves
            //Piece taking symbol (x)
            if (pieceLandedon.Item1 != Piece.None)
               algNotation += "x";
            //Convert to alg notaion (e4, d5, etc)
            algNotation += ((char)(newSpot.x + 97)).ToString() + (8 - newSpot.y).ToString();
            //TODO: Checks and mates

            result.moves.Add(algNotation);
            state[newSpot.y, newSpot.x] = pieceMoving;
            state[oldSpot.y, oldSpot.x] = EmptySpot;
         }
      }
      public void saveToFile(string path) {
         using (var stream = File.Create(path)) {
            result.saveToFile(stream);
         }
      }
      /// <summary>
      /// Takes in two differences and determines if they are equal.
      /// </summary>
      private static bool SameDiffrence((int x, int y)[] a, (int x, int y)[] b) {
         return a.Length == b.Length && a.Intersect(b).Count() == a.Length;
      }

   }
   /// <summary>
   /// Each number corresponds to the ascii character that each piece translates to in algebraic notation
   /// Except for the pawn, of course
   /// </summary>
   public enum Piece {
      None = 0,
      Pawn = 1,
      King = 75,
      Queen = 81,
      Rook = 82,
      Bishop = 66,
      Knight = 78
   }
}
