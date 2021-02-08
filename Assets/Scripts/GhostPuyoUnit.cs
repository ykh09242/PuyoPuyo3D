using System.Collections;
using UnityEngine;

public class GhostPuyoUnit : MonoBehaviour
{
    Transform parent;
    PuyoUnit parentPuyoUnit;

    public void SetParent(PuyoUnit parent)
    {
        this.parent = parent.transform;
        parentPuyoUnit = parent;

        StartCoroutine(RepositionBlock());
    }

    private void PositionGhost()
    {
        var parentPos = Round(parent.position);
        var flootY = Playfield.Instance.GetFloor(parentPos.x, parentPos.z);
        var newPos = new Vector3(parentPos.x, flootY, parentPos.z);
        transform.position = newPos;
    }

    IEnumerator RepositionBlock()
    {
        while(parentPuyoUnit.ActivelyFalling)
        {
            var currentPosUnit = Playfield.Instance.GetUnitOnGridPos(Round(transform.position));
            if (currentPosUnit != null && currentPosUnit == parentPuyoUnit) break;

            PositionGhost();
            yield return null;
        }

        Destroy(gameObject);
    }

    public Vector3Int Round(Vector3 vec)
    {
        return new Vector3Int(Mathf.RoundToInt(vec.x),
                              Mathf.RoundToInt(vec.y),
                              Mathf.RoundToInt(vec.z));
    }
}
