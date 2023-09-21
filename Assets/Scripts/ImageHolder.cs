using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ImageHolder", menuName = "Game/ImageHolder")]
public class ImageHolder : DescriptionBaseSO
{
    [SerializeField] public Sprite[] sprites;
}
