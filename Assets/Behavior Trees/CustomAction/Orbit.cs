using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Orbit", story: "[Agent] Orbits [Target]", category: "Action/Navigation", id: "695fd3be485ad04b8a0b19e6804fc230")]
public partial class OrbitAction : Action
{
    [SerializeReference] public BlackboardVariable<TargetDetector> TargetDetector;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    private float m_timer = 0;
    private int m_direction = 0;
    private float m_enteredDistance = 0.0f;
    private NavMeshAgent m_navAgent;

    protected override Status OnStart()
    {
        m_direction = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        m_navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        m_enteredDistance = (TargetDetector.Value.PositionOnEnter).magnitude - 1.0f;
        m_timer = Mathf.Deg2Rad * Vector3.Angle(Vector3.right, (TargetDetector.Value.PositionOnEnter).normalized);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (TargetDetector.Value.IsWithinRadius)
        {
            m_navAgent.enabled = false;
            m_timer += Time.deltaTime * m_direction;
            var distance = Mathf.Max(3.0f, m_enteredDistance);
            float x = Mathf.Cos(m_timer) * distance;
            float z = Mathf.Sin(m_timer) * distance;
            Vector3 pos = Target.Value.position + new Vector3(x, m_navAgent.baseOffset, z);
            m_navAgent.transform.position = Vector3.Slerp(m_navAgent.transform.position, pos, Time.deltaTime * m_navAgent.speed);
            return Status.Running;
        }

        m_direction = 0;
        m_timer = 0;
        m_enteredDistance = 0;
        m_navAgent.enabled = true;
        m_navAgent.SetDestination(Target.Value.position);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

