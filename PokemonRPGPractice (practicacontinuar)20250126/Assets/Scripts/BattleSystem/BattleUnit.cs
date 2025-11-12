using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//poderosa librería para animar por código

public class BattleUnit : MonoBehaviour
{
    
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHudHandler battleHud;
    [SerializeField] RectTransform particleFXCenter;

    //propiedad para asociar HUD de batalla en el UI a la unidad de batalla, facilitando la refactorización
    public BattleHudHandler BattleHud
    {
        get
        {
            return battleHud;
        }
    }

    public bool IsPlayerUnit
    {
        get
        {
            return isPlayerUnit;
        }
    }

    Image image;
    Vector2 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public Creature Creature { get; set; }
    //inicializa la criatura en el UI
    public void Setup(Creature creature)
    {//después de cargar, la criatura llega null en setup battle
        Creature = creature;
        if (isPlayerUnit)
        {//Establecer imagen para batalla
            image.sprite = Creature.Base.CreatureBattleSprite;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {//Si no es criatura del jugador estará invertido en ekl eje X, mirando a la izquierda porque está en la esquina derecha superior
            image.sprite = Creature.Base.CreatureBattleSprite;//Error, al segundo efrenteamiento conrta entrenador la criatura llega null
            image.transform.localScale = new Vector3(-1, 1, 1);
        }
        BattleHud.gameObject.SetActive(true);

        image.color = originalColor;

        PlayEnterAnimation();
    }

    public void Clear()
    {
        BattleHud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-700f, originalPos.y);

        }
        else
        {
            image.transform.localPosition = new Vector3(+700f, originalPos.y);
        }
        image.transform.DOLocalMoveX(originalPos.x, 1f); //anima desplazamiento suavemente
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x+ 100f, .25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 100f, .25f));
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, .25f));
    }

    public void PlayHitAnimation(ParticleFXContainer particleFX=null)
    {
        //TODO: añadir variable para recivir un objeto BattleParticleFX (ParticleFXID,GameObject) para reproducirlo en su punto de ParticleFX
        //animación de daño
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, .1f));
        sequence.Append(image.DOColor(originalColor, .1f));
        if(particleFX != null)
        {
            //El sistema de particulas está configurado para autodestruirse después de su duración
            ParticleFXContainer particleInstance= Instantiate(particleFX, particleFXCenter);
        }
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, .5f));
        sequence.Join(image.DOFade(0f,.5f));

    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.DOFade(0,.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, .5f));

        sequence.Join(transform.DOScale(new Vector3(.3f, .3f, 1f), .5f));

        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.DOFade(1, .2f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, .5f));

        sequence.Join(transform.DOScale(new Vector3(-1f, 1f, 1f), .2f));

        yield return sequence.WaitForCompletion();
    }
}
