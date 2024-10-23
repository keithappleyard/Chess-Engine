using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess_Engine
{
    namespace Pieces
    {
        //Parent class to represent chess pieces
        public class Piece
        {
            public int Value { get; protected set; } = 0;
            public bool IsWhite;
            public bool IsUser;
            public string Name;
            public int x;
            public int y;
            public int Moves;

            public Piece(bool isWhite, bool isUser, int X, int Y, int moves = 0)
            {
                IsWhite = isWhite;
                IsUser = isUser;
                x = X;
                y = Y;
                Moves = moves;
            }

            //Function to determine whether a piece can move a specified place on the board
            public virtual bool CanMove(Spot[,] board, int EndX, int EndY)
            {
                if ((EndX >= 0 && EndX <= 7) && (EndY >= 0 && EndY <= 7))
                {
                    if (board[EndX, EndY].piece != null)
                    {
                        if (board[EndX, EndY].piece.IsWhite != IsWhite)
                        {
                            return true;
                        }
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        //Pawn piece
        public class Pawn : Piece
        {
            public Pawn(bool isWhite, bool isUser, int X, int Y, int moves = 0) : base(isWhite, isUser, X, Y, moves)
            {
                IsWhite = isWhite;
                IsUser = isUser;
                x = X;
                y = Y;
                Name = "Pawn";
                Value = 1;
                Moves = moves;
            }

            public override bool CanMove(Spot[,] board, int EndX, int EndY)
            {
                if ((EndX >= 0 && EndX <= 7) && (EndY >= 0 && EndY <= 7))
                {
                    //Taking a piece
                    if (Math.Abs(EndX - x) == 1 && board[EndX, EndY].piece != null && Math.Abs(EndY - y) == 1)
                    {
                        switch (IsUser)
                        {
                            case true:
                                if (EndY > y && board[EndX, EndY].piece.IsUser == false)
                                {
                                    return true;
                                }
                                return false;
                            case false:
                                if (EndY < y && board[EndX, EndY].piece.IsUser)
                                {
                                    return true;
                                }
                                return false;
                        }
                    }

                    //En passant
                    else if(board[EndX, EndY].piece == null && (EndY == 2 || EndY == 5) && Math.Abs(EndX-x) == 1 && board[EndX, y].piece != null && board[EndX, y].piece.Name == "Pawn" && board[EndX, y].piece.IsWhite == !board[x, y].piece.IsWhite && board[EndX, y].piece.Moves == 1)
                    {
                        switch (IsUser)
                        {
                            case true:
                                if (board[EndX, EndY - 1].piece != null && board[EndX, EndY - 1].piece.IsUser == false && board[EndX, EndY - 1].piece.Moves == 1)
                                {
                                    return true;
                                }
                                return false;
                            case false:
                                if (board[EndX, EndY + 1].piece != null && EndY < y && board[EndX, EndY + 1].piece.IsUser && board[EndX, EndY + 1].piece.Moves == 1)
                                {
                                    return true;
                                }
                                return false;
                        }
                    }

                    //Normal move
                    else if (x == EndX && board[EndX, EndY].piece == null)
                    {
                        if (Math.Abs(EndY - y) == 1 || (Math.Abs(EndY - y) == 2 && board[x, y].piece.Moves == 0))
                        {
                            switch (IsUser)
                            {
                                case true:
                                    if (EndY > y)
                                    {
                                        if(EndY == y + 2 && board[x, EndY-1].piece != null)
                                        {
                                            return false;
                                        }
                                        return true;
                                    }
                                    return false;
                                case false:
                                    if (EndY < y)
                                    {
                                        if (EndY == y - 2 && board[x, EndY + 1].piece != null)
                                        {
                                            return false;
                                        }
                                        return true;
                                    }
                                    return false;
                            }
                        }
                    }

                }
                return false;
                //add promotion
            }
        }

        //Knight piece
        public class Knight : Piece
        {
            public Knight(bool isWhite, bool isUser, int X, int Y, int moves = 0) : base(isWhite, isUser, X, Y, moves)
            {
                IsWhite = isWhite;
                x = X;
                y = Y;
                Name = "Knight";
                Value = 3;
                Moves = moves;
            }

            public override bool CanMove(Spot[,] board, int EndX, int EndY)
            {
                if (base.CanMove(board, EndX, EndY))
                {
                    if ((Math.Abs(EndX - x) == 1 && Math.Abs(EndY - y) == 2) || (Math.Abs(EndX - x) == 2 && Math.Abs(EndY - y) == 1))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public class Bishop : Piece
        {
            public Bishop(bool isWhite, bool isUser, int X, int Y, int moves = 0) : base(isWhite, isUser, X, Y, moves)
            {
                IsWhite = isWhite;
                x = X;
                y = Y;
                Name = "Bishop";
                Value = 3;
                Moves = moves;
            }

            public override bool CanMove(Spot[,] board, int EndX, int EndY)
            {
                if (base.CanMove(board, EndX, EndY))
                {
                    if (Math.Abs(EndX - x) == Math.Abs(EndY - y) && EndX != x && EndY != y)
                    {
                        for (int i = 1; i < Math.Abs(EndX - x); i++)
                        {
                            if (EndX > x)
                            {
                                if (EndY > y)
                                {
                                    if (board[x + i, y + i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (board[x + i, y - i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (EndY > y)
                                {
                                    if (board[x - i, y + i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (board[x - i, y - i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        //Rook piece
        public class Rook : Piece
        {
            public Rook(bool isWhite, bool isUser, int X, int Y, int moves = 0) : base(isWhite, isUser, X, Y, moves)
            {
                IsWhite = isWhite;
                x = X;
                y = Y;
                Name = "Rook";
                Value = 5;
                Moves = moves;
            }

            public override bool CanMove(Spot[,] board, int EndX, int EndY)
            {
                if (base.CanMove(board, EndX, EndY))
                {
                    if (EndX == x ^ EndY == y)
                    {
                        if (EndX != x)
                        {
                            for (int i = 1; i < Math.Abs(EndX - x); i++)
                            {
                                if (EndX > x)
                                {
                                    if (board[x + i, y].piece != null)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (board[x - i, y].piece != null)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 1; i < Math.Abs(EndY - y); i++)
                            {
                                if (EndY > y)
                                {
                                    if (board[x, y + i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (board[x, y - i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        //Queen piece
        public class Queen : Piece
        {
            public Queen(bool isWhite, bool isUser, int X, int Y, int moves = 0) : base(isWhite, isUser, X, Y, moves)
            {
                IsWhite = isWhite;
                x = X;
                y = Y;
                Name = "Queen";
                Value = 9;
                Moves = moves;
            }
            public override bool CanMove(Spot[,] board, int EndX, int EndY)
            {
                if (base.CanMove(board, EndX, EndY))
                {
                    if (EndX == x ^ EndY == y)
                    {
                        if (EndX != x)
                        {
                            for (int i = 1; i < Math.Abs(EndX - x); i++)
                            {
                                if (EndX > x)
                                {
                                    if (board[x + i, y].piece != null)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (board[x - i, y].piece != null)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 1; i < Math.Abs(EndY - y); i++)
                            {
                                if (EndY > y)
                                {
                                    if (board[x, y + i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (board[x, y - i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        return true;
                    }

                    if (Math.Abs(EndX - x) == Math.Abs(EndY - y) && (EndX != x && EndY != y))
                    {
                        for (int i = 1; i < Math.Abs(EndX - x); i++)
                        {
                            if (EndX > x)
                            {
                                if (EndY > y)
                                {
                                    if (board[x + i, y + i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (board[x + i, y - i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (EndY > y)
                                {
                                    if (board[x - i, y + i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (board[x - i, y - i].piece != null)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        return true;
                    }
                }        
                return false;
            }
        }

        //King piece
        public class King : Piece
        {

            public bool HasCastled;
            public King(bool isWhite, bool isUser, int X, int Y, bool hasCastled = false, int moves = 0) : base(isWhite, isUser, X, Y, moves)
            {
                IsWhite = isWhite;
                x = X;
                y = Y;
                Name = "King";
                HasCastled = hasCastled;
                Value = int.MaxValue;
                Moves = moves;
            }

            public override bool CanMove(Spot[,] board, int EndX, int EndY)
            {
                //castling
                if (board[EndX, EndY].piece == null && Moves == 0 && EndY == y && ((EndX == 2 && board[EndX - 2, EndY].piece != null) || (EndX == 6 && board[EndX + 1, EndY].piece != null)))
                {
                    switch (EndX)
                    {
                        case 2: 
                            if (!(board[EndX - 2, EndY].piece.Name == "Rook" && board[EndX - 2, EndY].piece.Moves == 0))
                            {
                                return false;
                            }
                            for (int i = 1; i < x; i++)
                            {
                                if (board[i, EndY].piece != null)
                                {
                                    return false;
                                }
                            }
                            return true;
                        case 6:
                            if (!(board[EndX + 1, EndY].piece.Name == "Rook" && board[EndX + 1, EndY].piece.Moves == 0))
                            {
                                return false;
                            }
                            for (int i = 1; i < EndX - x; i++)
                            {
                                if (board[x + i, EndY].piece != null)
                                {
                                    return false;
                                }
                            }
                            return true;
                    }
                }
                //normal move
                if (base.CanMove(board, EndX, EndY))
                {
                    if(Math.Abs(EndX-x) <= 1 && Math.Abs(EndY-y) <= 1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
