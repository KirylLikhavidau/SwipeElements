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
    public class SectionsMatcher : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly DataSaver _dataSaver;

        private GridInitializer _gridInitializer;
        private SectionDeactivator _sectionDeactivator;

        private List<GridSection> _horizontalSections = new List<GridSection>();
        private List<GridSection> _verticalSections = new List<GridSection>();
        private List<GridSection> _matchingSections = new List<GridSection>();

        public SectionsMatcher(GridInitializer gridInitializer, SignalBus signalBus, SectionDeactivator sectionDeactivator, DataSaver dataSaver)
        {
            _signalBus = signalBus;
            _dataSaver = dataSaver;

            _gridInitializer = gridInitializer;
            _sectionDeactivator = sectionDeactivator;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<GridNormalizedSignal>(() => TryGetMatchInAllSections().Forget());
            _signalBus.Subscribe<BlockMovedSignal>((signalArguments) => TryGetMatch(signalArguments).Forget());
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<GridNormalizedSignal>(() => TryGetMatchInAllSections().Forget());
            _signalBus.TryUnsubscribe<BlockMovedSignal>((signalArguments) => TryGetMatch(signalArguments).Forget());
        }

        private async UniTask TryGetMatch(BlockMovedSignal signalArguments)
        {
            if (_sectionDeactivator.IsDeactivatingSmth)
                await UniTask.WaitUntil(() => !_sectionDeactivator.IsDeactivatingSmth);

            List<GridSection> sectionsToDeactivate = new List<GridSection>();

            if (signalArguments.FirstSection.CurrentBlock != null && !signalArguments.FirstSection.IsInAnyProcess)
            {
                List<GridSection> sectionsFromFirstMatch = GetMatch(signalArguments.FirstSection);
                if (sectionsFromFirstMatch != null)
                    sectionsToDeactivate.AddRange(sectionsFromFirstMatch);
            }

            if (signalArguments.SecondSection.CurrentBlock != null && !signalArguments.SecondSection.IsInAnyProcess)
            {
                List<GridSection> sectionsFromSecondMatch = GetMatch(signalArguments.SecondSection);
                if (sectionsFromSecondMatch != null)
                    sectionsToDeactivate.AddRange(sectionsFromSecondMatch);
            }

            if (sectionsToDeactivate.Count > 0)
            {
                _signalBus.Fire(new BlocksDeactivatingSignal(sectionsToDeactivate));
            }
            else
            {
                _signalBus.Fire(new BlocksDeactivatedSignal());
            }
        }

        private async UniTask TryGetMatchInAllSections()
        {
            if (_sectionDeactivator.IsDeactivatingSmth)
                await UniTask.WaitUntil(() => !_sectionDeactivator.IsDeactivatingSmth);

            List<GridSection> sectionsToDeactivate = new List<GridSection>();
            List<GridSection> sectionsFromMatch = null;

            for (int y = 0; y < _gridInitializer.GridSections.GetLength(1); y++)
                for (int x = 0; x < _gridInitializer.GridSections.GetLength(0); x++)
                {
                    if (_gridInitializer.GridSections[x, y].CurrentBlock != null && !_gridInitializer.GridSections[x, y].IsInAnyProcess)
                    {
                        sectionsFromMatch = GetMatch(_gridInitializer.GridSections[x, y]);
                        if (sectionsFromMatch != null)
                            sectionsToDeactivate.AddRange(sectionsFromMatch);
                    }
                }

            if (sectionsToDeactivate.Count > 0)
                _signalBus.Fire(new BlocksDeactivatingSignal(sectionsToDeactivate));
            else
                _dataSaver.SaveGridLayout(_gridInitializer.GridSections);
        }

        private void CheckInOneDirection(DirectionEnum direction, BlockType sectionBlockType, int sectionX, int sectionY,
            bool traverseMode = false, int traverseSectionIndex = -1, bool flipSectionsLists = false)
        {
            int gridDimension;

            switch (direction)
            {
                case DirectionEnum.Left:
                case DirectionEnum.Right:
                    gridDimension = _gridInitializer.GridSections.GetLength(0);
                    break;
                case DirectionEnum.Up:
                case DirectionEnum.Down:
                    gridDimension = _gridInitializer.GridSections.GetLength(1);
                    break;
                default:
                    Debug.LogWarning("Direction is wrong(null)!");
                    gridDimension = 0;
                    break;
            }

            for (int offset = 1; offset < gridDimension; offset++)
            {
                int xOrY;

                switch (direction)
                {
                    case DirectionEnum.Left:
                        xOrY = sectionX - offset;
                        break;
                    case DirectionEnum.Right:
                        xOrY = sectionX + offset;
                        break;
                    case DirectionEnum.Up:
                        xOrY = sectionY + offset;
                        break;
                    case DirectionEnum.Down:
                        xOrY = sectionY - offset;
                        break;
                    default:
                        Debug.LogWarning("Direction is wrong(null)!");
                        xOrY = -1;
                        break;
                }

                if (xOrY < 0 || xOrY >= gridDimension) { break; }

                if (traverseMode)
                {
                    if (flipSectionsLists)
                    {
                        if (_gridInitializer.GridSections[xOrY, _verticalSections[traverseSectionIndex].IndexY].CurrentBlockType == sectionBlockType)
                            _horizontalSections.Add(_gridInitializer.GridSections[xOrY, _verticalSections[traverseSectionIndex].IndexY]);
                        else
                            break;
                    }
                    else
                    {
                        if (_gridInitializer.GridSections[_horizontalSections[traverseSectionIndex].IndexX, xOrY].CurrentBlockType == sectionBlockType)
                            _verticalSections.Add(_gridInitializer.GridSections[_horizontalSections[traverseSectionIndex].IndexX, xOrY]);
                        else
                            break;
                    }
                }
                else
                {
                    if (flipSectionsLists)
                    {
                        if (_gridInitializer.GridSections[sectionX, xOrY].CurrentBlockType == sectionBlockType)
                            _verticalSections.Add(_gridInitializer.GridSections[sectionX, xOrY]);
                        else
                            break;
                    }
                    else
                    {
                        if (_gridInitializer.GridSections[xOrY, sectionY].CurrentBlockType == sectionBlockType)
                            _horizontalSections.Add(_gridInitializer.GridSections[xOrY, sectionY]);
                        else
                            break;
                    }
                }
            }
        }

        private List<GridSection> GetMatch(GridSection movedSection)
        {
            _matchingSections.Clear();
            _horizontalSections.Clear();
            _verticalSections.Clear();

            BlockType movedSectionType = movedSection.CurrentBlockType;

            _horizontalSections.Add(movedSection);

            CheckInOneDirection(DirectionEnum.Left, movedSectionType, movedSection.IndexX, movedSection.IndexY);
            CheckInOneDirection(DirectionEnum.Right, movedSectionType, movedSection.IndexX, movedSection.IndexY);

            if (_horizontalSections.Count >= 3)
            {
                _matchingSections.AddRange(_horizontalSections);

                for (int i = 0; i < _horizontalSections.Count; i++)
                {
                    CheckInOneDirection(DirectionEnum.Up, movedSectionType, movedSection.IndexX, movedSection.IndexY,
                        traverseMode: true, traverseSectionIndex: i);
                    CheckInOneDirection(DirectionEnum.Down, movedSectionType, movedSection.IndexX, movedSection.IndexY,
                        traverseMode: true, traverseSectionIndex: i);

                    if (_verticalSections.Count < 2)
                    {
                        _verticalSections.Clear();
                    }
                    else
                    {
                        _matchingSections.AddRange(_verticalSections);
                        break;
                    }
                }
            }

            if (_matchingSections.Count >= 3)
            {
                return _matchingSections;
            }
            else
            {
                _horizontalSections.Clear();
                _verticalSections.Clear();
                _verticalSections.Add(movedSection);
            }

            CheckInOneDirection(DirectionEnum.Up, movedSectionType, movedSection.IndexX, movedSection.IndexY, flipSectionsLists: true);
            CheckInOneDirection(DirectionEnum.Down, movedSectionType, movedSection.IndexX, movedSection.IndexY, flipSectionsLists: true);

            if (_verticalSections.Count >= 3)
            {
                _matchingSections.AddRange(_verticalSections);

                for (int i = 0; i < _verticalSections.Count; i++)
                {
                    CheckInOneDirection(DirectionEnum.Left, movedSectionType, movedSection.IndexX, movedSection.IndexY,
                        traverseMode: true, traverseSectionIndex: i, flipSectionsLists: true);
                    CheckInOneDirection(DirectionEnum.Right, movedSectionType, movedSection.IndexX, movedSection.IndexY,
                        traverseMode: true, traverseSectionIndex: i, flipSectionsLists: true);

                    if (_horizontalSections.Count < 2)
                    {
                        _horizontalSections.Clear();
                    }
                    else
                    {
                        _matchingSections.AddRange(_horizontalSections);
                        break;
                    }
                }
            }

            if (_matchingSections.Count >= 3)
            {
                return _matchingSections;
            }

            return null;
        }
    }
}