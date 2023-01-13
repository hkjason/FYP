using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class EXAssignManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    [SerializeField]
    private Button assignButton;
    [SerializeField]
    private TMP_Dropdown questionType;
    [SerializeField]
    private TMP_InputField questionName;
    [SerializeField]
    private TMP_InputField questionInput;
    [SerializeField]
    private TMP_InputField answerInput;
    [SerializeField]
    private TMP_InputField aInput;
    [SerializeField]
    private TMP_InputField bInput;
    [SerializeField]
    private TMP_InputField cInput;
    [SerializeField]
    private TMP_InputField dInput;
    [SerializeField]
    private TMP_InputField dueDate;

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Button btn;
        btn = assignButton.GetComponent<Button>();
        btn.onClick.AddListener(OnAssign);

        questionType.onValueChanged.AddListener(delegate {
            ChangeType(questionType);
        });
    }

    async void OnAssign()
    {
        List<string> str = new List<string>();
        switch (questionType.value)
        {
            case 0:
                str = new List<string> { "userID", LoginManager.UID, "questionName", questionName.text,"question", questionInput.text, "questionType", "0", "answerA", aInput.text, "answerB", bInput.text, "answerC", cInput.text, "answerD", dInput.text, "duedate", dueDate.text};
                break;
            case 1:
                str = new List<string> { "userID", LoginManager.UID, "questionName", questionName.text,"question", questionInput.text, "questionType", "1", "answer", answerInput.text, "duedate", dueDate.text};
                break;
            default:
                Debug.Log("Value error");
                break;
        }

        var payload = StringEncoder(str);
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/assignex", c);
        var content = await res.Content.ReadAsStringAsync();

        if (string.Compare(content, "assign successful") == 0)
        {
            switch (questionType.value)
            {
                case 0:
                    ResetMCValue();
                    break;
                case 1:
                    ResetSQValue();
                    break;
                default:
                    Debug.Log("Value error");
                    break;
            }
        }
        else if (string.Compare(content, "question exists") == 0)
        {

        }
        else
        {
            Debug.Log("ASSIGN ERROR");
            return;
        }
    }

    void ChangeType(TMP_Dropdown change) 
    {
        switch (change.value)
        {
            case 0:

                break;
            case 1:
                break;
            default:
                Debug.Log("Value error");
                break;
        }
    }

    void ResetMCValue() {
        questionInput.text = "";
        aInput.text = "";
        bInput.text = "";
        cInput.text = "";
        dInput.text = "";
    }

    void ResetSQValue() {
        questionInput.text = "";
        answerInput.text = "";
    }

    string StringEncoder(List<string> list)
    {
        string str = "";
        str += "{";
        for (int i = 0; i < list.Count - 1;)
        {
            str += "\"" + list[i++] + "\": ";
            str += "\"" + list[i++] + "\"";
            if (i < list.Count - 1)
                str += ", ";
        }
        str += "}";
        return str;
    }
}
