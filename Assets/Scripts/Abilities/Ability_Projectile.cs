using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Ability_Projectile", menuName = "Abilities/Projectile")]
public class Ability_Projectile : Ability
{
    public override EAbilityType type => EAbilityType.Projectile;
    public Vector3 direction;
    public float speed;
    public float size;
    public int damage;
    public int health;
    public GameObject ProjectilePrefab;
    private Projectile p;

    [SerializeField] private VisualEffect aoeIndicatorFX;

    public override void Tick(float dt)
    {
        base.Tick(dt);
    }

    public override void Activate(GameObject owner, Transform target = null)
    {
        base.Activate(owner);
        if (status == EAbilityStatus.Ready)
        {
            status = EAbilityStatus.In_Execution;

            //Spawn a new projectile with this instance's parameters
            GameObject projectile = ProjectilePrefab != null ? Instantiate(ProjectilePrefab) : new GameObject();
            //projectile.transform.parent = owner.transform;
            p = projectile.GetComponent<Projectile>();
            if (p == null)
                p = projectile.AddComponent<Projectile>();

            p.m_Direction = owner.gameObject.CompareTag("Player") ? owner.GetComponent<PlayerController>().GetLookAtDirection() : owner.transform.forward;//
            p.m_Damage = damage + owner.gameObject.GetComponent<Stats>().StrengthBonus;
            p.m_Origin = owner.transform.position + p.m_Direction + Vector3.up;
            p.m_Speed = speed;
            p.m_Size = size;
            p.m_Health = health;
            //TODO: We could probably save some frames by caching this stuff... 
            p.m_LayerMask = owner.gameObject.CompareTag("Player") ? 1 << LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player");    //What layers do we want to be able to collide with? 
            p.Initialize();

            projectile.name = this.name + "_projectile";
            projectile.gameObject.layer = owner.layer;
            projectile.gameObject.layer = owner.gameObject.CompareTag("Player") ? LayerMask.NameToLayer("Player_Attack") : LayerMask.NameToLayer("Enemy_Attack");
        }
    }

}
