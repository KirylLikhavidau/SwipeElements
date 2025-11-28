using Grid.Components;

namespace Signals
{
    public class BlockMovedSignal
    {
        public GridSection FirstSection { get; private set; }
        public GridSection SecondSection { get; private set; }

        public BlockMovedSignal(GridSection firstSection, GridSection secondSection)
        {
            FirstSection = firstSection;
            SecondSection = secondSection;
        }
    }
}