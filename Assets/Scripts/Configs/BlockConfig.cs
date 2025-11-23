using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BlockConfig", menuName = "Configs/BlockConfig")]
    public class BlockConfig : ScriptableObject
    {
        [field: SerializeField] public RectTransform[] BlockPrefabs { get; private set; }
    }
}