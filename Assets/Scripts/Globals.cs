using System.Collections.Generic;
using UnityEngine.AI;

public enum InGameResource
{
    Gold,
    Wood,
    Stone
}

public class Globals
{

    public static int TERRAIN_LAYER_MASK = 1 << 3;

    //List of selected Units
    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();

    public static BuildingData[] BUILDING_DATA;

    public static Dictionary<InGameResource, GameResource> GAME_RESOURCES = new Dictionary<InGameResource, GameResource>()
    {
        { InGameResource.Gold, new GameResource("Gold", 1000) },
        { InGameResource.Wood, new GameResource("Wood", 1000) },
        { InGameResource.Stone, new GameResource("Stone", 1000) }
    };

    public static NavMeshSurface NAV_MESH_SURFACE;

    public static void UpdateNavMeshSurface()
    {
        NAV_MESH_SURFACE.UpdateNavMesh(NAV_MESH_SURFACE.navMeshData);
    }

    public static int FLAT_TERRAIN_LAYER_MASK = 1 << 10;
    public static int UNIT_MASK = 1 << 12;
    public static int TREE_MASK = 1 << 13;
    public static int ROCK_MASK = 1 << 14;
}
