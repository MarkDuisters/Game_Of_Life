using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SelectMouseOver : MonoBehaviour
{


    // Update is called once per frame
    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GetComponent<Cell_Behaviour>().alive = !GetComponent<Cell_Behaviour>().alive;
            GetComponent<Cell_Behaviour>().GetComponent<MeshRenderer>().enabled = GetComponent<Cell_Behaviour>().alive;
        }
    }
    void OnMouseEnter()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            GetComponent<Cell_Behaviour>().alive = !GetComponent<Cell_Behaviour>().alive;
            GetComponent<Cell_Behaviour>().GetComponent<MeshRenderer>().enabled = GetComponent<Cell_Behaviour>().alive;
        }
    }
}
