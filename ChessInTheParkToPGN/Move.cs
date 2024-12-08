namespace ChessInTheParkToPGN {
   public struct Turn {
      public Move whiteMove;
      public Move blackMove;
   }
   public struct Move {
      bool longCastle = false;
      bool shortCastle = false;
      Piece Piece;
      char file;
      int rank;
      public Move(Piece piece, char file, int rank) {
         this.Piece = piece;
         this.file = file;
         this.rank = rank;
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
