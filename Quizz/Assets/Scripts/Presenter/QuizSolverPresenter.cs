using System;
using System.Collections.Generic;
using System.Text;

public class QuizSolverPresenter
{
    private readonly IQuizSolverMainView _mainView;
    private readonly QuizRepository _repository;

    private List<Quiz> _cachedQuizzes;
    private Quiz _activeQuiz;
    private int _currentQuestionIndex;
    private Dictionary<int, List<bool>> _userAnswers;

    private IMultiChoiceQuestionSolverView _activeMultiChoiceView;
    private ITrueFalseQuestionSolverView _activeTrueFalseView;

    public QuizSolverPresenter(IQuizSolverMainView mainView, QuizRepository repository)
    {
        _mainView = mainView;
        _repository = repository;

        BindEvents();
    }

    public void Start()
    {
        _mainView.ShowStartScreen(true);
        _mainView.ShowSolvingScreen(false);
        _mainView.ShowSummaryScreen(false);

        RefreshAvailableQuizzes();
    }

    private void BindEvents()
    {
        _mainView.OnQuizSelected += HandleQuizSelection;
        _mainView.OnNextQuestionClicked += HandleNextQuestion;
        _mainView.OnPreviousQuestionClicked += HandlePreviousQuestion;
        _mainView.OnFinishQuizClicked += HandleFinishQuiz;
        _mainView.OnReturnToMenuClicked += Start;
    }

    private void RefreshAvailableQuizzes()
    {
        _cachedQuizzes = _repository.LoadAllQuizzes();
        List<string> names = new List<string>();
        foreach (var q in _cachedQuizzes)
        {
            names.Add(q.QuizName);
        }
        _mainView.SetAvailableQuizzes(names);
    }

    private void HandleQuizSelection(int index)
    {
        if (index >= 0 && index < _cachedQuizzes.Count)
        {
            _activeQuiz = _cachedQuizzes[index];
            _currentQuestionIndex = 0;
            _userAnswers = new Dictionary<int, List<bool>>();

            for (int i = 0; i < _activeQuiz.Questions.Count; i++)
            {
                int answerCount = _activeQuiz.Questions[i].Answers.Count;
                _userAnswers[i] = new List<bool>(new bool[answerCount]);
            }

            _mainView.ShowStartScreen(false);
            _mainView.ShowSolvingScreen(true);

            DisplayCurrentQuestion();
        }
    }

    private void DisplayCurrentQuestion()
    {
        _mainView.ClearQuestionArea();
        _activeMultiChoiceView = null;
        _activeTrueFalseView = null;

        Question currentQuestion = _activeQuiz.Questions[_currentQuestionIndex];
        List<bool> savedSelections = _userAnswers[_currentQuestionIndex];

        if (currentQuestion is MultipleChoiceQuestion mcQuestion)
        {
            _activeMultiChoiceView = _mainView.CreateMultiChoiceSolver();

            List<string> answerTexts = new List<string>();
            foreach (var ans in mcQuestion.Answers)
            {
                answerTexts.Add(ans.Text);
            }

            _activeMultiChoiceView.Setup(mcQuestion.QuestionText, answerTexts, savedSelections);
        }
        else if (currentQuestion is TrueFalseQuestion tfQuestion)
        {
            _activeTrueFalseView = _mainView.CreateTrueFalseSolver();
            _activeTrueFalseView.Setup(tfQuestion.QuestionText, savedSelections);
        }

        string progress = $"Pytanie {_currentQuestionIndex + 1} z {_activeQuiz.Questions.Count}";
        _mainView.UpdateProgressIndicator(progress);

        bool canGoBack = _currentQuestionIndex > 0;
        bool isLast = _currentQuestionIndex == _activeQuiz.Questions.Count - 1;
        _mainView.SetNavigationButtonsState(canGoBack, isLast);
    }

    private void SaveCurrentSelections()
    {
        if (_activeMultiChoiceView != null)
        {
            _userAnswers[_currentQuestionIndex] = _activeMultiChoiceView.GetCurrentSelections();
        }
        else if (_activeTrueFalseView != null)
        {
            _userAnswers[_currentQuestionIndex] = _activeTrueFalseView.GetCurrentSelections();
        }
    }

    private void HandleNextQuestion()
    {
        SaveCurrentSelections();

        if (_currentQuestionIndex < _activeQuiz.Questions.Count - 1)
        {
            _currentQuestionIndex++;
            DisplayCurrentQuestion();
        }
    }

    private void HandlePreviousQuestion()
    {
        SaveCurrentSelections();

        if (_currentQuestionIndex > 0)
        {
            _currentQuestionIndex--;
            DisplayCurrentQuestion();
        }
    }

    private void HandleFinishQuiz()
    {
        SaveCurrentSelections();

        float totalScore = 0f;
        float maxScore = 0f;
        StringBuilder reviewBuilder = new StringBuilder();

        for (int i = 0; i < _activeQuiz.Questions.Count; i++)
        {
            Question q = _activeQuiz.Questions[i];
            List<bool> selections = _userAnswers[i];

            float score = q.CalculateScore(selections);
            totalScore += score;
            maxScore += q.Multiplier;

            reviewBuilder.AppendLine($"Pytanie {i + 1}: {q.QuestionText}");
            reviewBuilder.AppendLine($"Zdobyte punkty: {score} / {q.Multiplier}");

            for (int j = 0; j < q.Answers.Count; j++)
            {
                string status = "";
                if (q.Answers[j].IsCorrect && selections[j]) status = "[Poprawnie zaznaczono]";
                if (!q.Answers[j].IsCorrect && !selections[j]) status = "[Poprawnie pominięto]";
                if (q.Answers[j].IsCorrect && !selections[j]) status = "[Brakująca odpowiedź]";
                if (!q.Answers[j].IsCorrect && selections[j]) status = "[Błędne zaznaczenie]";

                reviewBuilder.AppendLine($"- {q.Answers[j].Text} {status}");
            }
            reviewBuilder.AppendLine();
        }

        string finalScoreText = $"Wynik: {totalScore} / {maxScore}";

        _mainView.ShowSolvingScreen(false);
        _mainView.SetSummaryData(finalScoreText, reviewBuilder.ToString());
        _mainView.ShowSummaryScreen(true);
    }
}