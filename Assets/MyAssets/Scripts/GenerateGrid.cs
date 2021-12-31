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
    Vector3Int setDimensions = new Vector3Int (1, 1, 0);
    [SerializeField]
    float spacing = 1f;

    //privates to store our grid array
    [TableMatrix (SquareCells = true)]
    public GameObject[, , ] gridList = new GameObject[0, 0, 0];

    //private properties to make sure the strippipng system does not break our generated cube.
    MeshFilter refFilter;
    MeshRenderer refRenderer;
    Collider refCollider;

    // Start is called before the first frame update

    void Start ()
    {
        if (generateOnStart)
        {
            InitializeObject ();

            GG (setDimensions, spacing);

        }
    }

    // Generate a 3D grid with xyz mapped flat on the world axis.
    //Invoke a coroutine so we can control the spawnrate.
    [Command ("Generate_3D_grid")]
    [Button ("Generate 3D grid")]
    void GG (Vector3Int dimensions, float spacing)
    {
        gridList = new GameObject[dimensions.x, dimensions.y, dimensions.z];
        //We need Unity's editor version of the coroutine to properly spawn our objects.
#if UNITY_EDITOR
        EditorCoroutineUtility.StartCoroutine (GGCoroutine (dimensions, spacing), this);
#else
        StartCoroutine (GGCoroutine (dimensions, spacing));
#endif

    }

    IEnumerator GGCoroutine (Vector3Int dimensions, float spacing = 1)
    {
        InitializeObject ();
        RemoveGrid ();

        gridList = new GameObject[dimensions.x, dimensions.y, dimensions.z];

        //Little bit of a hacky solution, But this way our loop will keep running in the editor when the function gets invoked through Odin.

        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    //    print ($"{x},{y},{z}");
                    GameObject clone = Instantiate (refObject, new Vector3 (transform.position.x + x, transform.position.y - y, transform.position.z + z), Quaternion.identity, transform);
                    clone.name = clone.name + $"_{x},{y},{z}";
                    gridList[x, y, z] = clone;
                    yield return null;
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
        if (gridList.Length <= 0)
        {
            return;
        }

        try
        {

            //Little bit of a hacky solution, But this way our foreachloop will keep running in the editor when the function gets invoked through Odin.
            //This mainly because I was to lazy to set up a proper EditorCoroutine.
#if UNITY_EDITOR
            while (gridList.Length > 0)
            {
#endif
                int childCount = transform.childCount;
                GameObject[] getChildren = new GameObject[childCount];
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
                    gridList = new GameObject[0, 0, 0];
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

}