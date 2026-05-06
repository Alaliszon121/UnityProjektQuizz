using System;
using System.Collections.Generic;

[Serializable]
public class Quiz
{
    public string QuizName { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();

    public Quiz(string name)
    {
        QuizName = name;
    }
}