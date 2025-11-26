using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BalloonConfig", menuName = "Configs/BalloonConfig")]
    public class BalloonConfig : ScriptableObject
    {
        [field: SerializeField] public int StartCount { get; private set; } = 3;
        [field: SerializeField] public float WidthOffset { get; private set; } = 20f;
        [field: SerializeField] public float MedianMovingSpeed { get; private set; }
        [field: SerializeField] public float SinusAmplitude { get; private set; } = 1f;
        [field: SerializeField] public float SinusFrequency { get; private set; } = 3f;
    }
}