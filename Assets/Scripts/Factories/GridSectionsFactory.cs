using Configs;
using Enums;
using Grid.Components;
using UnityEngine;
using Zenject;

namespace Factories
{
    public class GridSectionsFactory
    {
        private readonly DiContainer _diContainer;
        private GridSection _sectionPrefab;
        private RectTransform[] _blockPrefabs;

        public GridSectionsFactory(DiContainer diContainer, GridConfig gridConfig, BlockConfig blockConfig)
        {
            _diContainer = diContainer;
            _sectionPrefab = gridConfig.SectionPrefab;
            _blockPrefabs = blockConfig.BlockPrefabs;
        }

        public GridSection Create(BlockType type, RectTransform sectionParent)
        {
            GridSection section = _diContainer.InstantiatePrefabForComponent<GridSection>(_sectionPrefab, sectionParent);
            GridBlock block;

            switch (type)
            {
                case BlockType.Fire:
                    block = _diContainer.InstantiatePrefabForComponent<GridBlock>(_blockPrefabs[(int)BlockType.Fire], section.transform);
                    section.FillSection(BlockType.Fire, block);
                    break;
                case BlockType.Water:
                    block = _diContainer.InstantiatePrefabForComponent<GridBlock>(_blockPrefabs[(int)BlockType.Water], section.transform);
                    section.FillSection(BlockType.Water, block);
                    break;
                default:
                    section.SetEmpty();
                    break;
            }

            return section;
        }
    }
}