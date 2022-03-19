using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

public class MapGenerator : MonoBehaviour
{
    [Header("Tile Resources")]
    public List<GameObject> tilePrefabs;

    public GameObject startTile;
    public GameObject goalTile;

    [Header("Map Properties")]
    [Range(3,30)]
    public int width = 3;
    [Range(3, 30)]
    public int depth = 3;

    public Transform parent;

    [Header("Generated Tiles")]
    public List<GameObject> tiles;

    private int startWidth;
    private int startDepth;

    // Start is called before the first frame update
    void Start()
    {
        startWidth = width;
        startDepth = depth;
        BuildMap();
        BakeNavMeshes();
    }

    // Update is called once per frame
    void Update()
    {
        if(width != startWidth || depth != startDepth)
        {
            ResetMap();
            BuildMap();
            BakeNavMeshes();
            Invoke(nameof(BakeNavMeshes), 0.2f);
        }
    }

    public void ResetMap()
    {
        startWidth = width;
        startDepth = depth;        
        var size = tiles.Count;

        for (int i = 0; i < size; i++)
        {
            Destroy(tiles[i]);
        }
        tiles.Clear(); // removes all tiles        
    }

    public void BuildMap()
    {
        var offset = new Vector3(20.0f, 0.0f, 20.0f);

        // choose a random start/goal position, cannot be equal the start and cannot be larger than the grid

        List<int> possibleDepth = Enumerable.Range(1, depth).ToList();
        List<int> possibleWidth = Enumerable.Range(1, width).ToList();

        var indexD = Random.Range(0, possibleDepth.Count);
        var randomStartGridRow = possibleDepth[indexD];
        possibleDepth.RemoveAt(indexD); //removed randomStartGridRow from the list

        indexD = Random.Range(0, possibleDepth.Count);
        var randomGoalGridRow = possibleDepth[indexD];
        possibleDepth.Clear();

        var indexW = Random.Range(0, possibleWidth.Count);
        var randomStartGridCol = possibleWidth[indexW];
        possibleWidth.RemoveAt(indexW);//removed randomStartGridCol from the list

        indexW = Random.Range(0, possibleWidth.Count);
        var randomGoalGridCol = possibleWidth[indexW];
        possibleWidth.Clear();

        Debug.Log("randomStartGridRow " + randomStartGridRow);
        Debug.Log("randomStartGridCol " + randomStartGridCol);
        Debug.Log("randomGoalGridRow " + randomGoalGridRow);
        Debug.Log("randomGoalGridCol " + randomGoalGridCol);

        // generate more tiles
        for (int row = 1; row <= depth; row++)
        {                
            for (int col = 1; col <= width; col++)
            {
                var titlePosition = new Vector3(col * 20.0f, 0.0f, row * 20.0f) - offset;
                                
                if (row == randomGoalGridRow && col == randomGoalGridCol)
                {
                    // place the goal tile
                    tiles.Add(Instantiate(goalTile, titlePosition, Quaternion.identity, parent));
                }
                else if (row == randomStartGridRow && col == randomStartGridCol)
                    {
                        // place the Start tile
                        tiles.Add(Instantiate(startTile, titlePosition, Quaternion.identity, parent));
                    }
                    else
                    {
                        var randomPrefabIndex = Random.Range(0, 4);
                        var randomRotation = Quaternion.Euler(0.0f, Random.Range(0, 4) * 90.0f, 0.0f);
                        tiles.Add(Instantiate(tilePrefabs[randomPrefabIndex], titlePosition, randomRotation, parent));
                    }
            }//end for loop col
            
        }//end for loop row
        
    }

    public void BakeNavMeshes()
    {
        foreach (var tile in tiles)
        {            
            tile.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }

}
