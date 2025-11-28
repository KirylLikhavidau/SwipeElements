using Bootstrap;
using Configs;
using Input;
using Saving;
using Signals;
using System;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private DataSaver _dataSaverPrefab;
        [SerializeField] private InputConfig _inputConfig;
        [SerializeField] private PrefabsConfig _blockConfig;
        [SerializeField] private DoTweenConfig _doTweenConfig;
        [SerializeField] private BalloonConfig _balloonConfig;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            BindInstallerInterfaces();
            BindSignals();

            BindConfigs();

            BindDataSaver();
            BindInput();

            BindSceneLoader();
        }

        private void BindSceneLoader()
        {
            Container
                .BindInterfacesAndSelfTo<SceneLoader>()
                .AsSingle()
                .WithArguments(1, 3)
                .NonLazy();
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
                .DeclareSignal<GridNormalizedSignal>()
                .OptionalSubscriber();
            Container
                .DeclareSignal<NextButtonClickedSignal>();
            Container
                .DeclareSignal<RestartButtonClickedSignal>();
            Container
                .DeclareSignal<EmptyGridSignal>();
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
                .Bind<PrefabsConfig>()
                .FromInstance(_blockConfig)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<DoTweenConfig>()
                .FromInstance(_doTweenConfig)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<BalloonConfig>()
                .FromInstance(_balloonConfig)
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

        private void BindDataSaver()
        {
            Container
                .Bind<DataSaver>()
                .FromComponentInNewPrefab(_dataSaverPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}