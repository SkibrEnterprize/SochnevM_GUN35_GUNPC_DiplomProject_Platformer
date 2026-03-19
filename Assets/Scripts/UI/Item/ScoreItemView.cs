using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreItemView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _score;
      

    public void UpdateView(string score)
    {
        _score.text = score;
    }
    
}
