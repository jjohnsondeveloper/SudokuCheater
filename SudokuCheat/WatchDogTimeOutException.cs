using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuCheat
{
    sealed class WatchDogTimeOutException : SudokuException
    {
        public WatchDogTimeOutException(string message) : base("Timeout Exception caused by: " + message) { }
    }
}
