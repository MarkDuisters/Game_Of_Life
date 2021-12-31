using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Cell : MonoBehaviour
{

    [SerializeField]
    bool alive;
    [SerializeField]
    int amountOfNeighbors = 0;
    [SerializeField]
    Vector3Int listIndex;

}