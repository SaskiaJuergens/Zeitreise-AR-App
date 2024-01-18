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
    private List<GameObject> personSprites = new List<GameObject>();

    void Start()
    {
        //GameObject portraitInstance = Instantiate(PortraitPrefab, transform);
        //portraitInstance.SetActive(false);
        //personSprites.Add(portraitInstance);

        // Initialisiere die Telefone
        GameObject portraitInstance = Instantiate(PortraitPrefab, transform);
        Debug.Log(portraitInstance.transform.childCount);
        //personSprites = new GameObject[25];
        for (int i = 0; i < 25; i++)
        {
            Debug.Log("inizalisiere Foto" + i);
            personSprites.Add(portraitInstance.transform.GetChild(i).gameObject);
            personSprites[i].SetActive(false);
           // personSprites.Add(portraitInstance);
        }


        // Initialize the personSprites list by cloning the PortraitPrefab
        //for (int i = 0; i < 25; i++)
        //{
        //    GameObject portraitInstance = Instantiate(PortraitPrefab, transform);
        //    portraitInstance.SetActive(false);
        //    personSprites.Add(portraitInstance);
        //}

        // Zeige das erste Portrait an
        ZeigeAktuellesFoto();

        Debug.Log(plusButton);
        Debug.Log(plusButton.onClick);
        // Register button click events
        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);
    }

    // Plus-Button-Methode
    public void OnPlusButtonClicked()
    {
        currentIndex = (currentIndex + 1) % personSprites.Count;
        ZeigeAktuellesFoto();
    }

    // Minus-Button-Methode
    public void OnMinusButtonClicked()
    {
        currentIndex = (currentIndex - 1 + personSprites.Count) % personSprites.Count;
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
