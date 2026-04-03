using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Slider _chargeSlider;
    [SerializeField] private Image _fillImage; 

    public void Show(bool visible)
    {
        _root.SetActive(visible);
        if (visible) _chargeSlider.value = 0;
    }

    public void UpdateProgress(float progress)
    {
        _chargeSlider.value = progress;
       
        _fillImage.color = Color.Lerp(Color.yellow, Color.red, progress);
    }
}