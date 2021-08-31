using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBtn : MonoBehaviour
{
    public bool isActive;
    public Sprite activeSpr;
    public Sprite lockedSpr;
    private Image btnImg;
    public Text LvlText;
    public Image[] starsArr;
    public int level;
    public GameObject confPanel;
    private Button myBtn;
    private GameData gameData;
    private int starsActive;
    // Start is called before the first frame update
    void Start()
    {
        btnImg = GetComponent<Image>();
        myBtn = GetComponent<Button>();
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActiveStars();
        ShowLevel();
        DecideSpr();
    }

    void ActiveStars()
    {
        for (int i = 0; i<starsActive; i++)
        {
                starsArr[i].enabled = true;
        }
    }
    void DecideSpr()
    {
        if (isActive)
        {
            btnImg.sprite = activeSpr;
            myBtn.enabled = true;
            LvlText.enabled = true;
        }
        else
        {
            btnImg.sprite = lockedSpr;
            myBtn.enabled = false;
            LvlText.enabled = false;
        }
    }

    void ShowLevel()
    {
        LvlText.text = "" + level;
    }
    public void activeConfPanel(int level)
    {
        confPanel.GetComponent<ConfirmPanel>().level = level;
        confPanel.SetActive(true);
    }

    void LoadData()
    {
        if(gameData != null)
        {
            if(gameData.saveData.isActiveArr[level - 1])
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }
            starsActive = gameData.saveData.starsArr[level - 1];
        }
    }
}
