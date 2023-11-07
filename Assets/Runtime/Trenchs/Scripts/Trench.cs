using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Trench : MonoBehaviour
{
    private PolygonCollider2D polygonCollider;

    private const int width = 24;
    private const int height = 18;
    private const double pxToUnit = 0.12;

    void Start()
    {
        // Initialize the trench and scale it.
        DrawTrench();
        AtualizeCollider();

        //transform.localScale *= 12;
    }

    private Texture2D texture;
    private SpriteRenderer spriteRenderer;

    private void DrawTrench()
    {
        Color[] pixels;

        // Create a new 2D texture for the trench.
        texture = new Texture2D(width, height)
        {
            filterMode = FilterMode.Point
        };

        pixels = texture.GetPixels();

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Color color = Color.clear;

                if(y >= height - 5)
                {
                    if(x > 4 - (height - y) && x < width - 5 + (height - y))
                    {
                        color = Color.white;
                    }
                }
                else if(y >= 2 && y <= 4)
                {
                    if(!(x > 20 - (height - y) && x < width - 21 + (height - y)))
                    {
                        color = Color.white;
                    }
                }
                else if(y < 2)
                {
                    if(x < 5 || x >= width - 5)
                    {
                        color = Color.white;
                    }
                }
                else
                {
                    color = Color.white;
                }

                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        spriteRenderer.sprite = sprite;
    }


    void AtualizeCollider()
    {
        try
        {
            polygonCollider = GetComponent<PolygonCollider2D>();
        }
        catch
        {
            polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            AtualizeCollider();
        }


        for(int y = 0; y < height; y++)
        {

            for(int x = 0; x < width; x++)
            {
                Color pixelColor = texture.GetPixel(x, y);




            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(spriteRenderer == null)
        {
            return;
        }

        // Calculate the pixel position of the object entering the trench.
        Vector2 pixel = new Vector2(
            x: Mathf.FloorToInt((other.transform.position.x + ((float)pxToUnit * (width / 2)) - transform.position.x) * 100 / (width / 2)),
            y: Mathf.FloorToInt((other.transform.position.y + ((float)pxToUnit * (height / 2)) - transform.position.y) * 100 / (height / 2))
        );

        if(pixel.x >= 0 && pixel.x < width && pixel.y >= 0 && pixel.y <= height)
        {
            // Clear the pixel at the calculated position.
            texture.SetPixel((int)pixel.x, (int)pixel.y, Color.clear);
            texture.Apply();
        }
    }
}
