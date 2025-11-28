using Enums;
using Grid.Components;

namespace Signals
{
    public class SectionDetectedSignal
    {
        public DirectionEnum Direction { get; private set; }
        public GridSection Section { get; private set; }

        public SectionDetectedSignal(GridSection section, DirectionEnum direction)
        {
            Section = section;
            Direction = direction;
        }
    }
}