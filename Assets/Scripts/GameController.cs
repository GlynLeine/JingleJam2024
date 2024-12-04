using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.Splines;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [SerializeField] public static float m_TickRateSeconds = 0.2f;
    private float m_Accumulator;

    PlayerController m_PlayerController;
    GameObject m_PlayerRef;
    [SerializeField] private GameObject m_Camera;
    [SerializeField] private Vector3 m_CameraOffset;
    [SerializeField] private float m_CameraSplineDist = 50.0f;
    [SerializeField] private float m_CameraSpeed = 4.0f;
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
            m_Camera.transform.position = Vector3.Lerp(m_CamTgtPos, m_PlayerRef.transform.position + m_CameraOffset, distToNearestSplinePoint / m_CameraSplineDist);
            Vector3 tangent = splineTangent.normalized; 
            m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.gameObject.transform.rotation, Quaternion.FromToRotation(Vector3.forward,new Vector3(tangent.x, 0.0f, tangent.z)), distToNearestSplinePoint / m_CameraSplineDist);
        }
        else
        {
            m_Camera.transform.position = new Vector3(m_CameraOffset.x + m_PlayerRef.transform.position.x, m_CameraOffset.y + m_PlayerRef.transform.position.y, m_CameraOffset.z + m_PlayerRef.transform.position.z);
            float cur = m_Camera.transform.eulerAngles.y;
            m_Camera.transform.rotation =  Quaternion.Euler(0.0f, cur + InputSystem.actions.FindAction("Look").ReadValue<Vector2>().x * m_CameraSpeed * Time.deltaTime, 0.0f); 
        }
    }
    void LateUpdate()
    {
        //Accumulate Deltatime until a tick
        m_Accumulator += Time.deltaTime;
        while (m_Accumulator > m_TickRateSeconds)
        {
            m_Accumulator -= m_TickRateSeconds;
            m_PlayerController.Tick();
        }
    }

    public static float GetTickRate()
    {
        return m_TickRateSeconds;
    }
}
