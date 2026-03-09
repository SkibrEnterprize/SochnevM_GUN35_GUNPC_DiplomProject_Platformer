using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    public void UpdateHealth(int health)
    {
        healthSlider.value = health;
        Debug.Log("Update Health view");
    }
}