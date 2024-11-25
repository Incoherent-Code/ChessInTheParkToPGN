namespace ChessInTheParkToPGN {
   internal class Program {
      static void Main(string[] args) {
         if (args.Length == 0 || !File.Exists(args[0])) {
            Console.WriteLine("Please start the analyzer with a valid file path!");
            Environment.Exit(1);
         }

      }
   }
}
