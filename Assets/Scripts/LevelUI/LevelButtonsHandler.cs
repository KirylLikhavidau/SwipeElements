using Signals;
using System;
using UnityEngine.UI;
using Zenject;

namespace LevelUI
{
    public class LevelButtonsHandler : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        private Button _nextButton;
        private Button _restartButton;

        public LevelButtonsHandler(SignalBus signalBus, Button nextButton, Button restartButton)
        {
            _signalBus = signalBus;
            _nextButton = nextButton;
            _restartButton = restartButton;
        }

        public void Initialize()
        {
            _nextButton.onClick.AddListener(OnNextClicked);
            _restartButton.onClick.AddListener(OnRestartClicked);
        }

        public void Dispose()
        {
            _nextButton.onClick.RemoveListener(OnNextClicked);
            _restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        private void OnNextClicked()
        {
            _signalBus.Fire<NextButtonClickedSignal>();
        }

        private void OnRestartClicked()
        {
            _signalBus.Fire<RestartButtonClickedSignal>();
        }
    }
}