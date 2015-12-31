using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuCheat
{
    abstract class SudokuException : Exception
    {
        public SudokuException(string message) : base(message) { }
    }
}
