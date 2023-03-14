using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour {
    //~ inspector (private)
    [SerializeField][Tooltip("Name of the next scene to load")] private string sceneName;

    //~ unity methods (private)
    private void OnCollisionEnter(Collision other) {
        //~ if collision with the player loads the given scene
        if(other.gameObject == GameManager.Instance.Player.gameObject) GameManager.LoadScene(sceneName);
    }
}
