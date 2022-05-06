using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    public bool IsActive { set; get; }

    private const float GRAVITY = 2.0f;
    private float verVelocity;
    private float speed;
    private bool isSliced = false;

    public Sprite bombSprite;
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    private int spriteIndex;
    private float lastSpriteUpdate;
    private float spriteUpdateDelta = 0.025f;

    public void LaunchBomb(float verVelocity, float xSpeed, float xStart)
    {
        IsActive = true;
        this.speed = xSpeed;
        this.verVelocity = verVelocity;
        transform.position = new Vector3(xStart, 0, 0);
        isSliced = false;

        spriteIndex = 0;
        spriteRenderer.sprite = bombSprite;
    }

    public void Slice()
    {
        if (isSliced) return;
        if (verVelocity < 0.5f)
            verVelocity = 0.5f;
        speed = speed * 0.5f;
        isSliced = true;

        GameManager.Instance.SliceAllFruits();
        GameManager.Instance.LoseLifepoint();
    }

    private void Update()
    {
        if (!IsActive) return;

        if (!isSliced)
        {
            verVelocity -= GRAVITY * Time.deltaTime;
            transform.position += new Vector3(speed, verVelocity, 0) * Time.deltaTime;
        }

        if (isSliced)
        {
            if (spriteIndex < sprites.Length - 1 && Time.time - lastSpriteUpdate > spriteUpdateDelta)
            {
                lastSpriteUpdate = Time.time;
                spriteIndex++;
                spriteRenderer.sprite = sprites[spriteIndex];
            } 
            if (spriteIndex == sprites.Length - 1)
            {
                transform.position += new Vector3(0, -1000, 0);
            }
        }
        if (transform.position.y < -1)
        {
            IsActive = false;
        }

    }
}
