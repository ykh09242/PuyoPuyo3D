using System.Collections;
using UnityEngine;

public class Puyo : MonoBehaviour
{
    public PuyoUnit PuyoUnit;
    public GhostPuyoUnit GhostPuyoUnit;

    public float FallSpeed = 1;

    [HideInInspector]
    public PuyoUnit[] UnitArray = new PuyoUnit[2];

    private float interval = 0;

    private Vector3Int left = Vector3Int.left;
    private Vector3Int right = Vector3Int.right;
    private Vector3Int forward = new Vector3Int(0, 0, 1);
    private Vector3Int back = new Vector3Int(0, 0, -1);
    private Vector3Int down = Vector3Int.down;

    private GameObject ghostParent;

#if !UNITY_EDITOR && UNITY_ANDROID
    Vector2 initialPosition;
#endif

    private void Start()
    {
        // PUYO
        UnitArray[0] = Instantiate(PuyoUnit, transform);
        UnitArray[1] = Instantiate(PuyoUnit, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity, transform);

        // GHOST PUYO
        if (GhostPuyoUnit != null)
        {
            ghostParent = new GameObject("GhostPuyo(Clone)");
            if (transform.parent != null)
                ghostParent.transform.SetParent(transform.parent);
            var ghost1 = Instantiate(GhostPuyoUnit, ghostParent.transform);
            var ghost2 = Instantiate(GhostPuyoUnit, ghostParent.transform);

            ghost1.SetParent(UnitArray[0]);
            ghost2.SetParent(UnitArray[1]);
        }

        UpdateGameBoard();
    }

    private void Update()
    {
        AutoDrop();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveForward();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveBack();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RotateLeft();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RotateRight();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveDrop();
        }
#elif !UNITY_EDITOR && UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                initialPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                var direction = touch.position - initialPosition;

                if (direction.x >= 100f)
                {
                    MoveRight();
                }
                else if (direction.x <= -100)
                {
                    MoveLeft();
                }

                if (direction.y >= 100f)
                {
                    MoveForward();
                }
                else if (direction.y <= -100)
                {
                    MoveBack();
                }
            }
        }
#endif
    }

    private void OnDestroy()
    {
        if (ghostParent != null)
        {
            Clear(ghostParent.transform);
            Destroy(ghostParent);
        }
    }

    private void AutoDrop()
    {
        if (interval >= FallSpeed)
        {
            MoveDown();
            interval = 0;
        }
        else
        {
            interval += Time.deltaTime;
        }
    }

    public void MoveLeft()
    {
        if (ValidMove(left))
        {
            Move(left, transform);
        }
    }

    public void MoveRight()
    {
        if (ValidMove(right))
        {
            Move(right, transform);
        }
    }

    public void MoveForward()
    {
        if (ValidMove(forward))
        {
            Move(forward, transform);
        }
    }

    public void MoveBack()
    {
        if (ValidMove(back))
        {
            Move(back, transform);
        }
    }

    public void MoveDown()
    {
        if (ValidMove(down))
        {
            Move(down, transform);
        }
        else
        {
            DisableSelf();
        }
    }

    public void MoveDrop()
    {
        FallSpeed = 0.005f;
    }

    public void RotateLeft()
    {
        Vector3Int vect = GetClockwiseRotationVector();
        if (ValidRotate(vect))
        {
            Move(vect, UnitArray[1].transform);
        }
    }

    public void RotateRight()
    {
        Vector3Int vect = GetCounterClockwiseRotationVector();
        if (ValidRotate(vect))
        {
            Move(vect, UnitArray[1].transform);
        }
    }

    private void Move(Vector3Int vector, Transform target)
    {
        ClearCurrentGameboardPosition();
        target.position += vector;
        UpdateGameBoard();
    }

    private void ClearCurrentGameboardPosition()
    {
        foreach (Transform child in transform)
        {
            var pos = Round(child.position);
            Playfield.Instance.Clear(pos);
        }
    }

    private void UpdateGameBoard()
    {
        foreach (Transform child in transform)
        {
            var puyoUnit = child.GetComponent<PuyoUnit>();
            if (puyoUnit == null) continue;

            var pos = Round(child.position);
            Playfield.Instance.Add(pos, puyoUnit);
        }
    }

    private Vector3Int GetClockwiseRotationVector()
    {
        Vector3Int pos = Round(transform.position);
        Vector3Int puyoUnitPos = Round(UnitArray[1].transform.position);

        if (Vector3Int.Distance(puyoUnitPos + left, pos) == 0)
        {
            return new Vector3Int(-1, 0, -1);
        }
        else if (Vector3Int.Distance(puyoUnitPos + forward, pos) == 0)
        {
            return new Vector3Int(-1, 0, 1);
        }
        else if (Vector3Int.Distance(puyoUnitPos + right, pos) == 0)
        {
            return new Vector3Int(1, 0, 1);
        }
        else if (Vector3Int.Distance(puyoUnitPos + back, pos) == 0)
        {
            return new Vector3Int(1, 0, -1);
        }

        return Vector3Int.zero;
    }

    private Vector3Int GetCounterClockwiseRotationVector()
    {
        Vector3Int pos = Round(transform.position);
        Vector3Int puyoUnitPos = Round(UnitArray[1].transform.position);

        if (Vector3Int.Distance(puyoUnitPos + left, pos) == 0)
        {
            return new Vector3Int(-1, 0, 1);
        }
        else if (Vector3Int.Distance(puyoUnitPos + forward, pos) == 0)
        {
            return new Vector3Int(1, 0, 1);
        }
        else if (Vector3Int.Distance(puyoUnitPos + right, pos) == 0)
        {
            return new Vector3Int(1, 0, -1);
        }
        else if (Vector3Int.Distance(puyoUnitPos + back, pos) == 0)
        {
            return new Vector3Int(-1, 0, -1);
        }

        return Vector3Int.zero;
    }

    private bool ActivelyFalling()
    {
        return UnitArray[0].ActivelyFalling ||
               UnitArray[1].ActivelyFalling;
    }

    ///////////////////////////
    // Movement Constraints //
    /////////////////////////

    bool ValidMove(Vector3Int direction)
    {
        foreach (Transform child in transform)
        {
            Vector3Int newPosition = Round(child.position + direction);

            if (!Playfield.Instance.FreeSpace(newPosition, transform))
            {
                return false;
            }
        }
        return true;
    }

    bool ValidRotate(Vector3Int direction)
    {
        Vector3Int newPosition = Round(UnitArray[1].transform.position + direction);
        return Playfield.Instance.FreeSpace(newPosition, transform);
    }

    ////////////////
    // PuyoUnits //
    ///////////////

    private void DropPuyoUnits()
    {
        foreach (Transform child in transform)
        {
            var puyoUnit = child.GetComponent<PuyoUnit>();
            if (puyoUnit == null) continue;

            StartCoroutine(puyoUnit.DropToFloor());
        }
    }

    ////////////////
    // Utilities //
    ///////////////

    public void Clear(Transform transform)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public Vector3Int Round(Vector3 vec)
    {
        return new Vector3Int(Mathf.RoundToInt(vec.x),
                              Mathf.RoundToInt(vec.y),
                              Mathf.RoundToInt(vec.z));
    }

    void DisableSelf()
    {
        var controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        DropPuyoUnits();
        enabled = false;
        Playfield.Instance.ActivePuyo = null;
        StartCoroutine(SpawnNextBlock());
    }

    IEnumerator SpawnNextBlock()
    {
        yield return new WaitUntil(() => !ActivelyFalling());

        Playfield.Instance.SpawnPuyo();
    }
}