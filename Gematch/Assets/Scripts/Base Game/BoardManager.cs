using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileTypeEnum
{
    breakable,
    blank,
    normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileTypeEnum tiletype;
}

public class BoardManager : MonoBehaviour
{
    [Header("General vars")]
    public int width;
    public int height;
    public int SpawnOffset;

    [Header("Offset timers")]
    public float MakingNewGemsWaitTime;
    public float refillDelay;
    public float ExplosionEffectLifeTime;
    public float GemSwitchSpeed;
    public float timeToWaitCheckForMatch;
    public float timeToWaitBeforeMoveIfNoMatch;

    [Header("Objects")]
    public GameObject tilePrefab;
    public GameObject[] gemsArr;
    public GameObject [] EffectsArr;
    public GameObject[,] allGems;
    public Gem currentGem;
    public TileType[] boardLayout;
    public EndGameManager endGameManager;
    public World world;
    public int Level;

    [Header("Score")]
    public int baseValue = 10;
    public int comboValue = 1;

    [Header("LevelGoals")]
    public float initProgg = 0.24f;
    public int[] arrScoreGoals;

    [Header("Game state")]
    public GameState curentState = GameState.move;

    #region Private 

    private bool[,] blankSpaces;
    private FindMatches findMatches;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    
    #endregion

