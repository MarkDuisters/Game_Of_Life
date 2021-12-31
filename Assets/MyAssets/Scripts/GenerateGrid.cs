using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using Sirenix.OdinInspector;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{

    [SerializeField]
    GameObject refObject;
    [SerializeField]
    Material refMat;
    [SerializeField]
    bool generateOnStart = false;
    [SerializeField ()]
    bool ThreeDgrid = false;

    [SerializeField]
    Vector3Int setDimensions = new Vector3Int (1, 1, 1);
    [SerializeField]
    float spacing = 1f;
    [SerializeField]
    float generateDelay = 0;

    //privates to store our grid array

    GameObject[, , ] gridList;

    //private properties to make sure the strippipng system does not break our generated cube.
    MeshFilter refFilter;
    MeshRenderer refRenderer;
    Collider refCollider;

    // Start is called before the first frame update
    void Start ()
    {
        print ("werkt");
        if (generateOnStart)
        {
            InitializeObject ();
            if (ThreeDgrid)
            {
                GGThreeD (setDimensions, spacing, generateDelay);

            }
            else
            {
                GGTwoD ((Vector2Int) setDimensions, spacing, generateDelay);
            }
        }
    }

    // Generate a 2D grid with y mapped flat on the world z axis.
    //Invoke a coroutine so we can control the spawnrate.
    [Command ("Generate_2D_grid")]
    [Button ("Generate 2D grid")]
    void GGTwoD (Vector2Int dimensions, float spacing, float generationDelay = 0f)
    {
        StartCoroutine (GGTwoDCoroutine (dimensions, spacing, generationDelay));
    }

    IEnumerator GGTwoDCoroutine (Vector2Int dimensions, float spacing, float generationDelay = 0f)
    {
        gridList = new GameObject[dimensions.x, dimensions.y, 0];

        InitializeObject ();

        // RemoveGrid ();

        for (int y = 0; y < dimensions.y; y++)
        {

            print ("werkt2dloopY");
            for (int x = 0; x < dimensions.x; x++)
            {
                print ("werkt2dloopX");
                print ($"{x},{y}");

                GameObject clone = Instantiate (refObject, new Vector3 (transform.position.x + x, transform.position.y - y, 0), Quaternion.identity, transform);
                clone.name = clone.name + $"_{x},{y}";
                gridList[x, y, 0] = clone;
                yield return new WaitForSeconds (generationDelay);
            }
        }
    }

    // Generate a 3D grid with xyz mapped flat on the world axis.
    //Invoke a coroutine so we can control the spawnrate.
    [Command ("Generate_3D_grid")]
    [Button ("Generate 3D grid")]
    void GGThreeD (Vector3Int dimensions, float spacing, float generationDelay = 0f)
    {
        gridList = new GameObject[dimensions.x, dimensions.y, dimensions.z];
        StartCoroutine (GGThreeDCoroutine (dimensions, spacing, generationDelay));
    }

    IEnumerator GGThreeDCoroutine (Vector3Int dimensions, float spacing, float generationDelay = 0f)
    {
        InitializeObject ();
        RemoveGrid ();

        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    print ($"{x},{y},{z}");
                    GameObject clone = Instantiate (refObject, new Vector3 (transform.position.x + x, transform.position.y - y, transform.position.z + z), Quaternion.identity, transform);
                    clone.name = clone.name + $"_{x},{y},{z}";
                    yield return new WaitForSeconds (generationDelay);
                }
            }
        }
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

            foreach (GameObject go in gridList)
            {
                Destroy (go);
                gridList = null;
            }

        }
        catch (System.Exception e)
        {
            print (e);
        }

    }
}