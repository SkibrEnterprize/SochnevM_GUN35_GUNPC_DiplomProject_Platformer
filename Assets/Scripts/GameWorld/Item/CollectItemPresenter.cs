public class CollectItemPresenter
{   
    private readonly CollectItemModel _model;
    private readonly ScoreItemView _view;

    public CollectItemPresenter(CollectItemModel model, ScoreItemView view)
    {
        _model = model;
        _view = view;

        _model.OnCountChanged += UpdateView;
        UpdateView(_model.Score);
    }

    private void UpdateView(int score)
    {
        string scoreText = score.ToString();
        _view.UpdateView(scoreText);
    }
}


