using NaughtyAttributes;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamageable, IAttacker
{
    [SerializeField]
    protected EnemyData m_enemyData;
    [SerializeField]
    protected TargetDetector m_targetDetector;
    [SerializeField]
    protected Ability m_abilityTemplate;
    [SerializeField, Expandable]
    protected Ability m_abilityInstance;
    protected bool m_isDead = false;
    protected BehaviorGraphAgent m_graphAgent;
    protected NavMeshAgent m_navAgent;
    public NavMeshAgent NavAgent => m_navAgent;
    public Transform Target => m_targetDetector.FindTarget(transform);
    public float MoveSpeed => m_enemyData.MoveSpeed;
    public bool IsWithinRadius => m_targetDetector.IsWithinRadius;

    protected abstract void Initialize();
    protected abstract void OnUpdate();
    protected abstract void OnDeath();

    protected void OnEnable()
    {
        if (m_graphAgent == null)
            m_graphAgent = GetComponent<BehaviorGraphAgent>();
        if (m_navAgent == null)
            m_navAgent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        if (m_isDead) return;
        m_abilityInstance = Instantiate(m_abilityTemplate);
        m_graphAgent.Start();
        Initialize();
    }

    protected void Update()
    {
        if (m_isDead) return;
            m_abilityInstance?.Tick(Time.deltaTime);
        m_graphAgent.Graph.Tick();
        OnUpdate();

        if (m_isDead)
        {
            m_graphAgent.End();
            OnDeath();
        }
    }

    public abstract void Attack();

    public abstract void TakeDamage();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        m_targetDetector.OnDrawGizmos(transform);
    }
#endif
}
