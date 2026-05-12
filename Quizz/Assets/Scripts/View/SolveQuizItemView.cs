using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SolverQuizItemView : MonoBehaviour, ISolverQuizItemView
{
    public TMP_Text QuizNameText;
    public Button SelectButton;

    public event Action OnQuizClicked;

    public string QuizName
    {
        set => QuizNameText.text = value;
    }

    private void Awake()
    {
        SelectButton.onClick.AddListener(() => OnQuizClicked?.Invoke());
    }
}