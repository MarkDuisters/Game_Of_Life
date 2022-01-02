#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DebugHandles : MonoBehaviour
{

    [SerializeField]
    Material debugMaterial;
    [SerializeField]
    bool enableDebug;
    // Update is called once per frame

    void OnDrawGizmos()
    {
        if (!enableDebug)
        {
            return;
        }

        Handles.Label(transform.position, GetComponent<Cell_Behaviour>().listIndex.ToString());

    }

    Vector4 ConvertVector(Vector3 input)
    {

        return new Vector4(input.x / input.magnitude, input.y / input.magnitude, input.x / input.magnitude, 0f);
    }
}
#endif