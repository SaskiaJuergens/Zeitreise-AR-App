using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.IO.Ports;

public class ArduinoTest_Cube : MonoBehaviour{
    SerialPort sp = new SerialPort("COM3", 9600); // COM3 durch den tatsächlichen COM-Port ersetzen

    void Start(){
        try
        {
            sp.Open();
            sp.ReadTimeout = 16;
            Debug.Log("Verbindung zum COM-Port erfolgreich hergestellt");
        }
        catch (Exception ex)
        {
            Debug.LogError("Fehler beim Öffnen des COM-Ports: " + ex.Message);
        }
    }


    void Update(){
        if (sp.IsOpen){
            try{
                string serialInput = sp.ReadLine();
                // Hier kannst du mit den empfangenen Daten arbeiten
                Debug.Log(serialInput);
            }
            catch (System.Exception){
                // Fehlerbehandlung für die serielle Kommunikation
            }
        }
    }

    void OnDestroy(){
        if (sp.IsOpen)
            sp.Close();
    }
}
