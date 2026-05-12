using System;
using System.Collections;
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
    public Transform AvailableQuizzesContainer;
    public GameObject SolverQuizItemPrefab;

    [Header("Solving Screen UI")]
    public TMP_Text ProgressIndicator;
    public TMP_Text TimerText;
    public Transform QuestionContainer;
    public Button NextButton;
    public Button PrevButton;
    public Button FinishButton;

    [Header("Summary Screen UI")]
    public TMP_Text ScoreText;
    public TMP_Text TotalTimeText;
    public Transform SummaryContentContainer;
    public GameObject SummaryItemPrefab;
    public ScrollRect SummaryScrollRect;
    public Button ReturnToMenuButton;

    [Header("Audio")]
    public AudioSource SummaryAudioSource;
    public AudioSource TimerAudioSource;
    public AudioClip PerfectScoreClip;
    public AudioClip MistakeClip;
    public AudioClip TimerWarningClip;

    [Header("Prefabs")]
    public GameObject MultiChoicePrefab;
    public GameObject TrueFalsePrefab;

    public event Action OnNextQuestionClicked;
    public event Action OnPreviousQuestionClicked;
    public event Action OnFinishQuizClicked;
    public event Action OnReturnToMenuClicked;
    public event Action OnContinueSummaryAnimationRequested;
    public event Action<float> OnUpdateTick;

    private void Awake()
    {
        NextButton.onClick.AddListener(() => OnNextQuestionClicked?.Invoke());
        PrevButton.onClick.AddListener(() => OnPreviousQuestionClicked?.Invoke());
        FinishButton.onClick.AddListener(() => OnFinishQuizClicked?.Invoke());
        ReturnToMenuButton.onClick.AddListener(() => OnReturnToMenuClicked?.Invoke());
    }

    private void Update()
    {
        OnUpdateTick?.Invoke(Time.deltaTime);
    }

    public void ClearAvailableQuizzes()
    {
        foreach (Transform child in AvailableQuizzesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public ISolverQuizItemView CreateQuizItem()
    {
        GameObject obj = Instantiate(SolverQuizItemPrefab, AvailableQuizzesContainer);
        return obj.GetComponent<ISolverQuizItemView>();
    }

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

    public void PrepareSummaryView(float maxScore, string timeTakenText)
    {
        ScoreText.text = $"Wynik: 0 / {maxScore}";
        if (TotalTimeText != null)
        {
            TotalTimeText.text = timeTakenText;
        }
        foreach (Transform child in SummaryContentContainer) Destroy(child.gameObject);
        ReturnToMenuButton.gameObject.SetActive(false);
    }

    public ISummaryItemView CreateSummaryItem()
    {
        GameObject obj = Instantiate(SummaryItemPrefab, SummaryContentContainer);
        ISummaryItemView itemView = obj.GetComponent<ISummaryItemView>();

        itemView.OnItemAnimationFinished += () => OnContinueSummaryAnimationRequested?.Invoke();
        StartCoroutine(ScrollToBottom());

        return itemView;
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        SummaryScrollRect.verticalNormalizedPosition = 0f;
    }

    public void UpdateCurrentScore(float currentScore, float maxScore)
    {
        ScoreText.text = $"Wynik: {currentScore} / {maxScore}";
    }

    public void OnSummaryAnimationFinished()
    {
        ReturnToMenuButton.gameObject.SetActive(true);
    }

    public void PlaySummarySound(bool isPerfectScore)
    {
        if (SummaryAudioSource != null)
        {
            AudioClip clipToPlay = isPerfectScore ? PerfectScoreClip : MistakeClip;
            if (clipToPlay != null)
            {
                SummaryAudioSource.PlayOneShot(clipToPlay);
            }
        }
    }

    public void UpdateTimerDisplay(string timeText)
    {
        if (TimerText != null)
        {
            TimerText.text = timeText;
        }
    }

    public void PlayTimerWarningSound()
    {
        if (TimerAudioSource != null && TimerWarningClip != null)
        {
            TimerAudioSource.clip = TimerWarningClip;
            TimerAudioSource.Play();
        }
    }

    public void StopTimerWarningSound()
    {
        if (TimerAudioSource != null && TimerAudioSource.isPlaying)
        {
            TimerAudioSource.Stop();
        }
    }

    public IMultiChoiceQuestionSolverView CreateMultiChoiceSolver()
    {
        var obj = Instantiate(MultiChoicePrefab, QuestionContainer);
        return obj.GetComponent<IMultiChoiceQuestionSolverView>();
    }

    public ITrueFalseQuestionSolverView CreateTrueFalseSolver()
    {
        var obj = Instantiate(TrueFalsePrefab, QuestionContainer);
        return obj.GetComponent<ITrueFalseQuestionSolverView>();
    }
}