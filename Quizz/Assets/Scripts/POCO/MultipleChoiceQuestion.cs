using System;
using System.Collections.Generic;

[Serializable]
public class MultipleChoiceQuestion : Question
{
    public override float CalculateScore(List<bool> userSelections)
    {
        if (userSelections == null || userSelections.Count != Answers.Count)
            return 0f;

        int errors = 0;

        for (int i = 0; i < Answers.Count; i++)
        {
            if (userSelections[i] != Answers[i].IsCorrect)
            {
                errors++;
            }
        }

        if (errors == 0) return 1.0f * Multiplier;
        if (errors == 1) return 0.5f * Multiplier;

        return 0.0f;
    }
}