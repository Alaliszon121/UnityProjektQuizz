using System;

public interface IQuizCreatorMainView
{
    string QuizName { get; set; }
    bool IsSaveButtonEnabled { set; }

    event Action<string> OnQuizNameChanged;
    event Action OnAddMultiChoiceClicked;
    event Action OnAddTrueFalseClicked;
    event Action OnSaveQuizClicked;
    event Action OnCreateNewQuizClicked;
    event Action OnCloseWarningClicked;

    void ShowStartScreen(bool show);
    void ShowWarning(string message);
    void HideWarning();
    void ClearQuestionsList();
    void ClearAvailableQuizzes();

    ILoadQuizItemView CreateLoadQuizItem();
    IMultipleChoiceEditorView CreateMultiChoiceEditor();
    ITrueFalseEditorView CreateTrueFalseEditor();
}

public interface ILoadQuizItemView
{
    string QuizName { set; }
    event Action OnLoadClicked;
}

public interface IMultipleChoiceEditorView
{
    string QuestionText { get; set; }
    string MultiplierText { get; set; }
    bool IsAddAnswerButtonEnabled { set; }

    event Action<string> OnQuestionTextChanged;
    event Action<string> OnMultiplierChanged;
    event Action OnRemoveQuestionClicked;
    event Action OnAddAnswerClicked;

    void ClearAnswers();
    IAnswerEditorView CreateAnswerEditor();
    void DestroyView();
}

public interface ITrueFalseEditorView
{
    string QuestionText { get; set; }
    string MultiplierText { get; set; }
    bool IsTrue { get; set; }

    event Action<string> OnQuestionTextChanged;
    event Action<string> OnMultiplierChanged;
    event Action<bool> OnIsTrueChanged;
    event Action OnRemoveQuestionClicked;

    void DestroyView();
}

public interface IAnswerEditorView
{
    string AnswerText { get; set; }
    bool IsCorrect { get; set; }
    bool IsRemoveButtonEnabled { set; }

    event Action<string> OnAnswerTextChanged;
    event Action<bool> OnIsCorrectChanged;
    event Action OnRemoveAnswerClicked;

    void DestroyView();
}