using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerEditorView : MonoBehaviour, IAnswerEditorView
{
    public Toggle IsCorrectToggle;
    public TMP_InputField AnswerTextInput;
    public Button RemoveAnswerButton;

    public event Action<string> OnAnswerTextChanged;
    public event Action<bool> OnIsCorrectChanged;
    public event Action OnRemoveAnswerClicked;

    public string AnswerText
    {
        get => AnswerTextInput.text;
        set => AnswerTextInput.text = value;
    }

    public bool IsCorrect
    {
        get => IsCorrectToggle.isOn;
        set => IsCorrectToggle.isOn = value;
    }

    public bool IsRemoveButtonEnabled
    {
        set => RemoveAnswerButton.interactable = value;
    }

    private void Awake()
    {
        AnswerTextInput.onValueChanged.AddListener(val => OnAnswerTextChanged?.Invoke(val));
        IsCorrectToggle.onValueChanged.AddListener(val => OnIsCorrectChanged?.Invoke(val));
        RemoveAnswerButton.onClick.AddListener(() => OnRemoveAnswerClicked?.Invoke());
    }

    public void DestroyView()
    {
        Destroy(gameObject);
    }
}