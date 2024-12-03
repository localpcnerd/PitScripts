using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pit Gamemode", menuName = "New Pit Gamemode")]
public class NewPitMode : ScriptableObject 
{
    public enum targetType
    {
        any,
        sosig,
        targets
    }

    [Header("Data")]
    public string displayName;
    [Header("Spawner")]
    public bool useItemSpawner;
    public string[] itemsToSpawn;
    [Header("Targets")]
    public bool hasFriendlyTargets;
    public targetType targetTypeSelection;
    public SosigEnemyID[] sosigIDs;
    public SosigEnemyID[] friendlySosigIDs;
    public int sosigIFF;
    public int friendlySosigIFF;
    public GameObject targetPrefab;
    public GameObject friendlyTargetPrefab;
    public float targetSpawnProbability;
    [Header("Map")]
    public bool useNavBlockers;
}