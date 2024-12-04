using UnityEngine;
using UnityEngine.VFX;

public enum EAbilityType
{
    None = 0,
    Passive,
    Target_Self, 
    Projectile, 
    Circle_AoE, 
    Line_AoE, 
    Cone_AoE,
    EAbilityType_MAX
}

public enum EAbilityStatus
{
    None = 0, 
    Ready, 
    In_Execution, 
    In_Cooldown, 
    EAbilityStatus_MAX
}

[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/")]
public class Ability : ScriptableObject
{
    public EAbilityType type;
    public string name; 
    public string description;
    public string animationName;
    public VisualEffect vfx;
    public float castTimer = 0.0f;
    public float castTime = 0.0f;
    public float recastTimer = 0.0f;
    public float recastTime = 0.0f; 

    public virtual void Tick()
    {
        Debug.Log(name + " Tick"); 
    }

    public virtual void Activate(GameObject owner)
    {
        Debug.Log(name + " Activated");
    }
}
