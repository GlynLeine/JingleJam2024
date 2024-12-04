using Unity.Hierarchy;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Stats : MonoBehaviour
{
    #region Health
    [Header("Health Statistics")]
    [SerializeField] public int m_Health;
    public int m_MaxHealth;
    public int m_HealthRegenPerTick;
    #endregion

    #region Stamina
    [Header("Stamina Statistics")]
    public int m_Stamina;
    public int m_MaxStamina;
    public int m_StaminaRegenPerTick;
    #endregion

    #region Movement
    [Header("Movement Statistics")]
    public float m_MovementSpeed;
    public float m_MaxMovementSpeed;
    #endregion

    #region Combat
    [Header("Combat Statistics")]
    public int m_StrengthBonus;    //Add to each attack's base strength
    public float m_AttackSpeedScale;  //1.0 for normal! 
    public int m_DefenceBonus;   //Subtract from incoming damage
    #endregion


    //Move to Player / Enemy Controller!
    //public bool m_bCanMove;
    //public bool m_bIsSprinting; 
    private void Update()
    {
        //Clamp each stat 

        m_Health = Mathf.Clamp(m_Health, 0, m_MaxHealth); 
        m_Stamina = Mathf.Clamp(m_Stamina, 0, m_MaxStamina); 
        m_MovementSpeed = Mathf.Clamp(m_MovementSpeed, 0.0f, m_MaxMovementSpeed);
        m_AttackSpeedScale = Mathf.Clamp(m_AttackSpeedScale, 0.0f, 5.0f); 
    }

    public void Tick()
    {
        m_Health += m_HealthRegenPerTick;
        m_Stamina += m_StaminaRegenPerTick; 
    }

    public void Reset()
    {
        m_Health = m_MaxHealth;
        m_Stamina = m_MaxStamina;
        m_MovementSpeed = m_MaxMovementSpeed;
        m_DefenceBonus = 0;
        m_StrengthBonus = 0;
        m_HealthRegenPerTick = 0;
        m_StaminaRegenPerTick = 0; 
    }

}
