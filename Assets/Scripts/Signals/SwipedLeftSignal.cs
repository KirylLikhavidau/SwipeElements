using UnityEngine;

public class SwipedLeftSignal
{
    public Vector2 StartSwipePosition { get; private set; }

    public SwipedLeftSignal(Vector2 startSwipePosition)
    {
        StartSwipePosition = startSwipePosition;
    }
}

