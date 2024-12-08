using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using System.Linq;
using NUnit.Framework;
using static UnityEngine.GraphicsBuffer;

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
    private Vector3 nearestSplinePointCandidate;
    private Vector3 splineTangentCandidate;
    private Vector3 m_CamTgtPos;
    private SplineContainer splineContainer;
    private InputAction lookAction;
    private float nearestDist = Mathf.Infinity;
    int nearestSearchIteration = 0;
    const int nearestSearchFrameSpread = 30;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Accumulator = 0.0f;
        m_PlayerRef = GameObject.FindGameObjectWithTag("Player");
        m_PlayerController = m_PlayerRef.GetComponent<PlayerController>();

        if (spline != null)
            splineContainer = spline.GetComponent<SplineContainer>();

        lookAction = InputSystem.actions.FindAction("Look");
    }

    private void LateUpdate()
    {
        Vector3 playerPos = m_PlayerRef.transform.position;

        if (splineContainer != null)
        {
            int iterations = splineContainer.Splines.Count / nearestSearchFrameSpread;
            int startIteration = iterations * nearestSearchIteration;
            int endIteration = Mathf.Min(startIteration + iterations, splineContainer.Splines.Count);

            nearestSearchIteration = (nearestSearchIteration + 1) % nearestSearchFrameSpread;

            if (startIteration == 0)
            {
                nearestDist = Mathf.Infinity;
                splineTangent = splineTangentCandidate;
                nearestSplinePoint = nearestSplinePointCandidate;
            }

            //Find the nearest
            for (int i = startIteration; i < endIteration; i++)
            {
                Spline s = splineContainer.Splines.ElementAt(i);
                float t;
                float3 nearest;

                SplineUtility.GetNearestPoint<Spline>(s, playerPos, out nearest, out t);
                Vector3 splinePoint = (Vector3)nearest;
                Vector3 tangent = SplineUtility.EvaluateTangent<Spline>(s, t);

                float dist = Vector3.SqrMagnitude(playerPos - splinePoint);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    splineTangentCandidate = tangent;
                    nearestSplinePointCandidate = splinePoint;
                }
            }
        }

        float distToNearestSplinePoint = Vector3.Distance(playerPos, nearestSplinePoint);
        if (distToNearestSplinePoint < m_CameraSplineDist)
        {
            m_CamTgtPos = nearestSplinePoint + m_CameraOffset;
            m_Camera.transform.position = Vector3.Lerp(playerPos + m_CameraOffset, m_CamTgtPos, distToNearestSplinePoint / m_CameraSplineDist * Time.deltaTime);
            Vector3 tangent = splineTangent.normalized;
            m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.gameObject.transform.rotation, Quaternion.FromToRotation(Vector3.forward, new Vector3(tangent.x, 0.0f, tangent.z)), distToNearestSplinePoint / m_CameraSplineDist * m_CameraAutoRotationSpeed * Time.deltaTime);
        }
        else
        {
            m_Camera.transform.position = new Vector3(m_CameraOffset.x + playerPos.x, m_CameraOffset.y + playerPos.y, m_CameraOffset.z + playerPos.z);
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
            Gizmos.DrawRay(m_PlayerRef.transform.position + new Vector3(0.0f, 3.0f, 0.0f), new Vector3(lookAction.ReadValue<Vector2>().x, 0.0f, lookAction.ReadValue<Vector2>().y).normalized);
        }
    }
}
