using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enums;
using Grid.Components;
using Signals;
using System;
using UnityEngine;
using Zenject;

namespace Grid
{
    public class GridBlocksMover : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        private GridInitializer _gridInitializer;
        private DoTweenConfig _doTweenConfig;

        private bool _isMoving = false;

        public GridBlocksMover(GridInitializer gridInitializer, SignalBus signalBus, DoTweenConfig doTweenConfig)
        {
            _signalBus = signalBus;
            _gridInitializer = gridInitializer;
            _doTweenConfig = doTweenConfig;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<SectionDetectedSignal>(TryMove);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<SectionDetectedSignal>(TryMove);
        }

        private void TryMove(SectionDetectedSignal signalArguments)
        {
            if (_isMoving)
                return;

            int sectionX = signalArguments.Section.IndexX;
            int sectionY = signalArguments.Section.IndexY;

            switch (signalArguments.Direction)
            {
                case DirectionEnum.Up:
                    if (sectionY + 1 >= _gridInitializer.GridSections.GetLength(1))
                        return;

                    GridSection upSection = _gridInitializer.GridSections[sectionX, sectionY + 1];

                    if (upSection.CurrentBlockType == BlockType.Empty)
                        return;

                    Move(upSection, signalArguments.Section);
                    break;

                case DirectionEnum.Down:
                    if (sectionY - 1 < 0)
                        return;

                    GridSection downSection = _gridInitializer.GridSections[sectionX, sectionY - 1];

                    if (downSection.CurrentBlockType == BlockType.Empty)
                        return;

                    Move(downSection, signalArguments.Section);
                    break;

                case DirectionEnum.Left:
                    if (sectionX - 1 < 0)
                        return;

                    GridSection leftSection = _gridInitializer.GridSections[sectionX - 1, sectionY];

                    Move(leftSection, signalArguments.Section);
                    break;

                case DirectionEnum.Right:
                    if (sectionX + 1 >= _gridInitializer.GridSections.GetLength(0))
                        return;

                    GridSection rightSection = _gridInitializer.GridSections[sectionX + 1, sectionY];

                    Move(rightSection, signalArguments.Section);
                    break;
            }
        }

        private async void Move(GridSection targetSection, GridSection signalSection)
        {
            if (targetSection.IsInAnyProcess || signalSection.IsInAnyProcess) return;

            Sequence sequence = DOTween.Sequence();

            BlockType targetSectionBlockType = targetSection.CurrentBlockType;
            GridBlock targetSectionBlock = targetSection.CurrentBlock;
            Vector3 targetSectionBlockPosition;

            if (targetSectionBlock != null)
                targetSectionBlockPosition = targetSection.CurrentBlock.BlockRectTransform.position;
            else
                targetSectionBlockPosition = Vector3.zero;

            Vector3 signalSectionBlockPosition = signalSection.CurrentBlock.BlockRectTransform.position;

            targetSection.FillSection(signalSection.CurrentBlockType, signalSection.CurrentBlock);
            signalSection.CurrentBlock.BlockRectTransform.SetParent(targetSection.transform);

            if (targetSectionBlock != null)
                sequence.Append(signalSection.CurrentBlock.BlockRectTransform.DOMove(targetSectionBlockPosition, _doTweenConfig.SwappingBlocksDurationInSec));
            else
                sequence.Append(signalSection.CurrentBlock.BlockRectTransform.DOLocalMove(targetSectionBlockPosition, _doTweenConfig.SwappingBlocksDurationInSec));

            signalSection.FillSection(targetSectionBlockType, targetSectionBlock);

            if (targetSectionBlock != null)
            {
                targetSectionBlock.BlockRectTransform.SetParent(signalSection.transform);
                sequence.Join(targetSectionBlock.BlockRectTransform.DOMove(signalSectionBlockPosition, _doTweenConfig.SwappingBlocksDurationInSec));
            }

            if (!_isMoving)
            {
                //Сделать так чтобы не мог взаимодействовать пока идут процессы(нормализация, деактивация)
                _isMoving = true;
                await UniTask.WaitForSeconds(sequence.Duration());
                _isMoving = false;

                _signalBus.Fire(new BlockMovedSignal(targetSection, signalSection));
            }
        }
    }
}