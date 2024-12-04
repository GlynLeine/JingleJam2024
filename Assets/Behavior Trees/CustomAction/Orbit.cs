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
    private float m_enteredDistance = 0.0f;
    private NavMeshAgent m_navAgent;

    protected override Status OnStart()
    {
        if (m_direction == 0)
            m_direction = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        if (m_navAgent == null)
            m_navAgent = Wisp.Value.GetComponent<NavMeshAgent>();

        m_enteredDistance = (Wisp.Value.PositionOnEnter).magnitude - 1.0f;
        if (Wisp.Value.Timer == 0)
            Wisp.Value.Timer = Mathf.Deg2Rad * Vector3.Angle(Vector3.right, (Wisp.Value.PositionOnEnter).normalized);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        m_navAgent.enabled = false;
        Wisp.Value.Timer += Time.deltaTime * m_direction * Wisp.Value.MoveSpeed;
        var distance = Mathf.Max(4.0f, m_enteredDistance);
        float x = Mathf.Cos(Wisp.Value.Timer) * distance;
        float z = Mathf.Sin(Wisp.Value.Timer) * distance;
        Vector3 pos = Target.Value.position + new Vector3(x, 0, z);
        m_navAgent.transform.position = Vector3.Slerp(m_navAgent.transform.position, pos, Time.deltaTime * m_navAgent.speed);
        return Status.Success;
    }

    protected override void OnEnd()
    {
        m_navAgent.enabled = true;
    }
}

