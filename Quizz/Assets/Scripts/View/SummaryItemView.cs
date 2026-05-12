using System;
using UnityEngine;
using TMPro;

public interface ISummaryItemView
{
    void Setup(string questionText, string scoreInfo, string statusDetails);
    event Action OnItemAnimationFinished;
}

public class SummaryItemView : MonoBehaviour, ISummaryItemView
{
    public TMP_Text QuestionText;
    public TMP_Text ScoreInfoText;
    public TMP_Text DetailsText;

    public event Action OnItemAnimationFinished;

    public void Setup(string questionText, string scoreInfo, string statusDetails)
    {
        QuestionText.text = questionText;
        ScoreInfoText.text = scoreInfo;
        DetailsText.text = statusDetails;
    }

    public void TriggerAnimationFinished()
    {
        OnItemAnimationFinished?.Invoke();
    }
}