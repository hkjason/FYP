using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Localization : MonoBehaviour
{
    public static Localization instance { get; private set; }

    private int languageNum = 0;
    private LocalizationS0 textHolder0;
    private LocalizationS1 textHolder1;

    public TMP_FontAsset engFont;
    public TMP_FontAsset chiFont;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += FinishedSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= FinishedSceneLoad;
    }

    public void ChangeLocale()
    {
        if (languageNum == 0)
        {
            languageNum = 1;
        }
        else if (languageNum == 1)
        {
            languageNum = 0;
        }

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                textHolder0 = FindObjectOfType<LocalizationS0>();
                textHolder0.LoadText(languageNum);
                break;
            case 1:
                textHolder1 = FindObjectOfType<LocalizationS1>();
                textHolder1.LoadText(languageNum);
                break;
            default:
                Debug.LogError("Invalid build index");
                break;
        }
    }

    void FinishedSceneLoad(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0:
                textHolder0 = FindObjectOfType<LocalizationS0>();
                textHolder0.LoadText(languageNum);
                break;
            case 1:
                textHolder1 = FindObjectOfType<LocalizationS1>();
                textHolder1.LoadText(languageNum);
                break;
            default:
                Debug.LogError("Invalid build index");
                break; 
        }
    }

    public int GetLangNum()
    {
        return this.languageNum;
    }
}
