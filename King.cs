using SplashKitSDK;
namespace Chess
{
    class King : Piece
    { 
        public King(string side, Position position) : base(side, position)
        {
            
        }
        public override string Type => "King";
    }
}
