#include <Arduino.h>
#if defined(ESP32)
  #include <WiFi.h>
#elif defined(ESP8266)
  #include <ESP8266WiFi.h>
#endif
#include <Firebase_ESP_Client.h>

//Provide the token generation process info.
#include "addons/TokenHelper.h"
//Provide the RTDB payload printing info and other helper functions.
#include "addons/RTDBHelper.h"

////////// button /////////////////
#define b1 18 
#define b2 17 
unsigned long timebutton1;  
unsigned long timebutton2;  
int intervalbutton1 = 200; 
int intervalbutton2 = 200;  
///////////////////////////////////////////////////



// Insert your network credentials
#define WIFI_SSID "Skyforce"
#define WIFI_PASSWORD "d0senmilch"

// Insert Firebase project API Key
#define API_KEY "AIzaSyBtlnAf-7PwTm1u7D53NnggdMMlQV3fwpY"

// Insert RTDB URLefine the RTDB URL */
#define DATABASE_URL "https://zeituhr-40996-default-rtdb.europe-west1.firebasedatabase.app/" 

//Define Firebase Data object
FirebaseData fbdo;

FirebaseAuth auth;
FirebaseConfig config;

unsigned long sendDataPrevMillis = 0;
int count = 0;
bool signupOK = false;

void setup(){
  pinMode(b1, INPUT); 
  pinMode(b2, INPUT); 
  USBSerial.begin(9600); 
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  USBSerial.print("Connecting to Wi-Fi");
  while (WiFi.status() != WL_CONNECTED){
    Serial.print(".");
    delay(300);
  }
  USBSerial.println();
  USBSerial.print("Connected with IP: ");
  USBSerial.println(WiFi.localIP());
  USBSerial.println();

  /* Assign the api key (required) */
  config.api_key = API_KEY;

  /* Assign the RTDB URL (required) */
  config.database_url = DATABASE_URL;

  /* Sign up */
  if (Firebase.signUp(&config, &auth, "", "")){
    USBSerial.println("ok");
    signupOK = true;
  }
  else{
    USBSerial.printf("%s\n", config.signer.signupError.message.c_str());
  }

  /* Assign the callback function for the long running token generation task */
  config.token_status_callback = tokenStatusCallback; //see addons/TokenHelper.h
  
  Firebase.begin(&config, &auth);
  Firebase.reconnectWiFi(true);
}

void loop(){
 bool data18 = digitalRead(b1); 
  bool data17 = digitalRead(b2); 
 
  // if (millis() - timebutton1 >=intervalbutton1 && data18 == 1) { 
  //   USBSerial.println("18 taste"); 
  //   timebutton1 = millis(); 
  // } 
 
  // if (millis() - timebutton2 >=intervalbutton2 && data17 == 1) { 
  //   USBSerial.println("17 taste"); 
  //    timebutton2 = millis(); 
 
  // } 


  Firebase.RTDB.setInt(&fbdo, "test/Minute", 1);






  // if (data18 == 1 && Firebase.ready() && signupOK && (millis() - sendDataPrevMillis >300 || sendDataPrevMillis == 0 && data17==0)){
  //   sendDataPrevMillis = millis();
  //     //Write an Int number on the database path test/int
  //   if (Firebase.RTDB.setInt(&fbdo, "test/int", mm)){
  //     USBSerial.println("PASSED");
  //     USBSerial.println("PATH: " + fbdo.dataPath());
  //     USBSerial.println("TYPE: " + fbdo.dataType());
  //   }
  //   else {
  //     USBSerial.println("FAILED");
  //     USBSerial.println("REASON: " + fbdo.errorReason());
  //   }
  //   count++;
    
  //   // Write an int number on the database path test/float
  //  if (Firebase.RTDB.setInt(&fbdo, "test/int", int(random(1,4)))){
  //     USBSerial.println("PASSED");
  //     USBSerial.println("PATH: " + fbdo.dataPath());
  //     USBSerial.println("TYPE: " + fbdo.dataType());
  //   }
  //   else {
  //     USBSerial.println("FAILED");
  //     USBSerial.println("REASON: " + fbdo.errorReason());
  //   }
  // }


}