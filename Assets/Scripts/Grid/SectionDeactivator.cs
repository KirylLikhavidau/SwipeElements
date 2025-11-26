using Cysharp.Threading.Tasks;
using Enums;
using Grid.Components;
using Signals;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Grid
{
    public class SectionDeactivator : IInitializable, IDisposable
    {
        private const string Kill = nameof(Kill);

        private readonly SignalBus _signalBus;
        private GridInitializer _gridInitializer;

        private SectionDeactivator(SignalBus signalBus, GridInitializer gridInitializer)
        {
            _signalBus = signalBus;
            _gridInitializer = gridInitializer;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<BlocksDeactivatingSignal>((signalArguments) => Deactivate(signalArguments.Sections));
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<BlocksDeactivatingSignal>((signalArguments) => Deactivate(signalArguments.Sections));
        }

        private async void Deactivate(List<GridSection> gridSections)
        {
            List<UniTask> tasks = new List<UniTask>();

            for (int i = 0; i < gridSections.Count; i++)
            {
                gridSections[i].IsInAnyProcess = true;

                Animator blockAnimator = gridSections[i].CurrentBlock.BlockAnimator;
                blockAnimator.SetTrigger(Kill);
                BlockDestroyAnimatorState behaviour = blockAnimator.GetBehaviour<BlockDestroyAnimatorState>();
                if (behaviour != null)
                    tasks.Add(UniTask.WaitUntil(() => behaviour.DestroyEnded));
            }

            await UniTask.WhenAll(tasks);

            for (int i = 0; i < gridSections.Count; i++)
            {
                if (gridSections[i].CurrentBlock != null)
                {
                    gridSections[i].CurrentBlock.gameObject.SetActive(false);
                    gridSections[i].SetEmpty();
                }

                gridSections[i].IsInAnyProcess = false;
            }

            if (GridIsEmpty())
                _signalBus.Fire<EmptyGridSignal>();
            else
                _signalBus.Fire<BlocksDeactivatedSignal>();
        }

        private bool GridIsEmpty()
        {
            for (int y = 1; y < _gridInitializer.GridSections.GetLength(1); y++)
                for (int x = 0; x < _gridInitializer.GridSections.GetLength(0); x++)
                {
                    if (_gridInitializer.GridSections[x, y].CurrentBlockType != BlockType.Empty)
                    {
                        return false;
                    }
                }

            return true;
        }
    }
}