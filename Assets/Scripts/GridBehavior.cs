using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GridBehavior : MonoBehaviour
{
    // Références vers les lignes et les cellules du plateau.
    public RowBehavior[] rows;
    public CellBehavior[] cells;

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
        // Mémoriser le point de départ pour s'assurer d'avoir parcouru toutes les cellules au maximum une fois.
        int startingIndex = index;
        // Tant que la cellule actuelle est occupée, [...]
        int safetyCounter = 0; // Limite de sécurité
        const int MAX_ITERATIONS = 100; // Nombre maximal d'itérations
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
            safetyCounter++;
            if (safetyCounter >= MAX_ITERATIONS)
            {
                return null;
            }
        } 
         // Sortir de la boucle signifie qu'une cellule vide a été trouvée, il faut la renvoyer.
         return cells[index];
    } 

    // Possibilité de sélectionner n'importe quelle cellule en fonction de ses coordonnées.
    public CellBehavior GetCell(Vector2Int coordinates)
    {
        if (coordinates.x >= 0 && coordinates.x < rows[0].cells.Length && coordinates.y >= 0 && coordinates.y < rows.Length)
        // Renvoie d'une cellule en fonction des coordonnées fournies.
        {
            return rows[coordinates.y].cells[coordinates.x];
        }
        else
        {
            return null;
        }
        
    }
    
    public CellBehavior GetAdjacentCell(CellBehavior cell, Vector2Int direction) 
    {
        if (cell == null)
        {
            return null;
        }
        // Récupération des coordonnées de la cellule.
        Vector2Int coordinates = cell.coordinates;
        // Calcul des coordonnées de la cellule adjacente en fonction de la direction spécifiée.
        coordinates.x += direction.x;
        // Inverser la direction sur l'axe Y pour correspondre à la logique du plateau.
        coordinates.y -= direction.y;
        return GetCell(new Vector2Int(coordinates.x, coordinates.y));
    }

    public int GetOccupiedTileCount()
    {
        int count = 0;

        foreach (var row in rows)
        {
            foreach (var cell in row.cells)
            {
                if (cell.occupied)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
