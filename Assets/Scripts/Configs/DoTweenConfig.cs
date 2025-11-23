using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "DoTweenConfig", menuName = "Configs/DoTweenConfig")]
    public class DoTweenConfig : ScriptableObject
    {
        [field: SerializeField] public float SwappingBlocksDurationInSec { get; private set; }
    }
}