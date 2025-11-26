using Pools;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class PoolInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform _balloonContainer;

        public override void InstallBindings()
        {
            BindBalloonPool();
        }

        private void BindBalloonPool()
        {
            Container
                .Bind<BalloonPool>()
                .AsSingle()
                .WithArguments(_balloonContainer);
        }
    }
}