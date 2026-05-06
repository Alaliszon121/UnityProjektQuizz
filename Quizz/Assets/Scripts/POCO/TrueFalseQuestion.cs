using System;
using System.Collections.Generic;

[Serializable]
public class TrueFalseQuestion : Question
{
    public TrueFalseQuestion(string questionText, bool isTrueCorrect, float multiplier = 1.0f)
    {
        QuestionText = questionText;
        Multiplier = multiplier;

        Answers = new List<Answer>
        {
            new Answer("Prawda", isTrueCorrect),
            new Answer("Fa³sz", !isTrueCorrect)
        };
    }

    public override float CalculateScore(List<bool> userSelections)
    {
        if (userSelections == null || userSelections.Count != Answers.Count)
            return 0f;

        bool isCorrect = (userSelections[0] == Answers[0].IsCorrect) &&
                         (userSelections[1] == Answers[1].IsCorrect);

        return isCorrect ? 1.0f * Multiplier : 0.0f;
    }
}