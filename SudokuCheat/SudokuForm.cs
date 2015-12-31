using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuCheat
{
    public partial class SudokuCheater : Form
    {
        #region Constants: Sizes and Spacing
        private const int leftRightSpacing = 50,
            upDownSpacing = 50,
            numBoxesPerRow = 9,
            numRows = 9,
            leftRightOffset = 10,
            upDownOffset = 10,
            boxWidth = 30,
            boxHeight = 30,
            formWidth = (2 * leftRightOffset) + (numBoxesPerRow * leftRightSpacing),
            formHeight = (numRows * upDownSpacing) + (upDownOffset * 2 + 2 * upDownSpacing),
            labelTop = formHeight - 60,
            labelLeft = 10,
            labelWidth = 250,
            penWidth = 5;
        private const float fontSize = 14.0f;
        #endregion

        /// <summary>
        /// TextBoxes that contain the values of the user input puzzle
        /// </summary>
        private TextBox[,] sudokuTextBoxes;
        private Label timeLabel;
        private Button solveButton, clearButton;
        public SudokuCheater()
        {
            InitializeComponent();
            SetUpForm();
        }


        private void SetUpForm()
        {
            #region Form Size
            this.Width = formWidth;
            this.Height = formHeight;
            #endregion

            #region Text Boxes
            sudokuTextBoxes = new TextBox[numBoxesPerRow, numRows];
            int x = upDownOffset, y;
            for (int i = 0; i < numRows; ++i)
            {
                y = leftRightOffset;
                for (int j = 0; j < numBoxesPerRow; ++j)
                {
                    sudokuTextBoxes[i, j] = new TextBox();
                    sudokuTextBoxes[i, j].GotFocus += new EventHandler(TextBoxSelected);
                    sudokuTextBoxes[i, j].Click += new EventHandler(TextBoxSelected);
                    sudokuTextBoxes[i, j].TextChanged += new EventHandler(SudokuBoxReceivedInput);
                    sudokuTextBoxes[i, j].BorderStyle = BorderStyle.Fixed3D;
                    sudokuTextBoxes[i, j].Width = boxWidth;
                    sudokuTextBoxes[i, j].Height = boxHeight;
                    sudokuTextBoxes[i, j].Font = new Font(sudokuTextBoxes[i,j].Font.FontFamily, fontSize);
                    sudokuTextBoxes[i, j].TextAlign = HorizontalAlignment.Center;
                    sudokuTextBoxes[i, j].Tag = String.Format("{0},{1}", i, j);
                    sudokuTextBoxes[i, j].Left = y;
                    sudokuTextBoxes[i, j].Top = x;
                    this.Controls.Add(sudokuTextBoxes[i, j]);
                    y += leftRightSpacing;
                }
                x += upDownSpacing;
            }
            #endregion

            #region Buttons
            solveButton = new Button();
            solveButton.Text = "Solve";
            solveButton.Top = x;
            solveButton.Left = 150;
            solveButton.Click += new EventHandler(SolvePuzzle);

            clearButton = new Button();
            clearButton.Text = "Clear (Esc)";
            clearButton.Top = x;
            clearButton.Left = 250;
            clearButton.Click += new EventHandler(ClearPuzzle);
            #endregion

            #region Label
            timeLabel = new Label();
            timeLabel.Visible = false;
            timeLabel.Top = labelTop;
            timeLabel.Left = labelLeft;
            timeLabel.Width = labelWidth;
            this.Controls.Add(timeLabel);
            #endregion

            #region Finalize Form
            this.AcceptButton = solveButton;
            this.CancelButton = clearButton;
            this.Controls.Add(solveButton);
            this.Controls.Add(clearButton);
            this.StartPosition = FormStartPosition.CenterScreen;
            #endregion
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black);
            pen.Width = penWidth;
            Graphics graphics = this.CreateGraphics();
            graphics.DrawLine(pen, leftRightOffset, upDownSpacing * 3 + upDownOffset - pen.Width * 2, this.Width - leftRightOffset * 3, upDownSpacing * 3 + upDownOffset - pen.Width * 2);
            graphics.DrawLine(pen, leftRightOffset, upDownSpacing * 6 + upDownOffset - pen.Width * 2, this.Width - leftRightOffset * 3, upDownSpacing * 6 + upDownOffset - pen.Width * 2);
            graphics.DrawLine(pen, 3 * leftRightSpacing, upDownOffset, 3 * leftRightSpacing, this.Height - upDownOffset * 13);
            graphics.DrawLine(pen, 6 * leftRightSpacing, upDownOffset, 6 * leftRightSpacing, this.Height - upDownOffset * 13);
            pen.Dispose();
            graphics.Dispose();
        }

        public void SudokuBoxReceivedInput(Object sender, EventArgs e)
        {
            TextBox thisBox = (TextBox)sender;
            if (thisBox.Text.Length == 1)
            {
                int val = (int)thisBox.Text[0];
                if (val >= 0x30 && val <= 0x39) SendKeys.Send("{TAB}");
                else
                {
                    thisBox.Clear();
                }
            }
            else if (thisBox.Text.Length > 1)
            {
                char lastEntered = thisBox.Text[thisBox.Text.Length - 1];
                thisBox.Text = lastEntered.ToString();
            }
        }

        public void TextBoxSelected(Object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Focus();
            textBox.SelectAll();
        }
        public void ClearPuzzle(Object sender, EventArgs e)
        {
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    sudokuTextBoxes[i, j].Text = String.Empty;
                    sudokuTextBoxes[i, j].ForeColor = System.Drawing.Color.Black;
                }
            }
            sudokuTextBoxes[0, 0].Focus();
        }
        public void SolvePuzzle(Object sender, EventArgs e)
        {
            int[,] puzzle = new int[9, 9];
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    if (sudokuTextBoxes[i, j].Text.Trim() == String.Empty)
                    {
                        puzzle[i, j] = 0;
                    }
                    else
                    {
                        if(!Int32.TryParse(sudokuTextBoxes[i,j].Text, out puzzle[i,j])) 
                        {
                            MessageBox.Show("Failed!");
                            this.Dispose();
                        }
                    }
                }
            }
            try
            {
                SudokuSolver sudoku = new SudokuSolver(puzzle);
                timeLabel.Visible = true;
                timeLabel.Text = String.Format("Time to solve: {0} milliseconds",sudoku.TimeElapsed);
                for (int i = 0; i < 9; ++i)
                {
                    for (int j = 0; j < 9; ++j)
                    {
                        if (sudokuTextBoxes[i, j].Text == String.Empty || sudokuTextBoxes[i, j].Text == "0")
                            sudokuTextBoxes[i, j].ForeColor = System.Drawing.Color.Red;
                        else sudokuTextBoxes[i, j].ForeColor = System.Drawing.Color.Black;
                        sudokuTextBoxes[i, j].Text = String.Format("{0}", sudoku[i, j]);
                    }
                }
            }
            catch (SudokuException se)
            {
                MessageBox.Show(se.Message);
            }
            clearButton.Focus();
        }
    }
}
