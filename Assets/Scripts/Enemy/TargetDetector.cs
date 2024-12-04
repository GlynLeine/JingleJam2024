using System;
using UnityEngine;

[Serializable]
public class TargetDetector
{
    [SerializeField]
    private float m_searchRadius = 7.0f;
    public float SearchRadius => m_searchRadius;

    [SerializeField]
    private float m_orbitRadius = 7.0f;
    public float OrbitRadius => m_searchRadius;
    [SerializeField]
    private LayerMask m_playerMask;

    private Vector3 m_positionOnEnter;
    public Vector3 PositionOnEnter => m_positionOnEnter;
    private Transform m_currentTarget = null;
    public Transform CurrentTarget => m_currentTarget;
    public bool IsWithinRadius => m_currentTarget != null;
    private bool m_isWithinOrbitRadius = false;
    public bool IsWithinOrbitRadius => m_isWithinOrbitRadius;

    public void Start()
    {

    }

    public void Update(Transform transform)
    {
        Collider[] cols = new Collider[1];
        int numCols = Physics.OverlapSphereNonAlloc(transform.position, m_searchRadius, cols, m_playerMask);
        if (numCols > 0)
        {
            m_currentTarget = cols[0].transform;
            m_positionOnEnter = m_currentTarget.position - transform.position;
        }
        else
        {
            m_currentTarget = null;
            m_positionOnEnter = Vector3.zero;
        }

        if (IsWithinRadius)
        {
            cols = new Collider[1];
            numCols = Physics.OverlapSphereNonAlloc(transform.position, m_orbitRadius, cols, m_playerMask);
            m_isWithinOrbitRadius = numCols > 0;
        }
        else
            m_isWithinOrbitRadius = false;

    }

#if UNITY_EDITOR
    public void OnDrawGizmos(Transform transform)
    {
        if (IsWithinRadius)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right);
            Gizmos.DrawLine(transform.position, transform.position + m_positionOnEnter);
        }

        Gizmos.color = IsWithinRadius ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, m_searchRadius);

        Gizmos.color = IsWithinOrbitRadius ? Color.green : Color.magenta;
        Gizmos.DrawWireSphere(transform.position, m_orbitRadius);
    }
#endif
}
