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
    public TextMeshProUGUI currentScore;
    public int score;
    private float chrono;
    [SerializeField]
    private TextMeshProUGUI currentChrono;

    public void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        gameOver.alpha = 0f; // On voit plus l’écran de GameOver si jamais on le voyait.
        gameOver.interactable = false; // Ne peut plus recevoir d’input.

        board.ClearBoard(); // On nettoie tout.
        board.CreateTile(); // Création de nos deux tiles en passant par board qu’on a importé.
        board.CreateTile();
        board.enabled = true; // Et oui, va falloir mettre enabled, on va désactiver la board en cas de game over, donc là on s’assure de le remettre.
        SetScore(0); // On réinitialise le score au début d'une nouvelle partie.
        bestScore.text = LoadBestScores().ToString(); // On met à jour l'affichage du meilleur score.
        SetChrono(0f); // On réinitialise le chrono au début d'une nouvelle partie.
        board.isNewGamePaused = true; // On met le jeu en pause de début pour la nouvelle partie.
    }

    public void GameOver()
    {
        board.enabled = false; // Board n'est plus clickable.
        Debug.Log("Game Over DEPUIS GameOver() !");
        board.enabled = false; // On désactive le board pour arreter les inputs, déjà.
        gameOver.interactable = true;  // On rend intarissable l’écran de GameOver qui a toujours été là mais était en alpha 0, ce qu’on change avec la Coroutine qui vient : 
        StartCoroutine(Fade(gameOver, 1f, 1f)); // Animation d’apparition de l’écran objet tout juste ajouté.
        board.isNewGamePaused = true; // On met le jeu en pause de début lors du Game Over.
    }

    private void SetScore(int newScore)
    {
        score = newScore; // On met à jour le score avec la nouvelle valeur.
        currentScore.text = score.ToString(); // On met à jour l'affichage du score actuel.
        SaveBestScores();
    }

    private void SaveBestScores()
    {
        int bestScore = LoadBestScores();
        if (score > bestScore) // Si le score actuel est supérieur au meilleur score enregistré,
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore); // On l'enregistre dans les PlayerPrefs.
        }
    }

    private int LoadBestScores()
    {
        return PlayerPrefs.GetInt("BestScore", 0); // On retourne le meilleur score enregistré, ou 0 s'il n'y en a pas.
    }

    public void IncreaseScore(int amount) // La fonction appelée par Board.cs pour augmenter le score du joueur.
    {
        SetScore(score + amount);
    }

    public void SetChrono(float newChrono)
    {
        chrono = newChrono; // On met à jour le chrono avec la nouvelle valeur.
        currentChrono.text = chrono.ToString("F2"); // On met à jour l'affichage du chrono actuel.
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
}
