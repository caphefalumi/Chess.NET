using SplashKitSDK;

namespace Chess
{
    public static class BoardEvent
    {
        private static IPiece? _selectedPiece = null; // Currently selected piece
        private static HashSet<Position> _legalMoves = new HashSet<Position>(); // Store legal moves
        private static Board _board = Board.GetInstance();

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
                Position oldPosition = _selectedPiece.Position;

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

                IPiece capturedPiece = null;

                // Check for en passant capture
                if (_selectedPiece is Pawn && Math.Abs(oldPosition.File - newPosition.File) == 1 &&
                    Board.FindPieceAt(newPosition) == null)
                {
                    // This might be an en passant capture
                    int capturedPawnRank = oldPosition.Rank;
                    Position capturedPawnPos = new Position(newPosition.File, capturedPawnRank);
                    IPiece possiblePawn = Board.FindPieceAt(capturedPawnPos);

                    if (possiblePawn is Pawn pawn && pawn.CanBeEnPassantCaptured)
                    {
                        capturedPiece = possiblePawn;
                        Board.Pieces.Remove(capturedPiece);
                        Console.WriteLine($"En passant capture: {capturedPiece.Name}");
                    }
                }
                else
                {
                    // Check if a piece exists at the destination
                    capturedPiece = Board.FindPieceAt(newPosition);
                    if (capturedPiece != null)
                    {
                        if (capturedPiece.Color == _selectedPiece.Color)
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
                            Board.Pieces.Remove(capturedPiece);
                            Console.WriteLine($"Captured {capturedPiece.Name}");
                        }
                    }
                }

                // Record move before updating position
                Board.RecordMove(_selectedPiece, oldPosition, newPosition, capturedPiece);

                // Update the position if move is valid
                _selectedPiece.Position = newPosition;
                _selectedPiece.HasMoved = true;

                // Print FEN after move
                Console.WriteLine($"FEN: {_board.GetFEN()}");

                _selectedPiece = null;
                Board.Shapes.Clear();
                _legalMoves.Clear(); // Clear highlighted moves
            }
        }

        public static void DrawLegalMoves()
        {
            if (_selectedPiece is null) return;
            foreach (Position pos in _legalMoves)
            {
                int x = pos.File * 80 + 40; // Center of the square
                int y = pos.Rank * 80 + 40;

                Circle circle;
                IPiece pieceAtPos = Board.FindPieceAt(pos);

                // Check for en passant capture
                bool isEnPassant = false;
                if (_selectedPiece is Pawn &&
                    Math.Abs(_selectedPiece.Position.File - pos.File) == 1 &&
                    pieceAtPos == null)
                {
                    int capturedPawnRank = _selectedPiece.Position.Rank;
                    Position capturedPawnPos = new Position(pos.File, capturedPawnRank);
                    IPiece possiblePawn = Board.FindPieceAt(capturedPawnPos);

                    if (possiblePawn is Pawn pawn && pawn.CanBeEnPassantCaptured)
                    {
                        isEnPassant = true;
                    }
                }

                if (pieceAtPos != null || isEnPassant)
                {
                    // Draw big circle for capture moves
                    circle = new Circle(Color.Red, x, y, 15);
                }
                else
                {
                    // Draw small circle for normal moves
                    circle = new Circle(Color.Red, x, y, 7);
                }
                Board.Shapes.Add((IShape)circle);
            }
        }

        public static void FlashSquare(int file, int rank, Color flashColor, int duration)
        {
            int x = file * 80 + 35;
            int y = rank * 80 + 35;

            Rectangle flashRectangle = new Rectangle(Color.RGBAColor(flashColor.R, flashColor.G, flashColor.B, 150), x, y, 80, 80);
            Board.Shapes.Add(flashRectangle);
        }

        public static void HandleMouseEvents()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Board.Shapes.Clear();
                if (_selectedPiece is null)
                {
                    SelectPiece();
                    DrawLegalMoves();
                }
                else
                {
                    MovePiece();
                }
            }
        }
    }
}