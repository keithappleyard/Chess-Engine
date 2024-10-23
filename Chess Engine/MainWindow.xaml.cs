using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chess_Engine.Pieces;
using Chess_Engine.AI;

namespace Chess_Engine
{
    public class Board
    {
        //Two dimensional Array to represent chess board
        Spot[,] board = new Spot[8, 8];
        Grid Squares;
        public List<Move> Moves = new List<Move>();
        public bool UserTurn;
        public bool UserWhite;
        public int TurnCount;
        private StackPanel CheckMateScreen;
        private StackPanel DrawScreen;
        private TextBlock MoveLog;
        string SpecialMoveText;

        public Spot[,] GetBoard { get { return board; } }

        public Grid GetSquares { get { return Squares; } }

        public Board(Grid squares, StackPanel checkMateScreen, StackPanel drawScreen, TextBlock moveLog)
        {
            Squares = squares;
            CheckMateScreen = checkMateScreen;
            DrawScreen = drawScreen;
            MoveLog = moveLog;

            //Setting the X and Y value coordinates for every square
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    board[x, y] = new Spot(x, y);
                }
            }
        }

        //Setting the starting positions of every piece
        public void SetStartingPositions(bool UserIsWhite)
        {
            UserWhite = UserIsWhite;
            UserTurn = UserIsWhite;

            for (int i = 0; i < 8; i++)
            {
                board[i, 1].piece = new Pawn(UserIsWhite, true, i, 1);
                board[i, 6].piece = new Pawn(!UserIsWhite, false, i, 6);
            }

            board[0, 0].piece = new Rook(UserIsWhite, true, 0, 0);
            board[7, 0].piece = new Rook(UserIsWhite, true, 7, 0);
            board[0, 7].piece = new Rook(!UserIsWhite, false, 0, 7);
            board[7, 7].piece = new Rook(!UserIsWhite, false, 7, 7);

            board[1, 0].piece = new Knight(UserIsWhite, true, 1, 0);
            board[6, 0].piece = new Knight(UserIsWhite, true, 6, 0);
            board[1, 7].piece = new Knight(!UserIsWhite, false, 1, 7);
            board[6, 7].piece = new Knight(!UserIsWhite, false, 6, 7);

            board[2, 0].piece = new Bishop(UserIsWhite, true, 2, 0);
            board[5, 0].piece = new Bishop(UserIsWhite, true, 5, 0);
            board[2, 7].piece = new Bishop(!UserIsWhite, false, 2, 7);
            board[5, 7].piece = new Bishop(!UserIsWhite, false, 5, 7);

            board[3, 0].piece = new Queen(UserIsWhite, true, 3, 0);
            board[3, 7].piece = new Queen(!UserIsWhite, false, 3, 7);

            board[4, 0].piece = new King(UserIsWhite, true, 4, 0);
            board[4, 7].piece = new King(!UserIsWhite, false, 4, 7);

            UpdateGUI(Squares, board);
        }

        //Function to play a move
        public void PlayMove(Move move)
        {
            if (DoMove(move)) //If the move is valid
            {
                SpecialMoveText = "";
                Piece piece = board[move.ToX, move.ToY].piece;
                //If move is a castle move
                if (move.IsCastleMove)
                {
                    if(move.ToX > move.FromX)
                    {
                        SpecialMoveText = $"[Castle] {move.CapturedPiece.Name}: ({move.ToX + 1},{move.CapturedPiece.y}) => ({move.CapturedPiece.x},{move.CapturedPiece.y}), ";
                        Console.Write(SpecialMoveText);
                    }
                    else
                    {
                        SpecialMoveText = $"[Castle] {move.CapturedPiece.Name}: ({move.ToX - 2},{move.CapturedPiece.y}) => ({move.CapturedPiece.x},{move.CapturedPiece.y}), ";
                        Console.Write(SpecialMoveText);
                    }
                }
                //If move is a promotion
                if (move.IsPromotion)
                {
                    SpecialMoveText = "[Promotion] ";
                    Console.Write(SpecialMoveText);
                }
                //If move log is empty
                if (MoveLog.Text == ""){
                    MoveLog.Text = $"{SpecialMoveText}{piece.Name}: ({move.FromX},{move.FromY}) => ({move.ToX},{move.ToY});";
                }
                else
                {
                    MoveLog.Text = $"{MoveLog.Text}\n{SpecialMoveText}{piece.Name}: ({move.FromX},{move.FromY}) => ({move.ToX},{move.ToY});";
                }
                Console.WriteLine($"{SpecialMoveText}{piece.Name}: ({move.FromX},{move.FromY}) => ({move.ToX},{move.ToY});"); //Log move
                TurnCount++;
                UpdateGUI(Squares, board);

                if (IsCheckMate())
                {
                    Console.WriteLine("CheckMate!");
                    CheckMateScreen.IsEnabled = true;
                    CheckMateScreen.Visibility = Visibility.Visible;
                }

                if (IsStaleMate())
                {
                    Console.WriteLine("You drew!");
                    DrawScreen.IsEnabled = true;
                    DrawScreen.Visibility = Visibility.Visible;
                }

                return;
            }

            Console.WriteLine("Error: move could not be played");
        }

        //Function to return king
        public Piece GetKing(bool isWhite)
        {
            int KingX = -1;
            int KingY = -1;

            //find position of king
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    //if the current square is the opposite colour king
                    if (board[x, y].piece != null && board[x, y].piece.Name == "King" && board[x, y].piece.IsWhite == isWhite)
                    {
                        KingX = x;
                        KingY = y;
                        break;
                    }
                }

                if (KingX != -1)
                    break;
            }

            if (KingX == -1) //if not king was found
            {
                Console.WriteLine("Error: no king was found");
                return null;
            }

            return board[KingX, KingY].piece;

        }

        //Function to check the king is in check
        public bool InCheck(bool isWhite)
        {
            Piece king = GetKing(isWhite);

            if(king == null)
            {
                Console.WriteLine("Error: king was not found!");
                return false;
            }

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board[x, y].piece != null && board[x, y].piece.IsWhite != isWhite)
                    {
                        //if the current piece is able to move to the king square
                        if (board[x, y].piece.CanMove(board, king.x, king.y)) //returning because the coordinates at the king coordinates are empty
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //Updates each square according to what piece is inside
        public void UpdateGUI(Grid squares, Spot[,] spots)
        {
            Button button;

            foreach (UIElement element in squares.Children)
            {
                Grid square = (Grid)element;

                int x = Grid.GetColumn((Grid)element); //x value corresponds to the get column function of a grid element
                int y = 7 - Grid.GetRow((Grid)element); //y value corresponds to 7 minus the get row function of a grid element

                foreach (UIElement element2 in square.Children)
                {
                    button = element2 as Button;

                    UpdateImage(button, spots[x, y].piece);
                }
            }
        }

        //Updates the image on a square according to what piece is occupying it
        public void UpdateImage(Button button, Piece piece)
        {
            string path;

            if (piece == null) //emptys squares with no pieces on
            {
                ((Image)button.Content).Source = null;
                return;
            }

            switch (piece.Name)
            { 
                //returns path to corresponding image of piece
                case "Pawn":
                    if (piece.IsWhite)
                    {
                        path = "pack://application:,,,/Resources/WhitePawn.png";
                    }
                    else
                    {
                        path = "pack://application:,,,/Resources/BlackPawn.png";
                    }
                    break;
                case "Bishop":
                    if (piece.IsWhite)
                    {
                        path = "pack://application:,,,/Resources/WhiteBishop.png";
                    }
                    else
                    {
                        path = "pack://application:,,,/Resources/BlackBishop.png";
                    }
                    break;
                case "Rook":
                    if (piece.IsWhite)
                    {
                        path = "pack://application:,,,/Resources/WhiteRook.png";
                    }
                    else
                    {
                        path = "pack://application:,,,/Resources/BlackRook.png";
                    }
                    break;
                case "Knight":
                    if (piece.IsWhite)
                    {
                        path = "pack://application:,,,/Resources/WhiteKnight.png";
                    }
                    else
                    {
                        path = "pack://application:,,,/Resources/BlackKnight.png";
                    }
                    break;
                case "Queen":
                    if (piece.IsWhite)
                    {
                        path = "pack://application:,,,/Resources/WhiteQueen.png";
                    }
                    else
                    {
                        path = "pack://application:,,,/Resources/BlackQueen.png";
                    }
                    break;
                case "King":
                    if (piece.IsWhite)
                    {
                        path = "pack://application:,,,/Resources/WhiteKing.png";
                    }
                    else
                    {
                        path = "pack://application:,,,/Resources/BlackKing.png";
                    }
                    break;
                default:
                    return;
            }
            var SourceUri = new Uri(path, UriKind.Absolute); //sets the image inside squares to the image its supposed to be
            Image image = (Image)button.Content;
            image.Source = new BitmapImage(SourceUri);
        }
        
        //Attempt to virtually move piece and return false if move is invalid
        public bool DoMove(Move move)
        {
            if (move == null)
            {
                Console.WriteLine("Error: move was null.");
                return false;
            }

            if (board[move.FromX, move.FromY].piece == null)
            {
                Console.WriteLine("Error: piece inside 'From' coordinates was null");

                return false;
            }

            Piece piece = board[move.FromX, move.FromY].piece;

            //if user is not playing correct pieces
            if((!piece.IsUser && UserTurn) || (piece.IsUser && !UserTurn))
            {
                return false;
            }

            //if move was outside of board
            if(move.ToX < 0 || move.ToX > 7 || move.ToY < 0 || move.ToY > 7)
            {
                Console.WriteLine("Error: move was outside of chess board.");
                return false;
            }

            //if move is valid
            if (piece.CanMove(board, move.ToX, move.ToY))
            {
                //if move is castle move
                if (piece.Name == "King" && piece.Moves == 0 && (board[0, move.FromY].piece != null || board[7, move.FromY].piece != null) && (move.ToX == 2 || move.ToX == 6) && (move.ToY == 0 || move.ToY == 7))
                {
                    if (InCheck(piece.IsWhite))
                    {
                        return false;
                    }
                    switch (move.ToX)
                    {
                        case 2:
                            if(board[0, move.ToY].piece != null && board[0, move.ToY].piece.Name == "Rook" && board[0, move.ToY].piece.Moves == 0)
                            {
                                move.CapturedPiece = board[0, move.ToY].piece;

                                board[3, move.ToY].piece = board[0, move.ToY].piece;
                                board[0, move.ToY].piece = null;
                                board[3, move.ToY].piece.x = 3;
                                board[3, move.ToY].piece.y = move.ToY;

                                move.IsCastleMove = true;
                                King king = piece as King;
                                king.HasCastled = true;
                                break;
                            }
                            break;
                        case 6:
                            if (board[7, move.ToY].piece != null && board[7, move.ToY].piece.Name == "Rook" && board[7, move.ToY].piece.Moves == 0)
                            {
                                move.CapturedPiece = board[7, move.ToY].piece;
                                board[5, move.ToY].piece = board[7, move.ToY].piece;
                                board[7, move.ToY].piece = null;
                                board[5, move.ToY].piece.x = 5;
                                board[5, move.ToY].piece.y = move.ToY;

                                move.IsCastleMove = true;
                                King king = piece as King;
                                king.HasCastled = true;
                                break;
                            }
                            break;
                        default:
                            Console.WriteLine("Error: castle move could not be played");
                            return false;
                    }
                }

                //if move is en passant
                if (board[move.ToX, move.ToY].piece == null && piece.Name == "Pawn" && Math.Abs(piece.x - move.ToX) == 1 && (move.ToY == 2 || move.ToY == 5) && (board[move.ToX, move.ToY - 1].piece != null || board[move.ToX, move.ToY + 1].piece != null))
                {
                    switch (move.ToY)
                    {
                        case 2:
                            if (board[move.ToX, move.ToY + 1].piece != null)
                            {
                                if (Moves.Last().ToX != move.ToX || Moves.Last().ToY != move.ToY + 1)
                                {
                                    return false;
                                }

                                if (board[move.ToX, move.ToY + 1].piece.Moves == 1)
                                {
                                    move.CapturedPiece = board[move.ToX, move.ToY + 1].piece;
                                    board[move.ToX, move.ToY + 1].piece = null;
                                    break;
                                }

                                return false;
                            }
                            break;
                        case 5:
                            if (board[move.ToX, move.ToY - 1].piece != null)
                            {
                                if (Moves.Last().ToX != move.ToX || Moves.Last().ToY != move.ToY - 1)
                                {
                                    return false;
                                }

                                if (board[move.ToX, move.ToY - 1].piece.Moves == 1)
                                {
                                    move.CapturedPiece = board[move.ToX, move.ToY - 1].piece;
                                    board[move.ToX, move.ToY - 1].piece = null;
                                    break;
                                }

                                return false;
                            }
                            break;
                        default:
                            Console.WriteLine("Error: en passant could not be played");
                            break;
                    }
                }

                UserTurn = !UserTurn;

                //if move involved taking pieces
                if (board[move.ToX, move.ToY].piece != null)
                {
                    move.CapturedPiece = board[move.ToX, move.ToY].piece;
                }

                //swap positions on board
                board[move.ToX, move.ToY].piece = board[move.FromX, move.FromY].piece;
                board[move.FromX, move.FromY].piece = null;

                //edit piece data
                board[move.ToX, move.ToY].piece.x = move.ToX;
                board[move.ToX, move.ToY].piece.y = move.ToY;
                board[move.ToX, move.ToY].piece.Moves++;

                //promotion - might change later
                if (board[move.ToX, move.ToY].piece.Name == "Pawn" && (board[move.ToX, move.ToY].y == 0 || board[move.ToX, move.ToY].y == 7))
                {
                    Piece _piece = board[move.ToX, move.ToY].piece;
                    board[move.ToX, move.ToY].piece = new Queen(_piece.IsWhite, _piece.IsUser, _piece.x, _piece.y, _piece.Moves);
                    move.IsPromotion = true;

                    //edit piece data
                }

                //If the move places the king in check
                if(InCheck(board[move.ToX, move.ToY].piece.IsWhite))
                {
                    UndoMove(move);
                    return false;
                }

                Moves.Add(move);
                return true;
            }
            //if move is not valid
            return false;
        }

        //Undo the most recent move
        public void UndoMove(Move move)
        {
            if(board[move.ToX, move.ToY].piece == null)
            {
                Console.WriteLine("Error: 'To' coordinates did not contain a piece.");
                return;
            }

            UserTurn = !UserTurn;

            //if move was a castle move
            if (move.IsCastleMove)
            {
                Piece piece = board[move.ToX, move.ToY].piece;
                King king = piece as King;
                king.HasCastled = false;

                if (move.FromX < move.ToX)
                {
                    board[7, move.ToY].piece = move.CapturedPiece;
                    move.CapturedPiece.x = 7;
                    board[5, move.ToY].piece = null;
                }
                else
                {
                    board[0, move.ToY].piece = move.CapturedPiece;
                    move.CapturedPiece.x = 0;
                    board[3, move.ToY].piece = null;
                }
            }

            //if move was a promotion
            if (move.IsPromotion)
            {
                Piece piece = board[move.ToX, move.ToY].piece;
                board[move.ToX, move.ToY].piece = new Pawn(piece.IsWhite, piece.IsUser, piece.x, piece.y, piece.Moves);
            }

            //swap positions on the board to previous positions
            board[move.FromX, move.FromY].piece = board[move.ToX, move.ToY].piece;
            board[move.ToX, move.ToY].piece = null;

            if(board[move.FromX, move.FromY].piece == null)
            {
                Console.WriteLine("Error: 'From' coordinates are empty");

                return;
            }

            //edit piece data to previous piece data
            board[move.FromX, move.FromY].piece.x = move.FromX;
            board[move.FromX, move.FromY].piece.y = move.FromY;
            board[move.FromX, move.FromY].piece.Moves--;

            //if move involved taking pieces
            if (move.CapturedPiece != null)
            {
                board[move.CapturedPiece.x, move.CapturedPiece.y].piece = move.CapturedPiece;
            }

            Moves.Remove(move);
        }

        //Order the moves inputted depending on moves that involve capturing and piece value
        public List<Move> OrderMoves(List<Move> moves)
        {
            List<Move> orderedMoves = new List<Move>();
            List<Move> captureMoves = new List<Move>();
            List<Move> nonCaptureMoves = new List<Move>();

            //Seperate each move into moves where pieces are captured and moves where pieces are not
            foreach (Move move in moves)
            {
                if(move.CapturedPiece != null)
                {
                    captureMoves.Add(move);
                }
                else
                {
                    nonCaptureMoves.Add(move);
                }
            }

            //sort highest value piece getting captured first
            captureMoves.Sort((m1, m2) => m2.CapturedPiece.Value - m1.CapturedPiece.Value);
            //sort worst value pieces capturing another piece first
            captureMoves.Sort((m1, m2) => board[m1.FromX, m1.FromY].piece.Value - board[m2.FromX, m2.FromY].piece.Value);
            //sort moves based on their value
            nonCaptureMoves.Sort((m1, m2) =>
            {
                Piece p1 = board[m1.FromX, m1.FromY].piece;
                Piece p2 = board[m2.FromX, m2.FromY].piece;

                if(p1.Name == "King")
                {
                    return 1;
                }
                if (p2.Name == "King")
                {
                    return -1;
                }

                return p2.Value - p1.Value;
            });

            //add moves from both lists on to a separate list
            orderedMoves.AddRange(captureMoves);
            orderedMoves.AddRange(nonCaptureMoves);

            return orderedMoves;
        }

        //Generate list of possible moves
        public List<Move> GenerateMoveList()
        {
            List<Move> moves = new List<Move>();

            //iterates through every square on board
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board[x, y].piece == null) continue;
                    Piece piece = board[x, y].piece;

                    //generates every type of move for the current piece
                    GenerateMoves(piece, moves);
                }
            }

            return moves;
        }

        //Generate list of possible capture moves
        public List<Move> GenerateCaptureMoveList()
        {
            //iterates through every square on board
            List<Move> CaptureMoves = new List<Move>();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board[x, y].piece == null) continue;
                    Piece piece = board[x, y].piece;
                    List<Move> PieceMoves = new List<Move>();
                    //Add all moves from each piece to the PieceMoves list
                    GenerateMoves(piece, PieceMoves);

                    //Filter out moves that are in quiet positions
                    foreach(Move move in PieceMoves)
                    {
                        //Add move if it places the enemy in check
                        if (DoMove(move))
                        {
                            if (InCheck(!piece.IsWhite))
                            {
                                CaptureMoves.Add(move);
                            }
                            UndoMove(move);
                            continue;
                        }

                        //Add move if a piece is captured 
                        if(move.CapturedPiece != null && move.IsCastleMove == false)
                        {
                            if(move.CapturedPiece.IsWhite == board[move.FromX, move.FromY].piece.IsWhite) { continue; }
                            CaptureMoves.Add(move);
                        }
                    }
                }
            }
            return CaptureMoves;
        }

        //Return boolean value of whether the game is a checkmate
        public bool IsCheckMate()
        {
            //Get colour of whoever's turn it is
            bool isWhite;
            if (UserTurn)
            {
                isWhite = UserWhite;
            }
            else
            {
                isWhite = !UserWhite;
            }

            //If the player is in check
            if (InCheck(isWhite))
            {
                //If no moves were generated
                if(GenerateMoveList().Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        //Return boolean value of whether the game is a stalemate
        public bool IsStaleMate()
        {
            //Get colour of whoever's turn it is
            bool isWhite;
            if (UserTurn)
            {
                isWhite = UserWhite;
            }
            else
            {
                isWhite = !UserWhite;
            }

            //If the player is not in check
            if (!InCheck(isWhite))
            {
                //If no moves were generated
                if (GenerateMoveList().Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        //Return boolean value of whether the game has ended
        public bool IsTerminalNode()
        {
            if (IsCheckMate() || IsStaleMate()) return true;

            return false;
        }

        //Add all possible moves of a piece to a list of moves
        void GenerateMoves(Piece piece, List<Move> moves)
        {
            //Identify which what type of piece to calculate moves for
            switch (piece.Name)
            {
                case "Pawn":
                    GeneratePawnMoves(piece, moves);
                    break;
                case "Knight":
                    GenerateKnightMoves(piece, moves);
                    break;
                case "Bishop":
                    GenerateBishopMoves(piece, moves);
                    break;
                case "Rook":
                    GenerateRookMoves(piece, moves);
                    break;
                case "Queen":
                    GenerateQueenMoves(piece, moves);
                    break;
                case "King":
                    GenerateKingMoves(piece, moves);
                    break;
                default:
                    return;
            }
        }

        void GeneratePawnMoves(Piece piece, List<Move> moves)
        {
            //Used to change the direction of where the pawns can move
            int direction = piece.IsUser ? 1 : -1;

            //Iterate through each surrounding square and two in front
            for(int x = -1; x < 2; x++)
            {
                for(int y = 1; y < 3; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x == 0 && y == 0) continue;
                    if (piece.x + x < 0 || piece.y + direction * y < 0) continue;
                    if (piece.x + x > 7 || piece.y + direction * y > 7) continue;

                    //Add move to list if it is legal
                    Move move = new Move(piece.x, piece.y, piece.x + x, piece.y + direction * y);
                    if (DoMove(move))
                    {
                        UndoMove(move);
                        moves.Add(move);
                    }
                }
            }
        }

        void GenerateKnightMoves(Piece piece, List<Move> moves)
        {
            //Iterate through each square it could move to
            for (int x = -2; x < 3; x++)
            {
                for(int y = -2; y < 3; y++)
                {
                    if(x == 0 || y == 0) continue;
                    if(x == y) continue;
                    if (piece.x + x < 0 || piece.y + y < 0) continue;
                    if (piece.x + x > 7 || piece.y + y > 7) continue;

                    //Add move to list if it is legal
                    Move move = new Move(piece.x, piece.y, piece.x + x, piece.y + y);
                    if (DoMove(move))
                    {
                        UndoMove(move);
                        moves.Add(move);
                    }
                }
            }
        }

        void GenerateBishopMoves(Piece piece, List<Move> moves)
        {
            //Iterate through each square it could move to
            for (int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    if (piece.x == x && piece.y == y) continue;
                    if (!(Math.Abs(piece.x - x) == Math.Abs(piece.y - y))) continue;

                    //Add move to list if it is legal
                    Move move = new Move(piece.x, piece.y, x, y);
                    if (DoMove(move))
                    {
                        UndoMove(move);
                        moves.Add(move);
                    }
                }
            }
        }
        void GenerateRookMoves(Piece piece, List<Move> moves)
        {
            //Iterate through each square it could move to
            for (int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    if (piece.x != x && piece.y != y) continue;
                    if (piece.x == x && piece.y == y) continue;

                    //Add move to list if it is legal
                    Move move = new Move(piece.x, piece.y, x, y);
                    if (DoMove(move))
                    {
                        UndoMove(move);
                        moves.Add(move);
                    }
                }
            }
        }

        void GenerateQueenMoves(Piece piece, List<Move> moves)
        {
            GenerateRookMoves(piece, moves);
            GenerateBishopMoves(piece, moves);
        }

        void GenerateKingMoves(Piece piece, List<Move> moves)
        {
            //Iterate through each square it could move to
            if (piece.Moves == 0)
            {
                //Add move to list if it is legal
                Move move = new Move(piece.x, piece.y, 2, piece.y);
                if (DoMove(move))
                {
                    UndoMove(move);
                    moves.Add(move);
                }

                //Add move to list if it is legal
                move = new Move(piece.x, piece.y, 6, piece.y);
                if (DoMove(move))
                {
                    UndoMove(move);
                    moves.Add(move);
                }
            }

            //Iterate through each square it could move to
            for (int x = -1; x < 2; x++)
            {
                for(int y = -1; y < 2; y++)
                {
                    if (piece.x + x < 0 || piece.y + y < 0) continue;
                    if (piece.x + x > 7 || piece.y + y > 7) continue;
                    if (x == 0 && y == 0) continue;

                    //Add move to list if it is legal
                    Move move = new Move(piece.x, piece.y, piece.x + x, piece.y + y);
                    if (DoMove(move))
                    {
                        UndoMove(move);
                        moves.Add(move);
                    }
                }
            }

        }
    }

    //data structure to represent each square
    public struct Spot
    {
        Piece Piece;
        int X;
        int Y;

        public Spot(int x, int y, Piece piece = null)
        {
            Piece = piece;
            X = x;
            Y = y;
        }

        public int x
        {
            get { return X; }
            set { X = value; }
        }

        public int y
        {
            get { return Y; }
            set { Y = value; }
        }

        public Piece piece
        {
            get { return Piece; }
            set { Piece = value; }
        }
    }

    //move data structure
    public class Move
    {
        public int FromX;
        public int FromY;
        public int ToX;
        public int ToY;
        public Piece CapturedPiece;
        public bool IsCastleMove;
        public bool IsPromotion;

        public Move(int fromX, int fromY, int toX, int toY, Piece capturedPiece = null, bool isPromotion = false)
        {
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
            CapturedPiece = capturedPiece;
            IsCastleMove = false;
            IsPromotion = isPromotion;
        }
    }

    //class to represent chess board
    public partial class MainWindow : Window
    {
        public static Board board;
        public Piece SelectedPiece;
        public ChessMinimax aiPlayer;
        bool UseQuiscence;
        TextBlock moveLog;
        public MainWindow(bool userWhite, int maxDepth, bool useQuiscence)
        {
            InitializeComponent();

            var grid = (Grid)this.FindName("Squares");
            moveLog = (TextBlock)this.FindName("MoveLog");

            //Create a new instance of a board
            board = new Board(grid, CheckMateScreen, DrawScreen, moveLog);

            //Create a new AI player
            aiPlayer = new ChessMinimax(board, maxDepth);

            //Set the starting positions of pieces
            board.SetStartingPositions(userWhite);

            UseQuiscence = useQuiscence;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Grid grid = (Grid)button.Parent;

            int x = Grid.GetColumn(grid);
            int y = 7 - Grid.GetRow(grid);

            if (SelectedPiece != null)
            {
                Move move = new Move(SelectedPiece.x, SelectedPiece.y, x, y);
                board.PlayMove(move);
                SelectedPiece = null;

                return;
            }
            SelectedPiece = board.GetBoard[x, y].piece;

        }

        private void Undo_Button(object sender, RoutedEventArgs e)
        {
            if(board.Moves.Count != 0)
            {
                board.UndoMove(board.Moves.Last());
                board.UpdateGUI(board.GetSquares, board.GetBoard);

                Console.WriteLine("[Undo]");
                MoveLog.Text = $"{moveLog.Text}\n[Undo]";
                return;
            }
            Console.WriteLine("Error: could not undo move");
        }

        private void Ai_Move(object sender, RoutedEventArgs e)
        {
            Move aiMove = aiPlayer.FindBestMove(UseQuiscence, board.UserTurn);
            board.PlayMove(aiMove);
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            Window1 menu = new Window1();
            menu.Show();
            this.Close();
        }

        private void Leave(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
