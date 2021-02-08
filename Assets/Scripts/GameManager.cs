using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Text ScoreText;
    public Canvas GameOverCanvas;

    public int Score { get; private set; }

    public bool GameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (ScoreText != null)
            ScoreText.text = "Score : 0";
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void PlusScore(int destroyPuyo, int chain)
    {
        Score += destroyPuyo * 10 * chain;
        if (ScoreText != null)
            ScoreText.text = $"Score : {Score}";
        //Debug.Log($"PUYO COUNT : {destroyPuyo} | CHAIN : {chain}");
    }

    public void SetGameOver()
    {
        Debug.LogError("IN");
        GameOver = true;
        if (GameOverCanvas != null)
            GameOverCanvas.enabled = true;
    }
}
