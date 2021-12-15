using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingPlacement
{
  VALID,
  INVALID,
  FIXED
};

public class Building
{

    private BuildingData _data;
    private Transform _transform;
    private int _currentHealth;
    private BuildingPlacement _placement;
    private List<Material> _materials;
    private BuildingManager _buildingManager;

    //Creating new Building Instance with Building data reference
    public Building(BuildingData data)
    {
        _data = data;
        _currentHealth = data.HP;

        GameObject g = GameObject.Instantiate(
            Resources.Load($"Prefabs/Units/Buildings/{_data.Code}")
        ) as GameObject;
        _transform = g.transform;

        // set building mode as "valid" placement (enum)
        _placement = BuildingPlacement.VALID;

        _materials = new List<Material>();
        foreach (Material material in _transform.Find("Mesh").GetComponent<Renderer>().materials)
        {
            _materials.Add(new Material(material));
        }

        // (set the materials to match the "valid" initial state)
        _buildingManager = g.GetComponent<BuildingManager>();
        _placement = BuildingPlacement.VALID;
        SetMaterials();
    }

    public void SetMaterials() { SetMaterials(_placement); }
    public void SetMaterials(BuildingPlacement placement)
    {
        List<Material> materials;
        if (placement == BuildingPlacement.VALID)
        {
            Material refMaterial = Resources.Load("Materials/Valid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if (placement == BuildingPlacement.INVALID)
        {
            Material refMaterial = Resources.Load("Materials/Invalid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if (placement == BuildingPlacement.FIXED)
        {
            materials = _materials;
        }
        else
        {
            return;
        }
        _transform.Find("Mesh").GetComponent<Renderer>().materials = materials.ToArray();
    }

    //Set position of the GameObject in the scene
    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public void Place()
    {
        // set placement state
        _placement = BuildingPlacement.FIXED;
        // change building materials
        SetMaterials();
        // remove "is trigger" flag from box collider to allow for collisions with units
        _transform.GetComponent<BoxCollider>().isTrigger = false;

        // update game resources: remove the cost of the building
        // from each game resource
        foreach (KeyValuePair<string, int> pair in _data.Cost)
        {
            Globals.GAME_RESOURCES[pair.Key].AddAmount(-pair.Value);
        }
    }

    public bool CanBuy()
    {
        return _data.CanBuy();
    }

    public void CheckValidPlacement()
    {
        if (_placement == BuildingPlacement.FIXED) return;
        _placement = _buildingManager.CheckPlacement()
            ? BuildingPlacement.VALID
            : BuildingPlacement.INVALID;
    }

    public string Code { get => _data.Code; }
    public Transform Transform { get => _transform; }
    public int HP { get => _currentHealth; set => _currentHealth = value; }
    public int MaxHP { get => _data.HP; }
    public bool IsFixed { get => _placement == BuildingPlacement.FIXED; }
    public bool HasValidPlacement { get => _placement == BuildingPlacement.VALID; }

    //“computed” property that gives us the index of the abstract building type data instance in the global list
    public int DataIndex
    {
        get {
            for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
            {
                if (Globals.BUILDING_DATA[i].Code == _data.Code)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
