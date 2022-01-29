using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameGlobalParameters gameGlobalParameters;
    public GamePlayersParameters gamePlayersParameters;
    private Ray _ray;
    private RaycastHit _raycastHit;
    public static GameManager instance;
    public Vector3 startPosition;

    [HideInInspector]
    public bool gameIsPaused;
    public GameObject fov;

    [HideInInspector]
    public List<Unit> ownedProducingUnits = new List<Unit>();
    private float _producingRate = 3f; // in seconds
    private Coroutine _producingResourcesCoroutine = null;


    private void Awake()
    {
        DataHandler.LoadGameData();
        GetComponent<DayAndNightCycler>().enabled = gameGlobalParameters.enableDayAndNightCycle;

        Globals.NAV_MESH_SURFACE = GameObject.Find("Terrain").GetComponent<NavMeshSurface>();
        Globals.UpdateNavMeshSurface();

        // enable/disable FOV depending on game parameters
        GameObject.Find("FogOfWar").SetActive(gameGlobalParameters.enableFOV);

        _GetStartPosition();
        gameIsPaused = false;
        fov.SetActive(gameGlobalParameters.enableFOV);
    }

    public void Start()
    {
        instance = this;
        _producingResourcesCoroutine = StartCoroutine("_ProducingResources");
    }

    private void Update()
    {
        if (gameIsPaused) return;
        _CheckUnitsNavigation();
    }

    private void OnEnable()
    {
        EventManager.AddListener("PauseGame", _OnPauseGame);
        EventManager.AddListener("ResumeGame", _OnResumeGame);
        EventManager.AddListener("UpdateGameParameter:enableDayAndNightCycle", _OnUpdateDayAndNightCycle);
        EventManager.AddListener("UpdateGameParameter:enableFOV", _OnUpdateFOV);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("PauseGame", _OnPauseGame);
        EventManager.RemoveListener("ResumeGame", _OnResumeGame);
        EventManager.RemoveListener("UpdateGameParameter:enableDayAndNightCycle", _OnUpdateDayAndNightCycle);
        EventManager.RemoveListener("UpdateGameParameter:enableFOV", _OnUpdateFOV);
    }

    private void _OnPauseGame()
    {
        gameIsPaused = true;
        if (_producingResourcesCoroutine != null)
        {
            StopCoroutine(_producingResourcesCoroutine);
            _producingResourcesCoroutine = null;
        }
    }

    private void _OnResumeGame()
    {
        gameIsPaused = false;
        if (_producingResourcesCoroutine == null)
            _producingResourcesCoroutine = StartCoroutine("_ProducingResources");
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

    private void _OnUpdateDayAndNightCycle(object data)
    {
        bool dayAndNightIsOn = (bool)data;
        GetComponent<DayAndNightCycler>().enabled = dayAndNightIsOn;
    }
    private void _OnUpdateFOV(object data)
    {
        bool fovIsOn = (bool)data;
        fov.SetActive(fovIsOn);
    }

    private void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        DataHandler.SaveGameData();
#endif
    }

    private IEnumerator _ProducingResources()
    {
        while (true)
        {
            foreach (Unit unit in ownedProducingUnits)
                unit.ProduceResources();
            EventManager.TriggerEvent("UpdateResourceTexts");
            yield return new WaitForSeconds(_producingRate);
        }
    }
}
