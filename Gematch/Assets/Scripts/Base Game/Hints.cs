using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hints : MonoBehaviour
{
    private BoardManager board;
    public float hintDelay;
    private float hintDelaySec;
    public GameObject currentHint;
    public GameObject hintEffect;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        hintDelaySec = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (board.curentState == GameState.move)
        {
            hintDelaySec -= Time.deltaTime;
            if (hintDelaySec <= 0 && currentHint == null)
            {
                ShowHint();
                hintDelaySec = hintDelay;
            }
        }
        else
        {
            hintDelaySec = hintDelay; //reset timer for hint
        }
    }
    List<GameObject> CanMatchMe()
    {
        List<GameObject> possibleMatches = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allGems[i, j] != null)
                {
                    if (i < board.width - 1)
                    {

                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMatches.Add(board.allGems[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMatches.Add(board.allGems[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMatches;
    }

    GameObject pickRndHint()
    {
        List<GameObject> possibleMatches = new List<GameObject>();
        possibleMatches = CanMatchMe();
        if(possibleMatches.Count > 0)
        {
            int rndGem = Random.Range(0, possibleMatches.Count);
            return possibleMatches[rndGem];
        }
        return null;
    }

    private void ShowHint()
    {
        GameObject move = pickRndHint();
        if (move != null)
        {
            currentHint = Instantiate(hintEffect, move.transform.position, Quaternion.identity);
        }
    }
    public void killHint()
    {
        if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySec = hintDelay;
        }
    }
}
