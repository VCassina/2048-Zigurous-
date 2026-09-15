using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private BoardBehavior board;

    public void NeWGame()
    {
        
    }

    public void GameOver()
    {
        // Board n'est plus clickable.
        board.enabled = false;
        Debug.Log("Game Over DEPUIS GameOver() !");

    }
}
