using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuCheat
{
    class SudokuSolver
    {
        private int[,] solved;
        private const int timeOutMillis = 4000;
        private const int numRows = 9, numColumns = 9;
        /// <summary>
        /// The solution to the Sudoku puzzle
        /// </summary>
        public int[,] Solution { get { return solved; } private set { this.solved = value; } }
        public int this[int i, int j] { get { return solved[i, j]; } private set { solved[i, j] = value; } }
        /// <summary>
        /// The time in milliseconds taken to solve the puzzle
        /// </summary>
        public long TimeElapsed { get; private set; }

        public SudokuSolver(int[,] sudokuPuzzle)
        {
            if (sudokuPuzzle.Length != 81) throw new InvalidPuzzleException("Invalid puzzle: size not 9x9");
            Solution = sudokuPuzzle;
            CheckIfPuzzleIsValid();
            SolveIt();
        }

        private void CheckIfPuzzleIsValid()
        {
            List<List<int>> rows = new List<List<int>>();
            List<List<int>> cols = new List<List<int>>();
            for (int i = 0; i < numRows; ++i)
            {
                rows.Add(new List<int>());
                cols.Add(new List<int>());
            }
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numColumns; ++j)
                {
                    if (this[i, j] != 0)
                    {
                        if (rows[i].Contains(this[i, j]))
                            throw new InvalidPuzzleException(
                                String.Format("The number {1} has been entered more than once on row {0}", (i + 1), this[i, j]));
                        else if (cols[j].Contains(this[i, j]))
                            throw new InvalidPuzzleException(
                                String.Format("The number {1} has been entered more than once on column {0}", (j + 1), this[i, j]));
                        else if (this[i, j] < 0 || this[i,j] > 9)
                            throw new InvalidPuzzleException(
                                String.Format("The number {2} at row {0} column {1} is not valid", (i + 1), (j + 1), this[i, j]));
                    }
                    rows[i].Add(this[i, j]); cols[j].Add(this[i, j]);
                }
            }
            rows = null;
            cols = null;
        }

        private void SolveIt()
        {
            System.Diagnostics.Stopwatch watchDog = new System.Diagnostics.Stopwatch();
            watchDog.Start();
            //List contains all the blank spots on the grid - denoted by 0's
            List<int[]> blanks = new List<int[]>();
            //Populate the list of blanks spots with the coordinates of where they are
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numColumns; ++j)
                {
                    if (this[i, j] == 0) blanks.Add(new int[] { i, j });
                }
            }
            //start at the beginning
            int index = 0;
            //have a way to point to this row and column - denoted by r and c
            int r, //row 
                c, //column
                sr, //start of row
                sc, //start of column
                srl, //start row's limit
                scl; //start columns's limit
            //whether or not a search for a valid number is taking place
            bool searching;
            //run while index is valid
            while (index < blanks.Count)
            {
                if (watchDog.ElapsedMilliseconds > timeOutMillis) //stuck in loop
                    throw new WatchDogTimeOutException("a repeating cycle indicating a possibly invalid puzzle");
                //get this row and column
                r = blanks[index][0];
                c = blanks[index][1];
                do //run at least once (because of 0's and 0's aren't valid)
                {
                    searching = false; //assume this will work
                    ++this[r, c]; //increment the value here
                    //check the rows and columns
                    for (int i = 0; i < 9; ++i)
                    {
                        //if i == r, or i == c - those rows and columns will always equate so ignore those
                        if ((i != r && this[r, c] == this[i, c]) || (i != c && this[r, c] == this[r, i]))
                        {
                            //if we found a match, it means that's not a valid number
                            searching = true;
                            //quit looking through these rows and columns, we'll need a different number
                            break;
                        }
                    }
                    if (!searching)
                    {
                        //check squares
                        sr = r / 3;
                        sc = c / 3;
                        sr *= 3;
                        sc *= 3;
                        srl = sr + 3;
                        scl = sc + 3;
                        for (int i = sr; i < srl; ++i)
                        {
                            for (int j = sc; j < scl; ++j)
                            {
                                if ((r != i || c != j) && this[r, c] == this[i, j])
                                {
                                    searching = true;
                                    break;
                                }
                            }
                            if (searching) break;
                        }
                    }
                } while (searching);
                //if this number has exceeded 9, reset it and go back one index
                if (this[r, c] > 9)
                {
                    this[r, c] = 0;
                    --index;
                    //ensure that we don't have a negative index - print error (shouldn't happen)
                    if (index < 0)
                    {
                        throw new UnsolvableException();
                    }
                }
                //otherwise, go to next index (if there is one)
                else ++index;
            }
            watchDog.Stop();
            TimeElapsed = watchDog.ElapsedMilliseconds;
            watchDog = null;
        }
    }
}