# Chess-API.com Python Client

A simple Python script to connect to the Chess-API.com service and evaluate chess positions. This script can be used to test the API or make quick evaluations outside of your C# application.

## Requirements

- Python 3.7+
- Required packages:
  - `websockets`
  - `colorama`
  - `uuid`

## Installation

1. Make sure you have Python 3.7 or newer installed
2. Install the required packages:

```bash
pip install websockets colorama uuid
```

3. Download the `chess_api_client.py` script

## Usage

### Basic Usage

```bash
python chess_api_client.py "r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 0 3"
```

This will analyze the given position at default depth (12) and return the best move.

### Interactive Mode

```bash
python chess_api_client.py --interactive
```

In interactive mode, you can enter multiple FEN positions one after another without restarting the script. Type 'exit' to quit.

### Command Line Options

- `--depth` - Set the search depth (1-18, default: 12)
- `--variants` - Number of variant lines to analyze (1-5, default: 3)
- `--thinking-time` - Max thinking time per move in milliseconds (1-100, default: 50)
- `--interactive` - Run in interactive mode

For example:

```bash
python chess_api_client.py --depth 16 --variants 5 --thinking-time 100 "r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 0 3"
```

### Getting FEN String from Your Chess Game

You can use the `GetFen()` method from your C# `Board` class to get the FEN string, then copy and paste it into this Python script.

In the C# application, you might add a button or command to output the current position's FEN string.

## Example Output

The script provides colored output showing:
- Intermediate evaluations as the search progresses
- The final best move with evaluation
- Suggested continuation (sequence of moves)

## Integration with Your Chess Application

This script is designed to be used as a standalone tool, but you can also use it to verify that the Chess-API.com service is working correctly before integrating it into your C# application.

To extract a FEN string from your C# application, you could add temporary code like:

```csharp
// Get the current FEN string
string fen = _board.GetFen();
Console.WriteLine($"Current position FEN: {fen}");
```

Then copy that FEN string to use with this Python script. 