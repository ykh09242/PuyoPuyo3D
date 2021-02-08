using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public void RotateBlock()
    {
        if (Playfield.Instance.ActivePuyo == null) return;

        Playfield.Instance.ActivePuyo.RotateRight();
    }

    public void SetHighSpeed()
    {
        if (Playfield.Instance.ActivePuyo == null) return;

        Playfield.Instance.ActivePuyo.MoveDrop();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}