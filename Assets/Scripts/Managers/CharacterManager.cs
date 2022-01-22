using UnityEngine;
using UnityEngine.AI;

public class CharacterManager : UnitManager
{
    public NavMeshAgent agent;

    private Character _character;
    public override Unit Unit
    {
        get { return _character; }
        set { _character = value is Character ? (Character)value : null; }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);
        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            contextualSource.PlayOneShot(((CharacterData)Unit.Data).onMoveInvalidSound);
            return;
        }

        agent.destination = targetPosition;
        contextualSource.PlayOneShot(((CharacterData)Unit.Data).onMoveValidSound);
    }
}
