using System.Collections.Generic;

public class Globals
{

    public static int TERRAIN_LAYER_MASK = 1 << 3;

    //List of selected Units
    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();

    public static BuildingData[] BUILDING_DATA = new BuildingData[]
    {   //from BuildingData.cs public (BuildingData(string code, int healthpoints))
        new BuildingData("Hut", 100, new Dictionary<string, int>()
        {
          {"gold", 100}
        }),
        new BuildingData("Longhouse", 150, new Dictionary<string, int>()
        {
          {"gold", 150}
        }),
        new BuildingData("Oracle's Cavern", 120, new Dictionary<string, int>()
        {
          {"gold", 120}
        }),
        new BuildingData("Pit", 150, new Dictionary<string, int>()
        {
          {"gold", 150}
        }),
        new BuildingData("Savannah", 110, new Dictionary<string, int>()
        {
          {"gold", 110}
        }),
        new BuildingData("Pond", 120, new Dictionary<string, int>()
        {
          {"gold", 120}
        })
    };

    public static Dictionary<string, GameResource> GAME_RESOURCES = new Dictionary<string, GameResource>()
    {   //Dictionary{string, GameResource(string name, int initialAmount)}
        { "gold", new GameResource("Gold", 300) },
        { "wood", new GameResource("Wood", 300) },
        { "stone", new GameResource("Stone", 300) }
    };

}
