using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightHideShow : MonoBehaviour
{
    public GameObject crying;
    public GameObject fight;
    public GameObject fightnotificaion;
    public GameObject photo;
    public GameObject photonotification;
    public GameObject message;
    public GameObject messagenotification;
    public GameObject ringphone;
    public GameObject ringphonenotification;

    public bool mIsAppLeft;

    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                // Quit the application
                fight.SetActive(false);
                photo.SetActive(false);
                message.SetActive(false);
            }
        }
    }
    public void Enablecrying()
    {
        crying.SetActive(true);
        if (crying.activeInHierarchy == true)
        {
            Invoke("Disablecrying", 58);
            Invoke(" closering", 1);
        }
    }

    public void Disablecrying()
    {
        crying.SetActive(false);
    }

    public void closering()
    {
        ringphone.SetActive(false);
    }
    public void closeringnotification()
    {
        ringphonenotification.SetActive(false);
    }

    public void Enablefight()
    {
        fight.SetActive(true);
        if (fight.activeInHierarchy == true)
        {
            Invoke("Disablefight", 30);
            Invoke("Disablefightnotificaion", 1);
        }
    }

    public void Disablefight()
    {
        fight.SetActive(false);
    }

    public void Enablefightnotificaion()
    {
        fightnotificaion.SetActive(true);
    }

    public void Disablefightnotificaion()
    {
        fightnotificaion.SetActive(false);
    }


    public void EnablePhoto()
    {
        photo.SetActive(true);

        if (photo.activeInHierarchy == true)
        {
            Invoke("DisablePhoto", 7);
            Invoke("DisablePhotonotification", 1);
        }
    }

    public void DisablePhoto()
    {
        photo.SetActive(false);
    }

    public void DisablePhotonotification()
    {
        photonotification.SetActive(false);
    }

    public void mute()
    {
        AudioListener.pause = !AudioListener.pause;
    }

    public void Enablemessage()
    {
        message.SetActive(true);
        if (message.activeInHierarchy == true)
        {
            Invoke("Disablemessage", 13);
            Invoke("Disablemessagenotification", 1);
        }
    }

    public void Disablemessage()
    {
        message.SetActive(false);
    }

        public void Disablemessagenotification()
    {
        messagenotification.SetActive(false);
    }

    
        
    
}

