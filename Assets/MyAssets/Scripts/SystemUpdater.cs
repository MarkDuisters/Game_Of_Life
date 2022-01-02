using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SystemUpdater : MonoBehaviour
{
    [SerializeField]
    float updateDelay = 1;

    float oldTime;//Here we store the previoussytem time. This way we can always compare this value with the "up to date time" and check the difference.

   public UnityEvent _updateEvent;

    static public SystemUpdater instance;

    //For this system we are not going to use Unity's Update loop. Instead we are going to make our own itteration based on system time.
    //This system is dependent on the grids static instance to get its data, so preferably only run this during runtime.
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

        SystemUpdate();

    }


    // Update is called once per frame
    void SystemUpdate()
    {
        while (GenerateGrid.instance.gridInitialized)
        {

            if (Time.time >= oldTime + updateDelay)
            {
                _updateEvent.Invoke();

                oldTime = Time.time;

            }
        }

    }


}
