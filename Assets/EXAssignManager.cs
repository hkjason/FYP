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

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Button btn;
        btn = assignButton.GetComponent<Button>();
        btn.onClick.AddListener(OnAssign);
    }

    async void OnAssign()
    {
        List<string> str = new List<string> { "Question", questionInput.text, "Answer", answerInput.text };
        //var payload = StringEncoder(str);
        var payload = "placeholdertest";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/assignex", c);
        var content = await res.Content.ReadAsStringAsync();

        if (string.Compare(content, "assign successful") == 0)
        {

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
}
