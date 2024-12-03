using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPitTrigger : MonoBehaviour 
{
    public bool isStart; //false for end of course
    public NewPitController npc;
    [HideInInspector] public bool hasRun = false;

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "GameController") //triggered when player enters
        {
            if (isStart)
            {
                StartCourse();
            }
            else
            {
                EndCourse();
            }
        }
    }

    public void StartCourse()
    {
        if(!hasRun)
        {
            npc.OnStart();
        }

        hasRun = true;
    }

    public void EndCourse()
    {
        if(!hasRun)
        {
            npc.OnFinish();
        }
        
        hasRun = true;
    }
}
