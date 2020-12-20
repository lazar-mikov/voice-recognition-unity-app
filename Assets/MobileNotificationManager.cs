using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;
public class MobileNotificationManager : MonoBehaviour
{

    public AndroidNotificationChannel defaultNotificationChannel;
    
    private int identifier;
    public GameObject videoplayer;


    public void Enable()
    {
        videoplayer.SetActive(true);
    }

    private void Start()
    {



        defaultNotificationChannel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Description = "For Generic notifications",
            Importance = Importance.Default,
        };

        AndroidNotificationCenter.RegisterNotificationChannel(defaultNotificationChannel);


        AndroidNotification notification = new AndroidNotification()
        {
            Title = "Test Notification!",
            Text = "This is a test notification!",
            SmallIcon = "default",
            LargeIcon = "default",
            FireTime = System.DateTime.Now.AddSeconds(5),
        };

       

        identifier = AndroidNotificationCenter.SendNotification(notification, "default_channel");


        AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler = delegate (AndroidNotificationIntentData data)
        {
            var msg = "Notification received : " + data.Id + "\n";
            msg += "\n Notification received: ";
            msg += "\n .Title: " + data.Notification.Title;
            msg += "\n .Body: " + data.Notification.Text;
            msg += "\n .Channel: " + data.Channel;
            Debug.Log(msg);
        };

        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;

        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

        if (notificationIntentData != null)
        {
            Debug.Log("App was opened with notification!");
        }

    }


           




}
