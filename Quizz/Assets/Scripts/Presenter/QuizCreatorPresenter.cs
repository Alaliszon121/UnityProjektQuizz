using UnityEngine;
using TMPro;

public class QuizCreatorPresenter : MonoBehaviour
{
    [Header("Referencje MVP")]
    public QuizCreatorMainView MainView;
    private QuizRepository _repository;

    [Header("Prefaby EdytorÛw")]
    public GameObject MultiChoiceEditorPrefab;
    public GameObject TrueFalseEditorPrefab;

    // MODEL
    private Quiz _currentQuiz;

    private void Start()
    {
        _repository = new QuizRepository();
        _currentQuiz = new Quiz("");

        BindMainEvents();
    }

    private void BindMainEvents()
    {
        MainView.QuizNameInput.onValueChanged.AddListener(newName =>
        {
            _currentQuiz.QuizName = newName;
        });

        MainView.AddMultiChoiceButton.onClick.AddListener(() =>
        {
            _currentQuiz.Questions.Add(new MultipleChoiceQuestion());
            RefreshQuestionsList();
        });

        MainView.AddTrueFalseButton.onClick.AddListener(() =>
        {
            _currentQuiz.Questions.Add(new TrueFalseQuestion("", true));
            RefreshQuestionsList();
        });

        MainView.SaveQuizButton.onClick.AddListener(SaveCurrentQuiz);
    }

    private void RefreshQuestionsList()
    {
        foreach (Transform child in MainView.QuestionsListContainer)
        {
            Destroy(child.gameObject);
        }

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
        GameObject editorObj = Instantiate(MultiChoiceEditorPrefab, MainView.QuestionsListContainer);
        MultipleChoiceEditorView view = editorObj.GetComponent<MultipleChoiceEditorView>();

        view.QuestionTextInput.text = questionModel.QuestionText;
        view.MultiplierInput.text = questionModel.Multiplier.ToString();

        view.QuestionTextInput.onValueChanged.AddListener(val => questionModel.QuestionText = val);
        view.MultiplierInput.onValueChanged.AddListener(val =>
        {
            if (float.TryParse(val, out float result)) questionModel.Multiplier = result;
        });

        view.RemoveQuestionButton.onClick.AddListener(() =>
        {
            _currentQuiz.Questions.Remove(questionModel);
            RefreshQuestionsList();
        });

        // TODO Task 8: Obs≥uga przycisku "Dodaj Odpowiedü" w view.AddAnswerButton
    }

    private void CreateTrueFalseEditor(TrueFalseQuestion questionModel)
    {
        // PREZENTER
        GameObject editorObj = Instantiate(TrueFalseEditorPrefab, MainView.QuestionsListContainer);
        TrueFalseEditorView view = editorObj.GetComponent<TrueFalseEditorView>();

        view.QuestionTextInput.text = questionModel.QuestionText;
        view.MultiplierInput.text = questionModel.Multiplier.ToString();

        view.IsTrueToggle.isOn = questionModel.Answers[0].IsCorrect;

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
        });

        view.RemoveQuestionButton.onClick.AddListener(() =>
        {
            _currentQuiz.Questions.Remove(questionModel);
            RefreshQuestionsList();
        });
    }

    private void SaveCurrentQuiz()
    {
        // TODO: Walidacja przed zapisem
        _repository.SaveQuizToFile(_currentQuiz);
        Debug.Log("Zapisano quiz z nowym interfejsem Google Forms!");
    }
}