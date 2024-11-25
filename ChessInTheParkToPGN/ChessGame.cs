namespace ChessInTheParkToPGN {
   public class ChessGame {
      public Piece[][] state = [
         [Piece.Rook, Piece.Knight, Piece.Bishop, Piece.Queen, Piece.King, Piece.Bishop, Piece.Knight, Piece.Rook],
         [Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn],
         [Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None],
         [Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None],
         [Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None],
         [Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None, Piece.None],
         [Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn, Piece.Pawn],
         [Piece.Rook, Piece.Knight, Piece.Bishop, Piece.Queen, Piece.King, Piece.Bishop, Piece.Knight, Piece.Rook]
      ];

      public PGN result = new PGN();


   }
   /// <summary>
   /// Each number corresponds to the ascii character that each piece translates to in algebraic notation
   /// Except for the pawn, of course
   /// </summary>
   public enum Piece {
      None = 0,
      Pawn = 1,
      King = 107,
      Queen = 113,
      Rook = 114,
      Bishop = 98,
      Knight = 110
   }
}
