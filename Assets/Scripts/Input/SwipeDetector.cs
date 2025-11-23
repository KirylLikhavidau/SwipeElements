using Configs;
using Enums;
using Signals;
using System;
using UnityEngine;
using Zenject;

namespace Input
{
    public class SwipeDetector : IInitializable, IDisposable
    {
        private SignalBus _signalBus;
        private SwipeHandler _swipeHandler;

        private float _minimumDistance;
        private float _maximumTime;
        private float _directionThreshold;

        private Vector2 _startPosition;
        private Vector2 _endPosition;
        private float _startTime;
        private float _endTime;

        private SwipeDetector(SwipeHandler swipeHandler, InputConfig inputConfig, SignalBus signalBus)
        {
            _swipeHandler = swipeHandler;
            _signalBus = signalBus;

            _minimumDistance = inputConfig.MinimumDistance;
            _maximumTime = inputConfig.MaximumTime;
            _directionThreshold = inputConfig.DirectionThreshold;
        }

        public void Initialize()
        {
            _swipeHandler.OnStartTouch += SwipeStart;
            _swipeHandler.OnEndTouch += SwipeEnd;
        }

        private void SwipeStart(Vector2 position, float time)
        {
            _startPosition = position;
            _startTime = time;
        }

        private void SwipeEnd(Vector2 position, float time)
        {
            _endPosition = position;
            _endTime = time;
            DetectSwipe();
        }

        private void DetectSwipe()
        {
            if (Vector3.Distance(_startPosition, _endPosition) >= _minimumDistance &&
                _endTime - _startTime <= _maximumTime)
            {
                Debug.DrawLine(_startPosition, _endPosition, Color.red, 5f);
                Vector3 direction = _endPosition - _startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                SwipeDirection(direction2D);
            }
        }

        private void SwipeDirection(Vector2 direction)
        {
            if (Vector2.Dot(Vector2.up, direction) > _directionThreshold)
            {
                _signalBus.Fire(new SwipedSignal(_startPosition, DirectionEnum.Up));
            }
            else if (Vector2.Dot(Vector2.down, direction) > _directionThreshold)
            {
                _signalBus.Fire(new SwipedSignal(_startPosition, DirectionEnum.Down));
            }
            else if (Vector2.Dot(Vector2.left, direction) > _directionThreshold)
            {
                _signalBus.Fire(new SwipedSignal(_startPosition, DirectionEnum.Left));
            }
            else if (Vector2.Dot(Vector2.right, direction) > _directionThreshold)
            {
                _signalBus.Fire(new SwipedSignal(_startPosition, DirectionEnum.Right));
            }
        }

        public void Dispose()
        {
            _swipeHandler.OnStartTouch -= SwipeStart;
            _swipeHandler.OnEndTouch -= SwipeEnd;
        }

    }
}