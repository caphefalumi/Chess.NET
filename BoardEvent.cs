using SplashKitSDK;

namespace Chess
{
    public static class BoardEvent
    {
        private static IPiece _selectedPiece = null; // Currently selected piece
        private static Position _originalPosition; // Stores the original position before moving
        private static HashSet<Position> _legalMoves = new HashSet<Position>(); // Store legal moves

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
                        _originalPosition = new Position(piece.Position.File, piece.Position.Rank); // Store original position
                        _legalMoves = _selectedPiece.GetLegalMoves(); // Get the legal moves
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
                Position newPosition = new Position(newFile, newRank);

                Console.WriteLine($"Attempting to move {_selectedPiece.Name} to Rank: {newRank}, File: {newFile}");

                // Check if the move is in the legal moves list
                if (!_legalMoves.Contains(newPosition))
                {
                    Console.WriteLine("Invalid move: Not a legal move!");
                    _selectedPiece = null; // Deselect after moving

                    // Flash red square if invalid move
                    FlashSquare(newFile, newRank, Color.Red, 300);
                    return;
                }

                // Check if a piece exists at the destination
                IPiece targetPiece = Board.FindPieceAt(newPosition);
                if (targetPiece != null)
                {
                    if (targetPiece.Color == _selectedPiece.Color)
                    {
                        Console.WriteLine("Invalid move: Destination occupied by a friendly piece!");

                        // Flash red light on the occupied square
                        FlashSquare(newFile, newRank, Color.Red, 300);
                        _selectedPiece = null; // Deselect after moving

                        return;
                    }
                    else
                    {
                        // Capture logic: Remove the opponent's piece
                        Board.Pieces.Remove(targetPiece);
                        Console.WriteLine($"Captured {targetPiece.Name}");
                    }
                }

                // Update the position if move is valid
                _selectedPiece.Position = newPosition;
                _selectedPiece = null; // Deselect after moving
                _legalMoves.Clear(); // Clear highlighted moves
            }
        }

        public static void DrawLegalMoves()
        {
            if (_selectedPiece == null) return;
            BoardDrawer.DrawBoard();

            foreach (Position pos in _legalMoves)
            {
                int x = pos.File * 80 + 40; // Center of the square
                int y = pos.Rank * 80 + 40;

                IPiece pieceAtPos = Board.FindPieceAt(pos);
                if (pieceAtPos != null)
                {
                    // Draw big circle for capture moves
                    SplashKit.FillCircle(Color.Red, x, y, 15);
                }
                else
                {
                    // Draw small circle for normal moves
                    SplashKit.FillCircle(Color.Blue, x, y, 7);
                }
            }
            foreach (IPiece piece in Board.Pieces)
            {
                piece.Draw();
            }
            SplashKit.RefreshScreen();

        }

        public static void FlashSquare(int file, int rank, Color flashColor, int duration)
        {
            int x = file * 80;
            int y = rank * 80;

            BoardDrawer.DrawBoard();
            SplashKit.FillRectangle(Color.RGBAColor(flashColor.R, flashColor.G, flashColor.B, 150), x, y, 80, 80);
            foreach (IPiece piece in Board.Pieces)
            {
                piece.Draw();
            }

            SplashKit.RefreshScreen();
            SplashKit.Delay(duration);
        }

        public static void HandleEvents()
        {
            if (_selectedPiece is null)
            {
                SelectPiece();

            }
            else
            {
                DrawLegalMoves();
                MovePiece();
            }
        }
    }
}
