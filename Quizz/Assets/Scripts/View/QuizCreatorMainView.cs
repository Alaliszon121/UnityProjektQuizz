using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizCreatorMainView : MonoBehaviour
{
    // VIEW
    public TMP_InputField QuizNameInput;
    public Transform QuestionsListContainer;
    public Button AddMultiChoiceButton;
    public Button AddTrueFalseButton;
    public Button SaveQuizButton;

    [Header("Popup Walidacji")]
    public GameObject WarningPopupPanel;
    public TMP_Text WarningPopupText;
    public Button CloseWarningPopupButton;

    [Header("Ekran Startowy (Wczytywanie)")]
    public GameObject StartScreenPanel;
    public Transform AvailableQuizzesContainer;
    public Button CreateNewQuizButton;
}