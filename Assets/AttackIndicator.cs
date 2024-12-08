using NaughtyAttributes;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    private Color m_color;
    public Color Color
    {
        get => m_color;
        set
        {
            m_color = value;
            if (m_renderer == null)
                m_renderer = GetComponent<MeshRenderer>();
            m_renderer.material.SetColor("_Color", m_color);
        }
    }

    private bool m_isCircle = true;
    public bool IsCircle
    {
        get => m_isCircle;
        set
        {
            m_isCircle = value;
            if (m_renderer == null)
                m_renderer = GetComponent<MeshRenderer>();
            m_renderer.material.SetFloat("_Is_Circle", m_isCircle ? 1.0f : 0.0f);
        }
    }

    private float m_fill = 1.0f;
    public float Fill
    {
        get => m_fill;
        set
        {
            m_fill = value;
            if (m_renderer == null)
                m_renderer = GetComponent<MeshRenderer>();
            m_renderer.material.SetFloat("_Fill", m_fill);
        }
    }


    private bool m_useArrow = true;
    public bool UseArrow
    {
        get => m_useArrow;
        set
        {
            m_useArrow = value;
            if (m_renderer == null)
                m_renderer = GetComponent<MeshRenderer>();
            m_renderer.material.SetFloat("_Use_Arrow", m_useArrow ? 1.0f : 0.0f);
        }
    }

    private float m_coneRadius = 360.0f;
    public float ConeRadius
    {
        get => ConeRadius;
        set
        {
            m_coneRadius = value;
            if (m_renderer == null)
                m_renderer = GetComponent<MeshRenderer>();
            m_renderer.material.SetFloat("_Cone_Radius", m_coneRadius);
        }
    }

    private MeshRenderer m_renderer;
    private void Awake()
    {
        m_renderer = GetComponent<MeshRenderer>();
    }
}
