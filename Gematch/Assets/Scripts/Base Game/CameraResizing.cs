using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResizing : MonoBehaviour
{
    private BoardManager board;
    public float cameraOffset;
    public float paddingOffset;
    public float aspectRatio;
    public float yOffset = 1;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        if (board != null)
        {
            RepositionCam(board.width - 1, board.height - 1);
        }
    }

    void RepositionCam(float x, float y)
    {
        Vector3 tempPos = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        transform.position = tempPos;
        if(board.width > board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + paddingOffset) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + paddingOffset;
        }
    }




    void Update()
    {
        
    }
}
