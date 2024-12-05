using NaughtyAttributes;
using UnityEngine;


public class Wisp : Enemy
{
    public Ability AttackAbility;
    [SerializeField, Expandable]
    private Ability m_abilityInstance;
    public float MoveSpeed => m_enemyData.MoveSpeed;
    public float Timer = 0;
    public Vector3 PositionOnEnter => m_targetDetector.PositionOnEnter;
    public float OrbitRadius => m_targetDetector.OrbitRadius;
    public bool IsWithinOrbitRadius => m_targetDetector.IsWithinOrbitRadius;
    public bool IsWithinRadius => m_targetDetector.IsWithinRadius;
    protected override void Initialize()
    {
        m_abilityInstance = Instantiate(AttackAbility);
    }
    protected override void OnUpdate()
    {
        if (m_abilityInstance != null)
        {
            m_abilityInstance.Tick(Time.deltaTime);
        }
    }
    protected override void OnDeath() { }

    public override void Attack()
    {
        if (m_abilityInstance == null)
            m_abilityInstance = Instantiate(AttackAbility);

        (m_abilityInstance as Ability_Arc).Target = Target;
        m_abilityInstance.Activate(gameObject);
    }

    public override void TakeDamage()
    {

    }
}
