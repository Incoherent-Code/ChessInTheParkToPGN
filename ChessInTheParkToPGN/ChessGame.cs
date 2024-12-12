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
            //Invert board if from black pov
            if (analyzer.fromBlackPOV) {
               diff = diff.Select(cord => (7 - cord.x, 7 - cord.y)).ToArray();
            }
            //If not moving just one piece, maybe they are castling
            if (diff.Length != 2) {
               if (!blackIsMoving && SameDiffrence(diff, whiteLongCastleDiff)) {
                  result.moves.Add("0-0-0");
                  state[7, 0] = EmptySpot;
                  state[7, 2] = (Piece.King, false);
                  state[7, 3] = (Piece.Rook, false);
                  state[7, 4] = EmptySpot;
                  var shadowSpace = analyzer.fromBlackPOV ? (3, 0) : (4, 7);
                  analyzer.differences[i + 1] = analyzer.differences[i + 1].Where(x => x != shadowSpace).ToArray();
                  continue;
               }
               else if (!blackIsMoving && SameDiffrence(diff, whiteShortCastleDiff)) {
                  result.moves.Add("0-0");
                  state[7, 4] = EmptySpot;
                  state[7, 5] = (Piece.Rook, false);
                  state[7, 6] = (Piece.King, false);
                  state[7, 7] = EmptySpot;
                  var shadowSpace = analyzer.fromBlackPOV ? (3, 0) : (4, 7);
                  analyzer.differences[i + 1] = analyzer.differences[i + 1].Where(x => x != shadowSpace).ToArray();
                  continue;
               }
               else if (blackIsMoving && SameDiffrence(diff, blackLongCastleDiff)) {
                  result.moves.Add("0-0-0");
                  state[0, 0] = EmptySpot;
                  state[0, 2] = (Piece.King, true);
                  state[0, 3] = (Piece.Rook, true);
                  state[0, 4] = EmptySpot;
                  var shadowSpace = analyzer.fromBlackPOV ? (3, 7) : (4, 0);
                  analyzer.differences[i + 1] = analyzer.differences[i + 1].Where(x => x != shadowSpace).ToArray();
                  continue;
               }
               else if (blackIsMoving && SameDiffrence(diff, blackShortCastleDiff)) {
                  result.moves.Add("0-0");
                  state[0, 4] = EmptySpot;
                  state[0, 5] = (Piece.Rook, true);
                  state[0, 6] = (Piece.King, true);
                  state[0, 7] = EmptySpot;
                  var shadowSpace = analyzer.fromBlackPOV ? (3, 7) : (4, 0);
                  analyzer.differences[i + 1] = analyzer.differences[i + 1].Where(x => x != shadowSpace).ToArray();
                  continue;
               }
               else {
                  throw new Exception("Could not identify move " + (i + 1));
               }
            }
            var pieceInFirstSpot = state[diff[0].y, diff[0].x];
            var oldSpot = (pieceInFirstSpot.Item1 != Piece.None && pieceInFirstSpot.isBlack == blackIsMoving) ? diff[0] : diff[1];
            var newSpot = (pieceInFirstSpot.Item1 != Piece.None && pieceInFirstSpot.isBlack == blackIsMoving) ? diff[1] : diff[0];
            var pieceMoving = state[oldSpot.y, oldSpot.x];
            var pieceLandedon = state[newSpot.y, newSpot.x];

            //Build algebraic chess notation
            string algNotation = (pieceMoving.Item1 != Piece.Pawn) ? ((char)pieceMoving.Item1).ToString() : "";
            bool isEnPassant = pieceMoving.Item1 == Piece.Pawn && newSpot.x != oldSpot.x && pieceLandedon.Item1 == Piece.None;
            bool isTaking = pieceLandedon.Item1 != Piece.None || isEnPassant;
            var possiblePiecesToMove = state.FindIndexes((piece, index) => {
               return piece == pieceMoving && GetPeicePossibleMoves((index.Item2, index.Item1)).Contains(newSpot);
            });

            //En Passant
            if (isEnPassant)
               state[newSpot.y + (blackIsMoving ? -1 : 1), newSpot.x] = (Piece.None, false);

            //Ambiguous piece moves
            if (possiblePiecesToMove.Count != 1 || (pieceMoving.Item1 == Piece.Pawn && isTaking)) {
               if (possiblePiecesToMove.Count == 0)
                  throw new Exception($"Move {i + 1} is an illegal move.");
               if (pieceMoving.Item1 == Piece.Pawn) {
                  algNotation += (char)(oldSpot.x + 97);
               }
               else {
                  var FileIsAmbiguous = possiblePiecesToMove.GroupBy(x => x.Item1).Any(x => x.Count() > 1);
                  var RankIsAmbiguous = possiblePiecesToMove.GroupBy(x => x.Item2).Any(x => x.Count() > 1);
                  if (FileIsAmbiguous)
                     algNotation += (char)(oldSpot.x + 97);
                  if (RankIsAmbiguous)
                     algNotation += (8 - oldSpot.y).ToString();
               }
            }
            //Piece taking symbol (x)
            if (isTaking)
               algNotation += "x";
            //Convert to alg notaion (e4, d5, etc)
            algNotation += ((char)(newSpot.x + 97)).ToString() + (8 - newSpot.y).ToString();
            //TODO: Checks and mates

            if (isEnPassant)
               algNotation += "(ep)";

            result.moves.Add(algNotation);
            state[newSpot.y, newSpot.x] = pieceMoving;
            state[oldSpot.y, oldSpot.x] = EmptySpot;

            //Remove Shadow diff from next entry
            //The only valid diffs would be even so do not modify if the diff count is even
            if (analyzer.differences.Count > i + 1 && analyzer.differences[i + 1].Length % 2 == 1) {
               //Flip old spot back if it was flipped earlier
               (int x, int y) normOldSpot = analyzer.fromBlackPOV ? (7 - oldSpot.x, 7 - oldSpot.y) : oldSpot;
               analyzer.differences[i + 1] = analyzer.differences[i + 1].Where(x => x != normOldSpot).ToArray();
            }
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

      private List<(int x, int y)> GetPeicePossibleMoves((int x, int y) pos) {
         var piece = state[pos.y, pos.x];
         switch (piece.Item1) {
            case Piece.None:
               return [];
            case Piece.King:
               //This logic does not need more sofistication with checks, as this will only be used with ambiguous moves, which should never happen with kings
               return FilterInvalidMoves([
                  (pos.x - 1, pos.y),
                  (pos.x + 1, pos.y),
                  (pos.x, pos.y - 1),
                  (pos.x, pos.y + 1),
                  (pos.x - 1, pos.y - 1),
                  (pos.x + 1, pos.y - 1),
                  (pos.x - 1, pos.y + 1),
                  (pos.x + 1, pos.y + 1)
               ], piece.isBlack);
            case Piece.Pawn:
               var output = new List<(int x, int y)>();
               (int x, int y) aheadSpot = (pos.x, pos.y + (piece.isBlack ? 1 : -1));
               if (0 > aheadSpot.y || aheadSpot.y > 7)
                  return [];
               if (state[aheadSpot.y, aheadSpot.x].Item1 == Piece.None)
                  output.Add(aheadSpot);
               output.Add((aheadSpot.x + 1, aheadSpot.y));
               output.Add((aheadSpot.x - 1, aheadSpot.y));
               //If its the first time the pawn has moved, it can move 2
               if ((piece.isBlack && pos.y == 1) || (!piece.isBlack && pos.y == 6))
                  output.Add((aheadSpot.x, aheadSpot.y + (piece.isBlack ? 1 : -1)));
               return FilterInvalidMoves(output, piece.isBlack);
            case Piece.Knight:
               return FilterInvalidMoves([
                  (pos.x + 2, pos.y + 1),
                  (pos.x + 1, pos.y + 2),
                  (pos.x - 2, pos.y + 1),
                  (pos.x - 1, pos.y + 2),
                  (pos.x + 2, pos.y - 1),
                  (pos.x + 1, pos.y - 2),
                  (pos.x - 2, pos.y - 1),
                  (pos.x - 1, pos.y - 2)
                  ], piece.isBlack);
            case Piece.Rook:
               return GetValidSlidingMoves(pos, piece.isBlack, [(1, 0), (-1, 0), (0, 1), (0, -1)]);
            case Piece.Bishop:
               return GetValidSlidingMoves(pos, piece.isBlack, [(1, 1), (-1, 1), (1, -1), (-1, -1)]);
            case Piece.Queen:
               return GetValidSlidingMoves(pos, piece.isBlack, [(1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, 1), (1, -1), (-1, -1)]);
            default:
               return [];
         }
      }

      private List<(int x, int y)> FilterInvalidMoves(List<(int x, int y)> moves, bool isBlack) {
         return moves
            .Where(x => x.x >= 0 && x.y >= 0 && x.x < 8 && x.y < 8)
            //If empty spot or enemy spot
            .Where(x => this.state[x.y, x.x].Item1 == Piece.None || this.state[x.y, x.x].isBlack == !isBlack)
            .ToList();
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="pos"></param>
      /// <param name="isBlack"></param>
      /// <param name="directions">Directions that the peice can slide, ex (1,1) for up left, or (1,0) for right</param>
      /// <returns></returns>
      private List<(int x, int y)> GetValidSlidingMoves((int x, int y) pos, bool isBlack, (int x, int y)[] directions) {
         var output = new List<(int x, int y)>();
         foreach (var (dx, dy) in directions) {
            for (var i = 1; 0 <= pos.x + dx * i && pos.x + dx * i < 8 && 0 <= pos.y + dy * i && pos.y + dy * i < 8; i++) {
               (int x, int y) currentPos = (pos.x + dx * i, pos.y + dy * i);
               var piece = state[currentPos.y, currentPos.x];
               if (piece.Item1 != Piece.None) {
                  if (piece.isBlack == !isBlack)
                     output.Add(currentPos);
                  break;
               }
               output.Add(currentPos);
            }
         }
         return output;

      }
   }
}
