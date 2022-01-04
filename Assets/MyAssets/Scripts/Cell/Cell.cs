using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Cell : MonoBehaviour
{


    //rules
    public bool alive;
    public int minNeigbors = 2;
    public int birthNeighbors = 3;
    public int maxNeighbors = 3;

    public int amountOfNeighbors = 0;

    public Vector3Int listIndex;




}