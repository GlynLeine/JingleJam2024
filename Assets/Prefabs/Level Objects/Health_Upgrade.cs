using UnityEngine;
using UnityEngine.Animations;

public class Health_Upgrade : MonoBehaviour, IInteractable
{
    [SerializeField] private int m_HealthToAdd;
    private PlayerController playerRef;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        //I wanted to set this up programmatically, but Unity had other ideas...
        /*
        Canvas canvas = this.gameObject.GetComponent<Canvas>();
        Camera camera = FindFirstObjectByType<Camera>();
        //canvas.worldCamera = camera; 
        ConstraintSource source = new ConstraintSource(); 
        source.weight = 1.0f;
        source.sourceTransform = camera.gameObject.transform; 
        canvas.gameObject.GetComponent<LookAtConstraint>().AddSource(source); 
         * */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerRef == null)
        {
            return;
        }
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerRef.m_bCanInteract = true;
        playerRef.m_Interactable = this.gameObject;
    }
    private void OnTriggerExit(Collider other)
    {
        if (playerRef == null)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerRef.m_bCanInteract = false;
        playerRef.m_Interactable = null;
    }


    void IInteractable.OnInteract()
    {
        playerRef.Stats.MaxHealth += this.m_HealthToAdd;
        GameObject.Destroy(this.gameObject);
    }
}
