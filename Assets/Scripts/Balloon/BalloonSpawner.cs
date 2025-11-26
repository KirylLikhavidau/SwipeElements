using Configs;
using Cysharp.Threading.Tasks;
using Pools;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Balloons
{
    public class BalloonSpawner : ITickable
    {
        private BalloonPool _balloonPool;
        private RectTransform _canvasRectTransform;
        private BalloonConfig _balloonConfig;

        private List<Balloon> _activeBalloons;

        public BalloonSpawner(BalloonPool balloonPool, Canvas canvas, BalloonConfig balloonConfig)
        {
            _balloonPool = balloonPool;
            _balloonConfig = balloonConfig;
            _canvasRectTransform = canvas.gameObject.GetComponent<RectTransform>();

            _activeBalloons = new List<Balloon>();
        }

        public void Tick()
        {
            if (_activeBalloons.Count < _balloonConfig.StartCount)
            {
                StartBalloonLifeCycle().Forget();
            }
        }

        private async UniTask StartBalloonLifeCycle()
        {
            bool spawnSideLeft;

            if (Random.Range(0, 2) == 0)
                spawnSideLeft = true;
            else
                spawnSideLeft = false;


            Balloon balloon = _balloonPool.Spawn();

            float heightPosition = Random.Range(-_canvasRectTransform.rect.height / 2, _canvasRectTransform.rect.height / 2);
            float targetHeightPosition = Random.Range(-_canvasRectTransform.rect.height / 2, _canvasRectTransform.rect.height / 2);
            float widthPosition;
            float targetWidthPosition;

            if (spawnSideLeft)
            {
                widthPosition = Random.Range(_canvasRectTransform.rect.width / -2 - _balloonConfig.WidthOffset * 2,
                             _canvasRectTransform.rect.width / -2 - _balloonConfig.WidthOffset);
                targetWidthPosition = Random.Range(_canvasRectTransform.rect.width / 2 + _balloonConfig.WidthOffset,
                             _canvasRectTransform.rect.width / 2 + _balloonConfig.WidthOffset * 2);
            }
            else
            {
                widthPosition = Random.Range(_canvasRectTransform.rect.width / 2 + _balloonConfig.WidthOffset,
                    _canvasRectTransform.rect.width / 2 + _balloonConfig.WidthOffset * 2);
                targetWidthPosition = Random.Range(_canvasRectTransform.rect.width / -2 - _balloonConfig.WidthOffset * 2,
                    -_canvasRectTransform.rect.width / 2 - _balloonConfig.WidthOffset);
            }

            Vector2 startPosition = new Vector2(widthPosition, heightPosition);
            Vector2 targetPosition = new Vector2(targetWidthPosition, targetHeightPosition);

            _activeBalloons.Add(balloon);

            float movingDuration = Random.Range(_balloonConfig.MedianMovingSpeed / 1.5f, _balloonConfig.MedianMovingSpeed * 1.5f);
            balloon.InitVectors(startPosition, targetPosition, movingDuration, _balloonConfig.SinusFrequency, _balloonConfig.SinusAmplitude);

            await UniTask.WaitUntil(
                    () => spawnSideLeft
                        ? balloon.transform.localPosition.x > targetWidthPosition
                        : balloon.transform.localPosition.x < targetWidthPosition,
                    cancellationToken: balloon.GetCancellationTokenOnDestroy()
                );

            _balloonPool.Despawn(balloon);
            _activeBalloons.Remove(balloon);
        }
    }
}