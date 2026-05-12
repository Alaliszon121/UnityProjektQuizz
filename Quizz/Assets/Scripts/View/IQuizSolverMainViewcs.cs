using System;
using System.Collections.Generic;

public interface IQuizSolverMainView
{
    event Action OnNextQuestionClicked;
    event Action OnPreviousQuestionClicked;
    event Action OnFinishQuizClicked;
    event Action OnReturnToMenuClicked;
    event Action OnContinueSummaryAnimationRequested;
    event Action<float> OnUpdateTick;

    void ClearAvailableQuizzes();
    ISolverQuizItemView CreateQuizItem();

    void ShowStartScreen(bool show);
    void ShowSolvingScreen(bool show);
    void ShowSummaryScreen(bool show);

    void UpdateProgressIndicator(string progressText);
    void SetNavigationButtonsState(bool canGoBack, bool isLastQuestion);
    void ClearQuestionArea();

    void PrepareSummaryView(float maxScore, string timeTakenText);
    ISummaryItemView CreateSummaryItem();
    void UpdateCurrentScore(float currentScore, float maxScore);
    void OnSummaryAnimationFinished();
    void PlaySummarySound(bool isPerfectScore);

    void UpdateTimerDisplay(string timeText);
    void PlayTimerWarningSound();
    void StopTimerWarningSound();

    IMultiChoiceQuestionSolverView CreateMultiChoiceSolver();
    ITrueFalseQuestionSolverView CreateTrueFalseSolver();
}

public interface ISolverQuizItemView
{
    string QuizName { set; }
    event Action OnQuizClicked;
}

public interface IMultiChoiceQuestionSolverView
{
    void Setup(string questionText, List<string> answers, List<bool> previousSelections);
    List<bool> GetCurrentSelections();
}

public interface ITrueFalseQuestionSolverView
{
    void Setup(string questionText, List<bool> previousSelections);
    List<bool> GetCurrentSelections();
}