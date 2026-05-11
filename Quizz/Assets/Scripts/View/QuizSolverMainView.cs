using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizSolverMainView : MonoBehaviour, IQuizSolverMainView
{
    [Header("Screens")]
    public GameObject StartScreen;
    public GameObject SolvingScreen;
    public GameObject SummaryScreen;

    [Header("Start Screen UI")]
    public TMP_Dropdown QuizDropdown;
    public Button StartQuizButton;

    [Header("Solving Screen UI")]
    public TMP_Text ProgressIndicator;
    public Transform QuestionContainer;
    public Button NextButton;
    public Button PrevButton;
    public Button FinishButton;

    [Header("Summary Screen UI")]
    public TMP_Text ScoreText;
    public TMP_Text DetailedReviewText;
    public Button ReturnToMenuButton;

    [Header("Prefabs")]
    public GameObject MultiChoicePrefab;
    public GameObject TrueFalsePrefab;

    public event Action<int> OnQuizSelected;
    public event Action OnNextQuestionClicked;
    public event Action OnPreviousQuestionClicked;
    public event Action OnFinishQuizClicked;
    public event Action OnReturnToMenuClicked;

    private void Awake()
    {
        StartQuizButton.onClick.AddListener(() => OnQuizSelected?.Invoke(QuizDropdown.value));
        NextButton.onClick.AddListener(() => OnNextQuestionClicked?.Invoke());
        PrevButton.onClick.AddListener(() => OnPreviousQuestionClicked?.Invoke());
        FinishButton.onClick.AddListener(() => OnFinishQuizClicked?.Invoke());
        ReturnToMenuButton.onClick.AddListener(() => OnReturnToMenuClicked?.Invoke());
    }

    public void SetAvailableQuizzes(List<string> quizNames)
    {
        QuizDropdown.ClearOptions();
        QuizDropdown.AddOptions(quizNames);
    }

    public int GetSelectedQuizIndex() => QuizDropdown.value;

    public void ShowStartScreen(bool show) => StartScreen.SetActive(show);
    public void ShowSolvingScreen(bool show) => SolvingScreen.SetActive(show);
    public void ShowSummaryScreen(bool show) => SummaryScreen.SetActive(show);

    public void UpdateProgressIndicator(string progressText) => ProgressIndicator.text = progressText;

    public void SetNavigationButtonsState(bool canGoBack, bool isLastQuestion)
    {
        PrevButton.gameObject.SetActive(canGoBack);
        NextButton.gameObject.SetActive(!isLastQuestion);
        FinishButton.gameObject.SetActive(isLastQuestion);
    }

    public void ClearQuestionArea()
    {
        foreach (Transform child in QuestionContainer) Destroy(child.gameObject);
    }

    public void SetSummaryData(string scoreText, string detailedReview)
    {
        ScoreText.text = scoreText;
        DetailedReviewText.text = detailedReview;
    }

    public IMultiChoiceQuestionSolverView CreateMultiChoiceSolver()
    {
        var obj = Instantiate(MultiChoicePrefab, QuestionContainer);
        return obj.GetComponent<MultiChoiceQuestionSolverView>();
    }

    public ITrueFalseQuestionSolverView CreateTrueFalseSolver()
    {
        var obj = Instantiate(TrueFalsePrefab, QuestionContainer);
        return obj.GetComponent<TrueFalseQuestionSolverView>();
    }
}