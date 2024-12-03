using Atlas;
using Atlas.MappingComponents.Sandbox;
using FistVR;
using Sodalite.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewPitController : MonoBehaviour 
{
    [Header("Timer")]
    public bool runTimer;
    public float curTime;
    public float finalTime;
    public float savedBestTimeSosig;
    public float savedBestTimeTarget;

    [Header("UI")]
    public Text[] timers;
    public Text categoryName;
    public Text categoryRecord;

    [Header("Zones")]
    public NewPitZone[] zones;
    public int zoneInd;
    public int activeZone;

    [Header("Triggers")]
    public NewPitTrigger[] triggers;
    public bool canFinishCourse;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip zoneFinishClip;
    public AudioClip triggerStartClip;
    public AudioClip triggerFinishClip;

    [Header("Targets")]
    public GameObject targetPrefab;
    public GameObject friendlyTargetPrefab;
    public bool useTargets;

    [Header("Modes")]
    public NewPitMode[] modes;
    public NewPitMode selectedMode;
    private int selectedModeIndex;
    private float currentRecordTime;

    [Header("Spawner")]
    public GameObject itemSpawner;
    public Transform[] itemSpawnPoints;

    public void Start()
    {
        savedBestTimeSosig = PlayerPrefs.GetFloat("savedBestTimeSosig", 6030);
        savedBestTimeTarget = PlayerPrefs.GetFloat("savedBestTimeTarget", 6030);
        GM.CurrentPlayerBody.SetPlayerIFF(0);
    }

    public void OnStart()
    {
        curTime = 0;
        finalTime = 0;
        runTimer = true;
        zoneInd = 0;
        zones[zoneInd].StartZone();
        PlayClip(1);
    }

    public void OnFinish()
    {
        PlayClip(2);
        runTimer = false;
        finalTime = curTime;

        StartCoroutine(delayResetFinish());
        UpdateSavedScores();
    }

    public void UpdateUI()
    {
        foreach(var t in timers) 
        {
            int minutes = Mathf.FloorToInt(curTime / 60f);
            int seconds = Mathf.FloorToInt(curTime - minutes * 60);
            int ms = Mathf.FloorToInt((curTime % 1f) * 100);

            string time = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, ms);
            t.text = time;
        }

        categoryName.text = selectedMode.displayName;
    }

    public void ResetSavedTimes()
    {
        PlayerPrefs.DeleteKey("savedBestTimeSosig");
        savedBestTimeSosig = 3060;
        PlayerPrefs.DeleteKey("savedBestTimeTarget");
        savedBestTimeTarget = 3060;
    }

    public void PlayClip(int ind) 
    {
        switch(ind)
        {
            case 0:
                source.PlayOneShot(zoneFinishClip); break;
            case 1:
                source.PlayOneShot(triggerStartClip); break;
            case 2:
                source.PlayOneShot(triggerFinishClip); break;
        }
            
    }

    public void UpdateSavedScores()
    {
        if(useTargets)
        {
            if (finalTime < savedBestTimeTarget)
            {
                savedBestTimeTarget = finalTime;
                PlayerPrefs.SetFloat("savedBestTimeTarget", savedBestTimeTarget);
            }
        }
        else
        {
            if (finalTime < savedBestTimeSosig)
            {
                savedBestTimeSosig = finalTime;
                PlayerPrefs.SetFloat("savedBestTimeSosig", savedBestTimeSosig);
            }
        }
    }

    private IEnumerator SpawnAsync(string id)
    {
        //little delay to stop items from bouncing into each other and clipping and other nasty stuff
        var rand = UnityEngine.Random.Range(0, 0.5f);
        yield return new WaitForSeconds(rand);

        var t = itemSpawnPoints.GetRandom();

        if (!IM.OD.TryGetValue(id, out var obj))
        {
            Debug.LogWarning("No object found with id '" + id + "'.");
            yield break;
        }

        AnvilCallback<GameObject> callback = obj.GetGameObjectAsync();
        yield return callback;
        Instantiate(callback.Result, t.position, t.rotation).SetActive(true);
    }

    public void UpdateModeData()
    {
        selectedMode = modes[selectedModeIndex];

        //set up parameters for mode and what to do n fun stuff here
        //ui
        
        //items
        if(selectedMode.useItemSpawner)
        {
            itemSpawner.SetActive(true);
        }
        else
        {
            itemSpawner.SetActive(false);   
            foreach(var t in selectedMode.itemsToSpawn)
            {
                StartCoroutine(SpawnAsync(t));
            }
        }
    }

    public void CreateRecordData()
    {
        if(useTargets)
        {
            var t = selectedMode.displayName + "-Target";
        }
        else
        {
            var s = selectedMode.displayName + "-Sosig";
        }
        currentRecordTime

        float record = PlayerPrefs.GetFloat()
    }

    public void CycleSelectedMode(bool up)
    {
        if(up)
        {
            selectedModeIndex++;
            if(selectedModeIndex > modes.Length)
            {
                selectedModeIndex = 0;
            }
            UpdateModeData();
        }
        else
        {
            selectedModeIndex--;
            if(selectedModeIndex < modes.Length)
            {
                selectedModeIndex = modes.Length;
            }
            UpdateModeData();
        }
    }

    public IEnumerator delayResetFinish()
    {
        yield return new WaitForSeconds(1f);

        foreach (var trig in triggers)
        {
            trig.hasRun = false;
        }
    }

    public void Update()
    {
        UpdateUI();

        if(runTimer)
        {
            curTime += Time.deltaTime;
        }
    }
}
