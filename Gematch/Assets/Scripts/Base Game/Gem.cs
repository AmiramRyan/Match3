using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// calling making MakeAdjBomb (not actualy making it) when ther is no need (might be problem later)
public class Gem : MonoBehaviour
{
    #region Public
    [Header("Board vars")]
    public int coll; //xpos
    public int row; //ypos
    public int targetX;
    public int targetY;
    public bool match = false;
    public int prevColl;
    public int prevRow;

    [Header("Swipe vars")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("Power Ups")]
    public bool isCollBomb;
    public bool isRowBomb;
    public bool isColorBomb;
    public bool isAdjacentBomb;
    //public Sprite colorBomb;
    public GameObject colorBombSprite;
    public GameObject adjacentBombSprit;
    public GameObject rowBombSprite;
    public GameObject collBombSprite;


    public GameObject otherGem;
    #endregion

    #region Private
    private BoardManager board;
    private Vector2 firstTouchPos = Vector2.zero;
    private Vector2 releaseTouchPos = Vector2.zero;
    private Vector2 tempPos;
    private FindMatches findMatches;
    private Hints hintManger;
    
    #endregion

    void Start()
    {
        isCollBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        board = GameObject.FindWithTag("Board").GetComponent<BoardManager>();
        //board = FindObjectOfType<BoardManager>(); (Not so fast)
        findMatches = FindObjectOfType<FindMatches>();
        hintManger = FindObjectOfType<Hints>();
    }


    //Debugs Only
    /*private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentBombSprit, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }*/



    void Update()
    {

        targetX = coll; //if diff from current pos the gem will move on the x axis
        targetY = row;  //if diff from current pos the gem will move on the y axis

        //horizontal movement
        if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            //move to new pos
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, board.GemSwitchSpeed);
            if (board.allGems[coll, row] != this.gameObject)
            {
                board.allGems[coll, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //stay in place 
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allGems[coll, row] = this.gameObject;
        }
        //vertical movement
        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            //move to new pos
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, board.GemSwitchSpeed);
            if (board.allGems[coll, row] != this.gameObject)
            {
                board.allGems[coll, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //stay in place 
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            //board.allGems[coll, row] = this.gameObject;
        }
    }

    #region CoRoutines

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(board.timeToWaitCheckForMatch);
        if (otherGem != null)
        {
            if (!match && !otherGem.GetComponent<Gem>().match)
            {
                // if no match is made other gem gose to the active gem position
                otherGem.GetComponent<Gem>().row = row;
                otherGem.GetComponent<Gem>().coll = coll;
                // active gem go to prev position
                row = prevRow;
                coll = prevColl;
                yield return new WaitForSeconds(board.timeToWaitBeforeMoveIfNoMatch);
                board.currentGem = null;
                if (board.curentState == GameState.wait)
                {
                    board.curentState = GameState.move;
                }
            }
            else
            {
                if(board.endGameManager != null)
                {
                    if(board.endGameManager.req.gameType == GameType.move)
                    {
                        board.endGameManager.DecreaseCounterVal();
                    }
                }
                board.DestroyAllMatches();
            }
            otherGem = null;
        }
    }

    #endregion

    #region Func
    void clacAngle()
    {
        if (Mathf.Abs(releaseTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(releaseTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            board.curentState = GameState.wait;
            swipeAngle = Mathf.Atan2(releaseTouchPos.y - firstTouchPos.y, releaseTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI; //conver rediant to degree
            board.currentGem = this; //Ref for powerUps
            MoveGems();
        }
        else
        {
            if (board.curentState == GameState.wait)
            {
                board.curentState = GameState.move;
            }
        }
    }

    void MoveGems()
    {
        if (swipeAngle <= 45 && swipeAngle > -45 && coll < board.width - 1)
        {
            MoveGemsActual(Vector2.right);
        }
        else if ((swipeAngle <= -135 || swipeAngle > 135) && coll > 0)
        {
            MoveGemsActual(Vector2.left);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MoveGemsActual(Vector2.up);
        }
        else if (swipeAngle <= -45 && swipeAngle > -135 && row > 0)
        {
            MoveGemsActual(Vector2.down);
        }
        else
        {
            if (board.curentState == GameState.wait)
            {
               board.curentState = GameState.move;
            }
        }
    }

    void MoveGemsActual(Vector2 direction)
    {
        otherGem = board.allGems[coll + (int)direction.x, row + (int)direction.y];
        prevColl = coll;
        prevRow = row;
        if (otherGem != null)
        {
            otherGem.GetComponent<Gem>().coll += -1 * (int)direction.x;
            otherGem.GetComponent<Gem>().row += -1 * (int)direction.y;
            coll += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            if (board.curentState == GameState.wait) 
            { 
            board.curentState = GameState.move;
            }
        }
    }

    #endregion

    #region MakeBombs
    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject LineBomb = Instantiate(rowBombSprite, transform.position, Quaternion.identity);
        LineBomb.transform.parent = this.transform;
    }

    public void MakeCollBomb()
    {
        isCollBomb = true;
        GameObject LineBomb = Instantiate(collBombSprite, transform.position, Quaternion.identity);
        LineBomb.transform.parent = this.transform;
    }

    public void MakeColorBomb()
    {
        isColorBomb = true;
        GameObject colorBomb = Instantiate(colorBombSprite, transform.position, Quaternion.identity);
        colorBomb.transform.parent = this.transform;
        this.GetComponent<SpriteRenderer>().enabled = false;
    }
    
    public void MakeAdjBomb()
    {
        isAdjacentBomb = true;
        GameObject marker = Instantiate(adjacentBombSprit, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }

    #endregion

    #region MouseClicks

    private void OnMouseDown()
    {
        if (hintManger != null)
        {
            hintManger.killHint();
        }
        if (board.curentState == GameState.move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.curentState == GameState.move)
        {
            releaseTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clacAngle();
        }

    }

    #endregion
}
