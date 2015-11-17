using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticsLab
{
    class Grid
    {
        public Grid(string topSequence, string leftSequence, bool isScoringAlgorithm)
        {
            this.leftSequence = leftSequence;
            this.topSequence = topSequence;

            if (isScoringAlgorithm)
            {
                scoreGrid = new List<List<int>>();
                PopulateScoringPathGrid();
                // PrintScoreGrid();
            }
            else
            {
                pathGrid = new List<List<DirectionCost>>();
                PopulateExtractionPathGrid();
            }
        }

        private Grid() { }

        private enum Direction
        {
            None,
            Right,
            Down,
            Diag
        }

        private struct DirectionCost
        {
            internal Direction direction;
            internal int pathCost;
        }

        private List<List<DirectionCost>> pathGrid;
        private List<List<int>> scoreGrid;
        private string leftSequence;
        private string topSequence;

        private const int CONST_INDEL_COST = 5;
        private const int CONST_MISMATCH_COST = 1;
        private const int CONST_MATCH_COST = -3;

        // PRIVATE METHODS

        private void PopulateExtractionPathGrid()
        {
            for (int i = 0; i <= leftSequence.Length; i++)
            {
                pathGrid.Add(new List<DirectionCost>(topSequence.Length + 1));

                DirectionCost downDirCost = new DirectionCost();

                downDirCost.pathCost = i * 5;
                downDirCost.direction = Direction.Down;
                pathGrid[i].Add(downDirCost);

                for (int j = 1; j <= topSequence.Length; j++)
                {
                    DirectionCost rightDirCost = new DirectionCost();
                    rightDirCost.pathCost = j * 5;
                    rightDirCost.direction = Direction.Right;
                    pathGrid[i].Add(rightDirCost);
                }
            }
        }

        private void PopulateScoringPathGrid()
        {
            scoreGrid.Add(new List<int>());
            scoreGrid.Add(new List<int>());
            scoreGrid[1].Add(5);

            for (int j = 0; j <= topSequence.Length; j++)
            {
                scoreGrid[0].Add(j * 5);
            }
        }

        private void PrintPathGrid()
        {
            for (int i = 0; i < pathGrid.Count; i++)
            {
                for (int j = 0; j < pathGrid[i].Count; j++)
                {
                    Console.Write(pathGrid[i][j].pathCost + "\t");
                }

                Console.Write("\n");
            }
        }

        private void PrintScoreGrid()
        {
            for (int i = 0; i < scoreGrid.Count; i++)
            {
                for (int j = 0; j < scoreGrid[i].Count; j++)
                {
                    Console.Write(scoreGrid[i][j] + "\t");
                }

                Console.Write("\n");
            }
        }

        // PUBLIC METHODS

        public int CalculateExtractionSolution()
        {
            // Iterate over rows
            for (int i = 1; i < pathGrid.Count; i++)
            {
                // Iterate over columns
                for (int j = 1; j < pathGrid[i].Count; j++)
                {
                    DirectionCost newDirCost = GetMoveDirCost(i, j);
                    pathGrid[i][j] = newDirCost;
                }
            }

            // PrintPathGrid();

            return pathGrid[leftSequence.Length][topSequence.Length].pathCost;
        }

        public int CalculateScoreSolution()
        {
            int currentRow = 1;

            while (currentRow <= leftSequence.Length)
            {
                for (int i = 1; i <= topSequence.Length; i++)
                {
                    scoreGrid[1].Add(GetMinCost(ScoreRight(1, i), ScoreDown(1, i), ScoreDiag(1, i, leftSequence[currentRow - 1] == topSequence[i - 1])));
                }

                currentRow++;

                scoreGrid[0] = scoreGrid[1];
                scoreGrid[1] = new List<int>(topSequence.Length + 1);
                scoreGrid[1].Add(currentRow * 5);
            }

            return scoreGrid[0][topSequence.Length];
        }

        // HELPER METHODS

        private DirectionCost GetMoveDirCost(int currentRow, int currentCol)
        {
            char leftChar = leftSequence[currentRow - 1];
            char topChar = topSequence[currentCol - 1];

            bool isMatch = leftChar == topChar;

            DirectionCost right = new DirectionCost();
            right.direction = Direction.Right;
            right.pathCost = PathRight(currentRow, currentCol);

            DirectionCost down = new DirectionCost();
            down.direction = Direction.Down;
            down.pathCost = PathDown(currentRow, currentCol);

            DirectionCost diag = new DirectionCost();
            diag.direction = Direction.Diag;
            diag.pathCost = PathDiag(currentRow, currentCol, isMatch);

            return GetMinDirCost(right, down, diag);
        }

        private DirectionCost GetMinDirCost(DirectionCost rightCost, DirectionCost downCost, DirectionCost diagCost)
        {
            DirectionCost firstWinner = (rightCost.pathCost <= downCost.pathCost ? rightCost : downCost);
            return diagCost.pathCost <= firstWinner.pathCost ? diagCost : firstWinner;
        }

        private int GetMinCost(int first, int second, int third)
        {
            return Math.Min(first, Math.Min(second, third));
        }

        // MOVE COST METHODS

        private int PathRight(int currentRow, int currentCol)
        {
            return pathGrid[currentRow][currentCol - 1].pathCost + CONST_INDEL_COST;
        }

        private int ScoreRight(int currentRow, int currentCol)
        {
            return scoreGrid[currentRow][currentCol - 1] + CONST_INDEL_COST;
        }

        private int PathDown(int currentRow, int currentCol)
        {
            return pathGrid[currentRow - 1][currentCol].pathCost + CONST_INDEL_COST;
        }

        private int ScoreDown(int currentRow, int currentCol)
        {
            return scoreGrid[currentRow - 1][currentCol] + CONST_INDEL_COST;
        }

        private int PathDiag(int currentRow, int currentCol, bool isMatch)
        {
            return pathGrid[currentRow - 1][currentCol - 1].pathCost + (isMatch ? CONST_MATCH_COST : CONST_MISMATCH_COST);
        }

        private int ScoreDiag(int currentRow, int currentCol, bool isMatch)
        {
            return scoreGrid[currentRow - 1][currentCol - 1] + (isMatch ? CONST_MATCH_COST : CONST_MISMATCH_COST);
        }
    }
}
