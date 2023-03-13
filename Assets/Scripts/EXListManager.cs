using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine.UI;

public class EXListManager : MonoBehaviour
{
    private string URL = "http://localhost:3000";
    private HttpClient client;

    private ListItem currentQuestionData;
    private int currentMcSelection = -1;

    [SerializeField] private TMP_Dropdown sortSelect;

    [SerializeField] private Transform exListParent;
    [SerializeField] private GameObject exObject;
    [SerializeField] private Button submitButton;

    [Header("McButtons")]
    [SerializeField] private Button btnA;
    [SerializeField] private Button btnB;
    [SerializeField] private Button btnC;
    [SerializeField] private Button btnD;

    [Header("ExDisplay")]
    [SerializeField] private GameObject exDisplayParent;
    [SerializeField] private TMP_Text exName;
    [SerializeField] private TMP_Text exQ;
    [SerializeField] private TMP_Text btnTextA;
    [SerializeField] private TMP_Text btnTextB;
    [SerializeField] private TMP_Text btnTextC;
    [SerializeField] private TMP_Text btnTextD;
    [SerializeField] private TMP_InputField answerInput;

    [Space(5)]
    [SerializeField] private GameObject sortBar;
    [SerializeField] private Button exitExBtn;

    [Space(5)]
    [SerializeField] private Sprite whiteBtnImg;
    [SerializeField] private Sprite blueBtnImg;

    [Serializable]
    public class ListDataRoot
    {
        public ListData[] root;
    }
       
    [Serializable]
    public class ListData
    {
        public string EXERCISE_ID;
        public string QUESTION_NAME;
        public string DUEDATE;
    }

    [Serializable]
    public class ListItem
    {
        public string EXERCISE_ID;
        public string QUESTION_NAME;
        public string QUESTION;
        public string QUESTION_TYPE;
        public string ANSWER;
        public string ANSWER_A;
        public string ANSWER_B;
        public string ANSWER_C;
        public string ANSWER_D;
        public string DUEDATE; 
    }

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        submitButton.onClick.AddListener(SubmitAnswer);

        sortSelect.onValueChanged.AddListener( delegate{ ChangeSort(sortSelect);});

        btnA.onClick.AddListener(delegate { McOnClick(0); });
        btnB.onClick.AddListener(delegate { McOnClick(1); });
        btnC.onClick.AddListener(delegate { McOnClick(2); });
        btnD.onClick.AddListener(delegate { McOnClick(3); });

        exitExBtn.onClick.AddListener(delegate { ExitOnClick(); });

