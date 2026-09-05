using UnityEngine;
// TMTPro pour gérer les fonts.
using TMPro;
// Engine.UI pour gérer les composants UI.
using UnityEngine.UI;

public class TileBehavior : MonoBehaviour
{
    // Variables à utiliser : 
    // Représente l'état actuel de la tuile dans une variable de type TileState.
    public TileStates state { get; private set; }
    // Représente la cellule à laquelle cette tuile est a rattacher.
    public CellBehavior cell { get; private set; }
    // Le score de la tuile qu'on implantera automatiquement.
    private int score { get; set; }
    // Référence au composant TextMeshPro pour afficher le score de la tuile.
    private TextMeshProUGUI text;
    // Référence au composant Image pour afficher le fond de la tuile.
    private Image backgroundImage;

    // Répond aux attributions de Text et d'IMG à afficher selon les Prefab de TileStates.
    void Awake()
    {
        backgroundImage = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Applique le statut donné par un parent.
    // L'état du pion est configuré ici.
    public void SetState(TileStates state, int number)
    {
        // Ajouter l'état reçu à la tuile instantiée (l'objet actuel).
        this.state = state;
        this.score = number;
        // Mise à l'alpha 255 (souvent d'origine inconnue).
        // Tout en appliquant la couleur.
        Color bgColor = state.backgroundColor;
        bgColor.a = 1f;
        backgroundImage.color = bgColor;
        // Mettre à jour le texte affiché avec le nouveau score qui est convenablement converti en chaîne.
        text.text = number.ToString();
        text.color = state.textColor;
    }

    // Fait apparaître la tuile avec l'état initial et s'associe à la cellule transmise en paramètre par le parent.
    public void Spawn(CellBehavior cell)
    {
        // Vérification si la cellule transmise en paramètre n'est pas nulle avant de l'associer.
        if (cell == null)
        {
            Debug.LogWarning("Attempted to spawn tile in a null cell.");
        }
        else {
            // Fonctionne ! Il a bien trouvé une cellule valide et aléatoire pour s'associer.
            Debug.Log("Spawning tile in cell at coordinates: " + cell.coordinates);
        }

        // Associer cette tuile à la cellule transmise en paramètre.
        // Cela evite d'avoir à déclarer this.tile = tile dans la cellule sous conditionnement d'avoir déjà une tuile associée.
        // Ce qui compliquerait la gestion des références croisées entre les tuiles et les cellules.
        // this (tile en cours) dispose désormais d'une référence "cell" qui pointe vers cell.
        this.cell = cell;
        // this (tile en cours) dispose désormais d'une référence "cell.tile" qui pointe vers elle-même.
        // Et vérification si la cellule associée n'est pas nulle avant de mettre à jour sa référence vers cette tuile.
        if (this.cell != null)
        {
            this.cell.tile = this;
        }
        // Et application de la position de la tuile sur la cellule associée.
        transform.position = cell.transform.position;
    }
}
