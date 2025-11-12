using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] FacingDirectionEnum defaultDirection=FacingDirectionEnum.DOWN;

    //Parameters
    public float MoveX;
    public float MoveY;
    public bool IsMoving;

    //States
    //Serán diferentes animaciones para ejecutar
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;

    //References
    SpriteRenderer spriteRenderer;


    private void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;

        if (MoveX == 1)//Se mueve a la derecha
        {
            currentAnim = walkRightAnim;
        }
        else if(MoveX==-1) //Se mueve a la izquierda
        {
            currentAnim = walkLeftAnim;
        }
        else if (MoveY == 1) //Se mueve hacia arriba
        {
            currentAnim = walkUpAnim;
        }
        else if (MoveY == -1) //Se mueve hacia abajo
        {
            currentAnim = walkDownAnim;
        }

        if (currentAnim != prevAnim || IsMoving!=wasPreviouslyMoving)
        {//iniciar animación si esta cambió
            currentAnim.Start();
        }

        if (IsMoving)//solo se moverá mientras el jugador se mueva
        {
            currentAnim.HandleUpdate();
        }
        else
        {//Si está quieto solo muestra la primera frame
            spriteRenderer.sprite = currentAnim.Frames[0];
        }

        wasPreviouslyMoving = IsMoving;
    }

    public void ResetFacingPosition()
    {
        SetFacingDirection(defaultDirection);
    }

    public void SetFacingDirection(FacingDirectionEnum dir)
    {
        switch (dir)
        {
            case FacingDirectionEnum.RIGHT:
                MoveX = 1;
                break;
            case FacingDirectionEnum.LEFT:
                MoveX = -1;
                break;
            case FacingDirectionEnum.DOWN:
                MoveY = -1;
                break;
            case FacingDirectionEnum.UP:
                MoveY = 1;
                break;
        }
    }

    public FacingDirectionEnum DefaultDirection
    {
        get => defaultDirection;
    }
}
