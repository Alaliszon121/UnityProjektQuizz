using UnityEngine;

public class QuizAppBootstrapper : MonoBehaviour
{
    public QuizCreatorMainView MainView;

    private QuizCreatorPresenter _presenter;

    private void Start()
    {
        QuizRepository repository = new QuizRepository();

        _presenter = new QuizCreatorPresenter(MainView, repository);
        _presenter.Start();
    }
}