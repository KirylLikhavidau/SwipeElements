using Grid;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GridInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform _gridInitialStartPoint;

        public override void InstallBindings()
        {
            BindGridInitializer();
            BindInputProcessor();
            BindSectionMover();
            BindSectionsMatcher();
            BindDeactivator();
            BindNormalizer();
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