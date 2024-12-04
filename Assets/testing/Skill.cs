using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

public enum ESkillStatus
{
    Ready,
    In_Execution,
    In_Cooldown
};

public class Skill : ScriptableObject
{
    public string name;
    public string desc;
    public string animationName;
    public int mp_cost;
    public int hp_cost;
    public VisualEffect vfx;
    public float executionTimer = 0.0f; 
    public float recastTimer = 0.0f; 
    public float cast = 0.0f;  //How long it takes to cast the skill
    public float recast = 0.0f;    //How long, after the skill is cast, until we can trigger it again
    public float active = 0.0f;    //How long until the skill is "finished" ???
    public ESkillStatus status = ESkillStatus.Ready;

    public virtual void Activate() { }



}
