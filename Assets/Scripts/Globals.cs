using System.Collections.Generic;
using UnityEngine.AI;


public class Globals
{

    public static int TERRAIN_LAYER_MASK = 1 << 3;

    //List of selected Units
    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();

    public static BuildingData[] BUILDING_DATA;

    public static Dictionary<string, GameResource> GAME_RESOURCES = new Dictionary<string, GameResource>()
    {   //Dictionary{string, GameResource(string name, int initialAmount)}
        { "gold", new GameResource("Gold", 300) },
        { "blood", new GameResource("Blood", 0) },
        { "bronze", new GameResource("Bronze", 100) }
    };

    public static NavMeshSurface NAV_MESH_SURFACE;

    public static void UpdateNavMeshSurface()
    {
        NAV_MESH_SURFACE.UpdateNavMesh(NAV_MESH_SURFACE.navMeshData);
    }

    public static int FLAT_TERRAIN_LAYER_MASK = 1 << 10;
}
