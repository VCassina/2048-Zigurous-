using UnityEngine;

public class CellBehavior : MonoBehaviour
{
    // Référence à la tuile actuellement associée à cette cellule.
    public Vector2Int coordinates { get; set; }
    // Bools précalculés indiquant si la cellule est vide en temps réel.
    public bool empty => tile == null;
    public bool occupied => tile != null;
    // Référence à la tuile actuellement qui viendra s'associér à cette cellule à son Spawn().
    public TileBehavior tile { get; set; }
}
