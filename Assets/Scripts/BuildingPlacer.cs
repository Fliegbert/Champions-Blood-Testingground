using UnityEngine;
using UnityEngine.EventSystems;

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
        // instantiate headquarters at the beginning of the game
        _placedBuilding = new Building(GameManager.instance.gameParameters.initialBuilding);
        _placedBuilding.SetPosition(GameManager.instance.startPosition);
        // link the data into the manager
        _placedBuilding.Transform.GetComponent<BuildingManager>().Initialize(_placedBuilding);
        _PlaceBuilding();
        // make sure we have no building selected when the player starts
        // to play
        _CancelPlacedBuilding();
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
                {   //from Building class
                    _placedBuilding.CheckValidPlacement();
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
            Globals.BUILDING_DATA[buildingDataIndex]
        );
        // link the data into the manager
        building.Transform.GetComponent<BuildingManager>().Initialize(building);
        _placedBuilding = building;
        _lastPlacementPosition = Vector3.zero;
    }

    void _CancelPlacedBuilding()
    {
        // destroy the "phantom" building
        Destroy(_placedBuilding.Transform.gameObject);
        _placedBuilding = null;
    }

    void _PlaceBuilding()
    {
        _placedBuilding.Place();
        if (_placedBuilding.CanBuy())
            _PreparePlacedBuilding(_placedBuilding.DataIndex);
        else
        {
            EventManager.TriggerEvent("PlaceBuildingOff");
            _placedBuilding = null;
        }
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");

        // update the dynamic nav mesh
        Globals.UpdateNavMeshSurface();
    }
}
