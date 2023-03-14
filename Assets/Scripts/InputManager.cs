using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    /*
        fullscreen - (F11)   for switching fullscreen mode
        menu -       (ESC)   for showing the in-game menu
        move -       (WASD)  for moving on the XY-plane
        shoot -      (Space) for shooting
    */

    //~ public
    /** <summary> if set true handles player inputs </summary> */ public bool active  { get; private set; } = true;
    /** <summary> player 2d movement input </summary> */          public Vector2 move { get; private set; } = Vector2.zero;
    /** <summary> player shoot button </summary> */               public bool shoot { get; private set; } = false;

    //~ public methods
    /// <summary> if set true handles player inputs - resets values on false </summary>
    /// <param name="state"> the new active state of the input manager </param>
    public void SetActive(bool state) {
        if(!(this.active = state)){
            //~ reset input values when deactivated (since these won't get updated and should be cleared)
            this.move = Vector2.zero;
        }
    }

    //~ input events (private)
    //~ player inputs (values - update on value change)
    private void OnMove       (InputValue value) => this.move   = (this.active ? value.Get<Vector2>().normalized : Vector2.zero);
    private void OnShoot      (InputValue value) => this.shoot  = (this.active ? value.isPressed : false);
    //~ system inputs (button - called once on key down)
    private void OnMenu       (InputValue value) => GameManager.Instance.Menu();
    private void OnFullScreen (InputValue value) => GameManager.ToggleFullScreen();
}
