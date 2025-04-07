using SplashKitSDK;
using System.Collections.Generic;

namespace Chess
{
    public class TimeSelectionScreen : ScreenState
    {
        private readonly Game _game;
        private readonly Board _board;
        private readonly MatchConfiguration _config;
        private readonly Variant _selectedMode;
        
        private List<Button> _timeButtons = new List<Button>();
        private List<Button> _incrementButtons = new List<Button>();
        private Button _startGameButton;
        private Button _backButton;
        
        private readonly Dictionary<TimeControl, string> _timeLabels = new Dictionary<TimeControl, string>
        {
            { TimeControl.Bullet1, "1 min" },
            { TimeControl.Bullet3, "3 min" },
            { TimeControl.Blitz5, "5 min" },
            { TimeControl.TenMinutes, "10 min" },
            { TimeControl.FifteenMinutes, "15 min" },
            { TimeControl.ThirtyMinutes, "30 min" },
            { TimeControl.Unlimited, "∞" }
        };
        
        private readonly int[] _incrementValues = { 0, 1, 2, 5, 10 };

        public TimeSelectionScreen(Game game, Board board, Variant selectedMode)
        {
            _game = game;
            _board = board;
            _selectedMode = selectedMode;
            _config = new MatchConfiguration { Mode = selectedMode };
            
            int centerX = SplashKit.ScreenWidth() / 2;
            int y = 170;
            
            // Create time control buttons
            int index = 0;
            foreach (TimeControl time in Enum.GetValues(typeof(TimeControl)))
            {
                int x = 150 + (index % 4) * 110;
                int rowY = y + (index / 4) * 70;
                
                Button timeButton = new Button(_timeLabels[time], x, rowY, 100, 60);
                _timeButtons.Add(timeButton);
                index++;
            }
            
            // Create increment buttons
            y = 350;
            for (int i = 0; i < _incrementValues.Length; i++)
            {
                int x = 150 + (i % 6) * 85;
                Button incrementButton = new Button($"+{_incrementValues[i]}", x, y, 75, 40);
                _incrementButtons.Add(incrementButton);
            }
            
            _startGameButton = new Button("Start Game", centerX - 100, 500, 200, 50);
            _backButton = new Button("Back", 20, 20, 80, 30);
        }
        
        public override void HandleInput()
        {
            if (_backButton.IsClicked())
            {
                _game.ChangeState(new LocalGameMenuState(_game, _board));
                return;
            }
            
            if (_startGameButton.IsClicked())
            {
                _game.ChangeState(new GameplayScreen(_game, _board, _config));
                return;
            }
            
            // Handle time selection buttons
            for (int i = 0; i < _timeButtons.Count; i++)
            {
                if (_timeButtons[i].IsClicked())
                {
                    _config.TimeControl = (TimeControl)i;
                    break;
                }
            }
            
            // Handle increment selection buttons
            for (int i = 0; i < _incrementButtons.Count; i++)
            {
                if (_incrementButtons[i].IsClicked())
                {
                    _config.UseIncrement = _incrementValues[i] > 0;
                    _config.IncrementSeconds = _incrementValues[i];
                    break;
                }
            }
        }
        
        public override void Update()
        {
            _backButton.Update();
            _startGameButton.Update();
            
            foreach (Button button in _timeButtons)
            {
                button.Update();
            }
                
            foreach (Button button in _incrementButtons)
            {
                button.Update();
            }
        }
        
        public override void Render()
        {
            SplashKit.ClearScreen(Color.White);
            
            // Draw header
            SplashKit.DrawText("Select Time Control", Color.Black, Font.Arial, 28, 
                SplashKit.ScreenWidth() / 2 - 140, 80);
            
            // Draw time options section
            SplashKit.DrawLine(Color.LightGray, 100, 140, 600, 140);
            SplashKit.DrawText("Time per player:", Color.Black, Font.Arial, 18, 100, 150);
            
            // Draw all time buttons
            for (int i = 0; i < _timeButtons.Count; i++)
            {
                _timeButtons[i].Draw();
                if ((TimeControl)i == _config.TimeControl)
                {
                    DrawButtonHighlight(_timeButtons[i]);
                }
            }
            
            // Draw increment section
            SplashKit.DrawLine(Color.LightGray, 100, 320, 600, 320);
            SplashKit.DrawText("Increment per move:", Color.Black, Font.Arial, 18, 100, 330);
            
            // Draw all increment buttons
            for (int i = 0; i < _incrementButtons.Count; i++)
            {
                _incrementButtons[i].Draw();
                if (_config.UseIncrement && _config.IncrementSeconds == _incrementValues[i])
                {
                    DrawButtonHighlight(_incrementButtons[i]);
                }
            }
            
            // Draw bottom buttons
            _startGameButton.Draw();
            _backButton.Draw();
            
            SplashKit.RefreshScreen();
        }
        
        private void DrawButtonHighlight(Button button)
        {
            SplashKit.FillRectangle(Color.RGBAColor(50, 120, 200, 100), 
                button.X - 4, button.Y - 4, 
                button.Width + 8, button.Height + 8);
        }
        
        public override string GetStateName() => "TimeSelection";
    }
}
