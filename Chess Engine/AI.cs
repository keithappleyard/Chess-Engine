using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess_Engine.Pieces;

namespace Chess_Engine
{
    namespace AI
    {
        public class ChessMinimax
        {
            static Board board;

            //Maximum depth of minimax tree
            int MaxDepth;

            public ChessMinimax(Board Board, int maxDepth = 2)
            {
                board = Board;
                MaxDepth = maxDepth;
            }

            //Minimax algorithm
            int Minimax(int depth, bool isMaximising, int alpha, int beta, bool useQuiscence)
            {
                List<Move> moves = board.OrderMoves(board.GenerateMoveList());

                //If the game has ended or the maximum depth has been reached
                if (depth == MaxDepth || board.IsTerminalNode())
                {
                    int evalScore;
                    if (useQuiscence)
                    {
                        evalScore = Quiescence(alpha, beta);
                    }
                    else
                    {
                        evalScore = EvaluateBoard();
                    }
                    return evalScore;
                }

                //If the program is maximising
                if (isMaximising)
                {
                    int bestValue = int.MinValue;
                    foreach (Move move in moves)
                    {
                        board.DoMove(move);
                        int value = Minimax(depth + 1, false, alpha, beta, useQuiscence);
                        board.UndoMove(move);

                        bestValue = Math.Max(value, bestValue);

                        alpha = Math.Max(alpha, bestValue);
                        if (bestValue > beta)
                        {
                            break; //beta cut-off
                        }
                    }
                    return bestValue;
                }
                //If the program is minimising
                else
                {
                    int bestValue = int.MaxValue;

                    foreach (Move move in moves)
                    {
                        board.DoMove(move);
                        int value = Minimax(depth + 1, true, alpha, beta, useQuiscence);
                        board.UndoMove(move);

                        bestValue = Math.Min(value, bestValue);

                        beta = Math.Min(beta, bestValue);

                        if (bestValue < alpha)
                        {
                            break; //alpha cut-off
                        }
                    }
                    return bestValue;
                }
            }

            //Keep searching through moves where a piece is captured
            int Quiescence(int alpha, int beta)
            {
                List<Move> moves = board.OrderMoves(board.GenerateCaptureMoveList());

                int stand_pat = EvaluateBoard();

                if(moves.Count == 0)
                {
                    return stand_pat;
                }

                if (stand_pat >= beta)
                {
                    return beta;
                }

                if (alpha < stand_pat)
                {
                    alpha = stand_pat;
                }

                //Iterate through every capture move
                foreach (Move move in moves)
                {
                    board.DoMove(move);
                    int score = -Quiescence(-beta, -alpha);
                    board.UndoMove(move);

                    if (score >= beta)
                    {
                        return beta;
                    }

                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }
                return alpha;
            }

            //Calculate best possible move
            public Move FindBestMove(bool useQuiscence, bool isMax)
            {
                int bestValue = int.MinValue;
                List<Move> moves = board.OrderMoves(board.GenerateMoveList());

                //If no moves can be generated
                if (moves.Count == 0)
                {
                    Console.WriteLine("Error: No moves were generated.");
                    return null;
                }

                Move bestMove = moves[0];
                //For every move that can be played
                foreach (Move move in moves)
                {
                    board.DoMove(move);
                    int value = Minimax(0, isMax, int.MinValue, int.MaxValue, useQuiscence);
                    board.UndoMove(move);

                    value = board.UserTurn ? value * -1: value;

                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = move;
                    }
                }

                return bestMove;
            }

            //Evaluate current board state
            public int EvaluateBoard()
            {
                if (board.IsCheckMate())
                {
                    if (board.UserTurn)
                    {
                        return int.MaxValue;
                    }
                    else
                    {
                        return int.MinValue;
                    }
                }

                if (board.IsStaleMate())
                {
                    return 0;
                }

                King king = board.GetKing(!board.UserWhite) as King;
                int value = 0;

                //Add score based on material
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        Piece piece = board.GetBoard[x, y].piece;
                        if (piece == null) continue;

                        if (!piece.IsUser){
                            value += piece.Value * 10;
                        }
                        else
                        {
                            value -= piece.Value * 10;
                        }
                    }
                }

                //calculate ai mobility score
                int aiMobility = board.GenerateMoveList().Count;
                board.UserTurn = !board.UserTurn;
                //calculate user mobility score
                int playerMobility = board.GenerateMoveList().Count;
                board.UserTurn = !board.UserTurn;
                //calculate total mobility score
                int mobilityScore = (aiMobility - playerMobility);
                //add up mobility score
                value += mobilityScore;

                //add score based on castling
                if (king.HasCastled)
                {
                    if (!king.IsUser)
                    {
                        value += 25; //can be altered
                    }
                    else
                    {
                        value -= 25;
                    }
                }
                return value;
            }
        }
    }
}