        if (Userdata.instance.ROLE_TYPE == 1)
        {
            GetExList();
        }
    }

    async void GetExList()
    {
        var payload = "{\"userID\": " + Userdata.instance.UID + ", \"teacherID\": " + Userdata.instance.TEACHER_UID +"}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/getexlist", c);
        var content = await res.Content.ReadAsStringAsync();
        var data = JsonUtility.FromJson<ListDataRoot>("{\"root\":" + content + "}");
        DisplayEx(data);
    }

    void DisplayEx(ListDataRoot data)
    {
        for(int i = 0; i< data.root.Length; i++) 
        {
            GameObject exObj = Instantiate(exObject, Vector3.zero, Quaternion.identity, exListParent);
            TMP_Text[] exArray = exObj.GetComponentsInChildren<TMP_Text>();
            exArray[0].text = data.root[i].QUESTION_NAME;
            exArray[1].text = data.root[i].DUEDATE.Substring(8, 2) + "/" + data.root[i].DUEDATE.Substring(5, 2);
            Button btn = exObj.GetComponent<Button>();
            string eID = data.root[i].EXERCISE_ID;
            btn.onClick.AddListener(delegate { ExItemOnClick(eID); });
        }
    }

    async void ExItemOnClick(string eID) 
    {
        //reset mc buttons
        btnA.GetComponent<Image>().sprite = blueBtnImg;
        btnB.GetComponent<Image>().sprite = blueBtnImg;
        btnC.GetComponent<Image>().sprite = blueBtnImg;
        btnD.GetComponent<Image>().sprite = blueBtnImg;
        currentMcSelection = -1;


        var payload = "{\"eID\": " + eID + "}";
        HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
        var res = await client.PostAsync("exercise/getexdetails", c);
        var content = await res.Content.ReadAsStringAsync();
        content = content.Substring(1, content.Length - 2);
        currentQuestionData = JsonUtility.FromJson<ListItem>(content);

        exName.text = currentQuestionData.QUESTION_NAME;
        exQ.text = "Calculate:\n" + currentQuestionData.QUESTION;

        sortBar.SetActive(false);
        exListParent.gameObject.SetActive(false);

        switch (currentQuestionData.QUESTION_TYPE)
        {
            case "0":
                answerInput.gameObject.SetActive(false);

                List<string> suffleList = new List<string> { currentQuestionData.ANSWER_A, currentQuestionData.ANSWER_B, currentQuestionData.ANSWER_C, currentQuestionData.ANSWER_D };

                for (int i = 0; i < suffleList.Count; i++)
                {
                    string temp = suffleList[i];
                    int randomIndex = UnityEngine.Random.Range(i, suffleList.Count);
                    suffleList[i] = suffleList[randomIndex];
                    suffleList[randomIndex] = temp;
                }

                btnTextA.text = suffleList[0];
                btnTextB.text = suffleList[1];
                btnTextC.text = suffleList[2];
                btnTextD.text = suffleList[3];

                btnA.gameObject.SetActive(true);
                btnB.gameObject.SetActive(true);
                btnC.gameObject.SetActive(true);
                btnD.gameObject.SetActive(true);
                break;
            case "1":
                answerInput.gameObject.SetActive(true);
                btnA.gameObject.SetActive(false);
                btnB.gameObject.SetActive(false);
                btnC.gameObject.SetActive(false);
                btnD.gameObject.SetActive(false);
                break;
            default:
                Debug.Log("Value error");
                break;
        }

        exDisplayParent.SetActive(true);
    }

    void ChangeSort(TMP_Dropdown change)
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


    void McOnClick(int idx) 
    {
        btnA.GetComponent<Image>().sprite = blueBtnImg;
        btnB.GetComponent<Image>().sprite = blueBtnImg;
        btnC.GetComponent<Image>().sprite = blueBtnImg;
        btnD.GetComponent<Image>().sprite = blueBtnImg;

        if (currentMcSelection == idx)
        {
            currentMcSelection = -1;
        }
        else
        {
            currentMcSelection = idx;
            switch (idx) 
            {
                case 0:
                    btnA.GetComponent<Image>().sprite = whiteBtnImg;
                    break;
                case 1:
                    btnB.GetComponent<Image>().sprite = whiteBtnImg;
                    break;
                case 2:
                    btnC.GetComponent<Image>().sprite = whiteBtnImg;
                    break;
                case 3:
                    btnD.GetComponent<Image>().sprite = whiteBtnImg;
                    break;
            }
        }
    }

    void ExitOnClick()
    {
        sortBar.SetActive(true);
        exListParent.gameObject.SetActive(true);
        exDisplayParent.SetActive(false);
    }

    //Temp
    private TMP_Text displayText;

    async void SubmitAnswer()
    {
        switch (currentQuestionData.QUESTION_TYPE)
        {
            case "0":
                break;
            case "1":
                string answerStr = answerInput.text.Trim();

                List<string> str = new List<string> { "eID", currentQuestionData.EXERCISE_ID, "uID", Userdata.instance.UID, "answer", answerStr };
                var payload = ExtensionFunction.StringEncoder(str);
                HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
                await client.PostAsync("exercise/submitex", c);
                //Better display to be implemented by UI
                if (string.Compare(answerStr, currentQuestionData.ANSWER) == 0)
                {
                    displayText.text = "Correct";
                }
                else
                {
                    displayText.text = "Incorrect";
                }
                break;
            default:
                Debug.Log("Value error");
                break;
        }
    }

}
