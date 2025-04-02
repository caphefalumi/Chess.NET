#!/usr/bin/env python3
"""
Chess-API Client
---------------
A simple Python script to connect to chess-api.com and evaluate chess positions.
This can be used to test the API or evaluate positions outside of your C# application.
"""

import asyncio
import json
import sys
import time
import argparse
import websockets
import uuid
from colorama import init, Fore, Style

# Initialize colorama for colored terminal output
init()

API_URL = "wss://chess-api.com/v1"

class ChessAPIClient:
    """Client for accessing the Chess-API.com websocket service."""
    
    def __init__(self):
        self.connected = False
        self.websocket = None
        self.last_move = None
        self.best_move = None
        self.eval_history = []
        
    async def connect(self):
        """Connect to the Chess-API.com websocket."""
        try:
            self.websocket = await websockets.connect(API_URL)
            self.connected = True
            print(f"{Fore.GREEN}Connected to Chess-API.com{Style.RESET_ALL}")
            return True
        except Exception as e:
            print(f"{Fore.RED}Connection error: {e}{Style.RESET_ALL}")
            return False
    
    async def disconnect(self):
        """Disconnect from the websocket."""
        if self.connected and self.websocket:
            await self.websocket.close()
            self.connected = False
            print(f"{Fore.YELLOW}Disconnected from Chess-API.com{Style.RESET_ALL}")
    
    async def request_evaluation(self, fen, depth=12, variants=1, max_thinking_time=50):
        """Request evaluation of a FEN position."""
        if not self.connected:
            await self.connect()
            if not self.connected:
                return None
        
        # Generate a unique task ID
        task_id = str(uuid.uuid4())[:8]
        
        # Create request payload
        request = {
            "fen": fen,
            "taskId": task_id,
            "depth": min(18, max(1, depth)),
            "variants": min(5, max(1, variants)),
            "maxThinkingTime": min(100, max(1, max_thinking_time))
        }
        
        # Send request
        await self.websocket.send(json.dumps(request))
        print(f"{Fore.BLUE}Sent request for position: {Fore.WHITE}{fen}{Style.RESET_ALL}")
        
        # Listen for response
        self.eval_history = []
        self.best_move = None
        
        try:
            while True:
                response_text = await self.websocket.recv()
                response = json.loads(response_text)
                
                # Process the response
                self._process_response(response)
                
                # Exit when we have a final move
                if response.get("type") == "bestmove":
                    break
                    
            return self.best_move
            
        except Exception as e:
            print(f"{Fore.RED}Error receiving response: {e}{Style.RESET_ALL}")
            return None
            
    def _process_response(self, response):
        """Process a response from the API and update the state."""
        if response.get("type") == "move":
            # Intermediate move found during search
            self.last_move = response
            self.eval_history.append(response)
            
            # Print move info
            eval_str = f"{response.get('eval', 0):.2f}"
            if response.get('mate'):
                eval_str = f"M{response.get('mate')}"
                
            print(f"{Fore.CYAN}[Depth {response.get('depth', '?')}] "
                  f"Move: {response.get('from', '?')} → {response.get('to', '?')} "
                  f"({response.get('san', '?')}) | "
                  f"Eval: {eval_str}{Style.RESET_ALL}")
                  
        elif response.get("type") == "bestmove":
            # Final best move
            self.best_move = response
            
            # Print best move info with green color
            eval_str = f"{response.get('eval', 0):.2f}"
            if response.get('mate'):
                eval_str = f"M{response.get('mate')}"
                
            print(f"{Fore.GREEN}[BEST] "
                  f"Move: {response.get('from', '?')} → {response.get('to', '?')} "
                  f"({response.get('san', '?')}) | "
                  f"Eval: {eval_str}{Style.RESET_ALL}")
            
            # Print suggested continuation if available
            if response.get('continuationArr'):
                continuation = " → ".join(response.get('continuationArr', []))
                print(f"{Fore.GREEN}Continuation: {continuation}{Style.RESET_ALL}")
                
        elif response.get("type") == "info":
            # Info message
            print(f"{Fore.YELLOW}[Info] {response.get('text', '')}{Style.RESET_ALL}")
            
        else:
            # Unknown message type
            print(f"{Fore.YELLOW}Unknown message type: {response}{Style.RESET_ALL}")


async def main():
    """Main function to parse arguments and run the client."""
    parser = argparse.ArgumentParser(description="Chess-API.com client for position evaluation")
    parser.add_argument("fen", nargs="?", help="FEN position to evaluate")
    parser.add_argument("--depth", type=int, default=12, help="Search depth (max 18)")
    parser.add_argument("--variants", type=int, default=3, help="Number of variants to analyze (max 5)")
    parser.add_argument("--thinking-time", type=int, default=50, 
                        help="Maximum thinking time in milliseconds (max 100)")
    parser.add_argument("--interactive", action="store_true", 
                        help="Run in interactive mode, allowing multiple positions")
    
    args = parser.parse_args()
    
    client = ChessAPIClient()
    
    try:
        # Connect to the API
        if not await client.connect():
            return 1
        
        if args.interactive:
            # Interactive mode
            print(f"{Fore.CYAN}Chess-API.com Interactive Client{Style.RESET_ALL}")
            print(f"{Fore.CYAN}Type a FEN position to evaluate, or 'exit' to quit{Style.RESET_ALL}")
            
            while True:
                fen = input(f"{Fore.GREEN}Enter FEN> {Style.RESET_ALL}")
                if fen.lower() in ("exit", "quit", "q"):
                    break
                    
                if not fen.strip():
                    # If empty, use starting position
                    fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
                    print(f"{Fore.YELLOW}Using starting position{Style.RESET_ALL}")
                
                start_time = time.time()
                await client.request_evaluation(
                    fen, 
                    depth=args.depth,
                    variants=args.variants,
                    max_thinking_time=args.thinking_time
                )
                elapsed = time.time() - start_time
                print(f"{Fore.YELLOW}Total time: {elapsed:.2f} seconds{Style.RESET_ALL}")
                print()
                
        elif args.fen:
            # Single evaluation mode
            await client.request_evaluation(
                args.fen, 
                depth=args.depth,
                variants=args.variants,
                max_thinking_time=args.thinking_time
            )
        else:
            # No FEN provided, evaluate starting position
            starting_fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            print(f"{Fore.YELLOW}No FEN provided, using starting position{Style.RESET_ALL}")
            await client.request_evaluation(
                starting_fen, 
                depth=args.depth,
                variants=args.variants,
                max_thinking_time=args.thinking_time
            )
            
    except KeyboardInterrupt:
        print(f"{Fore.YELLOW}Interrupted by user{Style.RESET_ALL}")
    finally:
        await client.disconnect()
    
    return 0


if __name__ == "__main__":
    # Run the main function
    exit_code = asyncio.run(main())
    sys.exit(exit_code) 