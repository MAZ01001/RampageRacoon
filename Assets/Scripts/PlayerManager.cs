using UnityEngine;

public class PlayerManager : MonoBehaviour {
    //~ private
    private InputManager inputManager;

    //~ unity methods (private)
    private void Start() {
        //~ get components
        this.inputManager = this.GetComponent<InputManager>();
    }
    private void FixedUpdate() {
        // TODO player moves
        // this.inputManager.move;
        // TODO player shoots
        // this.inputManager.shoot;
    }
}
