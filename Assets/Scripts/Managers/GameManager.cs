using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public GameParameters gameParameters;
    private Ray _ray;
    private RaycastHit _raycastHit;
    public static GameManager instance;
    public Vector3 startPosition;

    private void Awake()
    {
        DataHandler.LoadGameData();
        GetComponent<DayAndNightCycler>().enabled = gameParameters.enableDayAndNightCycle;
        Globals.NAV_MESH_SURFACE = GameObject.Find("Terrain").GetComponent<NavMeshSurface>();
        Globals.UpdateNavMeshSurface();
        _GetStartPosition();
        GameObject.Find("FogOfWar").SetActive(gameParameters.enableFOV);
    }

    public void Start()
    {
        instance = this;
    }

    private void Update()
    {
        _CheckUnitsNavigation();
    }

    private void _CheckUnitsNavigation()
    {
        if (Globals.SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            {
                foreach (UnitManager um in Globals.SELECTED_UNITS)
                    if (um.GetType() == typeof(CharacterManager))
                        ((CharacterManager)um).MoveTo(_raycastHit.point);
            }
        }
    }

    private void _GetStartPosition()
    {
        startPosition = Utils.MiddleOfScreenPointToWorld();
    }
}
