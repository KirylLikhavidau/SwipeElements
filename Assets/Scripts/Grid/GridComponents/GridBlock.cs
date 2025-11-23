using UnityEngine;

namespace Grid.Components
{
    public class GridBlock : MonoBehaviour
    {
        [field: SerializeField] public RectTransform BlockRectTransform { get; private set; }
        [field: SerializeField] public Animator BlockAnimator { get; private set; }
    }
}