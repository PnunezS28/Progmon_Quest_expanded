using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFollowScript : MonoBehaviour
{
    [SerializeField] GameObject player;

    public static PlayerCameraFollowScript Instance;
    private void Awake()
    {
        if (Instance == null){
            Instance = this;
        }
        Debug.Log("PlayerCameraFollowScript Awake");
    }


    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        //si está permitido seguir al jugador seguirlo
        if(GameController.Instance.GameState != GameStateEnum.BATTLE && player!=null)
        {
            float playerx = player.transform.position.x;
            float playery = player.transform.position.y;

            this.gameObject.transform.position = new Vector3(playerx, playery, this.transform.position.z);
        }
    }
}
