using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
using Sodalite.Api;
using Sodalite.Utilities;

public class NewPitZone : MonoBehaviour 
{
	public Transform[] spawnPoints;
    public Transform[] friendlySpawnPoints;
    public GameObject navBlocker;
	public bool isZoneActive;
	[HideInInspector] public List<NewPitSosig> spawnedSosigs = new List<NewPitSosig>();
    [HideInInspector] public List<NewPitTarget> spawnedTargets = new List<NewPitTarget>();
    [HideInInspector] public List<NewPitSosig> friendlySpawnedSosigs = new List<NewPitSosig>();
    [HideInInspector] public List<NewPitTarget> friendlySpawnedTargets = new List<NewPitTarget>();
    public SosigEnemyID[] sosigIDs;
    public SosigEnemyID[] friendlySosigIDs;
	public NewPitController npc;

    private readonly SosigAPI.SpawnOptions spawnOptions = new SosigAPI.SpawnOptions
    {
        SpawnState = Sosig.SosigOrder.Disabled,
        SpawnActivated = true,
        EquipmentMode = SosigAPI.SpawnOptions.EquipmentSlots.Primary,
        SpawnWithFullAmmo = true
    };

    public void StartZone() 
	{
		isZoneActive = true;
		var rand = sosigIDs.GetRandom();
        var frand = friendlySosigIDs.GetRandom();
		foreach(var t in spawnPoints) 
		{
			if(npc.useTargets)
			{
				SpawnTarget(t, false, npc.targetPrefab);
			}
			else
			{
                SpawnSosig(t, rand, false);
            }
		}

        foreach(var t in friendlySpawnPoints)
        {
            if (npc.useTargets)
            {
                SpawnTarget(t, true, npc.friendlyTargetPrefab);
            }
            else
            {
                SpawnSosig(t, frand, true);
            }
        }
        navBlocker.SetActive(true);
	}

	public void FinishZone()
	{
        if(npc.useTargets)
        {
            foreach (var npt in friendlySpawnedTargets)
            {
                npt.isHit = true; //so any non-hit friendly targets auto clear themselves
                friendlySpawnedTargets.Remove(npt);
            }
        }
        else
        {
            foreach (var nps in friendlySpawnedSosigs)
            {
                nps.sos.ClearSosig();
                friendlySpawnedSosigs.Remove(nps);
            }
        }

        isZoneActive = false;
        navBlocker.SetActive(false);
		npc.zoneInd++;
		if(npc.zoneInd < npc.zones.Length)
		{
            npc.zones[npc.zoneInd].StartZone();
        }
		npc.PlayClip(0);
	}

    private void SpawnSosig(Transform sp, SosigEnemyID id, bool friendly)
    {
        spawnOptions.IFF = 0;
        Transform spawnPos = sp;

        var sosig = SosigAPI.Spawn(IM.Instance.odicSosigObjsByID[id], spawnOptions, spawnPos.position, spawnPos.rotation);

        var nps = sosig.gameObject.AddComponent<NewPitSosig>();
        nps.isFriendly = friendly;
        if (friendly)
        {
            friendlySpawnedSosigs.Add(nps);
        }
        else
        {
            spawnedSosigs.Add(nps);
        }
    }

	private void SpawnTarget(Transform sp, bool friendly, GameObject target)
	{
		GameObject go = Instantiate(target, sp.position, sp.rotation, sp);
        var npt = go.GetComponent<NewPitTarget>();
        npt.isFriendly = friendly;

        if(friendly)
        {
            friendlySpawnedTargets.Add(npt);
        }
        else
        {
            spawnedTargets.Add(npt);
        }
    }

    public void Update() 
	{
		if(isZoneActive) 
		{
			if(npc.useTargets)
			{
                foreach (var npt in spawnedTargets)
                {
                    if (npt.isHit)
                    {
                        spawnedTargets.Remove(npt);
                        break;
                    }
                }

                foreach (var npt in friendlySpawnedTargets)
                {
                    if (npt.isHit)
                    {
                        friendlySpawnedTargets.Remove(npt);
                        break;
                    }
                }

                if (spawnedTargets.Count <= 0 && isZoneActive)
                {
                    FinishZone();
                }
            }
			else
			{
                foreach (var nps in spawnedSosigs)
                {
                    if (nps.isDead)
                    {
                        nps.sos.ClearSosig();
                        spawnedSosigs.Remove(nps);
                        break;
                    }
                }

                foreach (var nps in friendlySpawnedSosigs)
                {
                    if (nps.isDead)
                    {
                        nps.sos.ClearSosig();
                        friendlySpawnedSosigs.Remove(nps);
                        break;
                    }
                }

                if (spawnedSosigs.Count <= 0 && isZoneActive)
                {
                    FinishZone();
                }
            }
		}
	}
}
