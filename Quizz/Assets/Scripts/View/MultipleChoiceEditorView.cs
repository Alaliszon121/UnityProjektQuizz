using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultipleChoiceEditorView : MonoBehaviour
{
    public TMP_InputField QuestionTextInput;
    public TMP_InputField MultiplierInput;
    public Button RemoveQuestionButton;

    [Header("Odpowiedzi")]
    public Transform AnswersContainer;
    public Button AddAnswerButton;
}