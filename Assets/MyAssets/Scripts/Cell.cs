using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Cell : MonoBehaviour
{

    [SerializeField]
    public bool alive;
    [SerializeField]
    public int amountOfNeighbors = 0;
    [SerializeField]
    public Vector3Int listIndex;

}