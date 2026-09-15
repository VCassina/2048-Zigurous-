using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private BoardBehavior board;
    [SerializeField]
    private CanvasGroup gameOver;

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
    }

    public void GameOver()
    {
        board.enabled = false; // Board n'est plus clickable.
        Debug.Log("Game Over DEPUIS GameOver() !");
        board.enabled = false; // On désactive le board pour arreter les inputs, déjà.
        gameOver.interactable = true;  // On rend intarissable l’écran de GameOver qui a toujours été là mais était en alpha 0, ce qu’on change avec la Coroutine qui vient : 
        StartCoroutine(Fade(gameOver, 1f, 1f)); // Animation d’apparition de l’écran objet tout juste ajouté.
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
