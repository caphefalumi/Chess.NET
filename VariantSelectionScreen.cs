﻿using SplashKitSDK;

namespace Chess
{
    public class VariantSelectionScreen : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private readonly Button _twoPlayerButton;
        private readonly Button _computerButton;
        private readonly Button _onlineButton;
        private readonly Button _customModeButton;
        private readonly Button _backButton;

        public VariantSelectionScreen(Game game, Board board)
        {
            _game = game;
            _board = board;

            int centerX = SplashKit.ScreenWidth() / 2;

            _twoPlayerButton = new Button("Two Player", centerX - 100, 200, 200, 50);
            _computerButton = new Button("Play Computer", centerX - 100, 270, 200, 50);
            _onlineButton = new Button("Play Online", centerX - 100, 340, 200, 50);
            _customModeButton = new Button("Custom Setup", centerX - 100, 410, 200, 50);
            _backButton = new Button("Back", centerX - 100, 480, 200, 50);
        }

        public override void HandleInput()
        {
            if (_twoPlayerButton.IsClicked())
            {
                _game.ChangeState(new TimeSelectionScreen(_game, _board, Variant.TwoPlayer));
            }
            else if (_computerButton.IsClicked())
            {
                _game.ChangeState(new TimeSelectionScreen(_game, _board, Variant.Computer));
            }
            else if (_onlineButton.IsClicked())
            {
                _game.ChangeState(new TimeSelectionScreen(_game, _board, Variant.Online));
            }
            else if (_customModeButton.IsClicked())
            {
                _game.ChangeState(new BoardSetupScreen(_game, _board));
            }
            else if (_backButton.IsClicked())
            {
                _game.ChangeState(new MainMenuScreen(_game, _board));
            }
        }

        public override void Update()
        {
            _twoPlayerButton.Update();
            _computerButton.Update();
            _onlineButton.Update();
            _customModeButton.Update();
            _backButton.Update();
        }

        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);

            SplashKit.DrawText("Select Game Mode", Color.Black, Font.Arial, 28,
                SplashKit.ScreenWidth() / 2 - 120, 100);

            _twoPlayerButton.Draw();
            _computerButton.Draw();
            _onlineButton.Draw();
            _customModeButton.Draw();
            _backButton.Draw();

            SplashKit.RefreshScreen();
        }
    }
}