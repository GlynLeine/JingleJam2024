using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using System.Linq;
using NUnit.Framework;

public class GameController : MonoBehaviour
{
    [SerializeField] public static float m_TickRateSeconds = 0.2f;
    private float m_Accumulator;

    PlayerController m_PlayerController;
    GameObject m_PlayerRef;
    [SerializeField] private GameObject m_Camera;
    [SerializeField] private Vector3 m_CameraOffset;
    [SerializeField] private float m_CameraSplineDist = 50.0f;
    [SerializeField] private float m_CameraLookSpeed = 4.0f;
    [SerializeField] private float m_CameraAutoRotationSpeed = 2.0f;
    [SerializeField] private GameObject spline;
    private Vector3 nearestSplinePoint;
    private Vector3 splineTangent;
    private Vector3 m_CamTgtPos;
    private SplineContainer splineContainer;
    private System.Collections.Generic.List<Vector3> nearestPoints;
    private System.Collections.Generic.List<Vector3> tangents;
    private InputAction lookAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Accumulator = 0.0f;
        m_PlayerRef = GameObject.FindGameObjectWithTag("Player");
        m_PlayerController = m_PlayerRef.GetComponent<PlayerController>();

        splineContainer = spline.GetComponent<SplineContainer>();
        nearestPoints = new System.Collections.Generic.List<Vector3>(splineContainer.Splines.Count);
        tangents = new System.Collections.Generic.List<Vector3>(splineContainer.Splines.Count);
        lookAction = InputSystem.actions.FindAction("Look");
    }

    private void LateUpdate()
    {
        if (splineContainer != null)
        {

            for (int i = 0; i < splineContainer.Splines.Count; i++)
            {
                Spline s = splineContainer.Splines.ElementAt(i);
                float t;
                float3 nearest;

                SplineUtility.GetNearestPoint<Spline>(s, m_PlayerRef.transform.position, out nearest, out t);
                nearestPoints.Add(SplineUtility.EvaluatePosition<Spline>(s, t));
                nearestPoints[i] += spline.transform.position;
                tangents.Add(SplineUtility.EvaluateTangent<Spline>(s, t));
            }

            //Find the nearest
            int nearestIdx = -1;
            float nearestDist = Mathf.Infinity;
            for (int i = 0; i < nearestPoints.Count; i++)
            {
                float dist = Vector3.Distance(m_PlayerRef.transform.position, nearestPoints[i]);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestIdx = i;
                }
            }

            splineTangent = tangents[nearestIdx];
            nearestSplinePoint = nearestPoints[nearestIdx];
        }


        float distToNearestSplinePoint = Vector3.Distance(m_PlayerRef.transform.position, nearestSplinePoint);
        if (distToNearestSplinePoint < m_CameraSplineDist)
        {
            m_CamTgtPos = nearestSplinePoint + m_CameraOffset;
            m_Camera.transform.position = Vector3.Lerp(m_PlayerRef.transform.position + m_CameraOffset, m_CamTgtPos, distToNearestSplinePoint / m_CameraSplineDist * Time.deltaTime);
            Vector3 tangent = splineTangent.normalized;
            m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.gameObject.transform.rotation, Quaternion.FromToRotation(Vector3.forward, new Vector3(tangent.x, 0.0f, tangent.z)), distToNearestSplinePoint / m_CameraSplineDist * m_CameraAutoRotationSpeed * Time.deltaTime);
        }
        else
        {
            m_Camera.transform.position = new Vector3(m_CameraOffset.x + m_PlayerRef.transform.position.x, m_CameraOffset.y + m_PlayerRef.transform.position.y, m_CameraOffset.z + m_PlayerRef.transform.position.z);
            float cur = m_Camera.transform.eulerAngles.y;
            m_Camera.transform.rotation = Quaternion.Euler(0.0f, cur + lookAction.ReadValue<Vector2>().x * m_CameraLookSpeed * Time.deltaTime, 0.0f);
        }
    }
    void Update()
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(nearestSplinePoint, 1.0f);
        Gizmos.DrawRay(nearestSplinePoint + new Vector3(0.0f, 3.0f, 0.0f), splineTangent);
        if (m_PlayerRef != null)
        {
            Gizmos.DrawRay(m_PlayerRef.transform.position + new Vector3(0.0f, 3.0f, 0.0f), new Vector3( lookAction.ReadValue<Vector2>().x, 0.0f, lookAction.ReadValue<Vector2>().y).normalized);
        }
    }
}
