using UnityEngine;

public class CollectableItemsManager : MonoBehaviour
{
    //Anzahl an Sekunden, die beim Aufnehmen des Objekts hinzugef�gt werden sollen
    public float timeToAdd;

    //kollidiert mit Spieler, dann..
    private void OnTriggerEnter(Collider other)
    {
        
        //Sucht DrugEffectManager-Skript im GameObject des Players
        DrugEffectManager drugEffectManager = other.gameObject.GetComponent<DrugEffectManager>();
        Debug.Log("DrugEffectManager found: " + drugEffectManager);

        //DrugEffectManager-Skript vorhanden? -> dann wird Timer erh�ht
        if (drugEffectManager != null)
        {
            drugEffectManager.IncreaseTimer(timeToAdd);

            //Zerst�rung!1!1!! krass
            Destroy(gameObject);
        }
    }
}
