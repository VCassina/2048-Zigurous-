using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class TimerBehavior : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    public float chrono = 0f;
   // Variable précalculée isNewGame en fonction de l'état du jeu, plus simple à manipuler.
    public bool newGamePause => gameManager != null && gameManager.board.isNewGamePaused;
  
      // Update is called once per frame
    void Update()
    {
        if (!newGamePause)
        {
            chrono += Time.deltaTime;
            gameManager.SetChrono(chrono);
        } 
        else
        {
            chrono = 0f;
        }
    }
}
