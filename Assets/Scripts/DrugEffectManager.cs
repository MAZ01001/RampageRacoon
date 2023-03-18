using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DrugEffectManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Anzahl Sekunden, die der Timer abzählt")]
    private float maxTime;

    [SerializeField]
    [Tooltip("Slider zur Anzeige des Timers")]
    private Slider timerSlider;

    [SerializeField]
    [Tooltip("Blend% zwischen den welten")]
    private BlendSlider blendSlider;

    private float timer;
    private float blendVal;         //sqr ease of timer
    private float addTime = 0f;
    public static DrugEffectManager Instance;

    private void Awake() {
        DrugEffectManager.Instance = this;
    }

    //Start Timer  Druffianzeige
    private void Start()
    {
        // Setzt den Slider zurück auf voll
        timerSlider.value = 1;

        // Setzt Timer auf max Zeit
        timer = maxTime;


    }

    //Aktualisierung der Timer-Anzeige
    private void FixedUpdate()
    {
        if(this.addTime != 0f){
            timer = Mathf.Clamp(this.timer + this.addTime, 0f, this.maxTime);
            this.addTime = 0;
        }
        timer -= Time.fixedDeltaTime;
        float t = timer / maxTime;
        if (t > 0)
        {
            blendVal = (t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2);
            blendSlider.BlendEnvironment(blendVal); // Sqr EaseInOut Timer
            //blendSlider.BlendEnvironment(t); //Linear Timer
            // Aktualisiert Anzeige Timer (Slider)
            timerSlider.value = timer / maxTime;
        }else GameManager.Instance.GameOver();
    }
    public float GetEnvironmentEffect()
    {
        return blendVal;
    }

    //wird aufgerufen vom Skript CollectableItemManager-Skript, timeToAdd wird dabei vom Skript mitgegeben
    public void IncreaseTimer(float timeToAdd) => this.addTime += timeToAdd;
}

