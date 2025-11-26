using UnityEngine;
using Balloons;

namespace Configs
{
    [CreateAssetMenu(fileName = "PrefabsConfig", menuName = "Configs/PrefabsConfig")]
    public class PrefabsConfig : ScriptableObject
    {
        [field: SerializeField] public RectTransform[] BlockPrefabs { get; private set; }
        [field: SerializeField] public Balloon[] BalloonPrefabs { get; private set; }
    }
}