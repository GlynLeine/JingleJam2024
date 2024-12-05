using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsTargetInRange", story: "[Wisp] Get [Target]", category: "Action", id: "ce0910481e07ee35b1909accbfcad1a3")]
public partial class IsTargetInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<Wisp> Wisp;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    protected override Status OnUpdate()
    {
        Target.Value = Wisp.Value.Target;
        return Status.Success;
    }
}

