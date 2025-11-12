using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    //Clase para nimar cualquier sprite 2D
    SpriteRenderer spriteRenderer;//después de cada periodo de tiempo, se cambia el sprite del renderer al siguiente frame
    List<Sprite> frames;//Cuando llegeue el final irá al principio
    float frameRate;

    int currentFrame;
    float timer;

    //Defecto 16 frames per second
    public SpriteAnimator(List<Sprite> frames,SpriteRenderer renderer,float frameRate=0.16f)
    {
        this.frames = frames;
        this.spriteRenderer = renderer;
        this.frameRate = frameRate;

    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0f;

        spriteRenderer.sprite = frames[0];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            //cuando han pasado suficientes frames, cambia el frame del sprite
            currentFrame = (currentFrame + 1)%frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer = 0;
        }
    }

    public List<Sprite> Frames
    {
        get
        {
            return frames;
        }
    }
}
