using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using Sirenix.OdinInspector;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
#if UNITY_EDITOR
#endif

public class GenerateGrid : MonoBehaviour
{

    [SerializeField]
    GameObject refObject;
    [SerializeField]
    Material refMat;
    [SerializeField]
    bool generateOnStart = false;
    [SerializeField]
    bool showBounds = false;

    [SerializeField]
    Vector3Int setDimensions = new Vector3Int (1, 1, 0);
    [SerializeField]
    float spacing = 1f;

    //privates to store our grid array

    public GameObject[, , ] gridListRead;

    //Create a singleton and a static reference to itself
    static public GenerateGrid instance;

    [SerializeField]
    Camera cam;

    //private properties to make sure the strippipng system does not break our generated cube.
    MeshFilter refFilter;
    MeshRenderer refRenderer;
    Collider refCollider;

    //privates for saving mesh bounds and center
    Vector3 boundSize;
    Vector3 boundCenter;

    void Start ()
    {
        //make sure only one instance exists.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy (this.gameObject);
        }

        //Initialize and generate a grid.
        if (generateOnStart)
        {
            InitializeObject ();

            GG (setDimensions, spacing);
        }
    }

    // Generate a 3D grid with xyz mapped flat on the world axis.
    //Invoke a coroutine so we can control the spawnrate.
    [Command ("Generate_grid")]
    [Button ("Generate grid")]
    void GG (Vector3Int dimensions, float spacing = 1)
    {
        //We need Unity's editor version of the coroutine to properly spawn our objects.
        //We also neet an if statement to check if we are playing or not to prevent the regular routine from partially running.
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            EditorCoroutineUtility.StartCoroutine (GGCoroutine (dimensions, spacing), this);
        }
#endif
        if (Application.isPlaying)
        {
            StartCoroutine (GGCoroutine (dimensions, spacing));
        }
    }

    IEnumerator GGCoroutine (Vector3Int dimensions, float spacing = 1)
    {
        InitializeObject ();
        RemoveGrid ();

        GameObject[, , ] gridList = new GameObject[dimensions.x, dimensions.y, dimensions.z];

        int zPosOffset = -dimensions.z / 2; //we need to count the offset manually because the index of the loop can't be negative since it is used for our array.
        for (int z = 0; z < dimensions.z; z++)
        {
            int yPosOffset = -dimensions.y / 2;
            for (int y = 0; y < dimensions.y; y++)
            {
                int xPosOffset = -dimensions.x / 2;
                for (int x = 0; x < dimensions.x; x++)
                {

                    GameObject clone = Instantiate (refObject, new Vector3 (transform.position.x + xPosOffset, transform.position.y + -yPosOffset, transform.position.z + zPosOffset), transform.rotation, transform);
                    clone.name = clone.name + $"_{x},{y},{z}";
                    if (clone.GetComponent<Cell_Behaviour> () == null)
                    {
                        clone.AddComponent<Cell_Behaviour> ();
                    }
                    gridList[x, y, z] = clone;

                    yield return null;
                    xPosOffset++;

                }
                yPosOffset++;

            }
            zPosOffset++;
        }

        gridListRead = gridList; //saves a copy of the list in case we need to read the values from another class.

    }

    void InitializeObject ()
    {
        if (refMat == null)
        {
            refMat = new Material (Shader.Find ("Standard"));
        }

        if (refObject == null)
        {
            refObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
            refObject.transform.parent = Camera.main.transform;

        }

        if (refObject != null)
        {
            refFilter = refObject.GetComponent<MeshFilter> ();
            refRenderer = refObject.GetComponent<MeshRenderer> ();
            refCollider = refObject.GetComponent<Collider> ();

            refObject.transform.localPosition = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - 10);
            refRenderer.material = refMat;
        }

    }

    [Button ("Remove grid.")]
    [Command ("RemoveGrid")]
    void RemoveGrid ()
    {
        try
        {

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
                    getChildren[i] = transform.GetChild (i).gameObject;
                }
                //next delate all of them
                foreach (GameObject go in getChildren)
                {
                    DestroyImmediate (go);
                    //after destroying all the children we set our reference list to 0;
                    gridListRead = new GameObject[0, 0, 0];
                    CalculateBounds (gridListRead);
                }
#if UNITY_EDITOR
            }
#endif
        }
        catch (System.Exception e)
        {
            print (e);
        }

    }

    void FitGridInCamera (Camera cam, GameObject[, , ] gridlist)
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        CalculateBounds (gridlist);

    }

    //calculates the bounds and center, then output them in the boundCenter and boundSize parameters.
    void CalculateBounds (GameObject[, , ] gridList)
    {
        Bounds bounds = new Bounds ();
        foreach (GameObject go in gridList)
        {
            bounds.Encapsulate (go.transform.position);
        }

        boundCenter = bounds.center;
        boundSize = bounds.size;

    }
#if UNITY_EDITOR 

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = new Color (0, 1, 0, 0.50f);
        Gizmos.DrawCube (boundCenter, boundSize * 1.15f);
    }
#endif

}