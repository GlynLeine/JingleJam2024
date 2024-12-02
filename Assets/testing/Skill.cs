using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

public class Skill : ScriptableObject
{
    public string name;
    public string desc;
    public string animationName; 
    public int mp_cost;
    public int hp_cost;
    public VisualEffect vfx;

    public virtual void Activate() { }



}
