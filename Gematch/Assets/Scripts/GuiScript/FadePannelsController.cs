using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePannelsController : MonoBehaviour
{
    public Animator fadePanel;
    public Animator infoPannel;
     
    public void OkBtn()
    {
        if (infoPannel != null && fadePanel != null)
        {
            fadePanel.SetBool("PannelOut", true);
            infoPannel.SetBool("InfoOut", true);
            StartCoroutine(GameStartCo());
        }
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);
        BoardManager board = FindObjectOfType<BoardManager>();
        board.curentState = GameState.move;
    }
}
