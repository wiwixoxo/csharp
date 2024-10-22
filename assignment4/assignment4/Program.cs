using System;

namespace CheckersGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }

    class Game
    {
        private Board board;
        private Player player1;
        private Player player2;
        private Player currentPlayer;

        public Game()
        {
            board = new Board();
            player1 = new Player('X'); // Player 1 uses 'X'
            player2 = new Player('O'); // Player 2 uses 'O'
            currentPlayer = player1;
        }

        public void Start()
        {
            bool gameOn = true;

            while (gameOn)
            {
                Console.Clear();
                board.DisplayBoard();
                Console.WriteLine($"{currentPlayer.Symbol}'s turn. Enter your move (e.g., B2 C3): ");
                string move = Console.ReadLine();

                if (ProcessMove(move))
                {
                    if (board.CheckWinCondition())
                    {
                        Console.Clear();
                        board.DisplayBoard();
                        Console.WriteLine($"{currentPlayer.Symbol} wins!");
                        gameOn = false;
                    }
                    else
                    {
                        SwitchTurn();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid move! Press Enter to try again.");
                    Console.ReadLine();
                }
            }
        }

        private bool ProcessMove(string move)
        {
            // Parse input: e.g., "B2 C3"
            string[] parts = move.Split(' ');
            if (parts.Length != 2)
                return false;

            // Convert positions to board indexes
            (int startX, int startY) = ConvertPosition(parts[0]);
            (int endX, int endY) = ConvertPosition(parts[1]);

            if (!IsValidCoordinate(startX, startY) || !IsValidCoordinate(endX, endY))
                return false;

            return board.MovePiece(startX, startY, endX, endY, currentPlayer.Symbol);
        }

        private void SwitchTurn()
        {
            currentPlayer = currentPlayer == player1 ? player2 : player1;
        }

        private (int, int) ConvertPosition(string position)
        {
            char col = position[0];
            int row = int.Parse(position[1].ToString());

            int x = row - 1;
            int y = col - 'A';
            return (x, y);
        }

        private bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
    }

    class Player
    {
        public char Symbol { get; private set; }

        public Player(char symbol)
        {
            Symbol = symbol;
        }
    }

    class Board
    {
        private char[,] grid;

        public Board()
        {
            grid = new char[8, 8];
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            // Initialize empty spaces
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    grid[i, j] = ' ';

            // Initialize player pieces
            for (int i = 0; i < 3; i++) // Player 1 ('X') pieces
                for (int j = 0; j < 8; j++)
                    if ((i + j) % 2 == 1) grid[i, j] = 'X';

            for (int i = 5; i < 8; i++) // Player 2 ('O') pieces
                for (int j = 0; j < 8; j++)
                    if ((i + j) % 2 == 1) grid[i, j] = 'O';
        }

        public void DisplayBoard()
        {
            Console.WriteLine("   A B C D E F G H");
            for (int i = 0; i < 8; i++)
            {
                Console.Write($"{i + 1} ");
                for (int j = 0; j < 8; j++)
                {
                    char cell = grid[i, j];
                    if ((i + j) % 2 == 1)
                        Console.Write(cell == ' ' ? "□ " : $"{cell} ");
                    else
                        Console.Write("░ ");
                }
                Console.WriteLine();
            }
        }

        public bool MovePiece(int startX, int startY, int endX, int endY, char playerSymbol)
        {
            // Check if the move is valid
            if (grid[startX, startY] != playerSymbol || grid[endX, endY] != ' ')
                return false;

            // Basic move: diagonal move for 1 step
            if (Math.Abs(startX - endX) == 1 && Math.Abs(startY - endY) == 1)
            {
                grid[startX, startY] = ' ';
                grid[endX, endY] = playerSymbol;
                return true;
            }

            // Capture move (jump over opponent)
            if (Math.Abs(startX - endX) == 2 && Math.Abs(startY - endY) == 2)
            {
                int middleX = (startX + endX) / 2;
                int middleY = (startY + endY) / 2;

                if (grid[middleX, middleY] != playerSymbol && grid[middleX, middleY] != ' ')
                {
                    grid[startX, startY] = ' ';
                    grid[middleX, middleY] = ' '; // Remove opponent piece
                    grid[endX, endY] = playerSymbol;
                    return true;
                }
            }

            return false;
        }

        public bool CheckWinCondition()
        {
            // Check if one player has no pieces left
            bool player1HasPieces = false, player2HasPieces = false;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (grid[i, j] == 'X') player1HasPieces = true;
                    if (grid[i, j] == 'O') player2HasPieces = true;
                }
            }

            return !(player1HasPieces && player2HasPieces);
        }
    }
}