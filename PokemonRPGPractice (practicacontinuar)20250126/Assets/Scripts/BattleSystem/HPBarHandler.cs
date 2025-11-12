using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarHandler : MonoBehaviour
{
    [SerializeField] GameObject health;

    public bool IsUpdating { get; private set; }

    public void SetHP(float hpNormalized)
    {//Actualiza la barra de vida, con el punto de pivot a la izquierda pasando la vida actual normalizada
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        IsUpdating = true;
        float curHP = health.transform.localScale.x;

        float changeAmt = curHP - newHP;

        if (curHP < newHP)
        {//animacion heal
            changeAmt = newHP - curHP;
            while (newHP - curHP > Mathf.Epsilon)
            {
                curHP += changeAmt * Time.deltaTime;
                SetHP(curHP);
                //Para la corrutina y la continua en la siguiente frame
                yield return null;
            }
        }
        else
        {
            //Anima la reducción de la barra de vida desde su inicio hasta que la diferencia de el objetivo y le progreso
            //actual es infinitesimal
            while (curHP - newHP > Mathf.Epsilon)
            {
                curHP -= changeAmt * Time.deltaTime;
                SetHP(curHP);
                //Para la corrutina y la continua en la siguiente frame
                yield return null;
            }
        }
        IsUpdating = false;



    }
}
