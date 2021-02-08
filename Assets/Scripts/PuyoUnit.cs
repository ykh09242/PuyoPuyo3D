using System.Collections;
using UnityEngine;

public class PuyoUnit : MonoBehaviour
{
    public bool ActivelyFalling { get; private set; } = true;
    public bool ForcedDownwards { get; private set; } = false;

    public int ColorIndex { get; private set; }

    private readonly Color[] colorArray = { Color.blue, Color.green, Color.red, Color.cyan };

    private void Awake()
    {
        ColorIndex = Random.Range(0, colorArray.Length);

        GetComponent<Renderer>().material.color = colorArray[ColorIndex];
    }

    public IEnumerator DropToFloor()
    {
        WaitForSeconds wait = new WaitForSeconds(.25f);
        Vector3Int currentPos = Round(transform.position);
        for (int y = currentPos.y - 1; y >= 0; y--)
        {
            Vector3Int downPos = new Vector3Int(currentPos.x, y, currentPos.z);
            if (Playfield.Instance.IsEmpty(downPos))
            {
                ForcedDownwards = true;
                Playfield.Instance.Clear(currentPos);
                transform.position += Vector3.down;
                Playfield.Instance.Add(downPos, this);
                currentPos = Round(transform.position);
                yield return wait;
            }
            else
            {
                ActivelyFalling = false;
                ForcedDownwards = false;
                break;
            }
        }
        ActivelyFalling = false;
        ForcedDownwards = false;
    }

    public void DropToFloorExternal()
    {
        StartCoroutine(DropToFloor());
    }

    public Vector3Int Round(Vector3 vec)
    {
        return new Vector3Int(Mathf.RoundToInt(vec.x),
                              Mathf.RoundToInt(vec.y),
                              Mathf.RoundToInt(vec.z));
    }

    public Color GetColor() => colorArray[ColorIndex];
}
