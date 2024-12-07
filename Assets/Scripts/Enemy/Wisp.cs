using NaughtyAttributes;
using UnityEngine;


public class Wisp : Enemy
{
    public float Timer = 0;
    public Vector3 PositionOnEnter => m_targetDetector.PositionOnEnter;
    public float OrbitRadius => m_targetDetector.OrbitRadius;
    public bool IsWithinOrbitRadius => m_targetDetector.IsWithinOrbitRadius;
    protected override void Initialize() { }
    protected override void OnUpdate() { }
    protected override void OnDeath() { }

    public override void Attack()
    {
        if (m_abilityInstance == null)
            m_abilityInstance = Instantiate(m_abilityTemplate);

        (m_abilityInstance as Ability_Arc).Target = Target;
        m_abilityInstance.Activate(gameObject);
    }

    public override void TakeDamage()
    {

    }
}
