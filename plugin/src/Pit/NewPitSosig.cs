using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPitSosig : MonoBehaviour 
{
    public Sosig sos;
    public bool isDead = false;
    public bool isFriendly;

    public void Start()
    {
        sos  = GetComponent<Sosig>();
    }

    public void Update()
    {
        if(sos.BodyState == Sosig.SosigBodyState.Dead)
        {
            isDead = true;
        }
    }
}
