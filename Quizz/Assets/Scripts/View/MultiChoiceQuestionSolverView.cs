using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiChoiceQuestionSolverView : MonoBehaviour, IMultiChoiceQuestionSolverView
{
    public TMP_Text QuestionLabel;
    public Transform OptionsContainer;
    public GameObject TogglePrefab; 

    private List<Toggle> _instantiatedToggles = new List<Toggle>();

    public void Setup(string questionText, List<string> answers, List<bool> previousSelections)
    {
        QuestionLabel.text = questionText;
        _instantiatedToggles.Clear();

        for (int i = 0; i < answers.Count; i++)
        {
            var go = Instantiate(TogglePrefab, OptionsContainer);
            var toggle = go.GetComponent<Toggle>();
            go.GetComponentInChildren<TMP_Text>().text = answers[i];

            if (previousSelections != null && i < previousSelections.Count)
                toggle.isOn = previousSelections[i];

            _instantiatedToggles.Add(toggle);
        }
    }

    public List<bool> GetCurrentSelections()
    {
        List<bool> results = new List<bool>();
        foreach (var t in _instantiatedToggles) results.Add(t.isOn);
        return results;
    }
}