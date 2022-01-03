using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

//When adding a system udpate to our object, we always need a grid to pull data from.
[RequireComponent(typeof(GenerateGrid))]
public class SystemUpdater : MonoBehaviour
{
    [SerializeField]
    GenerateGrid getGrid;




    [SerializeField]
    float updateDelay = 1;

    [SerializeField]
    int generation = 0;
    int generationCount = 0;


    float oldTime;//Here we store the previoussytem time. This way we can always compare this value with the "up to date time" and check the difference.
    [HideInInspector]
    public delegate void _updateEvent();
    public _updateEvent ONupdateEvent;


    static public SystemUpdater instance;


    //This system is dependent on the grid to get its data, so preferably only run this during runtime.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


        getGrid = GetComponent<GenerateGrid>();


        getGrid.Initialize(getGrid.setDimensions, getGrid.spacing, getGrid.generateOnStart, getGrid.removeGridOnInitialize, getGrid.randomCellOnInitialize, ((int)getGrid.selectUpdateMode));


    }

    void Update()
    {
        if (!getGrid.gridInitialized)
        {
            return;
        }
        else
        {

            SystemUpdate();
        }

    }
    // Update is called once per frame
    void SystemUpdate()
    {
        //        print("updating");
        if (Time.time >= oldTime + updateDelay)
        {
            generationCount++;
            generation = generationCount;

            ONupdateEvent();

            oldTime = Time.time;


        }



    }


}
