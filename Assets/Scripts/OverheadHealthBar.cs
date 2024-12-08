using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class OverheadHealthBar : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private UnityEngine.UI.Slider healthSlider;
    private Camera camera;
    private Canvas canvas;

    private void Awake()
    {
        camera = FindFirstObjectByType<Camera>();     //Note: Possibly bugprone
        canvas = this.gameObject.GetComponentInChildren<Canvas>();
        canvas.worldCamera = camera;

        healthSlider.gameObject.SetActive(false);
        //healthSlider = GetComponentInChildren<UnityEngine.UI.Slider>();
    }
    void FixedUpdate()
    {
        if (stats.Health < stats.MaxHealth)
        {
            healthSlider.gameObject.SetActive(true);
        }

        //Face the camera
        transform.LookAt(camera.transform.position);
        healthSlider.value = (stats.Health / stats.MaxHealth);

    }
}
