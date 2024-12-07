using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private GameObject m_FPSCounter;
    private TextMeshProUGUI m_FPSLabel;

    void Start()
    {
        m_FPSLabel = m_FPSCounter.GetComponentInChildren<TextMeshProUGUI>();    
    }

    // Update is called once per frame
    void Update()
    {

        m_FPSLabel.text = ((int)(1.0f / Time.deltaTime)).ToString() + " FPS";  
    }
}
