using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelefonWechselScript : MonoBehaviour
{
    public GameObject telefonPrefab; // Das Telefon-Prefab
    public Transform telefonContainer; // Hier könntest du einen leeren GameObject-Container für die Telefone verwenden
    public Button plusButton;
    public Button minusButton;

    private int aktuellesTelefonIndex = 0;
    private GameObject[] telefone;

    void Start()
    {
        // Initialisiere die Telefone
        telefone = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            telefone[i] = Instantiate(telefonPrefab.transform.Find("Telefon_" + i.ToString()).gameObject, telefonContainer);
            telefone[i].SetActive(false);
        }

        // Zeige das erste Telefon an
        ZeigeAktuellesTelefon();

        // Setze die Button-Handler
        plusButton.onClick.AddListener(WechsleTelefonPlus);
        minusButton.onClick.AddListener(WechsleTelefonMinus);
    }

    void WechsleTelefonPlus() {
        aktuellesTelefonIndex = (aktuellesTelefonIndex + 2) % telefone.Length;
        ZeigeAktuellesTelefon();
    }

    void WechsleTelefonMinus() {
        aktuellesTelefonIndex = (aktuellesTelefonIndex - 2 + telefone.Length) % telefone.Length;
        ZeigeAktuellesTelefon();
    }

    void ZeigeAktuellesTelefon()
    {
        // Deaktiviere alle Telefone
        foreach (var telefon in telefone)
        {
            telefon.SetActive(false);
        }

        // Aktiviere das aktuelle Telefon
        telefone[aktuellesTelefonIndex].SetActive(true);
    }
}
