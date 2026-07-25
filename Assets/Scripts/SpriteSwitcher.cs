using UnityEngine;
using System.Collections.Generic;

public class SpriteSwitcher : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
    }

    private void Update()
    {
        /// For cycling sprites
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    currentIndex--;
        //    if(currentIndex < 0)
        //    {
        //        currentIndex = sprites.Count - 1;
        //    }
        //    spriteRenderer.sprite = sprites[currentIndex];
        //}

        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    currentIndex++;
        //    if (currentIndex > sprites.Count-1)
        //    {
        //        currentIndex = 0;
        //    }
        //    spriteRenderer.sprite = sprites[currentIndex];
        //}

        if (Input.GetKey(KeyCode.Space))
        {
            spriteRenderer.sprite = sprites[1];
        }
        else
        {
            spriteRenderer.sprite = sprites[0];
        }
    }
}
