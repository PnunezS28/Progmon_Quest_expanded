using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePrefabTag : MonoBehaviour
{
    [SerializeField] int sceneId=0;

    public int SceneId => sceneId;
}
