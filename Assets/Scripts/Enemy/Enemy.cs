using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected EnemyData m_enemyData;
    [SerializeField]
    protected TargetDetector m_targetDetector;
    protected BehaviorGraphAgent m_graphAgent;
    protected NavMeshAgent m_navAgent;
    public NavMeshAgent NavAgent => m_navAgent;
    protected bool m_isDead = false;

    protected abstract void Initialize();
    protected abstract void OnUpdate();
    protected abstract void OnDeath();

    protected void OnEnable()
    {
        if (m_graphAgent == null)
            m_graphAgent = GetComponent<BehaviorGraphAgent>();
        if(m_navAgent == null)
            m_navAgent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        if (m_isDead) return;
        m_graphAgent.Start();
        Initialize();
    }

    protected void Update()
    {
        if (m_isDead) return;
        m_graphAgent.Graph.Tick();
        OnUpdate();

        if (m_isDead)
        {
            m_graphAgent.End();
            OnDeath();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        m_targetDetector.OnDrawGizmos(transform);
    }
#endif
}
