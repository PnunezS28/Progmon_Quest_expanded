using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailAnimator : MonoBehaviour
{
    [SerializeField] Vector2[] Points;
    [SerializeField] float StepLength=1.0f;
    [SerializeField] float StartDelay = 0.0f;
    [SerializeField] float StepDelay = 0.0f;
    [SerializeField] float MoveSpeed = 1.0f;

    int targetPointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (Points != null)
        {
            StartCoroutine(AnimateTrail());
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    IEnumerator AnimateTrail()
    {
        
        //Paso1. recivir punto objetivo
        if (StartDelay > 0.0f)
        {
            yield return new WaitForSeconds(StartDelay);
        }
        //paso2. marchar hasta punto objetivo a la velocidad indicada

        while (targetPointIndex < Points.Length)
        {
            //usar rect transform
            RectTransform transformRect = GetComponent<RectTransform>();
            Vector3 nextPoint = (Points[targetPointIndex] * StepLength);

            Vector3 targetPoint = transformRect.localPosition + nextPoint;

            while (Vector3.Distance(transformRect.localPosition, targetPoint) > float.Epsilon)
            {
                float moveStep = MoveSpeed * Time.deltaTime;
                transformRect.localPosition = Vector3.MoveTowards(transformRect.localPosition, targetPoint, moveStep);
                yield return new WaitForSeconds(.001f);

            }
            if (StepDelay > 0.0f)
            {
                yield return new WaitForSeconds(StepDelay);
            }
            //paso3. pasar al siguiente punto y repetir si hay siguiente punto
            targetPointIndex++;
        }

    }
}
