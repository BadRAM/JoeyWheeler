using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private GameObject[] tileArray;
    [SerializeField] private int levelAmount;
    private List<GameObject> gObjects = new List<GameObject>();
    private GameObject[] exitList;
    [SerializeField] private List<Vector3> UsedSpaces = new List<Vector3>();

    private Transform test;
    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void GenerateLevel()
    {

        for(var i = 0; i < levelAmount; i++)
        {
            GameObject obj;
            bool valid = true;
            List<Vector3> newUsedSpaces = new List<Vector3>();
            if (i == 0)
            {
                obj = Instantiate(RandomTile(), new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.parent = transform;
                obj.name = $"Flat {i}";
                UsedSpaces.Add(new Vector3(0, 0, 0));
                gObjects.Add(obj);
            }
            else
            {
                Transform ExitPoint = getRandomTarget();
                obj = Instantiate(RandomTile(), ExitPoint.position, Quaternion.identity);
                obj.transform.parent = transform;
                obj.name = $"Flat {gObjects.Count}";
                obj.transform.parent = transform;
                for (var j = 0; j < obj.transform.childCount; j++)
                {
                    if(j == 0)
                    {
                        moveObjectNextTo(ExitPoint.transform, obj.transform.GetChild(j));
                        newUsedSpaces.Add(Vector3Int.RoundToInt(ExitPoint.position));
                    }
                    else
                    {
                        Transform extendPoint = getRandomExit(obj.transform.GetChild(j - 1), "ExtendPoint");
                        Transform extendRec = getRandomExit(obj.transform.GetChild(j), "ExtendRec");
                        valid = extendObjectNextTo(extendPoint, extendRec);
                        if(valid == false)
                        {
                            Destroy(obj);
                            break;
                        }
                        newUsedSpaces.Add(Vector3Int.RoundToInt(extendPoint.position));
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
                }
                gObjects.Add(obj);

            }
            exitList = GameObject.FindGameObjectsWithTag("ExitPoint");
        }
    }

    private Transform getRandomTarget()
    {
        Transform toReturn;
        while (true)
        {
            toReturn = exitList[Random.Range(0, exitList.Length)].transform;
            toReturn.position = Vector3Int.RoundToInt(toReturn.position);
            Debug.Log(toReturn.position);
            if (UsedSpaces.Contains(Vector3Int.RoundToInt(toReturn.position)) == false)
            {
                return toReturn;
            }
        }
    }

    private void moveObjectNextTo(Transform ExitPoint, Transform obj)
    {
        Debug.Log(ExitPoint);
        obj.position = ExitPoint.position;
        Transform newExit = getRandomExit(obj);
        RotateObject(ExitPoint, newExit);
    }

    private bool extendObjectNextTo(Transform ExtendPoint, Transform ExtendRec)
    {
        if (UsedSpaces.Contains(Vector3Int.RoundToInt(ExtendPoint.position)) == true)
        {
            Debug.Log("not Valid");
            return false;
        }
        ExtendRec.parent.position = ExtendPoint.position;
        RotateObject(ExtendPoint, ExtendRec);
        return true;
    }
    
    private void RotateObject(Transform exitPoint, Transform newExitPoint)
    {
        //Debug.Log($"Exit point: {exitPoint} {exitPoint.parent}");
        //Debug.Log($"New exit Point: {newExitPoint} {newExitPoint.parent}");
        for(int i = 0; i < 4; i++)
        {
            newExitPoint.parent.Rotate(0, 90, 0);
            //Debug.Log(newExitPoint.position);
            if (exitPoint.parent.position == newExitPoint.position)
            {
                newExitPoint.gameObject.SetActive(false);
                exitPoint.gameObject.SetActive(false);
                i = 4;
            }
        }
    }

    private GameObject RandomTile()
    {
        return tileArray[Random.Range(0, tileArray.Length)];
    }

    private Transform getRandomExit(Transform Target, string tagString = "ExitPoint")
    {
        Debug.Log(Target);
        List<Transform> listOfNewExits = new List<Transform>();
        foreach(Transform child in Target)
        {
            //Debug.Log(child);
            if (child.tag == tagString)
            {
                //Debug.Log(child.tag);
                listOfNewExits.Add(child);
            }
        }
        //Debug.Log(listOfNewExits.Count);
        return listOfNewExits[Random.Range(0, listOfNewExits.Count)];
        //return listOfNewExits[5];
    }

}
