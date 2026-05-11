using System;
using System.Collections.Generic;

public class QuizCreatorPresenter
{
    private readonly IQuizCreatorMainView _mainView;
    private readonly QuizRepository _repository;
    private Quiz _currentQuiz;

    public QuizCreatorPresenter(IQuizCreatorMainView mainView, QuizRepository repository)
    {
        _mainView = mainView;
        _repository = repository;
        BindMainEvents();
    }

    public void Start()
    {
        _mainView.ShowStartScreen(true);
        RefreshAvailableQuizzes();
    }

    private void RefreshAvailableQuizzes()
    {
        _mainView.ClearAvailableQuizzes();
        List<Quiz> availableQuizzes = _repository.LoadAllQuizzes();

        foreach (Quiz quiz in availableQuizzes)
        {
            ILoadQuizItemView itemView = _mainView.CreateLoadQuizItem();
            itemView.QuizName = quiz.QuizName;
            itemView.OnLoadClicked += () => LoadExistingQuiz(quiz);
        }
    }

    private void BindMainEvents()
    {
        _mainView.OnQuizNameChanged += name =>
        {
            _currentQuiz.QuizName = name;
            UpdateValidationState();
        };

        _mainView.OnAddMultiChoiceClicked += () =>
        {
            MultipleChoiceQuestion newQuestion = new MultipleChoiceQuestion();
            _currentQuiz.Questions.Add(newQuestion);
            RefreshQuestionsList();
            UpdateValidationState();
        };

        _mainView.OnAddTrueFalseClicked += () =>
        {
            TrueFalseQuestion newQuestion = new TrueFalseQuestion("", true);
            _currentQuiz.Questions.Add(newQuestion);
            RefreshQuestionsList();
            UpdateValidationState();
        };

        _mainView.OnCloseWarningClicked += () =>
        {
            _mainView.HideWarning();
        };

        _mainView.OnSaveQuizClicked += TrySaveQuiz;
        _mainView.OnCreateNewQuizClicked += CreateNewQuiz;
    }

    private void CreateNewQuiz()
    {
        _currentQuiz = new Quiz("");
        _mainView.QuizName = "";
        _mainView.ShowStartScreen(false);
        _mainView.ClearQuestionsList();
        UpdateValidationState();
    }

    private void LoadExistingQuiz(Quiz loadedQuiz)
    {
        _currentQuiz = loadedQuiz;
        _mainView.QuizName = _currentQuiz.QuizName;
        _mainView.ShowStartScreen(false);
        RefreshQuestionsList();
        UpdateValidationState();
    }

    private void RefreshQuestionsList()
    {
        _mainView.ClearQuestionsList();

        foreach (Question question in _currentQuiz.Questions)
        {
            if (question is MultipleChoiceQuestion multiQuestion)
            {
                CreateMultiChoiceEditor(multiQuestion);
            }
            else if (question is TrueFalseQuestion tfQuestion)
            {
                CreateTrueFalseEditor(tfQuestion);
            }
        }
    }

    private void CreateMultiChoiceEditor(MultipleChoiceQuestion questionModel)
    {
        IMultipleChoiceEditorView view = _mainView.CreateMultiChoiceEditor();

        view.QuestionText = questionModel.QuestionText;
        view.MultiplierText = questionModel.Multiplier.ToString();

        view.OnQuestionTextChanged += val => questionModel.QuestionText = val;
        view.OnMultiplierChanged += val =>
        {
            if (float.TryParse(val, out float result)) questionModel.Multiplier = result;
        };

        view.OnRemoveQuestionClicked += () =>
        {
            _currentQuiz.Questions.Remove(questionModel);
            view.DestroyView();
            UpdateValidationState();
        };

        if (questionModel.Answers.Count < 2)
        {
            questionModel.Answers.Add(new Answer("", false));
            questionModel.Answers.Add(new Answer("", false));
        }

        RefreshAnswersList(questionModel, view);

        view.OnAddAnswerClicked += () =>
        {
            if (questionModel.Answers.Count < 10)
            {
                questionModel.Answers.Add(new Answer("", false));
                RefreshAnswersList(questionModel, view);
            }
        };
    }

