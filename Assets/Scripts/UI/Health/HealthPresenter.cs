using Player;
using UnityEngine;

public class HealthPresenter
{
    private readonly HealthModel _model;
    private readonly HealthView _view;

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
        Debug.Log("!!!!!!!!!!");
    }

    //public void ChangeHealth(int delta)
    //{
    //    _model.Health += delta;
    //}
}
