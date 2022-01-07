using UnityEngine;

public class Healthbar : MonoBehaviour
{
    //rectTransform public variable is assigned in the inspector
    public RectTransform rectTransform;

    private Transform _target;
    private Vector3 _lastTargetPosition;
    private Vector2 _pos;
    private float _yOffset;

    // Function Purpose: if the targets position is not the _target position anymore or its _lastTargetPosition, use the buildings setposition function
    private void Update()
    {
        if (!_target || _lastTargetPosition == _target.position)
            return;
        SetPosition();
    }

    //Function Purpose: Link the data into the BuildingManager.
    public void Initialize(Transform target, float yOffset)
    {
        _target = target;
        _yOffset = yOffset;
    }

    //Function Purpose: set the position of the healthbar and also fills the _lastTargetPosition variable
    public void SetPosition()
    {
        if (!_target) return;
        _pos = Camera.main.WorldToScreenPoint(_target.position);
        //sets the height of the healthbar  + the yOffset
        _pos.y += _yOffset;
        rectTransform.anchoredPosition = _pos;
        _lastTargetPosition = _target.position;
    }
}
