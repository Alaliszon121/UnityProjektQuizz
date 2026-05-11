using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrueFalseQuestionSolverView : MonoBehaviour, ITrueFalseQuestionSolverView
{
    public TMP_Text QuestionLabel;
    public Toggle TrueToggle;
    public Toggle FalseToggle;

    public void Setup(string questionText, List<bool> previousSelections)
    {
        QuestionLabel.text = questionText;

        // Zresetuj lub ustaw poprzednie
        if (previousSelections != null && previousSelections.Count >= 2)
        {
            TrueToggle.isOn = previousSelections[0];
            FalseToggle.isOn = previousSelections[1];
        }
    }

    public List<bool> GetCurrentSelections()
    {
        return new List<bool> { TrueToggle.isOn, FalseToggle.isOn };
    }
}