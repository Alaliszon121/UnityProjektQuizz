using System;
using System.Collections.Generic;

[Serializable]
public abstract class Question
{
    public string QuestionText { get; set; }
    public float Multiplier { get; set; } = 1.0f;
    public List<Answer> Answers { get; set; } = new List<Answer>();

    public abstract float CalculateScore(List<bool> userSelections);
}