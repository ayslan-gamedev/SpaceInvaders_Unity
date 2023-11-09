using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Trench : MonoBehaviour
{
    private PolygonCollider2D polygonCollider;

    private const int width = 24;
    private const int height = 18;
    private const double pxToUnit = 0.12;

    void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;

        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        DrawTrench();
        Regenerate(polygonCollider, spriteRenderer);
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

#pragma warning disable UNT0017 // SetPixels invocation is slow
        texture.SetPixels(pixels);
#pragma warning restore UNT0017 // SetPixels invocation is slow
        texture.Apply();

        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        spriteRenderer.sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(spriteRenderer == null)
        {
            return;
        }

        // Calculate the pixel position of the object entering the trench.
        Vector2 pixel = new(
            x: Mathf.FloorToInt((other.transform.position.x + ((float)pxToUnit * (width / 2)) - transform.position.x) * 100 / (width / 2)),
            y: Mathf.FloorToInt((other.transform.position.y + ((float)pxToUnit * (height / 2)) - transform.position.y) * 100 / (height / 2))
        );

        if(pixel.x >= 0 && pixel.x < width && pixel.y >= 0 && pixel.y <= height)
        {
            // Clear the pixel at the calculated position.
            texture.SetPixel((int)pixel.x, (int)pixel.y, Color.clear);
            texture.Apply();

            Regenerate(polygonCollider, spriteRenderer);
        }
    }

    private const float alphaCutoff = 0.5f;

    public void Regenerate(SpriteRenderer SpriteRender, PolygonCollider2D PolygonCollider2D)
    {
        Regenerate(PolygonCollider2D, SpriteRender);
    }

    public void Regenerate(PolygonCollider2D PolygonCollider2D, SpriteRenderer SpriteRender)
    {
        if(SpriteRender.sprite == null)
        {
            PolygonCollider2D.pathCount = 0;
            return;
        }

        if(SpriteRender.sprite.texture == null)
        {
            PolygonCollider2D.pathCount = 0;
            return;
        }

        if(SpriteRender.sprite.texture.isReadable == false)
        {
            PolygonCollider2D.pathCount = 0;
            throw new Exception(
                $"PixelCollider2D could not be regenerated because on \"{PolygonCollider2D.gameObject.name}\" because the sprite does not allow read/write operations.");
        }

        List<List<Vector2Int>> Pixel_Paths = Get_Unit_Paths(SpriteRender.sprite.texture, alphaCutoff);
        Pixel_Paths = Simplify_Paths_Phase_1(Pixel_Paths);

        List<List<Vector2>> World_Paths = Finalize_Paths(Pixel_Paths, SpriteRender.sprite);
        PolygonCollider2D.pathCount = World_Paths.Count;

        for(int i = 0; i < World_Paths.Count; i++)
        {
            PolygonCollider2D.SetPath(i, World_Paths[i].ToArray());
        }
    }

    private List<List<Vector2>> Finalize_Paths(List<List<Vector2Int>> Pixel_Paths, Sprite sprite)
    {
        var tex = sprite.texture;

        Vector2 pivot = sprite.pivot;
        pivot.x *= Mathf.Abs(sprite.bounds.max.x - sprite.bounds.min.x);
        pivot.x /= tex.width;
        pivot.y *= Mathf.Abs(sprite.bounds.max.y - sprite.bounds.min.y);
        pivot.y /= tex.height;

        List<List<Vector2>> Output = new();
        for(int p = 0; p < Pixel_Paths.Count; p++)
        {
            List<Vector2> Current_List = new();
            for(int o = 0; o < Pixel_Paths[p].Count; o++)
            {
                Vector2 point = Pixel_Paths[p][o];
                point.x *= Mathf.Abs(sprite.bounds.max.x - sprite.bounds.min.x);
                point.x /= tex.width;
                point.y *= Mathf.Abs(sprite.bounds.max.y - sprite.bounds.min.y);
                point.y /= tex.height;
                point -= pivot;
                Current_List.Add(point);
            }

            Output.Add(Current_List);
        }

        return Output;
    }

    private static List<List<Vector2Int>> Simplify_Paths_Phase_1(List<List<Vector2Int>> Unit_Paths)
    {
        List<List<Vector2Int>> Output = new();
        while(Unit_Paths.Count > 0)
        {
            List<Vector2Int> Current_Path = new(Unit_Paths[0]);
            Unit_Paths.RemoveAt(0);
            bool Keep_Looping = true;
            while(Keep_Looping)
            {
                Keep_Looping = false;
                for(int i = 0; i < Unit_Paths.Count; i++)
                {
                    if(Current_Path[^1] == Unit_Paths[i][0])
                    {
                        Keep_Looping = true;
                        Current_Path.RemoveAt(Current_Path.Count - 1);
                        Current_Path.AddRange(Unit_Paths[i]);
                        Unit_Paths.RemoveAt(i);
                        i--;
                    }
                    else if(Current_Path[0] == Unit_Paths[i][^1])
                    {
                        Keep_Looping = true;
                        Current_Path.RemoveAt(0);
                        Current_Path.InsertRange(0, Unit_Paths[i]);
                        Unit_Paths.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        List<Vector2Int> Flipped_Path = new(Unit_Paths[i]);
                        Flipped_Path.Reverse();
                        if(Current_Path[^1] == Flipped_Path[0])
                        {
                            Keep_Looping = true;
                            Current_Path.RemoveAt(Current_Path.Count - 1);
                            Current_Path.AddRange(Flipped_Path);
                            Unit_Paths.RemoveAt(i);
                            i--;
                        }
                        else if(Current_Path[0] == Flipped_Path[^1])
                        {
                            Keep_Looping = true;
                            Current_Path.RemoveAt(0);
                            Current_Path.InsertRange(0, Flipped_Path);
                            Unit_Paths.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            Output.Add(Current_Path);
        }

        for(int pa = 0; pa < Output.Count; pa++)
        {
            for(int po = 0; po < Output[pa].Count; po++)
            {
                Vector2Int Start;
                if(po == 0)
                {
                    Start = Output[pa][^1];
                }
                else
                {
                    Start = Output[pa][po - 1];
                }

                Vector2Int End;
                if(po == Output[pa].Count - 1)
                {
                    End = Output[pa][0];
                }
                else
                {
                    End = Output[pa][po + 1];
                }

                Vector2Int Current_Point = Output[pa][po];
                Vector2 Direction1 = Current_Point - (Vector2)Start;
                Direction1 /= Direction1.magnitude;
                Vector2 Direction2 = End - (Vector2)Start;
                Direction2 /= Direction2.magnitude;
                if(Direction1 == Direction2)
                {
                    Output[pa].RemoveAt(po);
                    po--;
                }
            }
        }

        return Output;
    }

    private static List<List<Vector2Int>> Get_Unit_Paths(Texture2D texture, float alphaCutoff)
    {
        List<List<Vector2Int>> Output = new();
        for(int x = 0; x < texture.width; x++)
        {
            for(int y = 0; y < texture.height; y++)
            {
                if(PixelSolid(texture, new Vector2Int(x, y), alphaCutoff))
                {
                    if(!PixelSolid(texture, new Vector2Int(x, y + 1), alphaCutoff))
                    {
                        Output.Add(new List<Vector2Int>() { new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1) });
                    }

                    if(!PixelSolid(texture, new Vector2Int(x, y - 1), alphaCutoff))
                    {
                        Output.Add(new List<Vector2Int>() { new Vector2Int(x, y), new Vector2Int(x + 1, y) });
                    }

                    if(!PixelSolid(texture, new Vector2Int(x + 1, y), alphaCutoff))
                    {
                        Output.Add(new List<Vector2Int>() { new Vector2Int(x + 1, y), new Vector2Int(x + 1, y + 1) });
                    }

                    if(!PixelSolid(texture, new Vector2Int(x - 1, y), alphaCutoff))
                    {
                        Output.Add(new List<Vector2Int>() { new Vector2Int(x, y), new Vector2Int(x, y + 1) });
                    }
                }
            }
        }

        return Output;

        static bool PixelSolid(Texture2D texture, Vector2Int point, float alphaCutoff)
        {
            if(point.x < 0 || point.y < 0 || point.x >= texture.width || point.y >= texture.height)
            {
                return false;
            }

            float pixelAlpha = texture.GetPixel(point.x, point.y).a;
            if(alphaCutoff == 0)
            {
                if(pixelAlpha != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if(alphaCutoff == 1)
            {
                if(pixelAlpha == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return pixelAlpha >= alphaCutoff;
            }
        }
    }
}
