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
}
