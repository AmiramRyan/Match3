using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public int score;
    public Image scoreBar;
    private BoardManager board;
    private GameData gameData;
    private int numberStars;

    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        gameData = FindObjectOfType<GameData>();
        scoreBar.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = ""+score;
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        for(int i = 0; i<board.arrScoreGoals.Length; i++)
        {
            if(score > board.arrScoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }
        if(gameData != null) //saves
        {
            //update star count
            int currentStars = gameData.saveData.starsArr[board.Level];
            if (numberStars > currentStars)
            {
                gameData.saveData.starsArr[board.Level] = numberStars;
            }
            //update highscore
            if (score > gameData.saveData.scoresArr[board.Level])
            {
                gameData.saveData.scoresArr[board.Level] = score;
            }
            gameData.Save();
        }
        if(board !=null && scoreBar != null)
        {
            int length = board.arrScoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.arrScoreGoals[length - 1];
        }
    }
}
