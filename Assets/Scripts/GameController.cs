using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using System.Linq;
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
    [SerializeField] private GameObject m_DeathScreen;
    private Vector3 nearestSplinePoint;
    private Vector3 splineTangent;
    private Vector3 nearestSplinePointCandidate;
    private Vector3 splineTangentCandidate;
    private Vector3 m_CamTgtPos;
    private Vector3 m_playerForward;
    private SplineContainer splineContainer;
    private InputAction lookAction;
    private float nearestDist = Mathf.Infinity;
    int startIteration = 0;
    const float nearestSearchFrameSpread = 0.25f;

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
            int frameCount = Mathf.CeilToInt(nearestSearchFrameSpread / Time.deltaTime);
            int iterations = Mathf.RoundToInt((float)splineContainer.Splines.Count / frameCount);
            int endIteration = Mathf.Min(startIteration + iterations, splineContainer.Splines.Count);

            //Find the nearest
            for (int i = startIteration; i < endIteration; i++)
            {
                Spline s = splineContainer.Splines.ElementAt(i);
                float t;
                float3 nearest;

                SplineUtility.GetNearestPoint<Spline>(s, playerPos, out nearest, out t);
                Vector3 splinePoint = (Vector3)nearest;
                Vector3 tangent = SplineUtility.EvaluateTangent<Spline>(s, t);
                tangent = new Vector3(tangent.x, 0f, tangent.z).normalized;

                float dist = Vector3.Distance(playerPos, splinePoint) - Vector3.Dot(m_playerForward, tangent);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    splineTangentCandidate = tangent;
                    nearestSplinePointCandidate = splinePoint;
                }
            }

            startIteration = endIteration;

            if (endIteration == splineContainer.Splines.Count)
            {
                startIteration = 0;
            }

            if (startIteration == 0)
            {
                nearestDist = Mathf.Infinity;
                splineTangent = splineTangentCandidate;
                nearestSplinePoint = nearestSplinePointCandidate;
                m_playerForward = (m_playerForward + m_PlayerController.forward) * 0.5f;
            }
        }

        float distToNearestSplinePoint = Vector3.SqrMagnitude(playerPos - nearestSplinePoint);
        if (distToNearestSplinePoint < (m_CameraSplineDist * m_CameraSplineDist))
        {
            float splineDistRatio = (Mathf.Sqrt(distToNearestSplinePoint) / m_CameraSplineDist) * Time.deltaTime;
            m_CamTgtPos = nearestSplinePoint + m_CameraOffset;
            m_Camera.transform.position = Vector3.Lerp(playerPos + m_CameraOffset, m_CamTgtPos, splineDistRatio);
            m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.gameObject.transform.rotation, Quaternion.FromToRotation(Vector3.forward, splineTangent), splineDistRatio * m_CameraAutoRotationSpeed);
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

        if (m_DeathScreen != null)
        {
            if (m_PlayerController.Stats.Health <= 0.0f)
            {
                m_DeathScreen.gameObject.SetActive(true);
            }
            else if (m_PlayerController.Stats.Health > 0.0f)
            {
                m_DeathScreen.gameObject.SetActive(false);
            }
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
            Gizmos.color = Color.green;
            Gizmos.DrawRay(m_PlayerRef.transform.position + new Vector3(0.0f, 2.5f, 0.0f), m_playerForward);
        }
    }

    public void OnRespawnButtonPressed()
    {
        //Reload the scene...!
        Debug.Log("Respawn Button Pressed");
        SceneManager.LoadScene("Main_Level");
    }

    public void OnMainMenuButtonPressed()
    {
        //Reload the scene...!
        Debug.Log("Main Menu Button Pressed");
        SceneManager.LoadScene("Main_Level");
    }

}
