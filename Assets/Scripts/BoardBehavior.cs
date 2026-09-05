using UnityEngine;
// Script gérant le comportement des listes (tableau dynamique).
using System.Collections.Generic;

public class BoardBehavior : MonoBehaviour
{
    // Référence au composant Grid attaché au même GameObject.
    public GridBehavior grid;
    // Liste dynamique des tuiles présentes sur le plateau.
    private List<TileBehavior> tiles;
    // Référence au prefab de la tuile à instancier.
    // A ajouter dans l'inspecteur.
    public TileBehavior tilePrefab;
    // Référence aux multiples états possibles d'une tuile.
    // Le contenu du dossier est à dropper dans l'inspecteur.
    public TileStates[] tileState;

    // Récupère les coordonnées des cellules du plateau via Grid.
    void Awake()
    {
        grid = GetComponentInChildren<GridBehavior>();
        // Ajouter une première liste (ici de 16, qu'on peut agrandir à souhait si besoin un jour).
        tiles = new List<TileBehavior>(16);
    }

    // Appelera deux fois CreateTile pour générer les deux premières Tiles.
    void Start()
    {
        CreateTile();
        CreateTile();
    }

    // Instantie une nouvelle Tile Prefab sur une cellule vide aléatoire récupéré via Spawn() et GetRandomEmptyCell().
    private void CreateTile()
    {
        // Instancier une tuile, et lui donne une position dans la hiérarchie du Grid, 
        // Ce dernier s'occupera de gérer la position.
        TileBehavior tile = Instantiate(tilePrefab, grid.transform);
        // Accorde à la tuile le premier état et une valeur de 2.
        tile.SetState(tileState[0], 2);
        // Ici, tout ce met en place : 
        // La tuile utilise sa méthode Spawn() rattachée à son script.
        // Transmettant la résultat de la méthode GetRandomEmptyCell(), à savoir une cellule vide aléatoire.
        // La tuile est maintenant positionnée sur une cellule vide aléatoire du plateau 
        // Et associée à un état et à sa cellule.
        tile.Spawn(grid.GetRandomEmptyCell());
    }
}
