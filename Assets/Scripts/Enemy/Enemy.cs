using NaughtyAttributes;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamageable, IAttacker
{
    protected Stats m_stats;
    protected AbilityManager m_abilityManager;
    [SerializeField]
    protected TargetDetector m_targetDetector = new();
    protected bool m_isDead = false;
    protected BehaviorGraphAgent m_graphAgent;
    protected NavMeshAgent m_navAgent;
    public NavMeshAgent NavAgent => m_navAgent;
    public Transform Target => m_targetDetector.FindTarget(transform);
    public float MoveSpeed => m_stats.MovementSpeed;
    public float AttackRadius => m_targetDetector.AttackRadius;
    public float SearchRadius => m_targetDetector.SearchRadius;
    public float FleeRadius => m_targetDetector.FleeRadius;
    public bool IsWithinRadius => m_targetDetector.IsWithinRadius;
    public bool IsWithinAttackRadius => m_targetDetector.IsWithinAttackRadius;
    public bool IsWithinFleeRadius => m_targetDetector.IsWithinFleeRadius;
    protected abstract void Initialize();
    protected abstract void OnUpdate();
    protected abstract void OnDeath();

    protected void OnEnable()
    {
        if (m_stats == null)
            m_stats = GetComponent<Stats>();
        if (m_abilityManager == null)
            m_abilityManager = GetComponent<AbilityManager>();
        if (m_graphAgent == null)
            m_graphAgent = GetComponent<BehaviorGraphAgent>();
        if (m_navAgent == null)
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
        m_navAgent.speed = m_stats.MovementSpeed;
        m_graphAgent.Graph.Tick();
        m_stats.Tick(); 
        OnUpdate();

        //Check Stats
        if(m_stats.Health <= 0)
        {
            m_isDead = true; 
        }

        if (m_isDead)
        {
            m_graphAgent.End();
            OnDeath();

            //NOTE: Do we want to destroy the gameobject on death, and handle spawning via a spawner? 
            //I'll just destroy them for now. 
            Destroy(this.gameObject);
        }
    }

    public abstract void Attack();

    public abstract void TakeDamage(float damage);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        m_targetDetector.OnDrawGizmos(transform);
    }
#endif
}
