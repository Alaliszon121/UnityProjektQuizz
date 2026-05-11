using System;
using System.Collections.Generic;

public interface IQuizSolverMainView
{
    event Action<int> OnQuizSelected;
    event Action OnNextQuestionClicked;
    event Action OnPreviousQuestionClicked;
    event Action OnFinishQuizClicked;
    event Action OnReturnToMenuClicked;

    void SetAvailableQuizzes(List<string> quizNames);
    int GetSelectedQuizIndex();

    void ShowStartScreen(bool show);
    void ShowSolvingScreen(bool show);
    void ShowSummaryScreen(bool show);

    void UpdateProgressIndicator(string progressText);
    void SetNavigationButtonsState(bool canGoBack, bool isLastQuestion);
    void ClearQuestionArea();
    void SetSummaryData(string scoreText, string detailedReview);

    IMultiChoiceQuestionSolverView CreateMultiChoiceSolver();
    ITrueFalseQuestionSolverView CreateTrueFalseSolver();
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