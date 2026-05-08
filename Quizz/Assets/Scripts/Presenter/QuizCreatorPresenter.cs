using UnityEngine;
using TMPro;

public class QuizCreatorPresenter : MonoBehaviour
{
    // VIEW
    [Header("Referencje MVP")]
    public QuizCreatorMainView MainView;

    // MODEL
    private QuizRepository _repository;
    private Quiz _currentQuiz;

    // VIEW
    [Header("Prefaby")]
    public GameObject MultiChoiceEditorPrefab;
    public GameObject TrueFalseEditorPrefab;
    public GameObject AnswerEditorPrefab;
    public GameObject LoadQuizItemPrefab;

    private void Start()
    {
        // PREZENTER
        _repository = new QuizRepository();

        BindMainEvents();
        InitializeStartScreen();
    }

    private void BindMainEvents()
    {
        // VIEW / PREZENTER
        MainView.QuizNameInput.onValueChanged.AddListener(newName =>
        {
            // MODEL
            _currentQuiz.QuizName = newName;
            // PREZENTER
            UpdateValidationState();
        });

        MainView.AddMultiChoiceButton.onClick.AddListener(() =>
        {
            // MODEL
            _currentQuiz.Questions.Add(new MultipleChoiceQuestion());
            // PREZENTER
            RefreshQuestionsList();
            UpdateValidationState();
        });

        MainView.AddTrueFalseButton.onClick.AddListener(() =>
        {
            // MODEL
            _currentQuiz.Questions.Add(new TrueFalseQuestion("", true));
            // PREZENTER
            RefreshQuestionsList();
            UpdateValidationState();
        });

        MainView.CloseWarningPopupButton.onClick.AddListener(() =>
        {
            // VIEW
            MainView.WarningPopupPanel.SetActive(false);
        });

        MainView.SaveQuizButton.onClick.AddListener(TrySaveQuiz);
    }

    public void InitializeStartScreen()
    {
        // VIEW
        MainView.StartScreenPanel.SetActive(true);

        foreach (Transform child in MainView.AvailableQuizzesContainer)
        {
            Destroy(child.gameObject);
        }

        // MODEL
        var availableQuizzes = _repository.LoadAllQuizzes();

        // PREZENTER
        foreach (var quiz in availableQuizzes)
        {
            // VIEW
            GameObject itemObj = Instantiate(LoadQuizItemPrefab, MainView.AvailableQuizzesContainer);
            LoadQuizItemView itemView = itemObj.GetComponent<LoadQuizItemView>();

            itemView.QuizNameText.text = quiz.QuizName;

            // PREZENTER
            itemView.LoadButton.onClick.AddListener(() => LoadExistingQuiz(quiz));
        }

        // PREZENTER
        MainView.CreateNewQuizButton.onClick.RemoveAllListeners();
        MainView.CreateNewQuizButton.onClick.AddListener(CreateNewQuiz);
    }

    private void CreateNewQuiz()
    {
        // MODEL
        _currentQuiz = new Quiz("");

        // VIEW
        MainView.QuizNameInput.text = "";
        MainView.StartScreenPanel.SetActive(false);

        // PREZENTER
        RefreshQuestionsList();
        UpdateValidationState();
    }

    private void LoadExistingQuiz(Quiz loadedQuiz)
    {
        // MODEL
        _currentQuiz = loadedQuiz;

        // VIEW
        MainView.QuizNameInput.text = _currentQuiz.QuizName;
        MainView.StartScreenPanel.SetActive(false);

        // PREZENTER
        RefreshQuestionsList();
        UpdateValidationState();
    }

