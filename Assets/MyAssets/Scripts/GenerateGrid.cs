using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
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

    //private properties to make sure the stripipng system does not break our generated cube.
    MeshFilter refFilter;
    MeshRenderer refRenderer;
    Collider refCollider;

    // Start is called before the first frame update
    void Start ()
    {
        if (generateOnStart)
        {
            InitializeObject ();
            if (ThreeDgrid)
            {
                StartCoroutine (GG3DCoroutine (setDimensions, spacing, generateDelay));

            }
            else
            {
                StartCoroutine (GG2DCoroutine ((Vector2Int) setDimensions, spacing, generateDelay));
            }
        }
    }

    // Generate a 2D grid with y mapped flat on the world z axis.
    //Invoke a coroutine so we can control the spawnrate.
    [Command ("Generate_2D_grid")]
    void GG2D (Vector2Int dimensions, float spacing, float generationDelay = 0f)
    {
        StartCoroutine (GG2DCoroutine (dimensions, spacing, generationDelay));
    }

    IEnumerator GG2DCoroutine (Vector2Int dimensions, float spacing, float generationDelay = 0f)
    {
        InitializeObject ();

        RemoveGrid ();

        for (int y = -dimensions.y / 2; y < dimensions.y / 2; y++)
        {
            for (int x = -dimensions.x / 2; x < dimensions.x / 2; x++)
            {
                print ($"{x},{y}");

                GameObject clone = Instantiate (refObject, new Vector3 (transform.position.x + x, transform.position.y - y, 0), Quaternion.identity, transform);
                clone.name = clone.name + $"_{x},{y}";
                yield return new WaitForSeconds (generationDelay);
            }
        }
    }

    // Generate a 3D grid with xyz mapped flat on the world axis.
    //Invoke a coroutine so we can control the spawnrate.
    [Command ("Generate_3D_grid")]
    void GG3D (Vector3Int dimensions, float spacing, float generationDelay = 0f)
    {
        StartCoroutine (GG3DCoroutine (dimensions, spacing, generationDelay));
    }

    IEnumerator GG3DCoroutine (Vector3Int dimensions, float spacing, float generationDelay = 0f)
    {
        InitializeObject ();
        RemoveGrid ();

        for (int z = -dimensions.z / 2; z < dimensions.z / 2; z++)
        {
            for (int y = -dimensions.y / 2; y < dimensions.y / 2; y++)
            {
                for (int x = -dimensions.x / 2; x < dimensions.x / 2; x++)
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

    [Command ("RemoveGrid")]
    [ContextMenu ("Remove grid.")]
    void RemoveGrid ()
    {

        for (int i = transform.childCount; i > 0;i--)
        {
            transform.GetChild (0);
        }

    }
}