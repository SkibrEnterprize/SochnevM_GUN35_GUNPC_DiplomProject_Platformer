using Player;
using UnityEngine;
using Zenject;

public class HealthPresenter
{
    private readonly HealthModel _model;
    private readonly HealthView _view;
    private readonly Color _criticalColor = Color.red;
    private readonly Color _normalColor = Color.green;
   

    public HealthPresenter(HealthModel model, HealthView view)
    {
        _model = model;
        _view = view;

        _model.OnHealthChanged += UpdateView;
        UpdateView(_model.Health);
    }

    private void UpdateView(int health)
    {
        _view.UpdateHealth(health);
        if (health <= 0)
        {
            _view.DisableFillArea();         
        }
        else if (health < 20)
        {
            _view.SetFillColor(_criticalColor);
        }
        else
        {
            _view.SetFillColor(_normalColor);
        }
        Debug.Log("!!!!!!!!!!");
    }

    //public void ChangeHealth(int delta)
    //{
    //    _model.Health += delta;
    //}
}