    private void RefreshQuestionsList()
    {
        // VIEW
        foreach (Transform child in MainView.QuestionsListContainer)
        {
            Destroy(child.gameObject);
        }

        // PREZENTER
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
        // PREZENTER
        GameObject editorObj = Instantiate(MultiChoiceEditorPrefab, MainView.QuestionsListContainer);
        MultipleChoiceEditorView view = editorObj.GetComponent<MultipleChoiceEditorView>();

        // VIEW
        view.QuestionTextInput.text = questionModel.QuestionText;
        view.MultiplierInput.text = questionModel.Multiplier.ToString();

        // PREZENTER
        view.QuestionTextInput.onValueChanged.AddListener(val => questionModel.QuestionText = val);
        view.MultiplierInput.onValueChanged.AddListener(val =>
        {
            if (float.TryParse(val, out float result)) questionModel.Multiplier = result;
        });

        view.RemoveQuestionButton.onClick.AddListener(() =>
        {
            // MODEL
            _currentQuiz.Questions.Remove(questionModel);
            // PREZENTER
            RefreshQuestionsList();
            UpdateValidationState();
        });

        // MODEL
        if (questionModel.Answers.Count < 2)
        {
            questionModel.Answers.Add(new Answer("", false));
            questionModel.Answers.Add(new Answer("", false));
        }

        // PREZENTER
        RefreshAnswersList(questionModel, view);

        view.AddAnswerButton.onClick.AddListener(() =>
        {
            if (questionModel.Answers.Count < 10)
            {
                // MODEL
                questionModel.Answers.Add(new Answer("", false));
                // PREZENTER
                RefreshAnswersList(questionModel, view);
            }
        });
    }

    private void RefreshAnswersList(MultipleChoiceQuestion questionModel, MultipleChoiceEditorView view)
    {
        // VIEW
        foreach (Transform child in view.AnswersContainer)
        {
            Destroy(child.gameObject);
        }

        // PREZENTER
        for (int i = 0; i < questionModel.Answers.Count; i++)
        {
            Answer currentAnswer = questionModel.Answers[i];

            // VIEW
            GameObject answerObj = Instantiate(AnswerEditorPrefab, view.AnswersContainer);
            AnswerEditorView answerView = answerObj.GetComponent<AnswerEditorView>();

            answerView.AnswerTextInput.text = currentAnswer.Text;
            answerView.IsCorrectToggle.isOn = currentAnswer.IsCorrect;

            answerView.RemoveAnswerButton.interactable = questionModel.Answers.Count > 2;

            // PREZENTER
            answerView.AnswerTextInput.onValueChanged.AddListener(val => currentAnswer.Text = val);
            answerView.IsCorrectToggle.onValueChanged.AddListener(isOn =>
            {
                // MODEL
                currentAnswer.IsCorrect = isOn;
                // PREZENTER
                UpdateValidationState();
            });

            answerView.RemoveAnswerButton.onClick.AddListener(() =>
            {
                // MODEL
                questionModel.Answers.Remove(currentAnswer);
                // PREZENTER
                RefreshAnswersList(questionModel, view);
                UpdateValidationState();
            });
        }

        // VIEW
        view.AddAnswerButton.interactable = questionModel.Answers.Count < 10;
    }

    private void CreateTrueFalseEditor(TrueFalseQuestion questionModel)
    {
        // PREZENTER
        GameObject editorObj = Instantiate(TrueFalseEditorPrefab, MainView.QuestionsListContainer);
        TrueFalseEditorView view = editorObj.GetComponent<TrueFalseEditorView>();

        // VIEW
        view.QuestionTextInput.text = questionModel.QuestionText;
        view.MultiplierInput.text = questionModel.Multiplier.ToString();
        view.IsTrueToggle.isOn = questionModel.Answers[0].IsCorrect;

        // PREZENTER
        view.QuestionTextInput.onValueChanged.AddListener(val => questionModel.QuestionText = val);
        view.MultiplierInput.onValueChanged.AddListener(val =>
        {
            if (float.TryParse(val, out float result)) questionModel.Multiplier = result;
        });

        view.IsTrueToggle.onValueChanged.AddListener(isOn =>
        {
            // MODEL
            questionModel.Answers[0].IsCorrect = isOn;
            questionModel.Answers[1].IsCorrect = !isOn;
            // PREZENTER
            UpdateValidationState();
        });

        view.RemoveQuestionButton.onClick.AddListener(() =>
        {
            // MODEL
            _currentQuiz.Questions.Remove(questionModel);
            // PREZENTER
            RefreshQuestionsList();
            UpdateValidationState();
        });
    }

    private void UpdateValidationState()
    {
        // MODEL
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

        // VIEW
        MainView.SaveQuizButton.interactable = isCriticalStateValid;
    }

    private void TrySaveQuiz()
    {
        // MODEL
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

        // PREZENTER
        if (errorMessage != "")
        {
            // VIEW
            MainView.WarningPopupText.text = errorMessage;
            MainView.WarningPopupPanel.SetActive(true);
        }
        else
        {
            // MODEL
            _repository.SaveQuizToFile(_currentQuiz);
            Debug.Log("Zapisano quiz! Walidacja zakoñczona sukcesem.");
        }
    }
}