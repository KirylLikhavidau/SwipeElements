using UnityEngine;

public class SwipedRightSignal
{
    public Vector2 StartSwipePosition { get; private set; }

    public SwipedRightSignal(Vector2 startSwipePosition)
    {
        StartSwipePosition = startSwipePosition;
    }
}

