using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//When adding a system udpate to our object, we always need a grid to pull data from.
[RequireComponent(typeof(GenerateGrid))]
public class SystemUpdater : MonoBehaviour
{
    [SerializeField]
    GenerateGrid getGrid;

    [SerializeField]
    float updateDelay = 1;

    float oldTime;//Here we store the previoussytem time. This way we can always compare this value with the "up to date time" and check the difference.

    public UnityEvent _updateEvent = new UnityEvent();

    static public SystemUpdater instance;


    //This system is dependent on the grid to get its data, so preferably only run this during runtime.
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (_updateEvent == null)
        {
            _updateEvent = new UnityEvent();

        }
        getGrid = GetComponent<GenerateGrid>();

        getGrid.Initialize();


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
            print("invoked event");
            _updateEvent.Invoke();

            oldTime = Time.time;

        }



    }


}
