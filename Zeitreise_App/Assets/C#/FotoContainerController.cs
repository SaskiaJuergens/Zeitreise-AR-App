using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class FotoContainerController : MonoBehaviour
{
    public GameObject PortraitPrefab; // Das Portrait-Prefab
    public Transform FotoContainer; // Hier k�nntest du einen leeren GameObject-Container f�r die Fotos verwenden
    public Button plusButton;
    public Button minusButton;

    private int currentIndex = 0;
    private GameObject[] personSprites;

    void Start()
    {
        // Initialisiere die Portraits
        personSprites = new GameObject[25];
        for (int i = 0; i < 25; i++)
        {
            personSprites[i] = Instantiate(PortraitPrefab, FotoContainer);
            personSprites[i].SetActive(false);
        }

        // Zeige das erste Portrait an
        ZeigeAktuellesFoto();

        // Register button click events
        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);
    }

    // Plus-Button-Methode
    public void OnPlusButtonClicked()
    {
        currentIndex = (currentIndex + 1) % personSprites.Length;
        ZeigeAktuellesFoto();
    }

    // Minus-Button-Methode
    public void OnMinusButtonClicked()
    {
        currentIndex = (currentIndex - 1 + personSprites.Length) % personSprites.Length;
        ZeigeAktuellesFoto();
    }

    void ZeigeAktuellesFoto()
    {
        // Deaktiviere alle Portraits
        foreach (var portrait in personSprites)
        {
            portrait.SetActive(false);
        }

        // Aktiviere das aktuelle Portrait
        personSprites[currentIndex].SetActive(true);
    }
}
