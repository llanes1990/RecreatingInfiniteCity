using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteCity.Model.Interfaces
{
    public interface IAsyncInput
    {
        bool IsComplete { get; }
        bool IsThinking { get; }
        object Acknowledge();
        void GetInputAsync();
    }
}