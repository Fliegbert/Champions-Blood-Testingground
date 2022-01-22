using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public GameGlobalParameters gameGlobalParameters;
    private Ray _ray;
    private RaycastHit _raycastHit;
    public static GameManager instance;
    public Vector3 startPosition;

    private void Awake()
    {
        DataHandler.LoadGameData();
        GetComponent<DayAndNightCycler>().enabled = gameGlobalParameters.enableDayAndNightCycle;

        Globals.NAV_MESH_SURFACE = GameObject.Find("Terrain").GetComponent<NavMeshSurface>();
        Globals.UpdateNavMeshSurface();

        // enable/disable FOV depending on game parameters
        GameObject.Find("FogOfWar").SetActive(gameGlobalParameters.enableFOV);

        _GetStartPosition();
    }

    public void Start()
    {
        instance = this;

        GameParameters[] gameParametersList =
          Resources.LoadAll<GameParameters>("ScriptableObjects/Parameters");
        foreach (GameParameters parameters in gameParametersList)
        {
            Debug.Log(parameters.GetParametersName());
            Debug.Log("> Fields shown in-game:");
            foreach (string fieldName in parameters.FieldsToShowInGame)
                Debug.Log($"    {fieldName}");
        }
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
