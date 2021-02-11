using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Rest_API_ReversiGame.Entities.GameFolder;
using Rest_API_ReversiGame.Enums;
using Rest_API_ReversiGame.Interface;

namespace Rest_API_ReversiGame.Entities
{
    public class Game : IGame
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string Player1Token { get; set; }
        public string Player2Token { get; set; }
        public ReversiColor[,] Board { get; set; }
        public ReversiColor ColorTurn { get; set; }

        private static readonly int[] DirY = {-1, -1, -1, 0, 0, 1, 1, 1};
        private static readonly int[] DirX = {-1, 0, 1, -1, 1, -1, 0, 1};

        public Game()
        {
            this.Board = new ReversiColor[8, 8];
            FillBoardWithNone();
            this.Board[3, 3] = ReversiColor.White;
            this.Board[3, 4] = ReversiColor.Black;
            this.Board[4, 3] = ReversiColor.Black;
            this.Board[4, 4] = ReversiColor.White;
        }

        private void FillBoardWithNone()
        {
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    Board[i, j] = ReversiColor.None;
                }
            }
        }


        public bool Pass()
        {
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    if (MovePossible(i, j)) return false;
                }
            }
            ColorTurn = ColorTurn == ReversiColor.Black ? ReversiColor.White : ReversiColor.Black;
            return true;
        }

        public bool Ended()
        {
            
            var player1 = Pass();
            var player2 = Pass();
            return player1 && player2;
        }

        public ReversiColor WhereasColor()
        {
            int blackCount = 0, whitecount = 0;

            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    switch (Board[i,j])
                    {
                        case ReversiColor.None:
                            break;
                        case ReversiColor.Black:
                            blackCount++;
                            break;
                        case ReversiColor.White:
                            whitecount++;
                            break;
                    }
                }
            }

            if (whitecount == blackCount)
            {
                return ReversiColor.None;
            } else if (blackCount > whitecount)
            {
                return ReversiColor.Black;
            }
            else
            {
                return ReversiColor.White;
            }

        }

        public bool MovePossible(int rowMove, int columnMove)
        {
            if (!WithinBoard(rowMove, columnMove)) return false;
            if (!Board[rowMove, columnMove].Equals(ReversiColor.None)) return false;
            if (Board[rowMove, columnMove].Equals(ColorTurn)) return false;
            if (SurroundingTilesCheck(rowMove, columnMove)) return false;
            if (!SurroundingLinesContainSameColor(rowMove, columnMove)) return false;
            return true;
        }

        public bool PerformMove(int rowMove, int columnMove)
        {
            if (!MovePossible(rowMove, columnMove)) return false;
            Board[rowMove, columnMove] = ColorTurn;
            List<Cordinate> cordinatesToFlip = new List<Cordinate>();
            bool succes = false;
            var opposingColor = ColorTurn == ReversiColor.Black ? ReversiColor.White : ReversiColor.Black;

            for (int i = 0; i < DirX.Length; i++)
            {
                int y = rowMove, x = columnMove;
                for (int j = 0; j < Board.GetLength(0); j++)
                {
                    y += DirY[i];
                    x += DirX[i];
                    if (!WithinBoard(y, x)) break;
                    if ((Board[y, x].Equals(ReversiColor.None))) continue;
                    if (Board[y, x].Equals(opposingColor)) cordinatesToFlip.Add(new Cordinate(y, x));
                    if (!Board[y, x].Equals(ColorTurn)) continue;
                    foreach (var cordinate in cordinatesToFlip)
                    {
                        Board[cordinate.Y, cordinate.X] = ColorTurn;
                        succes = true;
                    }
                }
            }

            ColorTurn = opposingColor;
            return succes;
        }

        public bool WithinBoard(int rowMove, int columnMove)
        {
            return (rowMove > -1 && rowMove < 8) && (columnMove > -1 && columnMove < 8);
        }

        public bool SurroundingLinesContainSameColor(int rowMove, int columnMove)
        {
            var opposingColor = ColorTurn == ReversiColor.Black ? ReversiColor.White : ReversiColor.Black;
            for (int i = 0; i < DirX.Length; i++)
            {
                bool sawOther = false;
                int y = rowMove, x = columnMove;
                for (int j = 0; j < Board.GetLength(0); j++)
                {
                    y += DirY[i];
                    x += DirX[i];
                    if (!WithinBoard(y, x)) break;
                    if ((Board[y, x].Equals(ReversiColor.None))) break;
                    if (Board[y, x].Equals(opposingColor)) sawOther = true;
                    if (Board[y, x].Equals(ColorTurn) && sawOther) return true;
                }
            }

            return false;
            /*

            #region StraightLines

            //Up Vertical
            if (LineContainsSameColor(rowMove, GetMaxXY(Direction.Up), columnMove, columnMove, Direction.Up,
                ColorTurn)) return true;

            //Left Horizontal
            if (LineContainsSameColor(rowMove, rowMove, columnMove, GetMaxXY(Direction.Left), Direction.Left,
                ColorTurn)) return true;

            //Right Horizontal
            if (LineContainsSameColor(rowMove, rowMove, columnMove, GetMaxXY(Direction.Right), Direction.Right,
                ColorTurn)) return true;

            //Down Vertical
            if (LineContainsSameColor(rowMove, GetMaxXY(Direction.Down), columnMove, columnMove, Direction.Down,
                ColorTurn))
                return true;

            #endregion


            #region DiagonalLines

            //Left-Up Diagonally 
            if (LineContainsSameColor(rowMove, GetMaxXY(Direction.Up), columnMove, GetMaxXY(Direction.Left),
                Direction.LeftUp,
                ColorTurn)) return true;

            //Right-Up Diagonally
            if (LineContainsSameColor(rowMove, GetMaxXY(Direction.Up), columnMove, GetMaxXY(Direction.Right),
                Direction.RightUp,
                ColorTurn)) return true;

            //Left-Down Diagonally 
            if (LineContainsSameColor(rowMove, GetMaxXY(Direction.Down), columnMove, GetMaxXY(Direction.Left),
                Direction.LeftDown,
                ColorTurn)) return true;

            //Right-Up Diagonally
            if (LineContainsSameColor(rowMove, GetMaxXY(Direction.Down), columnMove, GetMaxXY(Direction.Right),
                Direction.RightDown,
                ColorTurn)) return true;

            #endregion


            return false;
            */
        }

        private int GetMaxXY(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    return 0;
                case Direction.Right:
                    return 7;
                case Direction.Up:
                    return 0;
                case Direction.Down:
                    return 7;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }

        private bool LineContainsSameColor(int sY, int eY, int sX, int eX, Direction dir,
            ReversiColor colorTurn)
        {
            switch (dir)
            {
                #region StraightLines

                case Direction.Left:
                    for (int i = sX; i >= eX; i--)
                    {
                        if (Board[sY, i].Equals(colorTurn))
                        {
                            return true;
                        }
                    }

                    break;
                case Direction.Right:
                    for (int i = sX; i <= eX; i++)
                    {
                        if (Board[sY, i].Equals(colorTurn))
                        {
                            return true;
                        }
                    }

                    break;
                case Direction.Up:
                    for (int i = sY; i >= eY; i--)
                    {
                        if (Board[i, sX].Equals(colorTurn))
                        {
                            return true;
                        }
                    }

                    break;
                case Direction.Down:
                    for (int i = sY; i <= eY; i++)
                    {
                        if (Board[i, sX].Equals(colorTurn))
                        {
                            return true;
                        }
                    }

                    break;

                #endregion

                #region DiagonalLines

                case Direction.LeftUp:
                    while (sY >= eY && sX >= eX)
                    {
                        if (Board[sY, sX].Equals(colorTurn))
                        {
                            return true;
                        }

                        sY -= 1;
                        sX -= 1;
                    }

                    break;
                case Direction.RightUp:
                    while (sY >= eY && sX <= eX)
                    {
                        if (Board[sY, sX].Equals(colorTurn))
                        {
                            return true;
                        }

                        sY -= 1;
                        sX += 1;
                    }

                    break;
                case Direction.LeftDown:
                    while (sY <= eY && sX >= eX)
                    {
                        if (Board[sY, sX].Equals(colorTurn))
                        {
                            return true;
                        }

                        sY += 1;
                        sX -= 1;
                    }

                    break;
                case Direction.RightDown:
                    while (sY <= eY && sX <= eX)
                    {
                        if (Board[sY, sX].Equals(colorTurn))
                        {
                            return true;
                        }

                        sY += 1;
                        sX += 1;
                    }

                    break;

                #endregion

                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }

            return false;
        }


        public bool SurroundingTilesCheck(int rowMove, int columnMove)
        {
            var surroundingTiles = new ReversiColor[8];

            //Up Tile
            surroundingTiles[0] = getTileColor(rowMove - 1, columnMove);
            //Left Tile
            surroundingTiles[1] = getTileColor(rowMove, columnMove - 1);
            //Right Tile
            surroundingTiles[2] = getTileColor(rowMove, columnMove + 1);
            //Down Tile
            surroundingTiles[3] = getTileColor(rowMove + 1, columnMove);

            //LeftUp tile 
            surroundingTiles[4] = getTileColor(rowMove - 1, columnMove - 1);
            //RightUp tile 
            surroundingTiles[5] = getTileColor(rowMove - 1, columnMove + 1);
            //LeftDown
            surroundingTiles[6] = getTileColor(rowMove + 1, columnMove - 1);
            //RightDown
            surroundingTiles[7] = getTileColor(rowMove + 1, columnMove + 1);


            if (surroundingTiles.All(st => st.Equals(ReversiColor.None))) return true;
            for (int i = 0; i < 4; i++)
            {
                if (surroundingTiles[i].Equals(ColorTurn)) return true;
            }

            return false;
        }

        public ReversiColor getTileColor(int rowMove, int columnMove)
        {
            return WithinBoard(rowMove, columnMove) ? Board[rowMove, columnMove] : ReversiColor.None;
        }
    }
}