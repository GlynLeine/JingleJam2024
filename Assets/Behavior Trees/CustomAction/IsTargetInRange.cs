using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsTargetInRange", story: "[Enemy] Get [Target]", category: "Action", id: "ce0910481e07ee35b1909accbfcad1a3")]
public partial class IsTargetInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    protected override Status OnUpdate()
    {
        Target.Value = Enemy.Value.Target;
        return Status.Success;
    }
}

