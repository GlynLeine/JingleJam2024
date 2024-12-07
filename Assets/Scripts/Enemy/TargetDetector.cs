using System;
using UnityEngine;

[Serializable]
public class TargetDetector
{
    [SerializeField]
    private float m_searchRadius = 7.0f;
    public float SearchRadius => m_searchRadius;

    [SerializeField]
    private float m_attackRadius = 7.0f;
    public float AttackRadius => m_attackRadius;
    [SerializeField]
    private LayerMask m_playerMask;

    private Vector3 m_positionOnEnter;
    public Vector3 PositionOnEnter => m_positionOnEnter;
    private bool m_isWithinRadius = false;
    public bool IsWithinRadius => m_isWithinRadius; 
    private bool m_isWithinAttackRadius = false;
    public bool IsWithinAttackRadius => m_isWithinAttackRadius;

    public Transform FindTarget(Transform transform)
    {
        Transform target = null;
        Collider[] cols = new Collider[1];
        int numCols = Physics.OverlapSphereNonAlloc(transform.position, m_searchRadius, cols, m_playerMask);
        if (numCols > 0)
        {
            target = cols[0].transform;
            m_positionOnEnter = target.position - transform.position;
            m_isWithinRadius = true;
        }
        else
        {
            m_isWithinRadius = false;
            m_positionOnEnter = Vector3.zero;
        }

        if (m_isWithinRadius)
        {
            cols = new Collider[1];
            numCols = Physics.OverlapSphereNonAlloc(transform.position, m_attackRadius, cols, m_playerMask);
            m_isWithinAttackRadius = numCols > 0;
        }
        else
            m_isWithinAttackRadius = false;

        return target;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos(Transform transform)
    {
        if (m_isWithinRadius)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right);
            Gizmos.DrawLine(transform.position, transform.position + m_positionOnEnter);
        }

        Gizmos.color = m_isWithinRadius ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, m_searchRadius);

        Gizmos.color = IsWithinAttackRadius ? Color.green : Color.magenta;
        Gizmos.DrawWireSphere(transform.position, m_attackRadius);
    }
#endif
}
