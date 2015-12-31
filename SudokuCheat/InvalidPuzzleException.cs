using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuCheat
{
    sealed class InvalidPuzzleException : SudokuException
    {
        public InvalidPuzzleException() : base("This is not a valid puzzle") { }
        public InvalidPuzzleException(string message) : base(message) { }
    }
}
