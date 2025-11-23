using Enums;
using Grid.Components;
using Signals;
using System;
using UnityEngine;
using Zenject;

namespace Grid
{
    public class GridInputProcessor : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        private GridInitializer _gridInitializer;

        public GridInputProcessor(SignalBus signalBus, GridInitializer gridInitializer)
        {
            _signalBus = signalBus;
            _gridInitializer = gridInitializer;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<SwipedSignal>(ProcessInput);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<SwipedSignal>(ProcessInput);
        }

        private void ProcessInput(SwipedSignal signalArguments)
        {
            Vector2 movingDirection;

            switch (signalArguments.Direction)
            {
                case DirectionEnum.Up:
                    movingDirection = Vector2.up;
                    break;
                case DirectionEnum.Down:
                    movingDirection = Vector2.down;
                    break;
                case DirectionEnum.Left:
                    movingDirection = Vector2.left;
                    break;
                case DirectionEnum.Right:
                    movingDirection = Vector2.right;
                    break;
            }

            Collider2D hitCollider = Physics2D.OverlapPoint(signalArguments.StartSwipePosition);

            if (hitCollider != null)
            {
                if (hitCollider.gameObject.TryGetComponent(out GridSection section))
                {
                    if (section.CurrentBlockType != BlockType.Empty)
                    {
                        _signalBus.Fire(new SectionDetectedSignal(section, signalArguments.Direction));
                    }
                }
            }
        }
    }
}