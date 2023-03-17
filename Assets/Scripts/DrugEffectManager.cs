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
        timer -= Time.fixedDeltaTime;
        float t = timer / maxTime;
        if (t > 0)
        {
            blendSlider.BlendEnvironment(t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2); // Sqr EaseInOut Timer
            //blendSlider.BlendEnvironment(t); //Linear Timer
            // Aktualisiert Anzeige Timer (Slider)
            timerSlider.value = timer / maxTime;
        }
        //Timer abgelaufen?
        if (timer <= 0)
        {
            //WICHTIG: Szene vom GameOverScreen richtig eintippen -> hard gecodet
            SceneManager.LoadScene("GameOverTest");
        }
    }

    //wird aufgerufen vom Skript CollectableItemManager-Skript, timeToAdd wird dabei vom Skript mitgegeben
    public void IncreaseTimer(float timeToAdd)
    {
        // Erhöht den Timer um die gegebene Menge
        
        timer += timeToAdd;
        

        // Stellt sicher, dass der Timer nicht größer als die maximale Zeit ist
        timer = Mathf.Min(timer, maxTime);

        // Aktualisiert die Anzeige des Sliders
        
        timerSlider.value = timer;
        
        //timerSlider.maxValue = maxTime;
       

        //DIESES SKRIPT MUSS AN DEN PLAYER, NICHT AN DER DRUGEFFECTBAR SELBST!!!!!!
    }
}

