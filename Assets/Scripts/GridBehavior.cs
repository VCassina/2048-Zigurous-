using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GridBehavior : MonoBehaviour
{
    // Ici, les tableaux ne seront pas reconnus car
    private RowBehavior[] rows;
    private CellBehavior[] cells;
     // Regroupe les Rows et les Cells dans deux tableaux.
    void Awake()
    {
        // Regroupe les Rows et les Cells dans les tableaux privés.
        rows = GetComponentsInChildren<RowBehavior>();
        cells = GetComponentsInChildren<CellBehavior>();
    }

    // Distribura les coordonnées aux cellules en fonction de l'itération sur les rows et les cells.
    void Start()
    {
        // Compteur pour parcourir les rows.
        for (int y = 0; y < rows.Length; y++)
        {
            // Compteur pour parcourir les cells dans la row actuelle.
            for (int x = 0; x < cells.Length / rows.Length; x++)
            {
                // Assigner les coordonnées (x, y), y en fonction du Row actuel et x en fonction de la position dans la row.
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
                // Ca, c'est nickel.
                // Debug.Log("Setting coordinates for cell at index " + x + " in row " + y);
            }
        }
        
    }

    // Retournera une CELLULE vide aléatoire et renverra true si une cellule vide a été trouvée, null sinon (éviter les crashs).
    // Public car cette méthode doit pouvoir être appelée depuis le Board.
    // Déclaration eponyme pour la syntaxe même si on renverra null ou cells[x,y];
    public CellBehavior GetRandomEmptyCell()
    {
        // Partir d'un index aléatoire dans le tableau.
        int index = Random.Range(0, cells.Length);
        // C'est quoi l'index qu'il a choisi ? C'est bien aléatoire ! :thumbsup:
        // Debug.Log("Random starting index: " + index);
        // Mémoriser le point de départ pour s'assurer d'avoir parcouru toutes les cellules au maximum une fois.
        int startingIndex = index;
        // Tant que la cellule actuelle est occupée, [...]
        while (cells[index].occupied)
        {
            // Il va falloir venir chercher la cellule suivante.
            index++;
            // Si le hasard nous fait arriver au bout, il faut revenir au début du tableau.
            if (index >= cells.Length)
            {
                index = 0;
            }
            // Revenir au point de départ en cas de tour complet, il faut renvoyer que c'est complet (GameOver).
            if (index == startingIndex)
            {
                return null;
            }
        }
         // Sortir de la boucle signifie qu'une cellule vide a été trouvée, il faut la renvoyer.
         return cells[index];
    }
}
