using Cysharp.Threading.Tasks;
using Signals;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Bootstrap
{
    public class SceneLoader : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        private DataSaver _dataSaver;

        private int _firstLevelIndex;
        private int _maxLevelIndex;
        private int _currentActiveLevelIndex;

        private bool _initted;

        public SceneLoader(SignalBus signalBus, int firstLevelIndex, int maxLevelIndex, DataSaver dataSaver)
        {
            _signalBus = signalBus;

            _dataSaver = dataSaver;

            _firstLevelIndex = firstLevelIndex;
            _maxLevelIndex = maxLevelIndex;
            _currentActiveLevelIndex = _firstLevelIndex;
            _initted = false;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<EmptyGridSignal>(() => LoadNextLevelAsync().Forget());
            _signalBus.Subscribe<NextButtonClickedSignal>(() => LoadNextLevelAsync().Forget());
            _signalBus.Subscribe<RestartButtonClickedSignal>(() => ReloadLevelAsync().Forget());
            LoadNextLevelAsync().Forget();
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<EmptyGridSignal>(() => LoadNextLevelAsync().Forget());
            _signalBus.TryUnsubscribe<NextButtonClickedSignal>(() => LoadNextLevelAsync().Forget());
            _signalBus.TryUnsubscribe<RestartButtonClickedSignal>(() => ReloadLevelAsync().Forget());
        }

        private async UniTask LoadNextLevelAsync()
        {
            if (_initted)
            {
                if (_currentActiveLevelIndex + 1 <= _maxLevelIndex)
                    _currentActiveLevelIndex++;
                else
                    _currentActiveLevelIndex = _firstLevelIndex;
            }
            else
            {
                int startLevelIndex = _dataSaver.RequestSceneIndex();
                if (startLevelIndex > 0)
                {
                    _currentActiveLevelIndex = startLevelIndex;
                }

                _initted = true;
            }

            int unloadLevelIndex;
            if (_currentActiveLevelIndex - 1 >= _firstLevelIndex)
                unloadLevelIndex = _currentActiveLevelIndex - 1;
            else
                unloadLevelIndex = _maxLevelIndex;
            
            AsyncOperation operation;

            Scene unloadScene = SceneManager.GetSceneByBuildIndex(unloadLevelIndex);
            if (unloadScene.isLoaded)
            {
                operation = SceneManager.UnloadSceneAsync(unloadScene);
                await operation.ToUniTask();
            }

            operation = SceneManager.LoadSceneAsync(_currentActiveLevelIndex, LoadSceneMode.Additive);
            await operation.ToUniTask();

            _dataSaver.SaveSceneIndex(_currentActiveLevelIndex);
            _dataSaver.SaveGridLayout(null);
        }

        private async UniTask ReloadLevelAsync()
        {
            AsyncOperation operation;

            Scene unloadScene = SceneManager.GetSceneByBuildIndex(_currentActiveLevelIndex);
            if (unloadScene.isLoaded)
            {
                operation = SceneManager.UnloadSceneAsync(unloadScene);
                await operation.ToUniTask();
            }

            operation = SceneManager.LoadSceneAsync(_currentActiveLevelIndex, LoadSceneMode.Additive);
            await operation.ToUniTask();

            _dataSaver.SaveGridLayout(null);
        }
    }
}