    private void RefreshAnswersList(MultipleChoiceQuestion questionModel, IMultipleChoiceEditorView view)
    {
        view.ClearAnswers();

        foreach (Answer currentAnswer in questionModel.Answers)
        {
            IAnswerEditorView answerView = view.CreateAnswerEditor();

            answerView.AnswerText = currentAnswer.Text;
            answerView.IsCorrect = currentAnswer.IsCorrect;
            answerView.IsRemoveButtonEnabled = questionModel.Answers.Count > 2;

            answerView.OnAnswerTextChanged += val => currentAnswer.Text = val;
            answerView.OnIsCorrectChanged += isOn =>
            {
                currentAnswer.IsCorrect = isOn;
                UpdateValidationState();
            };

            answerView.OnRemoveAnswerClicked += () =>
            {
                questionModel.Answers.Remove(currentAnswer);
                RefreshAnswersList(questionModel, view);
                UpdateValidationState();
            };
        }

        view.IsAddAnswerButtonEnabled = questionModel.Answers.Count < 10;
    }

    private void CreateTrueFalseEditor(TrueFalseQuestion questionModel)
    {
        ITrueFalseEditorView view = _mainView.CreateTrueFalseEditor();

        view.QuestionText = questionModel.QuestionText;
        view.MultiplierText = questionModel.Multiplier.ToString();
        view.IsTrue = questionModel.Answers[0].IsCorrect;

        view.OnQuestionTextChanged += val => questionModel.QuestionText = val;
        view.OnMultiplierChanged += val =>
        {
            if (float.TryParse(val, out float result)) questionModel.Multiplier = result;
        };

        view.OnIsTrueChanged += isOn =>
        {
            questionModel.Answers[0].IsCorrect = isOn;
            questionModel.Answers[1].IsCorrect = !isOn;
            UpdateValidationState();
        };

        view.OnRemoveQuestionClicked += () =>
        {
            _currentQuiz.Questions.Remove(questionModel);
            view.DestroyView();
            UpdateValidationState();
        };
    }

    private void UpdateValidationState()
    {
        bool isCriticalStateValid = true;

        if (string.IsNullOrWhiteSpace(_currentQuiz.QuizName))
        {
            isCriticalStateValid = false;
        }

        foreach (var question in _currentQuiz.Questions)
        {
            if (question is MultipleChoiceQuestion mcQuestion)
            {
                bool hasAtLeastOneCorrect = false;
                foreach (var answer in mcQuestion.Answers)
                {
                    if (answer.IsCorrect) hasAtLeastOneCorrect = true;
                }

                if (!hasAtLeastOneCorrect) isCriticalStateValid = false;
            }
        }

        _mainView.IsSaveButtonEnabled = isCriticalStateValid;
    }

    private void TrySaveQuiz()
    {
        string errorMessage = "";

        if (_currentQuiz.Questions.Count == 0)
        {
            errorMessage = "Quiz musi zawieraæ przynajmniej jedno pytanie!";
        }
        else
        {
            for (int i = 0; i < _currentQuiz.Questions.Count; i++)
            {
                var q = _currentQuiz.Questions[i];
                if (string.IsNullOrWhiteSpace(q.QuestionText))
                {
                    errorMessage = $"Pytanie nr {i + 1} nie ma wpisanej treœci!";
                    break;
                }

                foreach (var a in q.Answers)
                {
                    if (string.IsNullOrWhiteSpace(a.Text))
                    {
                        errorMessage = $"Pytanie nr {i + 1} zawiera odpowiedŸ bez wpisanej treœci!";
                        break;
                    }
                }

                if (errorMessage != "") break;
            }
        }

        if (errorMessage != "")
        {
            _mainView.ShowWarning(errorMessage);
        }
        else
        {
            _repository.SaveQuizToFile(_currentQuiz);
        }
    }
}