using UnityEngine;
using UnityEngine.UI;

public class FotoContainerController : MonoBehaviour
{
    public Renderer backgroundRenderer;
    public Renderer personRenderer;
    public Material[] backgroundMaterials;  // Array von Hintergrundmaterialien
    public Material[] personMaterials;      // Array von Personenschichtmaterialien

    private int currentIndex = 0;

    // UI Buttons for controlling the materials
    public Button plusButton;
    public Button minusButton;

    void Start()
    {
        // Setze das Start-Hintergrundmaterial
        backgroundRenderer.material = backgroundMaterials[currentIndex];

        // Setze das Start-Personenmaterial
        personRenderer.material = personMaterials[currentIndex];

        // Register button click events
        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);
    }

    // Methode zum Wechseln des Hintergrundmaterials oder Personenmaterialien
    public void ChangeMaterial(int increment)
    {
        currentIndex = (currentIndex + increment + Mathf.Max(backgroundMaterials.Length, personMaterials.Length)) % Mathf.Max(backgroundMaterials.Length, personMaterials.Length);

        if (currentIndex < backgroundMaterials.Length)
        {
            backgroundRenderer.material = backgroundMaterials[currentIndex];
        }

        if (currentIndex < personMaterials.Length)
        {
            personRenderer.material = personMaterials[currentIndex];
        }
    }

    // Plus-Button-Methode
    public void OnPlusButtonClicked()
    {
        ChangeMaterial(1);
    }

    // Minus-Button-Methode
    public void OnMinusButtonClicked()
    {
        ChangeMaterial(-1);
    }
}
