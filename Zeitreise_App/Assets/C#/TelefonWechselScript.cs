using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using Unity.VisualScripting;

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
        GameObject telefonInstance = Instantiate(telefonPrefab.gameObject, transform);
        Debug.Log(telefonInstance.transform.childCount);

        telefone = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            Debug.Log("inizalisiere Telefon" + i);
            telefone[i] = telefonInstance.transform.GetChild(i).gameObject;
            telefone[i].SetActive(false);
        }

        // Zeige das erste Telefon an
        ZeigeAktuellesTelefon();



        // Setze die Button-Handler
        plusButton = GameObject.FindGameObjectWithTag("plusButton").GetComponent<Button>();
        minusButton = GameObject.FindGameObjectWithTag("minusButton").GetComponent<Button>();

        Debug.Log(plusButton);
        Debug.Log(plusButton.onClick);

        plusButton.onClick.AddListener(WechsleTelefonPlus);
        minusButton.onClick.AddListener(WechsleTelefonMinus);

    }

    void WechsleTelefonPlus()
    {
        aktuellesTelefonIndex = (aktuellesTelefonIndex + 1) % telefone.Length;
        ZeigeAktuellesTelefon();
    }

    void WechsleTelefonMinus()
    {
        aktuellesTelefonIndex = (aktuellesTelefonIndex - 1 + telefone.Length) % telefone.Length;
        ZeigeAktuellesTelefon();
    }

    void ZeigeAktuellesTelefon()
    {
        Debug.Log("Zeige aktuelles Telefon 1");
        // Deaktiviere alle Telefone
        foreach (var telefon in telefone)
        {
            telefon.SetActive(false);
        }

        // Aktiviere das aktuelle Telefon
        telefone[aktuellesTelefonIndex].SetActive(true);
        Debug.Log("Zeige aktuelles Telefon 2");
    }
}