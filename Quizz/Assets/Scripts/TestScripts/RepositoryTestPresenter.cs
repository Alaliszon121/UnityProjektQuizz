using UnityEngine;

public class RepositoryTestPresenter : MonoBehaviour
{
    private QuizRepository _repository;

    private void Start()
    {
        _repository = new QuizRepository();

        RunTest();
    }

    private void RunTest()
    {
        Debug.Log("--- ROZPOCZÊCIE TESTU REPOZYTORIUM ---");

        Quiz testQuiz = new Quiz("Testowy Quiz Inzynierski");

        MultipleChoiceQuestion multiQ = new MultipleChoiceQuestion();
        multiQ.QuestionText = "Które z tych zwierz¹t to ssaki?";
        multiQ.Answers.Add(new Answer("Pies", true));
        multiQ.Answers.Add(new Answer("Krokodyl", false));
        multiQ.Answers.Add(new Answer("Delfin", true));

        TrueFalseQuestion tfQ = new TrueFalseQuestion("Ziemia jest p³aska.", false);

        testQuiz.Questions.Add(multiQ);
        testQuiz.Questions.Add(tfQ);

        _repository.SaveQuizToFile(testQuiz);

        var allQuizzes = _repository.LoadAllQuizzes();

        foreach (var q in allQuizzes)
        {
            Debug.Log($"Wczytano Quiz: {q.QuizName}, Liczba pytañ: {q.Questions.Count}");
            foreach (var question in q.Questions)
            {
                Debug.Log($"- Pytanie: {question.QuestionText} | Typ obiektu: {question.GetType().Name}");
            }
        }

        Debug.Log("--- ZAKOÑCZENIE TESTU ---");
    }
}