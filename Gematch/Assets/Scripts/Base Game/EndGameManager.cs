using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    move,
    time
}

[System.Serializable]
public class EndGameReq
{
    public GameType gameType;
    public int counter;
}

public class EndGameManager : MonoBehaviour
{
    public EndGameReq req;
    public GameObject movesLabel;
    public GameObject TimeLabel;
    public Text counter;
    public int currentCounterVal;
    private float timerSeconds;
    private BoardManager board;
    public GameObject winPanel;
    public GameObject losePanel;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        SetGameType();
        SetUpGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(req.gameType == GameType.time)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterVal();
                timerSeconds = 1;
            }
        }
    }

    void SetUpGame()
    {
        currentCounterVal = req.counter;
        if (req.gameType == GameType.move)
        {
            movesLabel.SetActive(true);
            TimeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            TimeLabel.SetActive(true);
        }
        counter.text = "" + currentCounterVal;
    }

    public void DecreaseCounterVal()
    {
        if (board.curentState != GameState.pause)
        {
            if (currentCounterVal >= 2)
            {
                currentCounterVal--;
                counter.text = "" + currentCounterVal;
            }
            else
            {
                LoseGame();
            }
        }
        
    }

    public void WinGame()
    {
        winPanel.SetActive(true);
        board.curentState = GameState.win;
        currentCounterVal = 0;
        counter.text = "" + currentCounterVal;
    }

    public void LoseGame()
    {
        losePanel.SetActive(true);
        board.curentState = GameState.lose;
        currentCounterVal = 0;
        counter.text = "" + currentCounterVal;
    }

    void SetGameType()
    {
        if(board.world!= null)
        {
            if(board.world.levelsArr[board.Level]!= null)
            {
                req = board.world.levelsArr[board.Level].req;
            }
        }
    }
}
