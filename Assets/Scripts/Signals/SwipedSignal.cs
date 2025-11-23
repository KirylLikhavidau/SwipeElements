using Enums;
using UnityEngine;

namespace Signals
{
    public class SwipedSignal
    {
        public DirectionEnum Direction { get; private set; }
        public Vector2 StartSwipePosition { get; private set; }

        public SwipedSignal(Vector2 startSwipePosition, DirectionEnum direction)
        {
            StartSwipePosition = startSwipePosition;
            Direction = direction;
        }
    }
}