using UnityEngine;

public class SwipedDownSignal
{
    public Vector2 StartSwipePosition { get; private set; }
    
    public SwipedDownSignal(Vector2 startSwipePosition)
    {
        StartSwipePosition = startSwipePosition;    
    }
}

