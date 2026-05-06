using System;

[Serializable]
public class Answer
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }

    public Answer(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}