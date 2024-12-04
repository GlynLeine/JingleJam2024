using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.Splines;

public class GameController : MonoBehaviour
{
    [SerializeField] private float m_TickRateSeconds = 0.2f;
    private float m_Accumulator;

    PlayerController m_PlayerController;
    GameObject m_PlayerRef;
    [SerializeField] private GameObject m_Camera;
    [SerializeField] private Vector3 m_CameraOffset;
    [SerializeField] private float m_CameraSplineDist = 50.0f;
    [SerializeField] private GameObject spline;
    private Vector3 nearestSplinePoint;
    private Vector3 splineTangent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Accumulator = 0.0f;
        m_PlayerRef = GameObject.FindGameObjectWithTag("Player");
        m_PlayerController = m_PlayerRef.GetComponent<PlayerController>();

    }

    private void Update()
    {
        SplineContainer splineContainer = spline.GetComponent<SplineContainer>();
        if (splineContainer != null)
        {
            Spline s = splineContainer.Spline;
            float t;
            float3 nearest;

            SplineUtility.GetNearestPoint<Spline>(s, m_PlayerRef.transform.position, out nearest, out t);
            nearestSplinePoint = SplineUtility.EvaluatePosition<Spline>(s, t);
            nearestSplinePoint += spline.transform.position;
            splineTangent = SplineUtility.EvaluateTangent<Spline>(s, t);
        }


        float distToNearestSplinePoint = Vector3.Distance(m_PlayerRef.transform.position, nearestSplinePoint);
        if (distToNearestSplinePoint < m_CameraSplineDist)
        {
            Vector3 m_CamTgtPos = nearestSplinePoint + m_CameraOffset;
            m_Camera.transform.position = Vector3.Slerp(m_PlayerRef.transform.position, m_CamTgtPos, distToNearestSplinePoint / m_CameraSplineDist);
            m_Camera.transform.rotation = Quaternion.FromToRotation(Vector3.forward, splineTangent.normalized);
        }
        else
        {
            m_Camera.transform.position = new Vector3(m_CameraOffset.x + m_PlayerRef.transform.position.x, m_CameraOffset.y + m_PlayerRef.transform.position.y, m_CameraOffset.z + m_PlayerRef.transform.position.z);
        }
    }
    void LateUpdate()
    {
        //Accumulate Deltatime until a tick
        m_Accumulator += Time.deltaTime;
        if (m_Accumulator > m_TickRateSeconds)
        {
            m_Accumulator = 0.0f;
            m_PlayerController.Tick();
        }
    }
}
