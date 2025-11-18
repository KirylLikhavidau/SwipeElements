using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private InputConfig _inputConfig;
    
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
            .DeclareSignal<SwipedUpSignal>();
        Container
            .DeclareSignal<SwipedDownSignal>();
        Container
            .DeclareSignal<SwipedLeftSignal>();
        Container
            .DeclareSignal<SwipedRightSignal>();
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
    }

    private void BindInstallerInterfaces()
    {
        Container
            .BindInterfacesTo<BootstrapInstaller>()
            .FromInstance(this)
            .NonLazy();
    }
}
