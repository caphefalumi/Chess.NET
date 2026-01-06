using SplashKitSDK;
using Chess.Pieces;
using Chess.Moves;
using Chess.Interfaces;
using Chess.UI.Drawing;

using Chess.UI.States;
using Chess.Networking;
namespace Chess.Core
{
    public enum Variant
    {
        TwoPlayer,
        Network,
        Computer,
        Online,
        Custom
    }
} 
