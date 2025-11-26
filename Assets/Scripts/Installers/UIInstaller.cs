using LevelUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _nextButton;

        public override void InstallBindings()
        {
            BindButtonsHandler();
        }

        private void BindButtonsHandler()
        {
            Container
                .BindInterfacesAndSelfTo<LevelButtonsHandler>()
                .AsSingle()
                .WithArguments(_nextButton, _restartButton);
        }
    }
}