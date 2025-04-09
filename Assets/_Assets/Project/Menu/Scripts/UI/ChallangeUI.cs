using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallangeUI : MonoBehaviour
{

    public GameObject challange_Text;
    public Sprite challangeImage;

    void Start()
    {

    }

    public void Achieve()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = challangeImage;
    }
}
