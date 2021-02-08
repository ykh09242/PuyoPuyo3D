using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    public static Playfield Instance;

    [Header("Size")]
    public Vector3Int gridSize;

    [Header("Playfield Visulas")]
    public Transform bottomPlane;
    public Transform N, S, W, E;

    [Header("Material")]
    public Material bottomGrid;
    public Material NSGrid;
    public Material WEGrid;

    [Header("Well")]
    public Renderer NSWell;
    public Renderer WEWell;
    public Transform[] NSWEParent;

    [Header("Puyo")]
    public Puyo Puyo;
    public Transform PuyoParent;

    public PuyoUnit[,,] theGrid;

    public Material[,] nWell;
    public Material[,] sWell;
    public Material[,] wWell;
    public Material[,] eWell;

    public Puyo ActivePuyo { get; set; }

    private void Awake()
    {
        Instance = this;

        theGrid = new PuyoUnit[gridSize.x, gridSize.y, gridSize.z];
    }

    private void Start()
    {
        InstantiateWell();
        SpawnPuyo();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void OnDrawGizmos()
    {
        if (bottomPlane != null)
        {
            Vector3 scaler = new Vector3(gridSize.x * 0.1f, 1f, gridSize.z * 0.1f);
            bottomPlane.localScale = scaler;

            bottomPlane.position = new Vector3(transform.position.x + gridSize.x * 0.5f,
                                               transform.position.y,
                                               transform.position.z + gridSize.z * 0.5f);

            if (bottomGrid != null)
                bottomGrid.mainTextureScale = new Vector2(gridSize.x, gridSize.z);
        }

        if (N != null || S != null)
        {
            if (N != null)
            {
                //RESIZE BOTTOM PLANE
                Vector3 scaler = new Vector3(gridSize.x * 0.1f, 1f, gridSize.y * 0.1f);
                N.localScale = scaler;

                //REPOSITION
                N.position = new Vector3(transform.position.x + gridSize.x * 0.5f,
                                         transform.position.y + gridSize.y * 0.5f,
                                         transform.position.z + gridSize.z);
            }

            if (S != null)
            {
                //RESIZE BOTTOM PLANE
                Vector3 scaler = new Vector3(gridSize.x * 0.1f, 1f, gridSize.y * 0.1f);
                S.localScale = scaler;

                //REPOSITION
                S.position = new Vector3(transform.position.x + gridSize.x * 0.5f,
                                         transform.position.y + gridSize.y * 0.5f,
                                         transform.position.z);
            }

            if (NSGrid != null)
            {
                //RETILE MATERIAL
                NSGrid.mainTextureScale = new Vector2(gridSize.x, gridSize.y);
            }
        }

        if (W != null || E != null)
        {
            if (W != null)
            {
                //RESIZE BOTTOM PLANE
                Vector3 scaler = new Vector3(gridSize.z * 0.1f, 1f, gridSize.y * 0.1f);
                W.localScale = scaler;

                //REPOSITION
                W.position = new Vector3(transform.position.x,
                                         transform.position.y + gridSize.y * 0.5f,
                                         transform.position.z + gridSize.z * 0.5f);
            }

            if (E != null)
            {
                //RESIZE BOTTOM PLANE
                Vector3 scaler = new Vector3(gridSize.z * 0.1f, 1f, gridSize.y * 0.1f);
                E.localScale = scaler;

                //REPOSITION
                E.position = new Vector3(transform.position.x + gridSize.x,
                                         transform.position.y + gridSize.y * 0.5f,
                                         transform.position.z + gridSize.z * 0.5f);
            }

            if (WEGrid != null)
            {
                //RETILE MATERIAL
                WEGrid.mainTextureScale = new Vector2(gridSize.z, gridSize.y);
            }
        }

    }

    private void InstantiateWell()
    {
        if (NSWell == null || WEWell == null) return;

        if (NSWell != null)
        {
            nWell = new Material[gridSize.x, gridSize.y];
            sWell = new Material[gridSize.x, gridSize.y];
        }

        if (WEWell != null)
        {
            wWell = new Material[gridSize.z, gridSize.y];
            eWell = new Material[gridSize.z, gridSize.y];
        }

        var pos = transform.position;
        for (int y = 0; y < gridSize.y; y++)
        {
            var yPos = pos.y + y + 1;
            if (NSWell != null)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    var xPos = pos.x + x + 1;
                    var well1 = Instantiate(NSWell, NSWEParent[0]);
                    well1.name = $"n {y} - {x}";
                    well1.transform.localPosition = new Vector3(xPos, yPos, 0f);
                    well1.material.color = Color.clear;
                    nWell[x, y] = well1.material;

                    var well2 = Instantiate(NSWell, NSWEParent[1]);
                    well2.name = $"s {y} - {x}";
                    well2.transform.localPosition = new Vector3(xPos, yPos, gridSize.z);
                    well2.material.color = Color.clear;
                    sWell[x, y] = well2.material;
                }
            }

            if (WEWell != null)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    var zPos = pos.z + z + 1;
                    var well1 = Instantiate(WEWell, NSWEParent[2]);
                    well1.name = $"w {y} - {z}";
                    well1.transform.localPosition = new Vector3(0f, yPos, zPos);
                    well1.material.color = Color.clear;
                    wWell[z, y] = well1.material;

                    var well2 = Instantiate(WEWell, NSWEParent[3]);
                    well2.name = $"e {y} - {z}";
                    well2.transform.localPosition = new Vector3(gridSize.x, yPos, zPos);
                    well2.material.color = Color.clear;
                    eWell[z, y] = well2.material;
                }
            }
        }
    }

    public void SpawnPuyo()
    {
        if (WhatToDelete())
        {
            StartCoroutine(DelayDelete());
        }

        StartCoroutine(DelaySpawn());
    }

    private bool GameIsOver()
    {
        Vector3Int spawnPoint = new Vector3Int(Mathf.RoundToInt(gridSize.x * 0.5f),
                                               gridSize.y - 1,
                                               Mathf.RoundToInt(gridSize.z * 0.5f));

        return theGrid[spawnPoint.x, spawnPoint.y, spawnPoint.z] != null ||
               theGrid[spawnPoint.x + 1, spawnPoint.y, spawnPoint.z] != null;
    }

    IEnumerator DelayDelete()
    {
        DropAllColumns();
        yield return new WaitUntil(() => !AnyFallingBlocks());
        if (WhatToDelete())
        {
            StartCoroutine(DelayDelete());
        };

    }

    IEnumerator DelaySpawn()
    {
        yield return new WaitUntil(() => !AnyFallingBlocks() && !WhatToDelete());
        if (GameIsOver())
        {
            enabled = false;
            GameManager.Instance.SetGameOver();
        }
        else
        {
            Vector3Int spawnPoint = new Vector3Int(Mathf.RoundToInt(transform.position.x + gridSize.x * 0.5f),
                                                   Mathf.RoundToInt(transform.position.y + gridSize.y) - 1,
                                                   Mathf.RoundToInt(transform.position.z + gridSize.z * 0.5f));
            ActivePuyo = Instantiate(Puyo, spawnPoint, Quaternion.identity, PuyoParent);
        }
    }

    public Vector3Int Round(Vector3 vec)
    {
        return new Vector3Int(Mathf.RoundToInt(vec.x),
                              Mathf.RoundToInt(vec.y),
                              Mathf.RoundToInt(vec.z));
    }

    public bool WithinBorders(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x &&
               pos.z >= 0 && pos.z < gridSize.z &&
               pos.y >= 0 && pos.y < gridSize.y;
    }

    public PuyoUnit GetUnitOnGridPos(Vector3Int pos)
    {
        if (pos.y > gridSize.y - 1)
        {
            return null;
        }
        else
        {
            return theGrid[pos.x, pos.y, pos.z];
        }
    }

    private void UpdateWell()
    {
        if (NSWell == null || WEWell == null) return;

        for (int y = gridSize.y - 1; y >= 0; y--)
        {
            if (NSWell != null)
            {

                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        PuyoUnit current = theGrid[x, y, z];
                        if (current == null)
                        {
                            nWell[x, y].color = Color.clear;
                            continue;
                        }

                        nWell[x, y].color = current.GetColor();
                        break;
                    }

                    for (int z = gridSize.z - 1; z >= 0; z--)
                    {
                        PuyoUnit current = theGrid[x, y, z];
                        if (current == null)
                        {
                            sWell[x, y].color = Color.clear;
                            continue;
                        }

                        sWell[x, y].color = current.GetColor();
                        break;
                    }
                }
            }

            if (WEWell != null)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    for (int x = 0; x < gridSize.x; x++)
                    {
                        PuyoUnit current = theGrid[x, y, z];
                        if (current == null)
                        {
                            wWell[z, y].color = Color.clear;
                            continue;
                        }

                        wWell[z, y].color = current.GetColor();
                        break;
                    }

                    for (int x = gridSize.x - 1; x >= 0; x--)
                    {
                        PuyoUnit current = theGrid[x, y, z];
                        if (current == null)
                        {
                            eWell[z, y].color = Color.clear;
                            continue;
                        }

                        eWell[z, y].color = current.GetColor();
                        break;
                    }
                }
            }
        }
    }

    public int GetFloor(int x, int z)
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            if (theGrid[x, y, z] != null) continue;

            return y;
        }
        return 0;
    }

    public bool FreeSpace(Vector3Int pos, Transform parentTransform) => WithinBorders(pos) && (IsEmpty(pos) || theGrid[pos.x, pos.y, pos.z].transform.parent == parentTransform);

    public bool IsEmpty(Vector3Int pos) => WithinBorders(pos) && theGrid[pos.x, pos.y, pos.z] == null;

    public bool ColorMatches(Vector3Int pos, PuyoUnit puyoUnit) => WithinBorders(pos) && theGrid[pos.x, pos.y, pos.z].ColorIndex == puyoUnit.ColorIndex;

    public void Clear(Vector3Int pos)
    {
        theGrid[pos.x, pos.y, pos.z] = null;
        UpdateWell();
    }

    public void Add(Vector3Int pos, PuyoUnit obj)
    {
        theGrid[pos.x, pos.y, pos.z] = obj;
        UpdateWell();
    }

    public void Delete(PuyoUnit puyo)
    {
        Vector3Int pos = Round(puyo.transform.position);
        Clear(pos);
        Destroy(puyo.gameObject);
    }

    public bool WhatToDelete()
    {
        List<int> groupCount = new List<int>();
        List<PuyoUnit> groupToDelete = new List<PuyoUnit>();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.z; z++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    List<PuyoUnit> currentGroup = new List<PuyoUnit>();

                    if (theGrid[x, y, z] != null)
                    {
                        PuyoUnit current = theGrid[x, y, z];
                        if (groupToDelete.IndexOf(current) == -1)
                        {
                            AddNeighbors(current, ref currentGroup);
                        }
                    }

                    if (currentGroup.Count >= 4)
                    {
                        groupCount.Add(currentGroup.Count);
                        foreach (PuyoUnit puyo in currentGroup)
                        {
                            groupToDelete.Add(puyo);
                        }
                    }
                }
            }
        }

        if (groupToDelete.Count > 0)
        {
            if (groupCount.Count > 0)
            {
                int chain = groupCount.Count;
                for (int i = 0; i < chain; i++)
                {
                    int destroyPuyo = groupCount[i];
                    GameManager.Instance.PlusScore(destroyPuyo, chain);
                }
            }

            DeleteUnits(groupToDelete);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DropAllColumns()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.z; z++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (theGrid[x, y, z] == null) continue;

                    PuyoUnit puyoUnit = theGrid[x, y, z];
                    if (puyoUnit == null) continue;

                    puyoUnit.DropToFloorExternal();
                }
            }
        }
    }

    private void AddNeighbors(PuyoUnit currentUnit, ref List<PuyoUnit> currentGroup)
    {
        Vector3Int[] directions = { Vector3Int.right, Vector3Int.left,
                                    new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1), // foward, back
                                    Vector3Int.up, Vector3Int.down };
        if (currentGroup.IndexOf(currentUnit) == -1)
        {
            currentGroup.Add(currentUnit);
        }
        else
        {
            return;
        }

        foreach (var direction in directions)
        {
            Vector3Int next = Round(currentUnit.transform.position + direction);
            if (IsEmpty(next) || !ColorMatches(next, currentUnit)) continue;

            PuyoUnit nextUnit = theGrid[next.x, next.y, next.z];
            AddNeighbors(nextUnit, ref currentGroup);
        }
    }

    private void DeleteUnits(List<PuyoUnit> unitsToDelete)
    {
        foreach (PuyoUnit unit in unitsToDelete)
        {
            Delete(unit);
        }
    }

    public bool AnyFallingBlocks()
    {
        for (int y = gridSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    if (theGrid[x, y, z] == null) continue;

                    if (theGrid[x, y, z].ForcedDownwards)
                    {
                        return true;
                    }
                    else if (theGrid[x, y, z].ActivelyFalling)
                    {
                        return true;
                    }

                }
            }
        }

        return false;
    }
}
