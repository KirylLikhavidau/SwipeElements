using Cysharp.Threading.Tasks;
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
        //private List<UniTask> _deactivationTasks = new List<UniTask>();

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

        //private async void AwaitDeactivations(BlocksDeactivatingSignal signalArguments)
        //{
        //    _deactivationTasks.Add(Deactivate(signalArguments.Sections));

        //    if (_deactivationTasks.Count == 0)
        //    {
        //        await UniTask.WhenAll(_deactivationTasks);
        //        _deactivationTasks = new List<UniTask>();
        //    }
        //}

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

            _signalBus.Fire<BlocksDeactivatedSignal>();
        }
    }
}