using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
 //fix bomb making on wrong combo
public class FindMatches : MonoBehaviour
{
    private BoardManager board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<BoardManager>(); //ref the board
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }
    private IEnumerator FindAllMatchesCo()
    {
        //yield return new WaitForSeconds(.2f);
        yield return null;
        for (int i = 0; i< board.width; i++)
        {
            for(int j = 0; j< board.height; j++)
            {
                
                GameObject currGem = board.allGems[i, j]; //ref to current gem
                if (currGem != null) 
                {
                    //color bomb check
                    if (board.currentGem != null && board.currentGem.otherGem != null) //only check color bomb if a switch between 2 happend
                    {
                        currentMatches.Union(CheckForColorBomb(board.currentGem, board.currentGem.otherGem.GetComponent<Gem>()));
                    }
                    
                    if (i > 0 && i < board.width - 1) //check left and right
                    {
                        GameObject leftGem = board.allGems[i - 1, j];
                        GameObject rightGem = board.allGems[i + 1, j];
                        if (leftGem != null && rightGem != null)
                        {
                            
                            if (leftGem.tag == currGem.tag && rightGem.tag == currGem.tag)
                            {
                                Gem leftGemScript = leftGem.GetComponent<Gem>();
                                Gem rightGemScript = rightGem.GetComponent<Gem>();
                                Gem currentGemScript = currGem.GetComponent<Gem>();
                                
                                currentMatches.Union(CheckForRowBomb(leftGemScript, currentGemScript, rightGemScript));

                                currentMatches.Union(CheckForCollBomb(leftGemScript, currentGemScript, rightGemScript));
                                
                                currentMatches.Union(CheckForAdjBomb(leftGemScript, currentGemScript, rightGemScript));

                                GetNerbyPieces(leftGem, currGem, rightGem);
                            }
                        }
                    }
                    //Vertical matches check
                    if (j > 0 && j < board.height - 1) // check up and down
                    {
                        GameObject upGem = board.allGems[i, j +1];
                        GameObject downGem = board.allGems[i, j - 1];
                        if (upGem != null && downGem != null)
                        {
                            if (upGem.tag == currGem.tag && downGem.tag == currGem.tag)
                            {
                                Gem upGemScript = upGem.GetComponent<Gem>();
                                Gem downGemScript = downGem.GetComponent<Gem>();
                                Gem currentGemScript = currGem.GetComponent<Gem>();
                                
                                currentMatches.Union(CheckForCollBomb(upGemScript, currentGemScript, downGemScript));

                                currentMatches.Union(CheckForRowBomb(upGemScript, currentGemScript, downGemScript));

                                currentMatches.Union(CheckForAdjBomb(upGemScript, currentGemScript, downGemScript));

                                GetNerbyPieces(upGem, currGem, downGem);
                            }
                        }
                    }
                }
            }
        }
    }

    #region Collection Func

    List<GameObject> GetCollGems(int coll)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < board.height; i++) // get all the gems in the coll and set them to match=true
        {
            if (board.allGems[coll, i] != null)
            {
                gems.Add(board.allGems[coll, i]);
                //if one of them have a line bomb
                if (board.allGems[coll, i].GetComponent<Gem>().isRowBomb && !board.allGems[coll, i].GetComponent<Gem>().match)
                {
                    gems.Union(GetrowGems(i)); 
                }
                //if one of them is a color bomb
                else if (board.allGems[coll, i].GetComponent<Gem>().isColorBomb && !board.allGems[coll, i].GetComponent<Gem>().match)
                {
                    if (board.currentGem.match)
                    {
                        gems.Union(GetColordGems(board.currentGem.tag)); //get all the same colors as well
                    }
                    else
                    {
                        gems.Union(GetColordGems(board.currentGem.otherGem.GetComponent<Gem>().tag));
                    }

                }
                //if one is a Adjbomb
                else if (board.allGems[coll, i].GetComponent<Gem>().isAdjacentBomb && !board.allGems[coll, i].GetComponent<Gem>().match)
                {
                    gems.Union(GetAdjacentGems(coll,i));
                }
                board.allGems[coll, i].GetComponent<Gem>().match = true;
            }
        }
        return gems;
    }

    List<GameObject> GetrowGems(int row)
    {
        List<GameObject> gems = new List<GameObject>(); // get all the gems in the row and set them to match=true
        for (int i = 0; i < board.width; i++) 
        {
            if (board.allGems[i, row] != null)
            {
                gems.Add(board.allGems[i, row]);
                //if one of them have a line bomb
                if (board.allGems[i, row].GetComponent<Gem>().isCollBomb && !board.allGems[i, row].GetComponent<Gem>().match)
                {
                    gems.Union(GetCollGems(i)); //get the coll 
                }
                //if one of them is a color bomb
                else if (board.allGems[i, row].GetComponent<Gem>().isColorBomb && board.allGems[i, row].GetComponent<Gem>().match)
                {
                    if (board.currentGem.match)
                    {
                        gems.Union(GetColordGems(board.currentGem.tag)); //get all the same colors as well
                    }
                    else
                    {
                        gems.Union(GetColordGems(board.currentGem.otherGem.GetComponent<Gem>().tag));
                    }
                }
                //if one is a Adjbomb
                else if (board.allGems[i, row].GetComponent<Gem>().isAdjacentBomb && !board.allGems[i, row].GetComponent<Gem>().match)
                {
                    gems.Union(GetAdjacentGems(i, row));
                }
                board.allGems[i, row].GetComponent<Gem>().match = true;
            }
        }
        return gems;
    }

    List<GameObject> GetColordGems(string color)
    {
        List<GameObject> gems = new List<GameObject>(); // get all the gems in the same color and set them to match=true
        for (int i = 0; i<board.width; i++)
        {
            for (int j=0; j< board.height; j++)
            {
                if (board.allGems[i,j] != null && board.allGems[i, j].tag == color)
                {
                    gems.Add(board.allGems[i, j]);
                    //if one of them is a coll bomb
                    if (board.allGems[i, j].GetComponent<Gem>().isCollBomb && !board.allGems[i, j].GetComponent<Gem>().match)
                    {
                        gems.Union(GetCollGems(i)); 
                    }
                    //if one of them is a row bomb
                    if (board.allGems[i, j].GetComponent<Gem>().isRowBomb && !board.allGems[i, j].GetComponent<Gem>().match)
                    {
                        gems.Union(GetrowGems(j));
                    }
                    //if one is a Adjbomb
                    else if (board.allGems[i, j].GetComponent<Gem>().isAdjacentBomb && !board.allGems[i, j].GetComponent<Gem>().match)
                    {
                        gems.Union(GetAdjacentGems(i, j));
                    }
                    board.allGems[i, j].GetComponent<Gem>().match = true;
                }
               
            }
        }
        return gems;
    }

    List<GameObject> GetAdjacentGems(int col, int row)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = col-1; i<=col+1; i++)
        {
            for (int j = row-1; j<=row+1; j++)
            {
                if (i >= 0 && i < board.width && j < board.height && j >= 0)
                {
                    if (board.allGems[i, j] != null)
                    {
                        gems.Add(board.allGems[i, j]);
                        //if one of them is a coll bomb
                        if (board.allGems[i, j].GetComponent<Gem>().isCollBomb && !board.allGems[i, j].GetComponent<Gem>().match)
                        {
                            gems.Union(GetCollGems(i)); //get the coll 
                        }
                        //if one of them is a row bomb
                        else if (board.allGems[i, j].GetComponent<Gem>().isRowBomb && !board.allGems[i, j].GetComponent<Gem>().match)
                        {
                            gems.Union(GetrowGems(j));
                        }
                        //if one is a Colorbomb
                        else if (board.allGems[i, j].GetComponent<Gem>().isColorBomb && !board.allGems[i, j].GetComponent<Gem>().match)
                        {
                            int rnd = Random.Range(0, board.gemsArr.Length);
                            string rndTag = board.gemsArr[rnd].tag;
                            gems.Union(GetColordGems(rndTag));
                            /*if (board.currentGem.match)
                            {
                                gems.Union(GetColordGems(board.currentGem.tag)); //get all the same colors as well
                            }
                            else
                            {
                                gems.Union(GetColordGems(board.currentGem.otherGem.GetComponent<Gem>().tag));
                            }*/

                        }
                        //if its a nother Adjbomb
                        else if ((i != col || j !=row) && board.allGems[i, j].GetComponent<Gem>().isAdjacentBomb && !board.allGems[i, j].GetComponent<Gem>().match)
                        {
                            gems.Union(GetAdjacentGems(i, j));
                        }
                        board.allGems[i, j].GetComponent<Gem>().match = true;
                    }
                }
            }
        }
        return gems;
    }
    #endregion

    #region Make Power Bombs
    public void MakeLineBombs()
    {
        //if we moved
        if (board.currentGem != null)
        {
            float swipeAngle = board.currentGem.swipeAngle;
            //if one of the 2 gems matched
            if (board.currentGem.match && !Abomb(board.currentGem)) //if the active gem made a match and is NOT already a powerup
            {
                board.currentGem.match = false;
                //Decide bomb type from swipe angle
                if ((swipeAngle <= 45 && swipeAngle > -45) || (swipeAngle <= -135 || swipeAngle > 135))
                {
                    //make rowbomb
                    board.currentGem.MakeRowBomb();
                }
                else if ((swipeAngle > 45 && swipeAngle <= 135) || (swipeAngle <= -45 && swipeAngle > -135))
                {
                    //make collbomb
                    board.currentGem.MakeCollBomb();
                }
            }
            else if (board.currentGem.otherGem != null)
            {
                Gem otherGem = board.currentGem.otherGem.GetComponent<Gem>();
                if (otherGem.match && !Abomb(otherGem)) //if the passive gem made a match and is NOT already a powerup
                {
                    otherGem.match = false;
                    //Decide bomb type from swipe angle
                    if ((swipeAngle <= 45 && swipeAngle > -45) || (swipeAngle <= -135 || swipeAngle > 135))
                    {
                        //make rowbomb
                        otherGem.MakeRowBomb();
                    }
                    else if ((swipeAngle > 45 && swipeAngle <= 135) || (swipeAngle <= -45 && swipeAngle > -135))
                    {
                        //make collbomb
                        otherGem.MakeCollBomb();
                    }
                }
            }
        }
    }

    public void MakeColorBomb()
    {
        if (board.currentGem != null)
        {
            //if one of the 2 gems matched
            if (board.currentGem.match && !Abomb(board.currentGem)) //if the active gem made a match and is NOT already a powerup
            {
                board.currentGem.match = false;
                board.currentGem.MakeColorBomb();
            }
            else if (board.currentGem.otherGem != null && !Abomb(board.currentGem.otherGem.GetComponent<Gem>()))
            {
                Gem otherGem = board.currentGem.otherGem.GetComponent<Gem>();
                if (otherGem.match && !Abomb(otherGem)) //if the passive gem made a match and is NOT already a powerup
                {
                    otherGem.match = false;
                    otherGem.MakeColorBomb();
                }
            }
        }
    }

    public void MakeAdjBomb()
    {
        if (board.currentGem != null)
        {
            if (board.currentGem.match && !Abomb(board.currentGem)) //if the active gem made a match and is NOT already a powerup
            {
                board.currentGem.match = false;
                board.currentGem.MakeAdjBomb();
            }
            else if (board.currentGem.otherGem != null)
            {
                Gem otherGem = board.currentGem.otherGem.GetComponent<Gem>();
                if (otherGem.match  && !Abomb(otherGem)) //if the passive gem made a match and is NOT already a powerup
                {
                    otherGem.match = false;
                    otherGem.MakeAdjBomb();
                }
            }
        }
    }

    #endregion

    #region CheckForBombs
    private List<GameObject> CheckForRowBomb(Gem gem1, Gem gem2, Gem gem3)
    {
        List<GameObject> gems = new List<GameObject>();

        if (gem1.isRowBomb)
        {
            gems.Union(GetrowGems(gem1.row));
        }

        if (gem2.isRowBomb)
        {
            gems.Union(GetrowGems(gem2.row));
        }

        if (gem3.isRowBomb)
        {
            gems.Union(GetrowGems(gem3.row));
        }
        return gems;
    }

    private List<GameObject> CheckForCollBomb(Gem gem1, Gem gem2, Gem gem3)
    {
        List<GameObject> gems = new List<GameObject>();

        if (gem1.isCollBomb)
        {
            gems.Union(GetCollGems(gem1.coll));
        }

        if (gem2.isCollBomb)
        {
            gems.Union(GetCollGems(gem2.coll));
        }

        if (gem3.isCollBomb)
        {
            gems.Union(GetCollGems(gem3.coll));
        }
        return gems;
    }

    private List<GameObject> CheckForColorBomb(Gem activeGem , Gem passiveGem)
    {
        List<GameObject> gems = new List<GameObject>();
            if (activeGem.isColorBomb)
            {
                activeGem.match = true;
                gems.Union(GetColordGems(passiveGem.tag)); //get the color of the other gem as a search term
            }
            else if (passiveGem.isColorBomb)
            {
                passiveGem.match = true;
                gems.Union(GetColordGems(activeGem.tag));
            }
        return gems;
    }

    private List<GameObject> CheckForAdjBomb(Gem gem1, Gem gem2, Gem gem3)
    {
        List<GameObject> gems = new List<GameObject>();

        if (gem1.isAdjacentBomb)
        {
            gems.Union(GetAdjacentGems(gem1.coll, gem1.row));
        }

        if (gem2.isAdjacentBomb)
        {
            gems.Union(GetAdjacentGems(gem2.coll, gem2.row));
        }

        if (gem3.isAdjacentBomb)
        {
            gems.Union(GetAdjacentGems(gem3.coll, gem3.row));
        }
        return gems;
    }

    #endregion

    private bool Abomb(Gem gem)
    {
        bool isBomb = false;
        if (gem.isCollBomb || gem.isRowBomb || gem.isColorBomb || gem.isAdjacentBomb)
        {
            isBomb = true;
        }

        return isBomb;
    }

    private void AddToListAndMatch(GameObject gem)
    {
        if (!currentMatches.Contains(gem))
        {
            currentMatches.Add(gem);
        }
        gem.GetComponent<Gem>().match = true;
    }

    private void GetNerbyPieces(GameObject gem1, GameObject gem2, GameObject gem3)
    {
        AddToListAndMatch(gem1);
        AddToListAndMatch(gem2);
        AddToListAndMatch(gem3);
    }
}
