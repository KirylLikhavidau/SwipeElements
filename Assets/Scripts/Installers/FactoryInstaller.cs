using Factories;
using Zenject;

namespace Installers
{
    public class FactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindFactory();
        }

        private void BindFactory()
        {
            Container
                .Bind<GridSectionsFactory>()
                .AsSingle();
        }
    }
}