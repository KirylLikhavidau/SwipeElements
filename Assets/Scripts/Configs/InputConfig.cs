using UnityEngine;

[CreateAssetMenu(fileName = "InputConfig", menuName = "Configs/InputConfig")]
public class InputConfig : ScriptableObject
{
    [field: SerializeField] public float MinimumDistance {  get; private set; }
    [field: SerializeField] public float MaximumTime { get; private set; }
    [field: SerializeField, Range(0f, 1f)] public float DirectionThreshold { get; private set; }
}