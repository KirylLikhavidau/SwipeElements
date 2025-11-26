using Balloons;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class BalloonInstaller : MonoInstaller
    {
        [SerializeField] private Canvas _mainCanvas;

        public override void InstallBindings()
        {
            BindBalloonSpawner();
        }

        private void BindBalloonSpawner()
        {
            Container
                .BindInterfacesAndSelfTo<BalloonSpawner>()
                .AsSingle()
                .WithArguments(_mainCanvas);
        }
    }
}