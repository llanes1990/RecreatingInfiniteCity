using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteCity.Model.Tiles
{
    public class Crossroads : Tile
    {
        internal Crossroads() {}

        public override string Building
        {
            get { return "Crossroads"; }
        }

        public override string Rules
        {
            get
            {
                return
                    "All tiles must be placed adjacent to a Crossroads tiles as long as any Crossroads tile in play has less than four adjacent tiles.";
            }
        }
    }
}