using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultipleChoiceEditorView : MonoBehaviour, IMultipleChoiceEditorView
{
    public TMP_InputField QuestionTextInput;
    public TMP_InputField MultiplierInput;
    public Button RemoveQuestionButton;
    public Button AddAnswerButton;
    public Transform AnswersContainer;
    public GameObject AnswerEditorPrefab;

    public event Action<string> OnQuestionTextChanged;
    public event Action<string> OnMultiplierChanged;
    public event Action OnRemoveQuestionClicked;
    public event Action OnAddAnswerClicked;

    public string QuestionText
    {
        get => QuestionTextInput.text;
        set => QuestionTextInput.text = value;
    }

    public string MultiplierText
    {
        get => MultiplierInput.text;
        set => MultiplierInput.text = value;
    }

    public bool IsAddAnswerButtonEnabled
    {
        set => AddAnswerButton.interactable = value;
    }

    private void Awake()
    {
        QuestionTextInput.onValueChanged.AddListener(val => OnQuestionTextChanged?.Invoke(val));
        MultiplierInput.onValueChanged.AddListener(val => OnMultiplierChanged?.Invoke(val));
        RemoveQuestionButton.onClick.AddListener(() => OnRemoveQuestionClicked?.Invoke());
        AddAnswerButton.onClick.AddListener(() => OnAddAnswerClicked?.Invoke());
    }

    public void ClearAnswers()
    {
        foreach (Transform child in AnswersContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public IAnswerEditorView CreateAnswerEditor()
    {
        GameObject answerObj = Instantiate(AnswerEditorPrefab, AnswersContainer);
        return answerObj.GetComponent<AnswerEditorView>();
    }

    public void DestroyView()
    {
        Destroy(gameObject);
    }
}