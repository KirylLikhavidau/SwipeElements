using UnityEngine;

public class SwipedUpSignal
{
    public Vector2 StartSwipePosition { get; private set; }

    public SwipedUpSignal(Vector2 startSwipePosition)
    {
        StartSwipePosition = startSwipePosition;
    }
}