using System;

public interface ISolveQuizItemView
{
    string QuizName { set; }
    event Action OnQuizSelected;
}