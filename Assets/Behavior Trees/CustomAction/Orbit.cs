using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using Unity.AppUI.Core;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Orbit", story: "[Wisp] Orbit [Target]", category: "Action/Navigation", id: "695fd3be485ad04b8a0b19e6804fc230")]
public partial class OrbitAction : Action
{
    [SerializeReference] public BlackboardVariable<Wisp> Wisp;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    private int m_direction = 0;

    protected override Status OnStart()
    {
        if (m_direction == 0)
            m_direction = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

        if (Wisp.Value.Timer == 0)
            Wisp.Value.Timer = Mathf.Deg2Rad * Vector3.Angle(Vector3.right, (Wisp.Value.transform.position - Target.Value.transform.position).normalized);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Wisp.Value.NavAgent.enabled = false;
        Wisp.Value.Timer += Time.deltaTime * m_direction * Wisp.Value.MoveSpeed;
        float x = Mathf.Cos(Wisp.Value.Timer) * (Wisp.Value.OrbitRadius - 1f);
        float z = Mathf.Sin(Wisp.Value.Timer) * (Wisp.Value.OrbitRadius - 1f);
        Vector3 pos = Target.Value.position + new Vector3(x, 0, z);
        Wisp.Value.transform.position = Vector3.Slerp(Wisp.Value.transform.position, pos, Time.deltaTime * Wisp.Value.MoveSpeed);
        return Status.Success;
    }
}

