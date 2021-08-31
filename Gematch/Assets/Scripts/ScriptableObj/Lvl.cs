using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Lvl : ScriptableObject
{
    [Header("General vars")]
    public int width;
    public int height;

    [Header("Start tiles")]
    public TileType[] boardLayout;

    [Header("Available items")]
    public GameObject[] gemsArr;

    [Header("Goals")]
    public int[] ScoreGoals;

    [Header("EndGameReq")]
    public EndGameReq req;
    public BlankGoal[] levelGoals;
}
