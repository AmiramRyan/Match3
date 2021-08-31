using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] popSoundsArr;


    public void PlayPopSound()
    {
        int rndPop = Random.Range(0, popSoundsArr.Length);
        popSoundsArr[rndPop].Play();
    }
}
    
