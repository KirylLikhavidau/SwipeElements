using Configs;
using Enums;
using Factories;
using Grid.Components;
using UnityEngine;
using Zenject;

namespace Grid
{
    public class GridInitializer : IInitializable
    {
        public GridSection[,] GridSections { get; private set; }

        private GridSectionsFactory _sectionsFactory;
        private RectTransform _gridInitialStartPoint;
        private int _gridWidth;
        private int _gridHeight;
        private float _cellSize;
        private int[] _initialLayout;

        public GridInitializer(GridConfig gridConfig, GridSectionsFactory sectionsFactory, RectTransform gridInitialStartPoint)
        {
            _gridWidth = gridConfig.Width;
            _gridHeight = gridConfig.Height;
            _cellSize = gridConfig.CellSize;
            _gridInitialStartPoint = gridInitialStartPoint;
            _sectionsFactory = sectionsFactory;

            _initialLayout = new int[gridConfig.InitialLayout.Length];
            for (int i = 0; i < gridConfig.InitialLayout.Length; i++)
            {
                _initialLayout[i] = gridConfig.InitialLayout[i];
            }

            GridSections = new GridSection[_gridWidth, _gridHeight];

        }

        public void Initialize()
        {
            _gridInitialStartPoint.transform.localPosition = new Vector3(((_gridWidth-1) * _cellSize)/2 * -1, _gridInitialStartPoint.localPosition.y, 0);
            
            if (_initialLayout != null)
            {
                if (_initialLayout.Length == _gridWidth * _gridHeight)
                {
                    ProcessInitialLayout(_initialLayout);
                }
                else
                {
                    int[] newInitialLayout = new int[_gridWidth * _gridHeight];
                    
                    for (int i = 0; i < newInitialLayout.Length; i++)
                    {
                        if (i < _initialLayout.Length)
                        {
                            newInitialLayout[i] = _initialLayout[i];
                        }
                        else
                        {
                            newInitialLayout[i] = -1;
                        }
                    }

                    ProcessInitialLayout(newInitialLayout);
                }
            }
            else
            {
                float xOffset = 0;
                float yOffset = 0;

                for (int y = 0; y < _gridHeight; y++)
                {
                    for (int x = 0; x < _gridWidth; x++)
                    {
                        GridSection section;
                        section = _sectionsFactory.Create(BlockType.Fire, _gridInitialStartPoint);
                        ((RectTransform)section.transform).localPosition = new Vector2(xOffset, yOffset);
                        GridSections[x, y] = section;
                        xOffset += _cellSize;
                    }
                    xOffset = 0;
                    yOffset += _cellSize;
                }
            }
        }

        private void ProcessInitialLayout(int[] layout)
        {
            float xOffset = 0;
            float yOffset = 0;
            int layoutIndex = 0;

            for (int y = 0; y < _gridHeight; y++)
            {
                for (int x = 0; x < _gridWidth; x++)
                {
                    GridSection section;

                    switch (layout[layoutIndex])
                    {
                        case (int)BlockType.Fire:
                            section = _sectionsFactory.Create(BlockType.Fire, _gridInitialStartPoint);
                            ((RectTransform)section.transform).localPosition = new Vector2(xOffset, yOffset);
                            section.SetIndex(x, y);
                            GridSections[x, y] = section;
                            break;
                        case (int)BlockType.Water:
                            section = _sectionsFactory.Create(BlockType.Water, _gridInitialStartPoint);
                            ((RectTransform)section.transform).localPosition = new Vector2(xOffset, yOffset);
                            section.SetIndex(x, y);
                            GridSections[x, y] = section;
                            break;
                        default:
                            section = _sectionsFactory.Create(BlockType.Empty, _gridInitialStartPoint);
                            ((RectTransform)section.transform).localPosition = new Vector2(xOffset, yOffset);
                            section.SetIndex(x, y);
                            GridSections[x, y] = section;
                            break;
                    }
                    layoutIndex++;
                    xOffset += _cellSize;
                }
                xOffset = 0;
                yOffset += _cellSize;
            }
        }
    }
}