using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BuildingData _buildingData;

    public void Initialize(BuildingData buildingData)
    {
        _buildingData = buildingData;
    }

    //Trigger event HoverBuildingButton with the custom data
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.TriggerTypedEvent("HoverBuildingButton", new CustomEventData(_buildingData));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.TriggerEvent("UnhoverBuildingButton"/*,__OnUnhoverBuildingButton*/);
    }
}
