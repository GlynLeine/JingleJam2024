using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WispAttack", story: "[Wisp] attacks", category: "Action", id: "6d8ec7e692fa310b843026cc4c06ac06")]
public partial class WispAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Wisp> Wisp;
    protected override Status OnStart()
    {
        Wisp.Value.Attack();
        return Status.Success;
    }

}

