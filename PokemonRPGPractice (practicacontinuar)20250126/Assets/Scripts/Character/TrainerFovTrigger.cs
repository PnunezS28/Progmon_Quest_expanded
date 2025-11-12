using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFovTrigger : MonoBehaviour, IPlayerTrigerable
{
    [SerializeField] TrainerController controllerAsociado;
    [SerializeField] public float yOffset = -2.8f;
    [SerializeField] public float ylatteralOffset = -1.8f;
    [SerializeField] public float xOffsetLateral = -.2f;
    public void OnPlayerTriggered(PlayerController player)
    {
        GameController.Instance.onEnterTrainerView(controllerAsociado.TrainerId);
    }
}
