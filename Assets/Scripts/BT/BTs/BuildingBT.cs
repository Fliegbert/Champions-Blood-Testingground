using System.Collections.Generic;

using BehaviorTree;

[UnityEngine.RequireComponent(typeof(BuildingManager))]
public class BuildingBT : Tree
{
    BuildingManager manager;

    private void Awake()
    {
        manager = GetComponent<BuildingManager>();
    }

    protected override Node SetupTree()
    {
        Node _root;

        _root = new Sequence(new List<Node> {
            new CheckUnitIsMine(manager),
            new Timer(
                GameManager.instance.producingRate,
                new List<Node>()
                {
                    new TaskProduceResources(manager)
                },
                delegate
                {
                    EventManager.TriggerEvent("UpdateResourceTexts");
                }
            )
        });

        return _root;
    }
}
