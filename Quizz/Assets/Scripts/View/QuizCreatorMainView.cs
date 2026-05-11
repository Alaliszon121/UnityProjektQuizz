using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizCreatorMainView : MonoBehaviour, IQuizCreatorMainView
{
    public TMP_InputField QuizNameInput;
    public Button AddMultiChoiceButton;
    public Button AddTrueFalseButton;
    public Button SaveQuizButton;
    public Button CreateNewQuizButton;
    public Button CloseWarningPopupButton;

    public GameObject StartScreenPanel;
    public GameObject WarningPopupPanel;
    public TMP_Text WarningPopupText;

    public Transform AvailableQuizzesContainer;
    public Transform QuestionsListContainer;

    public GameObject LoadQuizItemPrefab;
    public GameObject MultiChoiceEditorPrefab;
    public GameObject TrueFalseEditorPrefab;

    public event Action<string> OnQuizNameChanged;
    public event Action OnAddMultiChoiceClicked;
    public event Action OnAddTrueFalseClicked;
    public event Action OnSaveQuizClicked;
    public event Action OnCreateNewQuizClicked;
    public event Action OnCloseWarningClicked;

    public string QuizName
    {
        get => QuizNameInput.text;
        set => QuizNameInput.text = value;
    }

    public bool IsSaveButtonEnabled
    {
        set => SaveQuizButton.interactable = value;
    }

    private void Awake()
    {
        QuizNameInput.onValueChanged.AddListener(val => OnQuizNameChanged?.Invoke(val));
        AddMultiChoiceButton.onClick.AddListener(() => OnAddMultiChoiceClicked?.Invoke());
        AddTrueFalseButton.onClick.AddListener(() => OnAddTrueFalseClicked?.Invoke());
        SaveQuizButton.onClick.AddListener(() => OnSaveQuizClicked?.Invoke());
        CreateNewQuizButton.onClick.AddListener(() => OnCreateNewQuizClicked?.Invoke());
        CloseWarningPopupButton.onClick.AddListener(() => OnCloseWarningClicked?.Invoke());
    }

    public void ShowStartScreen(bool show)
    {
        StartScreenPanel.SetActive(show);
    }

    public void ShowWarning(string message)
    {
        WarningPopupText.text = message;
        WarningPopupPanel.SetActive(true);
    }

    public void HideWarning()
    {
        WarningPopupPanel.SetActive(false);
    }

    public void ClearAvailableQuizzes()
    {
        foreach (Transform child in AvailableQuizzesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearQuestionsList()
    {
        foreach (Transform child in QuestionsListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public ILoadQuizItemView CreateLoadQuizItem()
    {
        GameObject item = Instantiate(LoadQuizItemPrefab, AvailableQuizzesContainer);
        return item.GetComponent<LoadQuizItemView>();
    }

    public IMultipleChoiceEditorView CreateMultiChoiceEditor()
    {
        GameObject editor = Instantiate(MultiChoiceEditorPrefab, QuestionsListContainer);
        return editor.GetComponent<MultipleChoiceEditorView>();
    }

    public ITrueFalseEditorView CreateTrueFalseEditor()
    {
        GameObject editor = Instantiate(TrueFalseEditorPrefab, QuestionsListContainer);
        return editor.GetComponent<TrueFalseEditorView>();
    }
}