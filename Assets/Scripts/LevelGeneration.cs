using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Generates a level
/// </summary>

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private GameObject[] tileArray;
    [SerializeField] private int levelAmount;
    private List<GameObject> gObjects = new List<GameObject>();
    private GameObject[] exitList;
    private List<Vector3> UsedSpaces = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Function used to generate Level
    void GenerateLevel()
    {
        exitList = GameObject.FindGameObjectsWithTag("ExitPoint");
        for (var i = 0; i < levelAmount; i++)
        {
            GameObject obj;
            bool valid = true;
            List<Vector3> newUsedSpaces = new List<Vector3>();
            // First block is always 1x1
            if (i == 0)
            {
                obj = Instantiate(levelBlock, new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.parent = transform;
                obj.name = $"Flat {i}";
                UsedSpaces.Add(new Vector3(0, 0, 0));
                gObjects.Add(obj);
            }
            else
            {
                // Gets a random block and places blocks next to each other
                Transform ExitPoint = getRandomTarget();
                obj = Instantiate(RandomTile(), ExitPoint.position, Quaternion.identity);
                obj.name = $"Flat {gObjects.Count}";
                obj.transform.parent = transform;
                for (var j = 0; j < obj.transform.childCount; j++)
                {
                    if (j == 0)
                    {
                        moveObjectNextTo(ExitPoint.transform, obj.transform.GetChild(j));
                        newUsedSpaces.Add(Vector3Int.RoundToInt(ExitPoint.position));
                    }
                    else
                    {
                        Transform extendPoint = getRandomExit(obj.transform.GetChild(j - 1), "ExtendPoint");
                        Transform extendRec = getRandomExit(obj.transform.GetChild(j), "ExtendRec");
                        valid = extendObjectNextTo(extendPoint, extendRec);
                        if (valid == false)
                        {
                            DestroyImmediate(obj);
                            break;
                        }
                        newUsedSpaces.Add(Vector3Int.RoundToInt(extendPoint.position));
                    }
                }
                if (valid)
                {
                    UsedSpaces.AddRange(newUsedSpaces);
                }
                else
                {
                    obj = Instantiate(levelBlock, ExitPoint.position, Quaternion.identity);
                    obj.transform.parent = transform;
                    moveObjectNextTo(ExitPoint.transform, obj.transform.GetChild(0));
                    UsedSpaces.Add(Vector3Int.RoundToInt(ExitPoint.position));
                }
                gObjects.Add(obj);

            }
            exitList = GameObject.FindGameObjectsWithTag("ExitPoint");
        }
    }

    // Moves Object next to selected exitpoint
    private void moveObjectNextTo(Transform ExitPoint, Transform obj)
    {
        Debug.Log(ExitPoint.parent.parent.name);
        obj.position = ExitPoint.position;
        Transform newExit = getRandomExit(obj);
        RotateObject(ExitPoint, newExit);
    }

    // Places extension object next to each other using their extendpoint and extendRec
    private bool extendObjectNextTo(Transform ExtendPoint, Transform ExtendRec)
    {
        if (UsedSpaces.Contains(Vector3Int.RoundToInt(ExtendPoint.position)) == true)
        {
            return false;
        }
        ExtendRec.parent.position = ExtendPoint.position;
        RotateObject(ExtendPoint, ExtendRec);
        return true;
    }
    
    // Rotates object until newexitpoint is aligned with the parent position of exitPoint
    private void RotateObject(Transform exitPoint, Transform newExitPoint)
    {
        for(int i = 0; i < 4; i++)
        {
            newExitPoint.parent.Rotate(0, 90, 0);
            if (exitPoint.parent.position == newExitPoint.position)
            {
                newExitPoint.gameObject.SetActive(false);
                exitPoint.gameObject.SetActive(false);
                i = 4;
            }
        }
    }

    // Returns a random tile from the tilearray 
    private GameObject RandomTile()
    {
        return tileArray[Random.Range(0, tileArray.Length)];
    }

    /// <summary>
    /// Gets a random object with the same same tag in the paremeter
    /// </summary>
    /// <param name="Target"> The Target Object</param>
    /// <param name="tagString">The name of the tag</param>
    /// <returns>Random Transform with tagString as tag</returns>
    private Transform getRandomExit(Transform Target, string tagString = "ExitPoint")
    {
        List<Transform> listOfNewExits = new List<Transform>();
        foreach(Transform child in Target)
        {
            if (child.tag == tagString)
            {
                listOfNewExits.Add(child);
            }
        }
        return listOfNewExits[Random.Range(0, listOfNewExits.Count)];
    }
    /// <summary>
    /// Gets a random exit target from the exitList, retries if target is invalid
    /// </summary>
    /// <returns>Transform of exit</returns>
    private Transform getRandomTarget()
    {
        Transform toReturn;
        while (true)
        {
            toReturn = exitList[Random.Range(0, exitList.Length)].transform;
            toReturn.position = Vector3Int.RoundToInt(toReturn.position);
            if (UsedSpaces.Contains(Vector3Int.RoundToInt(toReturn.position)) == false)
            {
                return toReturn;
            }
        }
    }

}
