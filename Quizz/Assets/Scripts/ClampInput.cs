using UnityEngine;
using TMPro;

public class ClampInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(ValidateValue);
        }
    }

    private void ValidateValue(string input)
    {
        if (float.TryParse(input, out float value))
        {
            if (value < 0 || value > 100)
            {
                UpdateField("1");
            }
        }
        else
        {
            UpdateField("1");
        }
    }

    private void UpdateField(string newValue)
    {
        inputField.text = newValue;
    }

    private void OnDestroy()
    {
        inputField.onEndEdit.RemoveListener(ValidateValue);
    }
}