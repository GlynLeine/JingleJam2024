using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsOrbiitng", story: "[Wisp] Can Orbit", category: "Conditions", id: "0170e405951ddde98ace6bf0830a1d39")]
public partial class IsOrbiitngCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Wisp> Wisp;

    public override bool IsTrue()
    {
        return Wisp.Value.CanOrbit();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
