using UnityEngine;

public class Stats : MonoBehaviour
{
    #region Health
    [Header("Health Statistics")]
    [SerializeField]
    private float m_Health;
    public float Health
    {
        get => m_Health;
        set => m_Health = value;
    }
        
    [SerializeField]
    private float m_MaxHealth;
    public float MaxHealth => m_MaxHealth;
    [SerializeField]
    private float m_HealthRegenPerTick;
    public float HealthRegenPerTick=>m_HealthRegenPerTick;
    #endregion

    #region Mana
    [Header("Mana Statistics")]
    [SerializeField]
    private float m_Mana;
    public float Mana => m_Mana;
    [SerializeField]
    private float m_MaxMana;
    public float MaxMana => m_MaxMana;
    [SerializeField]
    private float m_ManaRegenPerTick;
    public float ManaRegenPerTick => m_ManaRegenPerTick;
    #endregion

    #region Movement
    [Header("Movement Statistics")]
    [SerializeField]
    private float m_MovementSpeed;
    public float MovementSpeed => m_MovementSpeed;
    [SerializeField]
    private float m_MaxMovementSpeed;
    public float MaxMovementSpeed => m_MaxMovementSpeed;
    #endregion

    #region Combat
    [Header("Combat Statistics")]
    [SerializeField]
    private int m_StrengthBonus;    //Add to each attack's base strength
    public int StrengthBonus => m_StrengthBonus;
    [SerializeField]
    private float m_AttackSpeedScale;  //1.0 for normal! 
    public float AttackSpeecScale => m_AttackSpeedScale;
    [SerializeField]
    private int m_DefenceBonus;   //Subtract from incoming damage
    public int DefenceBonus => m_DefenceBonus;
    #endregion


    //Move to Player / Enemy Controller!
    //public bool m_bCanMove;
    //public bool m_bIsSprinting; 
    private void Update()
    {
        //Clamp each stat 

        m_Health = Mathf.Clamp(m_Health, 0, m_MaxHealth); 
        m_Mana = Mathf.Clamp(m_Mana, 0, m_MaxMana); 
        m_MovementSpeed = Mathf.Clamp(m_MovementSpeed, 0.0f, m_MaxMovementSpeed);
        m_AttackSpeedScale = Mathf.Clamp(m_AttackSpeedScale, 0.0f, 5.0f); 
    }

    public void Tick()
    {
        m_Health += m_HealthRegenPerTick;
        m_Mana += m_ManaRegenPerTick; 
    }

    public void Reset()
    {
        m_Health = m_MaxHealth;
        m_Mana = m_MaxMana;
        m_MovementSpeed = m_MaxMovementSpeed;
        m_DefenceBonus = 0;
        m_StrengthBonus = 0;
        m_HealthRegenPerTick = 0;
        m_ManaRegenPerTick = 0; 
    }

}
