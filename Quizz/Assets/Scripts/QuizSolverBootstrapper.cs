using UnityEngine;

public class QuizSolverBootstrapper : MonoBehaviour
{
    public QuizSolverMainView SolverView;

    private QuizSolverPresenter _presenter;

    private void Start()
    {
        QuizRepository repository = new QuizRepository();

        _presenter = new QuizSolverPresenter(SolverView, repository);
        _presenter.Start();
    }
}