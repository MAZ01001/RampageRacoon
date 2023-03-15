using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DrugEffectManager : MonoBehaviour
{
    // Anzahl Sekunden, die der Timer abzählt
    public float maxTime;

    // Slider zur Anzeige des Timers
    public Slider timerSlider;

    // Countdown-Timer
    private float timer;

    //Start Timer  Druffianzeige
    private void Start()
    {
        // Setzt Wert des Sliders auf max Anzahl Sekunden
        timerSlider.maxValue = maxTime;
        timerSlider.value = maxTime;

        // Setzt Timer auf max Zeit
        timer = maxTime;
    }

    //Aktualisierung der Timer-Anzeige
    private void Update()
    {
        
        timer -= Time.deltaTime;

        // Aktualisiert Anzeige Timer (Slider)
        timerSlider.value = timer;

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
        Debug.Log("IncreaseTimer called with timeToAdd: " + timeToAdd);
        
        // Erhöht den Timer um die gegebene Menge
        Debug.Log("timer before: " + timer);
        timer += timeToAdd;
        Debug.Log("timer after: " + timer);

        // Stellt sicher, dass der Timer nicht größer als die maximale Zeit ist
        timer = Mathf.Min(timer, maxTime);

        // Aktualisiert die Anzeige des Sliders
        Debug.Log("Slider value before: " + timerSlider.value);
        timerSlider.value = timer;
        Debug.Log("Slider value after: " + timerSlider.value);
        timerSlider.maxValue = maxTime;

        //DIESES SKRIPT MUSS AN DEN PLAYER, NICHT AN DER DRUGEFFECTBAR SELBST!!!!!!
    }
}

