using System;
using System.Collections;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.UI;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Net.Http.Headers;

public class ExcelInput : MonoBehaviour
{
    private string URL = "https://mongoserver-1-y3258239.deta.app/";
    private HttpClient client;

    public TextAsset textAssetData;

    public string[] connectionData;

    [SerializeField]
    private Button selectFileBtn;
    [SerializeField]
    private Button addStudentsBtn;

    private string filePath;

    [SerializeField]
    private UIManager uIManager;

    void Start()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(URL);
        client.DefaultRequestHeaders.Add("x-api-key", "c0pPE1CyrvbW_keBnGfJuxJfKr2HAPB3T3U6zCF2JcR4e");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        FileBrowser.SetDefaultFilter(".csv");

        selectFileBtn.onClick.AddListener(SelectFile);
        addStudentsBtn.onClick.AddListener(ReadCsvAddCourse);
    }



    async void ReadCsvAddCourse()
    {
        var textFile = Resources.Load<TextAsset>(filePath);
        string[] lines = textFile.text.Split(new string[] { "\n" }, StringSplitOptions.None);

        int lineNumber = lines.Length - 1;

        connectionData = new string[lineNumber - 1];

        for (int i = 0; i < lineNumber - 1; i++)
        {
            string[] data = lines[i + 1].Split(new string[] { "," }, StringSplitOptions.None);
            connectionData[i] = data[1];
        }

        string dataList = "";
        dataList = "{\"teacherId\":" + UserManager.instance.UID + ",\"courseId\":" + UserManager.instance.COURSEID + ",\"studentList\":" + ToJsonArray(connectionData) + "}";
        Debug.Log("dataList: " + dataList);
        HttpResponseMessage res; 
        HttpContent c = new StringContent(dataList, Encoding.UTF8, "application/json");
        try
        {
            uIManager.Loading();
            res = await client.PostAsync("course/batchRegister", c);
        }
        catch (HttpRequestException e)
        {
            uIManager.NotiSetText("Connection failure, please check network connection or server", "連接失敗，請檢查網絡連接或伺服器");
            return;
        }
        finally
        {
            uIManager.DoneLoading();
        }
        var content = await res.Content.ReadAsStringAsync();
        Debug.Log("excelinput 77: " + content);
        if (res.StatusCode.Equals(HttpStatusCode.InternalServerError))
        {
            uIManager.NotiSetText("Server Error, please try again later", "服務器錯誤，請稍後再試");
            return;
        }
        else if (res.StatusCode.Equals(HttpStatusCode.BadRequest))
        {
            if (string.Compare(content, "Error found in excel file.") == 0)
            {
                uIManager.NotiSetText("Error found in excel file", "在 excel 文件中發現錯誤");
                return;
            }
        }
        else if (res.StatusCode.Equals(HttpStatusCode.OK))
        {
            uIManager.NotiSetText("Students added successfully", "學生添加成功");
            return;
        }
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                Debug.Log(FileBrowser.Result[i]);

            //string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
            string fileName = FileBrowserHelpers.GetFilename(FileBrowser.Result[0]);
            string destinationPath = Path.Combine("Assets/Resources/", fileName);
            FileBrowserHelpers.CopyFile(FileBrowser.Result[0], destinationPath);

            filePath = Path.GetFileNameWithoutExtension(fileName);
        }
    }

    void SelectFile()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    string ToJsonArray(string[] strArray)
    {
        string jsonArray = "[" + strArray[0];

        for (int i = 1; i < strArray.Length - 1; i++)
        {
            jsonArray = jsonArray + ", " + strArray[i];
        }

        jsonArray = jsonArray + ", " + strArray[strArray.Length - 1] + "]";

        return jsonArray;
    }
}
