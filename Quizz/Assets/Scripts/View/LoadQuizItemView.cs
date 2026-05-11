using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadQuizItemView : MonoBehaviour, ILoadQuizItemView
{
    public TMP_Text QuizNameText;
    public Button LoadButton;

    public event Action OnLoadClicked;

    public string QuizName
    {
        set => QuizNameText.text = value;
    }

    private void Awake()
    {
        LoadButton.onClick.AddListener(() => OnLoadClicked?.Invoke());
    }
}