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
    public float range;
    public GameObject ProjectilePrefab;
    private Projectile p;

    [SerializeField] private VisualEffect aoeIndicatorFX;

    public override void Tick(float dt)
    {
        base.Tick(dt);

        if (status == EAbilityStatus.In_Execution && p != null)
        {
            p.Tick();
        }
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
            p.m_Origin = owner.transform.position;
            p.m_Direction = owner.gameObject.CompareTag("Player") ? owner.GetComponent<PlayerController>().GetLookAtDirection() : owner.transform.forward;//
            p.m_Damage = damage;
            p.m_Speed = speed;
            p.m_Size = size;
            p.m_Range = range;
            p.Initialize();

            //(owner.transform.position, direction, speed, size, range, damage))

            projectile.name = this.name + "_projectile";
            projectile.gameObject.layer = owner.layer;
        }
    }

}
