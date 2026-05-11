using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrueFalseEditorView : MonoBehaviour, ITrueFalseEditorView
{
    public TMP_InputField QuestionTextInput;
    public TMP_InputField MultiplierInput;
    public Toggle IsTrueToggle;
    public Button RemoveQuestionButton;

    public event Action<string> OnQuestionTextChanged;
    public event Action<string> OnMultiplierChanged;
    public event Action<bool> OnIsTrueChanged;
    public event Action OnRemoveQuestionClicked;

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

    public bool IsTrue
    {
        get => IsTrueToggle.isOn;
        set => IsTrueToggle.isOn = value;
    }

    private void Awake()
    {
        QuestionTextInput.onValueChanged.AddListener(val => OnQuestionTextChanged?.Invoke(val));
        MultiplierInput.onValueChanged.AddListener(val => OnMultiplierChanged?.Invoke(val));
        IsTrueToggle.onValueChanged.AddListener(val => OnIsTrueChanged?.Invoke(val));
        RemoveQuestionButton.onClick.AddListener(() => OnRemoveQuestionClicked?.Invoke());
    }

    public void DestroyView()
    {
        Destroy(gameObject);
    }
}