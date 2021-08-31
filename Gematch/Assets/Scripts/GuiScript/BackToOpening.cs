using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToOpening : MonoBehaviour
{
    public string scenceToLoad;
    private GameData gameData;
    private BoardManager board;
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<BoardManager>();
    }
    public void WinOk()
    {
        if (gameData != null) {
            gameData.saveData.isActiveArr[board.Level + 1] = true; //unlock next level
            gameData.Save();
        }
        SceneManager.LoadScene(scenceToLoad);
    }

    public void LoseOk()
    {
        SceneManager.LoadScene(scenceToLoad);
    }
}
