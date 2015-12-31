using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuCheat
{
    sealed class UnsolvableException : SudokuException
    {
        public UnsolvableException() : base("This puzzle cannot be solved")
        {
            
        }
    }
}
