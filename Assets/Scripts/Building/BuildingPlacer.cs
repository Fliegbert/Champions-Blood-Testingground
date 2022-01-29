using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BuildingPlacer : MonoBehaviour
{
    //to find out which building is picked
    private Building _placedBuilding = null;
    private Ray _ray;
    private RaycastHit _raycastHit;
    private Vector3 _lastPlacementPosition;
    private UIManager _uiManager;

    private void Start()
    {
        SpawnBuilding(
            GameManager.instance.gameGlobalParameters.initialBuilding,
            GameManager.instance.gamePlayersParameters.myPlayerId,
            GameManager.instance.startPosition,
            new List<ResourceValue>()
            {
                new ResourceValue(InGameResource.Gold, 5),
                //new ResourceValue(InGameResource.Wood, 3),
                //new ResourceValue(InGameResource.Stone, 2),
            }
        );
    }

    public void SpawnBuilding(BuildingData data, int owner, Vector3 position)
    {
        SpawnBuilding(data, owner, position, new List<ResourceValue>() { });
    }
    public void SpawnBuilding(BuildingData data, int owner, Vector3 position, List<ResourceValue> production)
    {
        // keep a reference to the previously placed building, if there is one
        Building prevPlacedBuilding = _placedBuilding;

        // instantiate building
        _placedBuilding = new Building(data, owner, production);
        _placedBuilding.SetPosition(position);

        // ====> (we remove the Initialize() call)

        // finish up the placement
        _PlaceBuilding();
        // make sure we get rid of the placed building placeholder
        _CancelPlacedBuilding();

        // restore the previous state
        _placedBuilding = prevPlacedBuilding;
    }

    private void Awake()
    {
        _uiManager = GetComponent<UIManager>();
    }

    public void SelectPlacedBuilding(int buildingDataIndex)
    {
        _PreparePlacedBuilding(buildingDataIndex);
    }

    void Update()
    {
        if (GameManager.instance.gameIsPaused) return;
        if (_placedBuilding != null)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _CancelPlacedBuilding();
                return;
            }

            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            {
                _placedBuilding.SetPosition(_raycastHit.point);
                if (_lastPlacementPosition != _raycastHit.point)
                {
                    _placedBuilding.CheckValidPlacement();
                    Dictionary<InGameResource, int> prod = _placedBuilding.ComputeProduction();
                    EventManager.TriggerEvent(
                        "UpdatePlacedBuildingProduction",
                        new object[] { prod, _raycastHit.point }
                    );
                }
                _lastPlacementPosition = _raycastHit.point;
            }
               //from Building class
            if (
                _placedBuilding.HasValidPlacement &&
                Input.GetMouseButtonDown(0) &&
                !EventSystem.current.IsPointerOverGameObject()
            )
            {
                _PlaceBuilding();
            }
        }
    }

    void _PreparePlacedBuilding(int buildingDataIndex)
    {
        // destroy the previous "phantom" if there is one
        if (_placedBuilding != null && !_placedBuilding.IsFixed)
        {
            Destroy(_placedBuilding.Transform.gameObject);
        }
        Building building = new Building(
            Globals.BUILDING_DATA[buildingDataIndex],
            GameManager.instance.gamePlayersParameters.myPlayerId
        );
        // ====> (we remove the Initialize() call)
        _placedBuilding = building;
        _lastPlacementPosition = Vector3.zero;
        EventManager.TriggerEvent("PlaceBuildingOn");
    }

    void _CancelPlacedBuilding()
    {
        // destroy the "phantom" building
        Destroy(_placedBuilding.Transform.gameObject);
        _placedBuilding = null;
    }

    void _PlaceBuilding(bool canChain = true)
    {
        _placedBuilding.ComputeProduction();
        _placedBuilding.Place();
        if (canChain)
        {
            if (_placedBuilding.CanBuy())
                _PreparePlacedBuilding(_placedBuilding.DataIndex);
            else
            {
                EventManager.TriggerEvent("PlaceBuildingOff");
                _placedBuilding = null;
            }
        }
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");

        // update the dynamic nav mesh
        Globals.UpdateNavMeshSurface();
        EventManager.TriggerEvent("PlaySoundByName", "onBuildingPlacedSound");
    }
}
