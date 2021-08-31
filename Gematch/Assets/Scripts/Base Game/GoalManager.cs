using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numOfGoals;
    public int numOfCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] lvlGoals;
    public List<GoalPannel> currentGoals = new List<GoalPannel>(); 
    public GameObject goalPrefab;
    public GameObject goalInfoParent;
    public GameObject goalGameParent;
    private BoardManager board;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        GetGoals();
        SetInfoGoals();
    }

    void GetGoals()
    {
        lvlGoals = board.world.levelsArr[board.Level].levelGoals;
    }
    void SetInfoGoals()
    {
        for (int i = 0; i < lvlGoals.Length; i++)
        {
            //make a new goal panel at parent position
            GameObject goal = Instantiate(goalPrefab, goalInfoParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalInfoParent.transform);

            //set img of the goal
            GoalPannel panel = goal.GetComponent<GoalPannel>();
            panel.thisSpr = lvlGoals[i].goalSprite;
            panel.thisStr = "0/" + lvlGoals[i].numOfGoals;
            //create panel
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPannel>();
            currentGoals.Add(panel);
            panel.thisSpr = lvlGoals[i].goalSprite;
            panel.thisStr = "0/" + lvlGoals[i].numOfGoals;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleated = 0;
        for (int i= 0; i<lvlGoals.Length; i++)
        {
            //update text
            currentGoals[i].thisText.text = "" + lvlGoals[i].numOfCollected + "/" + lvlGoals[i].numOfGoals;
            if (lvlGoals[i].numOfCollected >= lvlGoals[i].numOfGoals)
            {
                goalsCompleated++;
                currentGoals[i].thisText.text = "" + lvlGoals[i].numOfGoals + "/" + lvlGoals[i].numOfGoals;
            }
        }
        if (goalsCompleated >= lvlGoals.Length)
        {
            board.endGameManager.WinGame();
            Debug.Log("We won!");
        }
    }

    public void CompareGoals(string goalsToCompare)
    {
        for (int i= 0; i<lvlGoals.Length; i++)
        {
            if(goalsToCompare == lvlGoals[i].matchValue)
            {
                lvlGoals[i].numOfCollected++;
            }
        }
    }

}
