using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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

    private List<QuestionSummaryData> _summaryData;
    private int _currentSummaryIndex;
    private float _currentDisplayedScore;
    private float _totalMaxScore;

    private float _timeRemaining;
    private bool _isTimerRunning;
    private bool _warningSoundPlayed;

    public QuizSolverPresenter(IQuizSolverMainView mainView, QuizRepository repository)
    {
        _mainView = mainView;
        _repository = repository;

        BindEvents();
    }

    public void Start()
    {
        _isTimerRunning = false;
        _mainView.StopTimerWarningSound();

        _mainView.ShowStartScreen(true);
        _mainView.ShowSolvingScreen(false);
        _mainView.ShowSummaryScreen(false);

        RefreshAvailableQuizzes();
    }

    private void BindEvents()
    {
        _mainView.OnNextQuestionClicked += HandleNextQuestion;
        _mainView.OnPreviousQuestionClicked += HandlePreviousQuestion;
        _mainView.OnFinishQuizClicked += HandleFinishQuiz;
        _mainView.OnReturnToMenuClicked += Start;
        _mainView.OnContinueSummaryAnimationRequested += HandleContinueSummaryAnimation;
        _mainView.OnUpdateTick += HandleUpdateTick;
    }

    private void RefreshAvailableQuizzes()
    {
        _mainView.ClearAvailableQuizzes();
        _cachedQuizzes = _repository.LoadAllQuizzes();

        for (int i = 0; i < _cachedQuizzes.Count; i++)
        {
            int index = i;
            Quiz quiz = _cachedQuizzes[i];

            ISolverQuizItemView itemView = _mainView.CreateQuizItem();
            itemView.QuizName = quiz.QuizName;

            itemView.OnQuizClicked += () => HandleQuizSelection(index);
        }
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

            _timeRemaining = 600f;
            _isTimerRunning = true;
            _warningSoundPlayed = false;

            UpdateTimerText();

            _mainView.ShowStartScreen(false);
            _mainView.ShowSolvingScreen(true);

            DisplayCurrentQuestion();
        }
    }

    private void HandleUpdateTick(float deltaTime)
    {
        if (!_isTimerRunning) return;

        _timeRemaining -= deltaTime;

        if (_timeRemaining <= 10f && !_warningSoundPlayed)
        {
            _warningSoundPlayed = true;
            _mainView.PlayTimerWarningSound();
        }

        if (_timeRemaining <= 0f)
        {
            _timeRemaining = 0f;
            HandleFinishQuiz();
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(_timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60f);
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        _mainView.UpdateTimerDisplay(formattedTime);
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
        float timeTaken = 600f - _timeRemaining;
        int timeTakenMinutes = Mathf.FloorToInt(timeTaken / 60f);
        int timeTakenSeconds = Mathf.FloorToInt(timeTaken % 60f);
        string formattedTimeTakenText = string.Format("czas: {0:00}:{1:00}", timeTakenMinutes, timeTakenSeconds);

        _isTimerRunning = false;
        _mainView.StopTimerWarningSound();

        SaveCurrentSelections();

        _summaryData = new List<QuestionSummaryData>();
        _totalMaxScore = 0f;
        _currentDisplayedScore = 0f;
        _currentSummaryIndex = 0;

        for (int i = 0; i < _activeQuiz.Questions.Count; i++)
        {
            Question q = _activeQuiz.Questions[i];
            List<bool> selections = _userAnswers[i];

            float score = q.CalculateScore(selections);
            _totalMaxScore += q.Multiplier;

            StringBuilder details = new StringBuilder();
            for (int j = 0; j < q.Answers.Count; j++)
            {
                string status = "";
                if (q.Answers[j].IsCorrect && selections[j]) status = "[Poprawnie zaznaczono]";
                if (!q.Answers[j].IsCorrect && !selections[j]) status = "[Poprawnie pominięto]";
                if (q.Answers[j].IsCorrect && !selections[j]) status = "[Brakująca odpowiedź]";
                if (!q.Answers[j].IsCorrect && selections[j]) status = "[Błędne zaznaczenie]";

                details.AppendLine($"- {q.Answers[j].Text} {status}");
            }

            _summaryData.Add(new QuestionSummaryData
            {
                QuestionText = $"Pytanie {i + 1}: {q.QuestionText}",
                Multiplier = q.Multiplier,
                EarnedScore = score,
                StatusDetails = details.ToString()
            });
        }

        _mainView.ShowSolvingScreen(false);
        _mainView.PrepareSummaryView(_totalMaxScore, formattedTimeTakenText);
        _mainView.ShowSummaryScreen(true);

        HandleContinueSummaryAnimation();
    }

    private void HandleContinueSummaryAnimation()
    {
        if (_currentSummaryIndex < _summaryData.Count)
        {
            QuestionSummaryData data = _summaryData[_currentSummaryIndex];
            _currentDisplayedScore += data.EarnedScore;

            bool isPerfectScore = data.EarnedScore >= data.Multiplier;
            _mainView.PlaySummarySound(isPerfectScore);

            ISummaryItemView itemView = _mainView.CreateSummaryItem();
            string scoreInfo = $"Zdobyte punkty: {data.EarnedScore} / {data.Multiplier}";

            itemView.Setup(data.QuestionText, scoreInfo, data.StatusDetails);
            _mainView.UpdateCurrentScore(_currentDisplayedScore, _totalMaxScore);

            _currentSummaryIndex++;
        }
        else
        {
            _mainView.OnSummaryAnimationFinished();
        }
    }
}