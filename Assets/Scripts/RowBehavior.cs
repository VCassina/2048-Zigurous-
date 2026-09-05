using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RowBehavior : MonoBehaviour
{
    // Déclaration du tableau des cellules de la Row.
     public CellBehavior[] cells;

    // Met en place un tableau regroupant ses cellules enfant.
    void Awake()
    {
        // Récupérer toutes les cellules enfant et les stocker dans un tableau.
        cells = GetComponentsInChildren<CellBehavior>();
    }
}
