using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    public Image[] starsArr;
    public string levelToLoad;
    public int level;
    private GameData gameData;
    private int starsActive;
    public Text scoreText;
    public Text starText;
    private int highScore;
    
    void OnEnable()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActiveStars();
        SetText();
    }
    void LoadData()
    {
        if (gameData != null)
        {
            starsActive = gameData.saveData.starsArr[level - 1];
            highScore = gameData.saveData.scoresArr[level - 1];
        }
    }

    void SetText()
    {
        scoreText.text = "" + highScore;
        starText.text = "" + starsActive + "/3";
    }
    void ActiveStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            starsArr[i].enabled = true;
        }
    }

    public void closeConfPanel()
    {
        this.gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(levelToLoad);
    }
}
