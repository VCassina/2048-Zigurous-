using UnityEngine;
// Script gérant le comportement des listes (tableau dynamique).
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Tilemaps;

public class BoardBehavior : MonoBehaviour
{
    // Référence au composant Grid attaché au même GameObject.
    public GridBehavior grid;
    // Liste dynamique des tuiles présentes sur le plateau.
    public List<TileBehavior> tiles;
    // Référence au prefab de la tuile à instancier.
    // A ajouter dans l'inspecteur.
    public TileBehavior tilePrefab;
    // Référence aux multiples états possibles d'une tuile.
    // Le contenu du dossier est à dropper dans l'inspecteur.
    public TileStates[] tileState;
    // Déclaration des valeurs maximales pour les dimensions du plateau.
    public int gridCubicDimension;
    // Récupère les coordonnées des cellules du plateau via Grid.
    // Indique si le jeu attend la fin d'une animation avant de traiter la prochaine entrée.
    private bool waiting = false;
    // Récupérer la durée de l'animation depuis le prefab de la tuile.
    private float animationDuration;
    // Référence au GameManager pour gérer la fin du jeu.
    public GameManager gameManager;
    
    void Awake()
    {
        grid = GetComponentInChildren<GridBehavior>();
        // Ajouter une première liste (ici de 16, qu'on peut agrandir à souhait si besoin un jour).
        tiles = new List<TileBehavior>(16);
        gridCubicDimension = grid.rows.Length;
        animationDuration = tilePrefab.duration;     
    }

    // Appelera deux fois CreateTile pour générer les deux premières Tiles.
    void Start()
    {
        CreateTile();
        CreateTile();
    }

