using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enums;
using Grid.Components;
using Signals;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Grid
{
    public class GridNormalizer : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        private GridInitializer _gridInitializer;
        private DoTweenConfig _doTweenConfig;

        private BlockType[,] _normalizedSections;
        private Dictionary<(int x, int y), (int moveX, int moveY)> _instructionsForMoveSections;

        public GridNormalizer(SignalBus signalBus, GridInitializer gridInitializer, DoTweenConfig doTweenConfig)
        {
            _signalBus = signalBus;
            _gridInitializer = gridInitializer;
            _doTweenConfig = doTweenConfig;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<BlocksDeactivatedSignal>(NormalizeGrid);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<BlocksDeactivatedSignal>(NormalizeGrid);
        }

        private void CreateNewNormalizeGrid(int width, int height)
        {
            _normalizedSections = new BlockType[width, height];
            _instructionsForMoveSections = new Dictionary<(int, int), (int, int)>();

            for (int y = 0; y < _normalizedSections.GetLength(1); y++)
                for (int x = 0; x < _normalizedSections.GetLength(0); x++)
                {
                    _normalizedSections[x, y] = _gridInitializer.GridSections[x, y].CurrentBlockType;
                }
        }

        private async void NormalizeGrid()
        {
            Debug.Log("Normalize started");

            Sequence sequence = DOTween.Sequence().SetLink(_gridInitializer.GridSections[0, 0].gameObject, LinkBehaviour.KillOnDestroy);

            CreateNewNormalizeGrid(_gridInitializer.GridSections.GetLength(0), _gridInitializer.GridSections.GetLength(1));
            CalculatePositions();

            for (int y = 1; y < _gridInitializer.GridSections.GetLength(1); y++)
                for (int x = 0; x < _gridInitializer.GridSections.GetLength(0); x++)
                {
                    if (_gridInitializer.GridSections[x, y].CurrentBlockType != BlockType.Empty)
                    {
                        ValueTuple<int, int> movePosition;
                        if (_instructionsForMoveSections.TryGetValue((x, y), out movePosition))
                        {
                            sequence.Join(NormalizeMove(_gridInitializer.GridSections[movePosition.Item1, movePosition.Item2], _gridInitializer.GridSections[x, y]));
                            _gridInitializer.GridSections[x, y].SetEmpty();

                            _gridInitializer.GridSections[movePosition.Item1, movePosition.Item2].IsInAnyProcess = true;
                            _gridInitializer.GridSections[x, y].IsInAnyProcess = true;
                        }
                    }
                }

            await sequence.AsyncWaitForCompletion();

            foreach (var KeyPosition in _instructionsForMoveSections.Keys)
            {
                _gridInitializer.GridSections[KeyPosition.x, KeyPosition.y].IsInAnyProcess = false;
            }

            foreach (var ValuePosition in _instructionsForMoveSections.Values)
            {
                _gridInitializer.GridSections[ValuePosition.moveX, ValuePosition.moveY].IsInAnyProcess = false;
            }

            Debug.Log("Normalize ended");

            _signalBus.Fire<GridNormalizedSignal>();
        }

        private Tween NormalizeMove(GridSection targetSection, GridSection startSection)
        {
            float movingDuration = _doTweenConfig.SwappingBlocksDurationInSec * (startSection.IndexY - targetSection.IndexY);

            targetSection.FillSection(startSection.CurrentBlockType, startSection.CurrentBlock);
            startSection.CurrentBlock.BlockRectTransform.SetParent(targetSection.transform);

            return startSection.CurrentBlock.BlockRectTransform.DOLocalMove(Vector3.zero, movingDuration)
                .SetLink(startSection.CurrentBlock.BlockRectTransform.gameObject, LinkBehaviour.KillOnDestroy);
        }

        private void CalculatePositions()
        {
            for (int y = 0; y < _normalizedSections.GetLength(1); y++)
                for (int x = 0; x < _normalizedSections.GetLength(0); x++)
                {
                    if (_normalizedSections[x, y] != BlockType.Empty)
                    {
                        int[] positionToMove = new int[2] { x, y };
                        MakeStepDown(positionToMove, x, y);
                        if (x == positionToMove[0] && y == positionToMove[1])
                        {
                            continue;
                        }
                        else
                        {
                            _instructionsForMoveSections.Add((x, y), (positionToMove[0], positionToMove[1]));
                            BlockType swipeSection = _normalizedSections[positionToMove[0], positionToMove[1]];
                            _normalizedSections[positionToMove[0], positionToMove[1]] = _normalizedSections[x, y];
                            _normalizedSections[x, y] = swipeSection;
                        }
                    }
                }

        }

        private void MakeStepDown(int[] positionToMove, int xPosition, int yPosition)
        {
            if (yPosition - 1 >= 0)
            {
                if (_normalizedSections[xPosition, yPosition - 1] != BlockType.Empty)
                {
                    positionToMove[0] = xPosition;
                    positionToMove[1] = yPosition;
                }
                else
                {
                    MakeStepDown(positionToMove, xPosition, yPosition - 1);
                }
            }
            else
            {
                positionToMove[0] = xPosition;
                positionToMove[1] = yPosition;
            }
        }
    }
}