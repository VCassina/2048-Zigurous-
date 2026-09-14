using UnityEngine;
// TMTPro pour gérer les fonts.
using TMPro;
// Engine.UI pour gérer les composants UI.
using UnityEngine.UI;
// System.Collections pour utiliser IEnumerator et les coroutines.
using System.Collections;

public class TileBehavior : MonoBehaviour
{
    // Variables à utiliser : 
    // Représente l'état actuel de la tuile dans une variable de type TileState.
    public TileStates state { get; private set; }
    // Tableau contenant tous les états possibles d'une tuile.
    public TileStates[] TileStates;
    // Représente la cellule à laquelle cette tuile est a rattacher.
    public CellBehavior cell { get; private set; }
    // Le score de la tuile qu'on implantera automatiquement.
    public int score { get; set; }
    // Référence au composant TextMeshPro pour afficher le score de la tuile.
    private TextMeshProUGUI text;
    // Référence au composant Image pour afficher le fond de la tuile.
    private Image backgroundImage;
    // Initialiser le temps écoulé et la durée de l'animation.
    private float elipsed;
    public float duration;

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

        // Applaiquer la couleur.
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

    // Et enfin la fonction de mouvement individuel.
    public void MoveTo(CellBehavior cell)
    {
        // Libérer l'ancienne cellule pour ne pas la laisser "occupied" fantôme.
        if (this.cell != null)
        {
            this.cell.tile = null;
        }
        // Même code que Spawn(), on doit prendre la cellule transmise en paramètre et mettre à jour les références croisées.
        this.cell = cell;
        if (this.cell != null)
        {
            this.cell.tile = this;
        }
        StartCoroutine(Animate(cell.transform.position));
    }

    // Fonction pour gérer la fusion d'une tuile vers une cellule cible.
    public void MergeTo(CellBehavior cell)
    {
        // Similaire à MoveTo(), mais après l'animation, la tuile sera détruite.
        if (this.cell != null) // Si nous recevons bien une cellule.
        {
            this.cell.tile = null; // Le pointeur vers la tuile est libéré.
        }
        this.cell = null;
        // Lancer l'animation de notre tuile actuellement en cours vers la cellule cible.
        StartCoroutine(Animate(cell.transform.position));
        // Après l'animation, la tuile sera détruite car nous sommes encore en train de fusionner.
        Destroy(gameObject, duration);
    }

    // Récupération de l'index de l'état actuel de la tuile.
    public int IndexOf(TileStates state)
    {
        Debug.Log("TileStates.Length: " + TileStates.Length);
        // Parcourir le tableau des états de la tuile.
        for (int i = 0; i < TileStates.Length; i++)
        {
            Debug.Log("Checking TileStates at index " + i + ": " + TileStates[i]);
            // Pour trouver l'index correspondant à l'état actuel et le retourner.
            if (TileStates[i] == state)
            {
                return i;
            }
        }
        return -1; // Retourne -1 si l'état actuel n'est pas trouvé, ce qui n'est pas normal.
    }

    // Un IEnumerator est un type utilisé pour les coroutines dans Unity, permettant d'animer des objets sur plusieurs frames.
    // Cependant rien n'est renvoyé directement par la coroutine, elle est simplement exécutée sur plusieurs frames.
    public IEnumerator Animate(Vector3 to)
    {
        // Stocker la position de départ de l'object tuile avant de commencer l'animation.
        Vector3 from = transform.position;
        // Boucle d'animation jusqu'à ce que le temps écoulé atteigne la durée.
        elipsed = 0f;
        while (elipsed < duration)
        {
           // Transformation avec Lerp entre la position de départ et la position cible en fonction du temps écoulé.
           transform.position = Vector3.Lerp(from, to, elipsed / duration);
           // Mettre à jour le temps écoulé.
           elipsed += Time.deltaTime;
           // Attendre la prochaine frame, concept de coroutine dans Unity.
           yield return null;
        }
        // S'assurer que la position finale est exactement celle souhaitée, même si la boucle d'animation n'a pas atteint exactement la fin.
        transform.position = to;
    }
}
