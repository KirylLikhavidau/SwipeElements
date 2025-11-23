using Grid.Components;
using System.Collections.Generic;

namespace Signals
{
    public class BlocksDeactivatingSignal
    {
        public List<GridSection> Sections { get; private set; }

        public BlocksDeactivatingSignal(List<GridSection> sections)
        {
            Sections = sections;
        }
    }
}