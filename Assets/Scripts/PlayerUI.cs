using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private GameObject m_HPBar;
    [SerializeField] private GameObject m_MPBar;
    [SerializeField] private GameObject m_SpiritBar;
    [SerializeField] private GameObject[] m_SkillBars;

    private PlayerController m_PlayerRef;
    private AbilityManager m_AbilityManagerRef;

    private Slider m_HPSlider;
    private TextMeshProUGUI m_CurrentHPLabel;
    private TextMeshProUGUI m_MaxHPLabel;

    private Slider m_MPSlider;
    private TextMeshProUGUI m_CurrentMPLabel;
    private TextMeshProUGUI m_MaxMPLabel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_PlayerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();  //Cache a ref to the player controller

        //Grab UI handles we care about
        m_HPSlider = m_HPBar.GetComponent<Slider>();
        m_CurrentHPLabel = m_HPBar.GetComponentsInChildren<TextMeshProUGUI>()[0];   //Indexing into this is a little evil, but we know where everything SHOULD be...
        m_MaxHPLabel = m_HPBar.GetComponentsInChildren<TextMeshProUGUI>()[1];

        m_MPSlider = m_MPBar.GetComponent<Slider>();
        m_CurrentMPLabel = m_MPBar.GetComponentsInChildren<TextMeshProUGUI>()[0];   //Indexing into this is a little evil, but we know where everything SHOULD be...
        m_MaxMPLabel = m_MPBar.GetComponentsInChildren<TextMeshProUGUI>()[1];
        m_AbilityManagerRef = m_PlayerRef.gameObject.GetComponent<AbilityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Set each attribute on the UI
        //Health
        {
            int health = (int)m_PlayerRef.m_Stats.m_Health;
            int maxHealth = (int)m_PlayerRef.m_Stats.m_MaxHealth;
            m_HPSlider.value = (float)health / (float)maxHealth;
            m_CurrentHPLabel.text = health.ToString();
            m_MaxHPLabel.text = maxHealth.ToString();
        }
        //Mana
        {
            int mana = (int)m_PlayerRef.m_Stats.m_Mana;
            int maxMana = (int)m_PlayerRef.m_Stats.m_MaxMana;
            m_MPSlider.value = (float)mana / (float)maxMana;
            m_CurrentMPLabel.text = mana.ToString();
            m_MaxMPLabel.text = maxMana.ToString();
        }

        //Abilities
        {
            //Note: Doing this in update IS slow... But if it works it works!
            //TODO: Fix later ;)
            for(int i = 0; i < m_AbilityManagerRef.GetAbilityCount(); i++)
            {
                Ability abl = m_AbilityManagerRef.GetAbilities()[i];
                m_SkillBars[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = abl.name.ToString();
                m_SkillBars[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text =abl.recastTimer.ToString("F2");

            }
        }
    }
}
