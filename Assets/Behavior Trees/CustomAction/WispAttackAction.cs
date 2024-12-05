using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WispAttack", story: "[Wips] does Attack", category: "Action", id: "6d8ec7e692fa310b843026cc4c06ac06")]
public partial class WispAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Wisp> Wips;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

