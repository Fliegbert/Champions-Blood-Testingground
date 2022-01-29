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
    public float producingRate = 3f; // in seconds


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
    }

    private void Update()
    {
        if (gameIsPaused) return;
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
    }

    private void _OnResumeGame()
    {
        gameIsPaused = false;
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
//#if !UNITY_EDITOR
        DataHandler.SaveGameData();
//#endif
    }
}