    // Gère les entrées clavier pour déplacer les tuiles sur le plateau.
    void Update()
    {
        // Vérifie si le jeu est en attente d'une animation avant de traiter les entrées clavier. Si waiting est faux, le jeu écoute les entrées.
        if (!waiting){
            if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow))
            {
            // Déplacement vers le haut, commence à la première colonne et se balaye vers le bas.
            // Et se déplace vers le bas. De la première colonne jusqu'à la dernière.
            MoveTiles(Vector2Int.up, 0, 1, 1, 1); 
            }
            else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
            // Déplacement vers la gauche, commence à la deuxième colonne (car la première est inutile, déjà à gauche).
            // Et se déplace vers la droite. De la première ligne jusqu'à la dernière.
            MoveTiles(Vector2Int.left, 1, 0, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
            // Déplacement vers le bas, commence à l'avant dernière ligne et balaye vers le haut.
            // En décrémentant les lignes.
            MoveTiles(Vector2Int.down, 0, gridCubicDimension - 2, 1, -1); 
            }
            else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
            // Déplacement vers la droite, commence à l'avant dernière colonne.
            // Et décrémente les colonnes avant de balayer toutes les cases normalement.
            MoveTiles(Vector2Int.right, gridCubicDimension - 2, 0, -1, 1); 
            }
        }
    }

    // Ordonne la mise en déplacement de toutes les cellules selon la direction spécifiée.
    // startX et startY définissent la cellule de départ pour le parcours.
    // incrementX et incrementY définissent l'incrément pour chaque itération sur les axes X et Y.
    // Le sens est décissif pour déterminer l'ordre de parcours des cellules et éviter les collisions incorrectes, voir le cours.
    void MoveTiles(Vector2Int direction, int startX, int startY, int incrementX, int incrementY)
    {
        // Ajout d'un booléen pour suivre si un mouvement a été effectué.
        bool moved = false;
        // Mise en place d'un safety pour éviter les boucles infinies si les paramètres sont incorrects.
        const int MAX_SAFETY_COUNT = 30; // Limite pour éviter les boucles infinies
        int safetyCounter = 0; // Compteur pour éviter les boucles infinies
        // Ordre de parcours des cellules en fonction de la direction sur l'axe X.
        for (int x = startX; x >= 0 && x < gridCubicDimension; x += incrementX)
        {
            safetyCounter++;
            if (safetyCounter > MAX_SAFETY_COUNT)
            {
                Debug.LogError("Safety limit reached in MoveTiles loop. Check your loop parameters.");
                break;
            }
            {
                // Ordre de parcours des cellules en fonction de la direction spécifiée sur l'axe Y.
                for (int y = startY; y >= 0 && y < gridCubicDimension; y += incrementY)
                {
                // Envoie des coordonnées et récupération de la cellule correspondante.
                Vector2Int coordinates = new Vector2Int(x, y);
                CellBehavior cell = grid.GetCell(coordinates);
                // Pour chaque case, donc, si elles sont occupées, il va falloir les déplacer : 
                    if(cell.occupied) 
                    {
                        // Nouvelle fonction propre aux déplacements individuels.
                        // Elle aura besoin de la cell et sa tuile pointée pour effectuer le déplacement correctement.
                        // Ainsi qu'une direction pour savoir dans quel sens déplacer la tuile.
                        moved |= MoveTile(cell.tile, direction);
                    }       
                }
            }  
        }
        // C'est une fois que tous les déplacements ont été effectués qu'on peut lancer l'animation.
        // Le tout conditionné par le fait qu'un mouvement ait effectivement eu lieu, renvoyant "true" depuis MoveTile.
        if (moved) {
            StartCoroutine(WaitForAnimation());
        }
    }

    private bool MoveTile(TileBehavior tile, Vector2Int direction)
    {
        // Besoin d'avoir la case adjacente dans la direction spécifiée pour savoir où déplacer la tuile et si elle est libre.
        // Pour cela, pause ! C'est à Grid de s'en charger dans une fonction dédiée, par exemple GetAdjacentCell().
        CellBehavior adjacentCell = grid.GetAdjacentCell(tile.cell, direction); 

        // Nous avons besoin d'une cellule tampon pour stocker la prochaine position de la tuile.
        // L'idée est de vérifier si la cellule adjacente est libre, encore et encore jusqu'à sortir du compteur.
        CellBehavior newCell = null;
        int safetyCounter = 0; // Limite de sécurité
        const int MAX_ITERATIONS = 10; // Nombre maximal d'itérations

        // Mise en place du bouclage pour décaler la tuile jusqu'à ce qu'elle ne puisse plus avancer.
        // Soit par rupture de la condition (adjacentCell != null) via occupation de la cellule.
         while(adjacentCell != null && safetyCounter < MAX_ITERATIONS) 
        {
            safetyCounter++;
            if(adjacentCell.occupied)
            {
                // Si la fonction CanMerge renvoie un "true" avec les tuiles concernées, on peut procéder à la fusion.
                if (CanMerge(tile, adjacentCell.tile))
                {
                    MergeTiles(tile, adjacentCell.tile);
                }
                break; // Ne pas oublier de casser la boucle à ce moment là.
            }
            // Sinon la cellule adjacente est libre, on peut déplacer la tuile.
            newCell = adjacentCell;
            // On met à jour la cellule adjacente pour initier un nouveau déplacement.      
            adjacentCell = grid.GetAdjacentCell(adjacentCell, direction); 
        }
       
        if(newCell != null) 
        {
            tile.MoveTo(newCell);
            return true;
        }  
        return false;
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
        CellBehavior emptyCell = grid.GetRandomEmptyCell();

        if (emptyCell == null)
        {
            Destroy(tile.gameObject);
            return;
        }

        tile.Spawn(emptyCell);
        tiles.Add(tile);
    }

    // Va return true si les deux tuiles peuvent fusionner, false sinon.
    private bool CanMerge(TileBehavior a, TileBehavior b)
    {
        return a != null && b != null && a.score == b.score;
    }

    private void MergeTiles(TileBehavior a, TileBehavior b)
    {
        // Destruction de la tuile dans le tableau des tuiles.
        tiles.Remove(a);
        // Appel de la méthode MergeTo sur la tuile à fusionner vers la CELLULE cible, ce qui permettra d'ajouter une animation.
        a.MergeTo(b.cell);
        // Récupération de l'index de l'état actuel de la tuile à fusionner.
        int bNewIndex = b.IndexOf(b.state)+1;

        // Modification du score.
        int newScore = b.score*2;

        // Mise à jour de l'état de la tuile cible avec le nouvel état et score.
        b.SetState(b.TileStates[bNewIndex], newScore);
    }

    // Lance une coroutine pour attendre la fin de l'animation.
    private IEnumerator WaitForAnimation()
    {
        // Remise à true de l'état d'attente pour indiquer que le jeu attend la fin de l'animation.
        // A ce stade, les inputs clavier sont ignorés jusqu'à la fin de l'animation.
        waiting = true;
        // Attente de la durée de l'animation avant de reprendre le contrôle du jeu.
        yield return new WaitForSeconds(animationDuration);
        // Fin de l'attente, le jeu peut maintenant reprendre le traitement des entrées clavier.
        waiting = false;

        // Creation des nouvelles tuiles après le mouvement si le plateau n'est pas plein.
        if (tiles.Count < grid.cells.Length)
        {
            CreateTile();
            Debug.Log("Created a new tile.");
            Debug.Log("Grid occupied cells: " + grid.GetOccupiedTileCount());
            Debug.Log("tiles.Count is :" + tiles.Count);
        }
        // Gestion d'un game over éventuel.
        if (CheckForGameOver())
        {
            Debug.Log("Game Over!");
            gameManager.GameOver();
        }
        else if (CheckForGameOver() == false)
        {
            Debug.Log("Game NOT over!");
        }
    }

    // Vider le plateau.
    public void ClearBoard()
    {
        // Vider les références des cellules aux tuiles.
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }
        foreach (TileBehavior tile in tiles)
        {
            // Destruction des tuiles restantes.
            Destroy(tile.gameObject);
        }
        // Vider la liste des tuiles.
        tiles.Clear(); 
    }

    // Vérifie si le jeu est terminé.
    public bool CheckForGameOver()
    {
        // Tant que les tuiles ne sont pas égales au nombre de cellules, le jeu n'est pas terminé.
        int occupiedTileCount = grid.GetOccupiedTileCount();

        if (occupiedTileCount < grid.cells.Length)
        {
            Debug.Log("Board is not full, game not over.");
            Debug.Log("Occupied tiles: " + occupiedTileCount + ", Grid cells count: " + grid.cells.Length);
            return false; // Le plateau n'est pas plein, donc le jeu n'est pas terminé.
        }

        // Il faut maintenant vérifier chaque cellule occupée directement depuis les cellules du plateau.
        foreach (var cell in grid.cells)
        {
            TileBehavior tile = cell.tile;

            if (tile == null)
            {
                continue;
            }

            CellBehavior up = grid.GetAdjacentCell(cell, Vector2Int.up);
            CellBehavior down = grid.GetAdjacentCell(cell, Vector2Int.down);
            CellBehavior left = grid.GetAdjacentCell(cell, Vector2Int.left);
            CellBehavior right = grid.GetAdjacentCell(cell, Vector2Int.right);
            // Et on vérifie si une fusion est possible avec l'une de ses voisines fraichement récupérées.
            if (up != null && CanMerge(tile, up.tile) || down != null && CanMerge(tile, down.tile) || 
            left != null && CanMerge(tile, left.tile) || right != null && CanMerge(tile, right.tile))
            {
                // Une fusion est possible, donc le jeu n'est pas terminé.
                Debug.Log("Merge possible, game not over.");
                Debug.Log("Because tile at " + tile.cell.transform.position + " can merge with a neighbor.");
                return false;
            }
        }
        // Aucune des conditions précédentes n'a été remplie, ainsi aucune fusion n'est possible sur le tableau plein et le jeu est donc terminé.
        return true;
    }
}
