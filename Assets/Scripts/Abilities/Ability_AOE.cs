using UnityEngine;

[CreateAssetMenu(fileName = "Ability_AOE", menuName = "Abilities/AOE")]
public class Ability_AOE : Ability
{
    public override EAbilityType type => EAbilityType.Circle_AoE;
    public int Damage;
    public float Range;
    public GameObject AOEIndicator;
    private AttackIndicator indicator;
    private AOE aoe;
    private float timer = 0;
    public override void Tick(float dt)
    {
        base.Tick(dt);

        if (status == EAbilityStatus.In_Execution && indicator != null)
        {
            if (timer < castTime)
            {
                indicator.Fill = timer / castTime;
                timer += dt;
                if (timer >= castTime)
                {
                    aoe?.Initialize(this);
                    timer = 0;
                }
            }
        }
    }

    public override void Activate(GameObject owner, Transform target = null)
    {
        base.Activate(owner);
        if (status == EAbilityStatus.Ready)
        {
            status = EAbilityStatus.In_Execution;

            GameObject AOE = Instantiate(AOEIndicator);
            AOE.transform.parent = owner.transform;
            AOE.transform.localPosition = Vector3.up * .5f;
            AOE.name = name + "_AOE";
            AOE.gameObject.layer = owner.layer;
            aoe = AOE.AddComponent<AOE>();
            aoe.transform.localScale = Vector3.one * Range;

            indicator = AOE.GetComponent<AttackIndicator>();
            indicator.IsCircle = true;
            indicator.UseArrow = false;
            indicator.Color = Color.red;
            indicator.ConeRadius = 360.0f;
        }
    }
}
