using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMiddlePoint : MonoBehaviour
{
    [SerializeField] Piece curPiece;
    void LateUpdate()
    {
        transform.position = (Vector3)curPiece._position + new Vector3(0.5f, 0.5f, 0f);
    }
}
