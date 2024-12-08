using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToRandomPositionAwayFromTarget", story: "[Enemy] moves to random point away from [Target]", category: "Action", id: "d921fce4f986ddd08d409d08a58ff232")]
public partial class MoveToRandomPositionAwayFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    private float m_PreviousStoppingDistance;
    private float m_DistanceThreshold = 0.2f;
    private float SlowDownDistance = 1.0f;
    private NavMeshAgent m_NavMeshAgent;

    protected override Status OnStart()
    {
        if (Enemy.Value == null)
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Enemy.Value == null)
        {
            return Status.Failure;
        }

        if (Enemy.Value.Target == null)
        {
            return Status.Failure;
        }

        if (m_NavMeshAgent == null)
        {
            Vector3 agentPosition, locationPosition;
            float distance = GetDistanceToLocation(out agentPosition, out locationPosition);
            if (distance <= m_DistanceThreshold)
            {
                return Status.Success;
            }

            float speed = m_NavMeshAgent.speed;

            if (SlowDownDistance > 0.0f && distance < SlowDownDistance)
            {
                float ratio = distance / SlowDownDistance;
                speed = Mathf.Max(0.1f, m_NavMeshAgent.speed * ratio);
            }

            Vector3 toDestination = locationPosition - agentPosition;
            toDestination.y = 0.0f;
            toDestination.Normalize();
            agentPosition += toDestination * (speed * Time.deltaTime);
            Enemy.Value.transform.position = agentPosition;

            // Look at the target.
            Enemy.Value.transform.forward = toDestination;
        }
        else if (m_NavMeshAgent.IsNavigationComplete())
        {
            GenerateRandomPosition();
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_NavMeshAgent != null)
        {
            if (m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.ResetPath();
            }
            m_NavMeshAgent.stoppingDistance = m_PreviousStoppingDistance;
        }

        m_NavMeshAgent = null;
    }

    protected override void OnDeserialize()
    {
        Initialize();
    }

    private Status Initialize()
    {
        if (GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition) <= m_DistanceThreshold)
        {
            return Status.Failure;
        }

        // If using a navigation mesh, set target position for navigation mesh agent.
        m_NavMeshAgent = Enemy.Value.GetComponentInChildren<NavMeshAgent>();
        if (m_NavMeshAgent != null)
        {
            if (m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.ResetPath();
            }
            m_PreviousStoppingDistance = m_NavMeshAgent.stoppingDistance;
            m_NavMeshAgent.stoppingDistance = m_DistanceThreshold;
            m_NavMeshAgent.SetDestination(locationPosition);
        }

        return Status.Running;
    }

    private float GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition)
    {
        agentPosition = Enemy.Value.transform.position;
        locationPosition = GenerateRandomPosition();
        return Vector3.Distance(new Vector3(agentPosition.x, locationPosition.y, agentPosition.z), locationPosition);
    }

    private Vector3 GenerateRandomPosition()
    {
        var diff = Target.Value.position - Enemy.Value.transform.position;
        var direction = -diff.normalized;
        var randomPos = Enemy.Value.transform.position + direction * 10.0f;
        return randomPos;
    }
}


public static class NavMeshExtensions
{
    public static bool IsNavigationComplete(this NavMeshAgent agent)
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }
}


