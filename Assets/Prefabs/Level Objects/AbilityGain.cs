using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class AbilityGain : MonoBehaviour, IInteractable
{
    [SerializeField] private SkillMenu m_SkillMenu;
    [SerializeField] private GameObject m_Prompt; 
    private PlayerController playerRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        if(m_SkillMenu == null) //When the player accepts a skill, destroy the thingy so they can't thing the thing if the thing... yeah
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        m_SkillMenu.gameObject.SetActive(false);
        m_Prompt.SetActive(true); 
        playerRef.m_bCanInteract = true;
        playerRef.m_Interactable = this.gameObject;


    }

    void OnTriggerExit(Collider other)
    {
        //Disable the menu if the player walks away
        m_Prompt.SetActive(false); 
        m_SkillMenu.gameObject.SetActive(false); 
        playerRef.m_bCanInteract = false;
        playerRef.m_Interactable = null;

    }

    public void OnInteract()
    {
        m_SkillMenu.gameObject.SetActive(true);
    }
}
