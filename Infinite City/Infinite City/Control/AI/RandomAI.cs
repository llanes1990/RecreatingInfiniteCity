using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteCity.Control.AI
{
    public class RandomAI : AI
    {
        protected override object GetInput()
        {
            return GetValidInputs().Random();
        }
    }
}