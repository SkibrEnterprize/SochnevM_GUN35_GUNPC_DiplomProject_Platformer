using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
        

    public void UpdateHealth(int health)
    {        
        _healthSlider.value = health;
    }

    public void SetFillColor(Color color)
    {
        _healthSlider.fillRect.GetComponent<Image>().color = color;
    }
    [ContextMenu("Do Something")]
    public void EnableFillArea(bool flag)
    {
        _healthSlider.fillRect.GetComponent<Image>().enabled = flag;
    }
}