using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    //private void Awake()
    //{
    //    _fillArea = healthSlider.fillRect.GetComponent<Image>();
    //}

    public void UpdateHealth(int health)
    {        
        healthSlider.value = health;
        Debug.Log("Update Health view");
    }

    public void SetFillColor(Color color)
    {
        healthSlider.fillRect.GetComponent<Image>().color = color;
    }
    [ContextMenu("Do Something")]
    public void DisableFillArea()
    {
        healthSlider.fillRect.GetComponent<Image>().enabled = false;
    }
}