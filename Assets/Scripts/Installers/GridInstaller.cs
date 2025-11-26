using Configs;
using Grid;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GridInstaller : MonoInstaller
    {
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private RectTransform _gridInitialStartPoint;

        public override void InstallBindings()
        {
            BindConfigs();

            BindGridInitializer();
            BindInputProcessor();
            BindSectionMover();
            BindSectionsMatcher();
            BindDeactivator();
            BindNormalizer();
        }

        private void BindConfigs()
        {
            Container
                .Bind<GridConfig>()
                .FromInstance(_gridConfig)
                .AsSingle()
                .NonLazy();
        }

        private void BindNormalizer()
        {
            Container
                .BindInterfacesAndSelfTo<GridNormalizer>()
                .AsSingle();
        }

        private void BindDeactivator()
        {
            Container
                .BindInterfacesAndSelfTo<SectionDeactivator>()
                .AsSingle();
        }

        private void BindSectionsMatcher()
        {
            Container
                .BindInterfacesAndSelfTo<SectionsMatcher>()
                .AsSingle();
        }

        private void BindSectionMover()
        {
            Container
                .BindInterfacesAndSelfTo<GridBlocksMover>()
                .AsSingle();
        }

        private void BindInputProcessor()
        {
            Container
                .BindInterfacesAndSelfTo<GridInputProcessor>()
                .AsSingle();
        }

        private void BindGridInitializer()
        {
            Container
                .BindInterfacesAndSelfTo<GridInitializer>()
                .AsSingle()
                .WithArguments(_gridInitialStartPoint);
        }
    }
}