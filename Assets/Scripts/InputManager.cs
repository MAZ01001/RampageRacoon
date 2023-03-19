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
    /** <summary> player 2d movement input </summary> */          public Vector2 move { get; private set; } = Vector2.zero;
    /** <summary> player shoot button </summary> */               public bool shoot { get; private set; } = false;

    //~ input events (private)
    //~ player inputs (values - update on value change)
    private void OnMove       (InputValue value) => this.move = value.Get<Vector2>().normalized;
    private void OnShoot      (InputValue value) => this.shoot = value.isPressed;
    //~ system inputs (button - called once on key down)
    private void OnMenu       (InputValue value) => GameManager.Instance.Menu();
    private void OnFullScreen (InputValue value) => GameManager.ToggleFullScreen();
}
