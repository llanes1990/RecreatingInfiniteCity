using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteCity.Model.Enums
{
    public enum State
    {
        /// <summary>
        ///   Signals the end of the current players turn
        /// </summary>
        NextTurn,
        ResolveTileActions,
    }
}