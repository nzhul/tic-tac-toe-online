using NetworkShared.Models;
using System;
using System.Data.Common;
using TicTacToe.Server.Utilities;

namespace TicTacToe.Server.Games
{
    public class Game
    {
        private const int GRID_SIZE = 3;

        public Game(string xUser, string oUser)
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            CurrentRoundStartTime = DateTime.UtcNow;
            XUser = xUser;
            OUser = oUser;
            Round = 1;
            Grid = new MarkType[GRID_SIZE, GRID_SIZE];
            CurrentUser = xUser;
        }

        public Guid Id { get; }

        public ushort Round { get; set; }

        public DateTime StartTime { get; }

        public DateTime CurrentRoundStartTime { get; set; }

        public string OUser { get; }

        public ushort OWins { get; set; }

        public bool OWantRematch { get; set; }

        public string XUser { get; }

        public ushort XWins { get; set; }

        public bool XWantRematch { get; set; }

        public string CurrentUser { get; set; }

        public MarkType CurrentType
        {
            get
            {
                return GetPlayerType(CurrentUser);
            }
        }

        public MarkType[,] Grid { get; }

        public string GetOpponent(string otherUserId)
        {
            if (otherUserId == XUser)
            {
                return OUser;
            }
            else
            {
                return XUser;
            }
        }

        public void AddWin(string winnerId)
        {
            var winnerType = GetPlayerType(winnerId);

            if (winnerType == MarkType.X)
            {
                XWins++;
            }
            else
            {
                OWins++;
            }
        }

        public void SetRematchReadiness(string userId)
        {
            var playerType = GetPlayerType(userId);
            if (playerType == MarkType.X)
            {
                XWantRematch = true;
            }
            else
            {
                OWantRematch = true;
            }
        }

        public void SwitchCurrentPlayer()
        {
            CurrentUser = GetOpponent(CurrentUser);
        }

        public void NewRound()
        {
            CurrentRoundStartTime = DateTime.UtcNow;
            ResetGrid();
            CurrentUser = XUser;
        }

        public bool BothPlayersReady()
        {
            return XWantRematch && OWantRematch;
        }

        public MarkResult MarkCell(byte index)
        {
            var (row, col) = BasicExtensions.GetRowCol(index);
            Grid[row, col] = GetPlayerType(CurrentUser);

            var (isWin, lineType) = CheckWin(row, col);
            var draw = CheckDraw();

            var result = new MarkResult();

            if (isWin)
            {
                result.Outcome = MarkOutcome.Win;
                result.WinLineType = lineType;
            }
            else if (draw)
            {
                result.Outcome = MarkOutcome.Draw;
            }

            return result;
        }

        private MarkType GetPlayerType(string userId)
        {
            if (userId == XUser)
            {
                return MarkType.X;
            }
            else
            {
                return MarkType.O;
            }
        }

        private (bool, WinLineType) CheckWin(byte row, byte col)
        {
            var type = Grid[row, col];

            // check col
            for (int i = 0; i < GRID_SIZE; i++)
            {
                if (Grid[row, i] != type) break;
                if (i == GRID_SIZE - 1) return (true, ResolveLineTypeRow(row));
            }

            // check row
            for (int i = 0; i < GRID_SIZE; i++)
            {
                if (Grid[i, col] != type) break;
                if (i == GRID_SIZE - 1) return (true, ResolveLineTypeCol(col));
            }

            // check diagonal
            if (row == col)
            {
                // we are on a diagonal
                for (int i = 0; i < GRID_SIZE; i++)
                {
                    if (Grid[i, i] != type) break;
                    if (i == GRID_SIZE - 1) return (true, WinLineType.Diagonal);
                }
            }

            // check anti-diagonal
            if (row + col == GRID_SIZE - 1)
            {
                for (int i = 0; i < GRID_SIZE; i++)
                {
                    if (Grid[i, (GRID_SIZE - 1) - i] != type) break;
                    if (i == GRID_SIZE - 1) return (true, WinLineType.AntiDiagonal);
                }
            }

            return (false, WinLineType.None);
        }

        private WinLineType ResolveLineTypeRow(int row)
        {
            return (WinLineType)(row + 6);
        }

        private WinLineType ResolveLineTypeCol(int column)
        {
            return (WinLineType)(column + 3);
        }

        private bool CheckDraw()
        {
            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    if (Grid[row, col] == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ResetGrid()
        {
            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    Grid[row, col] = 0;
                }
            }

            return true;
        }
    }

    public struct MarkResult
    {
        public MarkOutcome Outcome { get; set; }

        public WinLineType WinLineType { get; set; }
    }
}
