using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public BoardBehavior board;
    [SerializeField]
    private CanvasGroup gameOver;
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

    public void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        gameOver.alpha = 0f; // On voit plus l’écran de GameOver si jamais on le voyait.
        gameOver.interactable = false; // Ne peut plus recevoir d’input.
        SaveBestScores();
        board.ClearBoard(); // On nettoie tout.
        board.CreateTile(); // Création de nos deux tiles en passant par board qu’on a importé.
        board.CreateTile();
        board.enabled = true; // Et oui, va falloir mettre enabled, on va désactiver la board en cas de game over, donc là on s’assure de le remettre.
        SetScore(0); // On réinitialise le score au début d'une nouvelle partie.
        bestScore.text = LoadBestScores().score.ToString(); // On met à jour l'affichage du meilleur score.

        // Et de son chrono en cuttant aprés 3 décimales en adaptant au format mm:ss : 
        float bestTime = LoadBestScores().time; // On récupère le meilleur chrono enregistré.
        int minutes = (int)(bestTime / 60);
        int secondes = (int)(bestTime % 60);
        // Application à l'affichage du meilleur chrono dans l'objet correspondant.
        bestChrono.text = $"{minutes:D2}:{secondes:D2}";

        SetChrono(0f); // On réinitialise le chrono au début d'une nouvelle partie.
        board.isNewGamePaused = true; // On met le jeu en pause de début pour la nouvelle partie.
        lastRunTimer = 0f; // On réinitialise le timer de la dernière partie.
    }

    public void GameOver()
    {
        board.enabled = false; // Board n'est plus clickable.
        board.enabled = false; // On désactive le board pour arreter les inputs, déjà.
        gameOver.interactable = true;  // On rend intarissable l’écran de GameOver qui a toujours été là mais était en alpha 0, ce qu’on change avec la Coroutine qui vient : 
        StartCoroutine(Fade(gameOver, 1f, 1f)); // Animation d’apparition de l’écran objet tout juste ajouté.
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
        int bestScore = LoadBestScores().score;
        if (score > bestScore) // Si le score actuel est supérieur au meilleur score enregistré,
        {
            ScoreData currentScoreData = new ScoreData(); // Nouvelle instance de ScoreData pour stocker les informations de score.
            currentScoreData.score = score; // On met à jour le meilleur score dans l'objet ScoreData.
            currentScoreData.time = timer.chrono;
            PlayerPrefs.SetString("ScoreData", JsonUtility.ToJson(currentScoreData)); // On l'enregistre dans les PlayerPrefs.
            Debug.Log("Saved ScoreData: " + JsonUtility.ToJson(currentScoreData));
        }
    }

    private ScoreData LoadBestScores()
    {
        return JsonUtility.FromJson<ScoreData>(PlayerPrefs.GetString("ScoreData", JsonUtility.ToJson(new ScoreData()))); 
        // On retourne le meilleur score enregistré sous format ScoreData.
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

    // Premier outil de développement, ici pour supprimer les informations stockées dans PlayerPrefs.
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs deleted !");
    }

    // Deuxieme outil pour afficher en Debug.Log ce qu'on obtiendra dans LoadBestScores().
    public void ShowLoadBestScores()
    {
        Debug.Log("Datas are : " + LoadBestScores().score + " - " + LoadBestScores().time);
    }
    // Même logique que pour l’apparition, mais ici concernant l'alpha.
    private IEnumerator Fade(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    // Nouvelle classe pour stocker les informations de score.
    [System.Serializable]
    public class ScoreData
    {
        public int score;
        public float time;
    }
}

