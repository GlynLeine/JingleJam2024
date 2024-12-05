using System;
using Unity.Behavior;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Can Orbit", story: "[Wisp] Can Orbit", category: "Conditions", id: "c10328a9655f19465936737e0ab44c72")]
public partial class CanOrbitCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Wisp> Wisp;

    public override bool IsTrue()
    {

        Wisp.Value.NavAgent.stoppingDistance = Wisp.Value.OrbitRadius;
        Wisp.Value.NavAgent.enabled = !Wisp.Value.IsWithinOrbitRadius;
        return Wisp.Value.IsWithinOrbitRadius;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
