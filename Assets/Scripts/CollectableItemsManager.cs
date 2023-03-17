using UnityEngine;

public class CollectableItemsManager : MonoBehaviour
{
    //Anzahl an Sekunden, die beim Aufnehmen des Objekts hinzugef�gt werden sollen
    public float timeToAdd;

    //kollidiert mit Spieler, dann..
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        //Sucht DrugEffectManager-Skript im GameObject des Players
        DrugEffectManager drugEffectManager = collision.gameObject.GetComponent<DrugEffectManager>();

        //DrugEffectManager-Skript vorhanden? -> dann wird Timer erh�ht
        if (drugEffectManager != null)
        {
            drugEffectManager.IncreaseTimer(timeToAdd);

            //Zerst�rung!1!1!! krass
            Destroy(gameObject);
        }
    }
}
