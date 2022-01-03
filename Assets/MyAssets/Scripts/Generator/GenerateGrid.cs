using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;


public class GenerateGrid : MonoBehaviour
{
    public enum UPDATEMODE { CELL, ROW, DEPTH, GRID }

    public UPDATEMODE selectUpdateMode = UPDATEMODE.GRID;

    [SerializeField]
    GameObject refObject;
    [SerializeField]
    Material refMat;

    public bool generateOnStart = false;
    public bool randomCellOnInitialize = false;
    public bool removeGridOnInitialize = false;
    public Vector3Int setDimensions = new Vector3Int(1, 1, 0);
    public float spacing = 1f;

    //privates to store our grid array

    public GameObject[,,] gridListRead;
    public bool gridInitialized = false;

    //Create a singleton and a static reference to itself
    static public GenerateGrid instance;

    [SerializeField]
    Camera cam;

    [SerializeField]
    int amountOfCells = 0;

    //private properties to make sure the strippipng system does not break our generated cube.
    MeshFilter refFilter;
    MeshRenderer refRenderer;
    Collider refCollider;

    //privates for saving mesh bounds and center
    Vector3 boundSize;
    Vector3 boundCenter;


    [Button]
    [Command("Generate_grid")]
    //Used to be void Start, but now gets activated by the SystemUpdater script
    public void Initialize(Vector3Int dimensions, float spacing = 1, bool generateOnStart = true, bool removeGrid = true, bool randomCells = true, int setUpdateMode = 2)
    {
        //make sure only one instance exists.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //    Destroy(gameObject);
        }

        //Initialize and generate a grid.

        InitializeObject();
        if (generateOnStart)
        {
            GG(dimensions, spacing, removeGrid, randomCells, (setUpdateMode));
        }


    }


    // Generate a 3D grid with xyz mapped flat on the world axis.
    //Invoke a coroutine so we can control the spawnrate.

    void GG(Vector3Int dimensions, float spacing = 1, bool removeGrid = false, bool randomCells = false, int setUpdateMode = 2)
    {
        //We need Unity's editor version of the coroutine to properly spawn our objects.
        //We also neet an if statement to check if we are playing or not to prevent the regular routine from partially running.
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            EditorCoroutineUtility.StartCoroutine(GGCoroutine(dimensions, spacing, removeGrid, randomCells, setUpdateMode), this);
        }
#endif
        if (Application.isPlaying)
        {
            StartCoroutine(GGCoroutine(dimensions, spacing, removeGrid, randomCells, setUpdateMode));
        }
    }

    IEnumerator GGCoroutine(Vector3Int dimensions, float spacing, bool removeGrid, bool randomCells, int setUpdateMode = 2)
    {
        if (removeGrid)
        {
            RemoveGrid();

        }

        gridInitialized = false;


        GameObject[,,] gridList = new GameObject[dimensions.x, dimensions.y, dimensions.z];

        int zPosOffset = -dimensions.z / 2; //we need to count the offset manually because the index of the loop can't be negative since it is used for our array.
        for (int z = 0; z < dimensions.z; z++)
        {
            int yPosOffset = -dimensions.y / 2;
            for (int y = 0; y < dimensions.y; y++)
            {
                int xPosOffset = -dimensions.x / 2;
                for (int x = 0; x < dimensions.x; x++)
                {

                    //we only generate the cells when we have removed the gird, otherwise we get de current child(index) and put this in the array.

                    GameObject clone = Instantiate(refObject, new Vector3(transform.position.x + xPosOffset * spacing, transform.position.y + -yPosOffset * spacing, transform.position.z + zPosOffset * spacing), Quaternion.identity, transform);
                    clone.name = clone.name + $"_{x},{y},{z}";
                    if (clone.GetComponent<Cell_Behaviour>() == null)
                    {
                        clone.AddComponent<Cell_Behaviour>();
                    }
                    gridList[x, y, z] = clone;
                    clone.GetComponent<Cell_Behaviour>().listIndex = new Vector3Int(x, y, z);//we store the index of the list on the cell locally for other script references.
                                                                                             // clone.GetComponent<Cell_Behaviour>().randomOnStart = randomCells;
                    clone.GetComponent<Cell_Behaviour>().RandomizeCell(randomCells);

                    xPosOffset++;



                    if (setUpdateMode == ((int)UPDATEMODE.CELL))
                    {
                        yield return null;
                    }


                }
                yPosOffset++;
                if (setUpdateMode == ((int)UPDATEMODE.ROW))
                {
                    yield return null;
                }
            }
            zPosOffset++;
            if (setUpdateMode == ((int)UPDATEMODE.DEPTH))
            {
                yield return null;
            }
        }
        if (setUpdateMode == ((int)UPDATEMODE.GRID))
        {
            yield return null;
        }

        amountOfCells = dimensions.x * dimensions.y * dimensions.z;
        gridListRead = gridList; //saves a copy of the list in case we need to read the values from another class.

        FitGridInCamera(cam, gridList);
        gridInitialized = true;
    }

    void InitializeObject()
    {
        if (refMat == null)
        {
            refMat = new Material(Shader.Find("Standard"));
        }

        if (refObject == null)
        {
            refObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            refObject.transform.parent = Camera.main.transform;

        }

        if (refObject != null)
        {
            refFilter = refObject.GetComponent<MeshFilter>();
            refRenderer = refObject.GetComponent<MeshRenderer>();
            refCollider = refObject.GetComponent<Collider>();

            refObject.transform.localPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - 10);
            refRenderer.material = refMat;
        }

    }

    [Button("Remove grid.")]
    [Command("RemoveGrid")]
    void RemoveGrid()
    {
        try
        {
            gridInitialized = false;
            int childCount = transform.childCount;
            GameObject[] getChildren = new GameObject[childCount];
            //Little bit of a hacky solution, But this way our foreachloop will keep running in the editor when the function gets invoked through Odin.
            //This mainly because I was to lazy to set up a proper EditorCoroutine.
#if UNITY_EDITOR
            while (childCount > 0)
            {
#endif

                //first get all the children
                for (int i = 0; i < childCount; i++)
                {
                    getChildren[i] = transform.GetChild(i).gameObject;
                }
                //next delate all of them
                foreach (GameObject go in getChildren)
                {
                    if (go != null)
                    {
                        SystemUpdater.instance.ONupdateEvent -= go.GetComponent<Cell_Behaviour>().UpdateThisCell;
                        DestroyImmediate(go);
                    }
                    //after destroying all the children we set our reference list to 0;


                }

                gridListRead = new GameObject[0, 0, 0];
                CalculateBounds(gridListRead);

#if UNITY_EDITOR
            }
#endif
        }
        catch (System.Exception e)
        {
            print(e);
        }

    }

    void FitGridInCamera(Camera getCam, GameObject[,,] gridlist)
    {
        if (getCam == null)
        {
            getCam = Camera.main;
        }
        CalculateBounds(gridlist);


        if (getCam.orthographic)
        {
            getCam.orthographicSize = boundSize.magnitude / 2.4f;
            getCam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 0.1f, boundSize.magnitude);
        }
        else
        {
            getCam.fieldOfView = boundSize.magnitude + 10;
            getCam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, boundSize.magnitude + 10);
        }
    }

    //calculates the bounds and center, then output them in the boundCenter and boundSize parameters.
    void CalculateBounds(GameObject[,,] gridList)
    {
        Bounds bounds = new Bounds();
        foreach (GameObject go in gridList)
        {
            if (go != null)
                bounds.Encapsulate(go.transform.position);
        }

        boundCenter = bounds.center;
        boundSize = bounds.size;

    }


}