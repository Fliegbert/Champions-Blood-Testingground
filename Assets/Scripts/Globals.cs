public class Globals
{

    public static int TERRAIN_LAYER_MASK = 1 << 3;

    public static BuildingData[] BUILDING_DATA = new BuildingData[]
    {   //from BuildingData.cs public (BuildingData(string code, int healthpoints))
        new BuildingData("Hut", 100),
        new BuildingData("Longhouse", 150),
        new BuildingData("Oracle's Cavern", 120),
        new BuildingData("Pit", 150),
        new BuildingData("Planesscape", 110),
        new BuildingData("Pond", 120)
    };

}
