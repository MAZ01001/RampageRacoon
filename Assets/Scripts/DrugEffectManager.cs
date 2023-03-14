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
}

