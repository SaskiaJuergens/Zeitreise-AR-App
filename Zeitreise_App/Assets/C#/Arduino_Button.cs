using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.IO.Ports;


public class Arduino_Button : MonoBehaviour {

    //SerialPort port = new SerialPort("COM3", 9600);

    void Start() {
        //port.Open();
        //port.ReadTimeout = 5000;
    }

    // Update is called once per frame
    void Update() {

        //if (port.IsOpen)
        //{
        //    try
        //    {
        //        button_pressed(port.ReadByte());
        //    }
        //    catch (System.Exception)
        //    {
        //        throw;
        //    }

        //}
        //port.Close();
    }

    void button_pressed(int mm)
    {
      //private int currentMinute = mm; //arduino mm wert speichern
      
        if(mm >= 0 && mm< 10)
        {
            //show telefon 1
        }
        if (mm >= 10 && mm < 20)
        {
            //show telefon 2
        }
        if (mm >= 20 && mm < 30)
        {
            //show telefon 3
        }
        if (mm >= 30 && mm < 40)
        {
            //show telefon 4
        }
        if (mm >= 40 && mm < 50)
        {
            //show telefon 5
        }
        if (mm >= 50 && mm < 60)
        {
            //show telefon 6
        }
    }
}
