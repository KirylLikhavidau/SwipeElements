using Enums;
using UnityEngine;

namespace Grid.Components
{
    public class GridSection : MonoBehaviour
    {
        public bool IsInAnyProcess = false;

        public int IndexX { get; private set; }
        public int IndexY { get; private set; }
        public BlockType CurrentBlockType { get; private set; }
        public GridBlock CurrentBlock { get; private set; }

        public void SetIndex(int x, int y)
        {
            IndexX = x;
            IndexY = y;
        }

        public void FillSection(BlockType type, GridBlock block)
        {
            CurrentBlockType = type;
            CurrentBlock = block;
        }

        public void SetEmpty()
        {
            CurrentBlockType = BlockType.Empty;
            CurrentBlock = null;
        }
    }
}