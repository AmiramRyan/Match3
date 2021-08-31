using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "World")]
public class World : ScriptableObject
{
    public Lvl[] levelsArr;
}
