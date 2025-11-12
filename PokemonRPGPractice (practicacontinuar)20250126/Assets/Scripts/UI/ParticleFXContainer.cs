using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFXContainer : MonoBehaviour
{
    [SerializeField]
    float duration = 1.0f;

    float time = 0.0f;

    //Esta clase sirve de contenedor físico para los efectos de particulas que se instancian como prefabs
    //limpiará su contenedor después de pasar el tiempo

    // Update is called once per frame
    void Update()
    {
        time+=Time.deltaTime;
        if (time >= duration)
        {
            Destroy(this.gameObject);
        }
    }
}
