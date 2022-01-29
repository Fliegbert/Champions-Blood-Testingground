using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    public string code;
    public string unitName;
    public int healthpoints;
    public string description;
    public GameObject prefab;
    public List<ResourceValue> cost;
    public float fieldOfView;


    public bool CanBuy()
    {
        foreach (ResourceValue resource in cost)
            if (Globals.GAME_RESOURCES[resource.code].Amount < resource.amount)
                return false;
        return true;
    }
    public List<SkillData> skills = new List<SkillData>();
    [Header("General Sounds")]
    public AudioClip onSelectSound;
    public InGameResource[] canProduce;
}
