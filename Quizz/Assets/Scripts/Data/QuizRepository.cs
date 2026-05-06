using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class QuizRepository
{
    private readonly string _saveDirectory;

    private readonly JsonSerializerSettings _jsonSettings;

    public QuizRepository()
    {
        _saveDirectory = Application.persistentDataPath;
        _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
    }

    public void SaveQuizToFile(Quiz quiz)
    {
        if (quiz == null || string.IsNullOrWhiteSpace(quiz.QuizName))
        {
            Debug.LogError("Repozytorium: Próba zapisu pustego quizu lub quizu bez nazwy.");
            return;
        }

        try
        {
            string safeFileName = GetSafeFilename(quiz.QuizName) + ".json";
            string fullPath = Path.Combine(_saveDirectory, safeFileName);

            string jsonContent = JsonConvert.SerializeObject(quiz, _jsonSettings);

            File.WriteAllText(fullPath, jsonContent);
            Debug.Log($"Repozytorium: Quiz zapisany pomyœlnie w: {fullPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Repozytorium: B³¹d podczas zapisu quizu: {ex.Message}");
        }
    }

    public List<Quiz> LoadAllQuizzes()
    {
        List<Quiz> loadedQuizzes = new List<Quiz>();

        try
        {
            string[] filePaths = Directory.GetFiles(_saveDirectory, "*.json");

            foreach (string path in filePaths)
            {
                string jsonContent = File.ReadAllText(path);

                Quiz loadedQuiz = JsonConvert.DeserializeObject<Quiz>(jsonContent, _jsonSettings);

                if (loadedQuiz != null)
                {
                    loadedQuizzes.Add(loadedQuiz);
                }
            }

            Debug.Log($"Repozytorium: Wczytano {loadedQuizzes.Count} quizów.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Repozytorium: B³¹d podczas wczytywania quizów: {ex.Message}");
        }

        return loadedQuizzes;
    }

    private string GetSafeFilename(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
}