using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class TargetDetector : MonoBehaviour
{
    private Vector3 m_positionOnEnter;
    public Vector3 PositionOnEnter => m_positionOnEnter;
    private Transform m_currentTarget = null;
    public Transform CurrentTarget => m_currentTarget;
    public bool IsWithinRadius => m_currentTarget != null;
    [SerializeField]
    private float m_searchRadius = 5.0f;
    public float SearchRadius => m_searchRadius;
    [SerializeField]
    private LayerMask m_playerMask;

    private void Update()
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
    }

    private float m_timer = 0.0f;
    private void OnDrawGizmos()
    {
        if (m_currentTarget != null)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, m_searchRadius);

        if (m_currentTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right);
            Gizmos.DrawLine(transform.position, transform.position + m_positionOnEnter);
        }
    }
}
