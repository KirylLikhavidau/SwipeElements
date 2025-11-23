using Grid.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Configs/GridConfig")]
    public class GridConfig : ScriptableObject
    {
        [field: SerializeField] public GridSection SectionPrefab { get; private set; }
        [field: SerializeField] public int Width { get; private set; }
        [field: SerializeField] public int Height { get; private set; }
        [field: SerializeField] public int CellSize { get; private set; }
        [field: SerializeField] public int[] InitialLayout { get; private set; }
    }
}