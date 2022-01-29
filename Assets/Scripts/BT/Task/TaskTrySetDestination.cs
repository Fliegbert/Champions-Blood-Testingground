using UnityEngine;

using BehaviorTree;

public class TaskTrySetDestinationOrTarget : Node
{
    CharacterManager _manager;

    private Ray _ray;
    private RaycastHit _raycastHit;

    public TaskTrySetDestinationOrTarget(CharacterManager manager) : base()
    {
        _manager = manager;
    }

    public override NodeState Evaluate()
    {
        if (_manager.IsSelected && Input.GetMouseButtonUp(1))
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            {
                Parent.Parent.SetData("destinationPoint", _raycastHit.point);
                _state = NodeState.SUCCESS;
                return _state;
            }
        }
        _state = NodeState.FAILURE;
        return _state;
    }
}