    #region Coroutines
    private IEnumerator CollapseRowCo2()
    {
        for(int i= 0; i<width; i++)
        {
            for(int j = 0; j<height; j++)
            {
                //if currspot is not blank and is empty
                if (!blankSpaces[i, j] && allGems[i, j] == null)
                {
                    for(int k = j+1; k<height; k++)
                    {
                        if(allGems[i,k] != null)
                        {
                            allGems[i, k].GetComponent<Gem>().row = j;
                            allGems[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(MakingNewGemsWaitTime);
        StartCoroutine(FillBoardCo());
    }
    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(refillDelay);
        RefillSpots();

        while (CheckForMatches())
        {
            comboValue += 1; //bouns score for more matches
            DestroyAllMatches();
            yield return new WaitForSeconds(2 * refillDelay);
        }
        findMatches.currentMatches.Clear();
        currentGem = null;
        if (IsDeadlock())
        {
            Debug.Log("DeadLoCK!");
            ShuffleBoard();
        }
        yield return new WaitForSeconds(refillDelay);
        if (curentState == GameState.wait)
        {
            curentState = GameState.move;
        }
        comboValue = 1; //reset the combo modifier
    }

    
    
    #endregion

    #region Initialzie func
    void Start()
    {
        blankSpaces = new bool[width, height];
        allGems = new GameObject[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        scoreManager = FindObjectOfType<ScoreManager>();
        soundManager = FindObjectOfType<SoundManager>();
        goalManager = FindObjectOfType<GoalManager>();
        endGameManager = FindObjectOfType<EndGameManager>();
        SetBoard();
        curentState = GameState.pause;
    }

    private void Awake()
    {
        if(PlayerPrefs.HasKey("Current Level"))
        {
            Level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if(world.levelsArr[Level] != null)
            {
                width = world.levelsArr[Level].width;
                height = world.levelsArr[Level].height;
                gemsArr = world.levelsArr[Level].gemsArr;
                arrScoreGoals = world.levelsArr[Level].ScoreGoals;
                boardLayout = world.levelsArr[Level].boardLayout;
            }
        }
    }
    public void GenrateBlackSpaces()
    {
        for (int i = 0; i<boardLayout.Length; i++)
        {
            if(boardLayout[i].tiletype == TileTypeEnum.blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }
    void SetBoard()
    {
        GenrateBlackSpaces();
        for (int i=0; i<width; i++)
        {
            for (int j=0; j< height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    //Create a grid
                    Vector2 tempPos = new Vector2(i, j + SpawnOffset);
                    Vector2 tilePos = new Vector2(i, j);
                    GameObject backGroundTile = Instantiate(tilePrefab, tilePos, Quaternion.identity) as GameObject;
                    backGroundTile.transform.parent = this.transform;
                    backGroundTile.name = "(" + i + "," + j + ")";
                    backGroundTile.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    //Create a random diamond
                    int rndGem = Random.Range(0, gemsArr.Length);
                    while (MatchCheck(i, j, gemsArr[rndGem])) //making sure no matches created at the start
                    {
                        rndGem = Random.Range(0, gemsArr.Length);
                    }
                    GameObject gem = Instantiate(gemsArr[rndGem], tempPos, Quaternion.identity) as GameObject;
                    gem.GetComponent<Gem>().row = j;
                    gem.GetComponent<Gem>().coll = i;
                    gem.transform.parent = this.transform;
                    gem.name = "(" + i + "," + j + ")";
                    allGems[i, j] = gem;
                }
            }
        }
        if (IsDeadlock())
        {
            ShuffleBoard();
        }
    }
    bool MatchCheck(int i, int j, GameObject gem)
    {
        //check left
        if (i > 1 && i < width)
        {
            if (allGems[i - 1, j] != null && allGems[i - 2, j] != null)
            {
                if (allGems[i - 2, j].tag == gem.tag && allGems[i - 1, j].tag == gem.tag)
                {
                    //Debug.Log("Gem: " + "(" + i + "," + j + ")" + " was: " + gem.tag);
                    return true;
                }
            }
        }
        //check down
        if (j > 1 && j < height)
        {
            if (allGems[i, j - 2] != null && allGems[i, j - 1] != null)
            {
                if (allGems[i, j - 2].tag == gem.tag && allGems[i, j - 1].tag == gem.tag)
                {
                    //Debug.Log("Gem: " + "(" + i + "," + j + ")" + " was: " + gem.tag);
                    return true;
                }
            }
        }
        return false;
    }



    #endregion


    #region Maintance Func
    private int CollOrRow() //conditions to make aBomb met check to deside colorBomb/adjBomb
    { 
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;

        for (int i = 0; i < matchCopy.Count; i++)
        {
            Gem thisGem = matchCopy[i].GetComponent<Gem>();
            int coll = thisGem.coll;
            int row = thisGem.row;
            int collMatch = 0;
            int rowMatch = 0;
            for (int j = 0; j < matchCopy.Count; j++)
            {
                //count matches in rows and column
                Gem nextGem = matchCopy[j].GetComponent<Gem>();
                if (nextGem == thisGem)
                {
                    continue;
                }
                if (nextGem.coll == thisGem.coll && nextGem.CompareTag(thisGem.tag))
                {
                    collMatch++;
                }
                if (nextGem.row == thisGem.row && nextGem.CompareTag(thisGem.tag))
                {
                    rowMatch++;
                }
            }
            //column == row => return 3
            //Adj => 2
            //Color => 1
            if (collMatch == 4 || rowMatch == 4) // colorbomb
            {
                return 1;
            }
            if(collMatch == 2 && rowMatch == 2) // adj bomb
            {
                return 2;
            }
            if (collMatch == 3 || rowMatch == 3) //coll/row bomb
            {
                return 3;
            }
        }

        return 0;
    } 
    private void CheckBombMaking()
    { 
        if(findMatches.currentMatches.Count > 3)
        {
            int typeBomb = CollOrRow();
            if(typeBomb == 1)
            {
                findMatches.MakeColorBomb();
            }
            else if(typeBomb == 2)
            {
                findMatches.MakeAdjBomb();
            }
            else if(typeBomb == 3)
            {
                findMatches.MakeLineBombs();
            }
        }
    }
    private void RefillSpots()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    if (allGems[i, j] == null)
                    {
                        Vector2 tempPos = new Vector2(i, j + SpawnOffset);
                        int rndGem = Random.Range(0, gemsArr.Length);
                        int maxIter = 0;
                        while(MatchCheck(i,j, gemsArr[rndGem]) && maxIter <100)
                        {
                            maxIter++;
                            rndGem = Random.Range(0, gemsArr.Length);
                        }
                        maxIter = 0;
                        GameObject gem = Instantiate(gemsArr[rndGem], tempPos, Quaternion.identity);
                        allGems[i, j] = gem;
                        gem.GetComponent<Gem>().row = j;
                        gem.GetComponent<Gem>().coll = i;
                    }
                    else
                    {
                        allGems[i, j].GetComponent<Gem>().prevColl = allGems[i, j].GetComponent<Gem>().coll;
                        allGems[i, j].GetComponent<Gem>().prevRow = allGems[i, j].GetComponent<Gem>().row;
                    }
                }
            }
        }
    }
    private void DestroyMatchAt(int coll, int row)
    {
        if (allGems[coll, row].GetComponent<Gem>().match)
        {
            findMatches.currentMatches.Remove(allGems[coll, row]);
            for (int i=0; i<EffectsArr.Length; i++)
            {
                if (allGems[coll, row].GetComponent<Gem>().tag == EffectsArr[i].tag)
                {
                    GameObject particel = Instantiate(EffectsArr[i], allGems[coll, row].transform.position, Quaternion.identity) as GameObject;
                    Destroy(particel, ExplosionEffectLifeTime);
                    break;
                }
            }
            if(goalManager != null)
            {
                goalManager.CompareGoals(allGems[coll, row].tag.ToString());
                goalManager.UpdateGoals();
            }
            Destroy(allGems[coll, row]);
            soundManager.PlayPopSound();
            scoreManager.IncreaseScore(baseValue * comboValue);
            allGems[coll, row] = null;
        }
    }
    public void DestroyAllMatches()
    {
        if (findMatches.currentMatches.Count >= 4)
        {
            CheckBombMaking();
        }
        findMatches.currentMatches.Clear();
        for (int i = 0; i < width ; i++)
        {
            for (int j = 0; j< height; j++)
            {
                if (allGems[i, j] != null)
                {
                    DestroyMatchAt(i, j);
                }
            }
        }
        StartCoroutine(CollapseRowCo2()); //collapse row if needed
    }
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    if (allGems[i, j].GetComponent<Gem>().match)
                    {
                        return true;
                    }
                }
            }
        }
            return false;
    }

    #endregion


    #region Deadlock Detection and hints
    private void SwitchGems(int coll, int row, Vector2 direction)
    {
        GameObject holder = allGems[coll + (int)direction.x, row + (int)direction.y] as GameObject;
        //switching the gem to second position
        allGems[coll + (int)direction.x, row + (int)direction.y] = allGems[coll, row];
        //set first gem to be the second gem
        allGems[coll, row] = holder;
    }

    private bool CheckMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null) 
                {
                    if (i < width - 2)
                    {
                        //gems right

                        if (allGems[i + 1, j] != null && allGems[i + 2, j] != null)
                        {
                            if (allGems[i + 1, j].tag == allGems[i, j].tag
                                && allGems[i + 2, j].tag == allGems[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        //gems up
                        if (allGems[i, j + 1] != null && allGems[i, j + 2] != null)
                        {
                            if (allGems[i, j + 1].tag == allGems[i, j].tag
                                && allGems[i, j + 2].tag == allGems[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int coll, int row, Vector2 direction)
    {
        SwitchGems(coll, row, direction);
        if (CheckMatches())
        {
            SwitchGems(coll, row, direction);
            return true;
        }
        SwitchGems(coll, row, direction);
        return false;
    }

    private bool IsDeadlock()
    {
        for(int i=0; i<width; i++)
        {
            for(int j = 0; j< height; j++)
            {
                if (allGems[i,j] != null)
                {
                    if (i < width - 1)
                    {

                        if(SwitchAndCheck(i, j, Vector2.right)) { 
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        //add every current gems to a list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    newBoard.Add(allGems[i, j]);
                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    //take a random gem from the list
                    int rndGem = Random.Range(0, newBoard.Count);
                    while (MatchCheck(i, j, newBoard[rndGem])) //making sure no matches created at the start
                    {
                        rndGem = Random.Range(0, newBoard.Count);
                    }
                    Gem gem = newBoard[rndGem].GetComponent<Gem>();
                    gem.coll = i;
                    gem.row = j;
                    allGems[i, j] = newBoard[rndGem];
                    //remove it from the list
                    newBoard.Remove(newBoard[rndGem]);
                }
            }
        }
        //Edge case - new board is deadlocked again...
        if (IsDeadlock())
        {
            ShuffleBoard();
        }
    }

    #endregion

    
}
