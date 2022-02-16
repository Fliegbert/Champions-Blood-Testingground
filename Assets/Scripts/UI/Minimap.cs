using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Minimap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 terrainSize;
    private Vector2 _uiSize;

    private Vector2 _lastPointerPosition;
    private bool _dragging = false;

    public RectTransform minimapContainerRectTransform;
    private Vector2 _offset;

    private void Start()
    {
        _uiSize = GetComponent<RectTransform>().sizeDelta;
        _lastPointerPosition = Input.mousePosition;
        _offset = minimapContainerRectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (!_dragging) return;

        Vector2 delta = (Vector2) Input.mousePosition - _lastPointerPosition;
        _lastPointerPosition = Input.mousePosition;

        if (delta.magnitude > Mathf.Epsilon)
        {
            Vector2 uiPos = ( new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _offset) / GameManager.instance.canvasScaleFactor;
            Vector3 realPos = new Vector3(
                uiPos.x / _uiSize.x * terrainSize.x,
                0f,
                uiPos.y / _uiSize.y * terrainSize.y
            );
            EventManager.TriggerEvent("MoveCamera", realPos);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _dragging = false;
    }
}
