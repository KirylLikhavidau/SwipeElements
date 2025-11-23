using Configs;
using Input;
using Signals;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private InputConfig _inputConfig;
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private BlockConfig _blockConfig;
        [SerializeField] private DoTweenConfig _doTweenConfig;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            BindInstallerInterfaces();
            BindSignals();

            BindConfigs();

            BindInput();
        }

        private void BindSignals()
        {
            Container
                .DeclareSignal<SwipedSignal>();
            Container
                .DeclareSignal<SectionDetectedSignal>();
            Container
                .DeclareSignal<BlockMovedSignal>();
            Container
                .DeclareSignal<BlocksDeactivatingSignal>();
            Container
                .DeclareSignal<BlocksDeactivatedSignal>();
            Container
                .DeclareSignal<GridNormalizedSignal>();
        }

        private void BindInput()
        {
            Container
                .BindInterfacesAndSelfTo<SwipeHandler>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<SwipeDetector>()
                .AsSingle();
        }

        private void BindConfigs()
        {
            Container
                .Bind<InputConfig>()
                .FromInstance(_inputConfig)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<GridConfig>()
                .FromInstance(_gridConfig)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<BlockConfig>()
                .FromInstance(_blockConfig)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<DoTweenConfig>()
                .FromInstance(_doTweenConfig)
                .AsSingle()
                .NonLazy();
        }

        private void BindInstallerInterfaces()
        {
            Container
                .BindInterfacesTo<BootstrapInstaller>()
                .FromInstance(this)
                .NonLazy();
        }
    }
}