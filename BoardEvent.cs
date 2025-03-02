using SplashKitSDK;

namespace Chess
{
    public static class BoardEvent
    {
        private static IPiece _selectedPiece; // Currently selected piece
        public static void SelectPiece()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                float mouseX = SplashKit.MouseX();
                float mouseY = SplashKit.MouseY();

                foreach (IPiece piece in Board.Pieces)
                {
                    int x = piece.Position.File * 80;
                    int y = piece.Position.Rank * 80;

                    // Check if the mouse click is within the piece's area
                    if (mouseX >= x && mouseX < x + 80 && mouseY >= y && mouseY < y + 80)
                    {
                        _selectedPiece = piece;
                        Console.WriteLine($"Selected: {_selectedPiece.Name}");
                        return;
                    }
                }
            }
        }


        public static void MovePiece()
        {
            if (_selectedPiece != null && SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                float mouseX = SplashKit.MouseX();
                float mouseY = SplashKit.MouseY();

                int newFile = (int)(mouseX / 80);
                int newRank = (int)(mouseY / 80);

                Console.WriteLine($"Moving {_selectedPiece.Name} to Rank: {newRank}, File: {newFile}");

                // Update the position
                _selectedPiece.Position = new Position(newFile, newRank);

                // Deselect the piece after moving
                _selectedPiece = null;
            }
        }

        public static void HandleEvents()
        {
            SelectPiece();
            MovePiece();
        }
    }
}
