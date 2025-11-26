using Balloons;
using Configs;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pools
{
    public class BalloonPool
    {
        private readonly DiContainer _diContainer;
        private PrefabsConfig _prefabsConfig;
        private RectTransform _poolContainer;
        
        private System.Random _random;
        private List<Balloon> _inactiveBalloons;

        private BalloonPool(DiContainer diContainer, PrefabsConfig prefabsConfig, RectTransform poolContainer)
        {
            _diContainer = diContainer;
            _prefabsConfig = prefabsConfig;
            _random = new System.Random();
            _inactiveBalloons = new List<Balloon>();
            _poolContainer = poolContainer;
        }

        public Balloon Spawn()
        {
            if (_inactiveBalloons.Count > 0)
            {
                Balloon balloon = _inactiveBalloons[0];
                balloon.gameObject.SetActive(true);
                _inactiveBalloons.Remove(balloon);
                return balloon;
            }
            else
            {
                Balloon prefab = _prefabsConfig.BalloonPrefabs[_random.Next(0, _prefabsConfig.BalloonPrefabs.Length)];
                return _diContainer.InstantiatePrefabForComponent<Balloon>(prefab, _poolContainer);
            }
        }

        public void Despawn(Balloon balloon)
        {
            if (balloon != null)
            {
                balloon.gameObject.SetActive(false);
                _inactiveBalloons.Add(balloon);
            }
        }
    }
}