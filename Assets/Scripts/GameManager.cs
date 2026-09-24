using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
// Ajout de liste.
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public BoardBehavior board;
    [SerializeField]
    private CanvasGroup gameOver;
    [SerializeField]
    private CanvasGroup scoreboard;
    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI bestChrono;
    public TextMeshProUGUI currentScore;
    public int score;
    private float chrono;
    [SerializeField]
    private TextMeshProUGUI currentChrono;
    // Timer de la dernière partie (pour gérer la réinitialisation).
    private float lastRunTimer;
    // Référence au TimerBehavior pour accéder au chrono actuel.
    [SerializeField]
    private TimerBehavior timer;
    [SerializeField]
    private CanvasGroup sideHUD;
    [SerializeField]
    private GameObject sideHUDReturn;
    [SerializeField]
    private GameObject sideHUDBestTries;
    [SerializeField]
    private TextMeshProUGUI bestScoreTEMP;
    [SerializeField]
    private TextMeshProUGUI bestTimeTEMP;
    private List<ScoreData> storedScoreData;

    public void Start()
    {
        NewGame();        
    }

    private void HideCanvasElements(CanvasGroup canvasGroup)
    {
        board.enabled = true; // On désactive temporairement la board pendant le fade.
        canvasGroup.alpha = 0f; // On masque l'élément du canvas.
        canvasGroup.blocksRaycasts = false; // Il ne peut plus recevoir d’input.
        canvasGroup.interactable = false; // On s'assure qu'il n'est pas interactif non plus.
    }
    private void ShowCanvasElements(CanvasGroup canvasGroup)
    {
        board.enabled = false; // On désactive temporairement la board pendant le fade.
        StartCoroutine(Fade(canvasGroup, 1f, 0.5f));
        canvasGroup.blocksRaycasts = true; // Il peut recevoir des inputs.
        canvasGroup.interactable = true; // On s'assure qu'il est interactif.
    }
    public void NewGame()
    {
        HideCanvasElements(gameOver);
        HideCanvasElements(scoreboard);
        sideHUDReturn.SetActive(false); // Masque complètement le GameObject.
        sideHUDBestTries.SetActive(true); // Affiche l'autre.
        SaveBestScores();
        board.ClearBoard(); // On nettoie tout.
        board.CreateTile(); // Création de nos deux tiles en passant par board qu’on a importé.
        board.CreateTile();
        SetScore(0); // On réinitialise le score au début d'une nouvelle partie.
        bestScore.text = LoadBestScores()[0].score.ToString(); // On met à jour l'affichage du meilleur score.

        // Et de son chrono en cuttant aprés 3 décimales en adaptant au format mm:ss : 
        float bestTime = LoadBestScores()[0].time; // On récupère le meilleur chrono enregistré.
        int minutes = (int)(bestTime / 60);
        int secondes = (int)(bestTime % 60);
        // Application à l'affichage du meilleur chrono dans l'objet correspondant.
        bestChrono.text = $"{minutes:D2}:{secondes:D2}"; 

        SetChrono(0f); // On réinitialise le chrono au début d'une nouvelle partie.
        board.isNewGamePaused = true; // On met le jeu en pause de début pour la nouvelle partie.
        lastRunTimer = 0f; // On réinitialise le timer de la dernière partie.
    }

    private void FullfillScoreboard()
    {
        // A venir.
    }

    public void GameOver()
    {
        ShowCanvasElements(gameOver); // On rend l'écran de Game Over visible et interactif.
        board.isNewGamePaused = true; // On met le jeu en pause de début lors du Game Over.
        SaveBestScores(); // On sauvegarde le meilleur score à la fin de la partie.
    }

    private void SetScore(int newScore)
    {
        score = newScore; // On met à jour le score avec la nouvelle valeur.
        currentScore.text = score.ToString(); // On met à jour l'affichage du score actuel.
    }

    private void SaveBestScores()
    {
        ScoreData currentScoreData = new ScoreData();
        currentScoreData.score = score; // On met à jour le meilleur score dans l'objet ScoreData.
        currentScoreData.time = timer.chrono; // On met à jour le temps dans l'objet ScoreData.

        // Ajout de l'objet à la liste des meilleurs scores.
        List<ScoreData> bestScores = LoadBestScores(); // Chargement de la liste.
        bestScores.Add(currentScoreData); // Ajout le ScoreData courant à la liste.

            bestScores.Sort((a, b) => b.score.CompareTo(a.score)); // Tri décroissant en fonction des scores.
            if (bestScores.Count > 5) // Si la liste est supérieure à 5 éléments.
            {
                bestScores = bestScores.GetRange(0, 5); // On ne garde que les 5 meilleurs scores.
            }
        // Puis création d'un objet ScoreDataList contenant la liste, tout est prêt pour l'envoie en save dans les PLayerPrefs.
        ScoreDataList scoreDataList = new ScoreDataList { scores = bestScores }; 
        PlayerPrefs.SetString("ScoreDataList", JsonUtility.ToJson(scoreDataList));
        // Ligne qui signifie : "Pour sauvegarder les données dans les PlayerPrefs, je mets l'objet JSON sous forme de chaîne de caractères.
        // Notre intitulé ScoreDataList, converti via le snippet JsonUtility en JSON, contient les données de scoreDataList.
        Debug.Log("ScoreDataList sauvegardé dans PlayerPrefs : " + JsonUtility.ToJson(scoreDataList));
        Debug.Log("LoadBestScores : " + JsonUtility.ToJson(LoadBestScores()));
        Debug.Log("LoadBestScores index qui doit normalement s'être incrémenté : " + LoadBestScores().Count + "index total : " + LoadBestScores()[0] + "Premier scoring : " + LoadBestScores()[0].score);
    }

    private List<ScoreData> LoadBestScores()
    {
        // Chargement des meilleurs scores depuis les PlayerPrefs au nom de l'intitulé "ScoreDataList" défini dans SaveBestScores, 
        // Et sinon, en générer une nouvelle.
        string jsonContent = PlayerPrefs.GetString("ScoreDataList", JsonUtility.ToJson(new ScoreDataList()));

        // Une nouvelle instance de ScoreDataList est créée et initialisée avec le contenu trouvé dans le JSON, toujours avec l'intitulé "ScoreDataList".
        ScoreDataList scoreDataList = JsonUtility.FromJson<ScoreDataList>(jsonContent);
       
        Debug.Log("Contenu JSON dans PlayerPrefs : " + jsonContent);
        Debug.Log("Contenu JSON converti en ScoreDataList : " + JsonUtility.ToJson(scoreDataList));

        // Retourne la liste des meilleurs scores ou bien la nouvelle vierge.   
        return scoreDataList.scores;
    }

    public void IncreaseScore(int amount) // La fonction appelée par Board.cs pour augmenter le score du joueur.
    {
        SetScore(score + amount);
    }

    public void SetChrono(float newChrono)
    {
        chrono = newChrono;
        // Convertir les secondes en minutes et secondes
        int minutes = (int)(chrono / 60);
        int secondes = (int)(chrono % 60);
        // Afficher au format "mm:ss" avec deux chiffres pour les minutes et les secondes
        currentChrono.text = $"{minutes:D2}:{secondes:D2}";
    }

    public void OpenScoreBoard()
    {
        FullfillScoreboard(); // On met à jour les informations du tableau des meilleurs scores.
        ShowCanvasElements(scoreboard); // On rend l'écran du tableau des meilleurs scores visible et interactif.
        sideHUDReturn.SetActive(true); // Affiche le bouton de retour.
        sideHUDBestTries.SetActive(false); // Masque le bouton des meilleurs essais.
    }

    public void CloseScoreBoard()
    {
        HideCanvasElements(scoreboard);
        sideHUDReturn.SetActive(false); // Masque le bouton de retour.
        sideHUDBestTries.SetActive(true); // Affiche le bouton des meilleurs essais.
    }

    // Premier outil de développement, ici pour supprimer les informations stockées dans PlayerPrefs.
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs deleted !");
    }

    // Deuxieme outil pour afficher en Debug.Log ce qu'on obtiendra dans LoadBestScores().
/*     public void ShowLoadBestScores()
    {
        Debug.Log("Datas are : " + LoadBestScores().score + " - " + LoadBestScores().time);
    } */

    // Même logique que pour l’apparition, mais ici concernant l'alpha.
    private IEnumerator Fade(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.blocksRaycasts = false; // Désactiver temporairement les interactions pendant le fade.
            sideHUD.blocksRaycasts = false; // Désactiver temporairement les interactions du sideHUD pendant le fade.
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = true; // Réactiver les interactions après la fin du fade.
        sideHUD.blocksRaycasts = true; // Réactiver les interactions du sideHUD après la fin du fade.
    }

    // Nouvelle classe pour stocker les informations de score.
    [System.Serializable]
    public class ScoreData
    {
        public int score;
        public float time;
    }

    // Wrapper nécessaire car JsonUtility ne sait pas (dé)sérialiser une liste en tant que racine du JSON.
    [System.Serializable]
    public class ScoreDataList
    {
        public List<ScoreData> scores = new List<ScoreData>();
    }
}

