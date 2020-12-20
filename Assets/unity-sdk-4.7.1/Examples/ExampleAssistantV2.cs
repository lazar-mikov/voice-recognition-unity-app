/**
* (C) Copyright IBM Corp. 2018, 2020.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/
#pragma warning disable 0649

using System.Collections;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System;

namespace IBM.Watson.Examples
{
    public class ExampleAssistantV2 : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string iamApikey;
        [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/assistant/api\"")]
        [SerializeField]
        private string serviceUrl;
        [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
        [SerializeField]
        private string versionDate;
        [Tooltip("The assistantId to run the example.")]
        [SerializeField]
        private string assistantId;
        #endregion

        private AssistantService service;
        public bool createSessionTested = false;
        public bool messageTested0 = false;
        public bool messageTested1 = false;
        private bool messageTested2 = false;
        private bool messageTested3 = false;
        private bool messageTested4 = false;
        private bool deleteSessionTested = false;
        private string sessionId;
        public string message;
        public string msg;
        public GameObject contentDisplayObject;               // Text gameobject where all the conversation is shown
        public InputField input;                              // InputField gameobject wher user types their message
        public string sender;
        public GameObject userBubble;                         // reference to user chat bubble prefab
        public GameObject botBubble;                          // reference to bot chat bubble prefab

        private const int messagePadding = 15;                // space between chat bubbles 
        private int allMessagesHeight = messagePadding; // int to keep track of where next message should be rendered
        public bool increaseContentObjectHeight;        // bool to check if content object height should be increased


        
        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
            Invoke("SubmitName", 1);
            string message = "Hi Shelly, what's up?";
        UpdateDisplay("user", message, "text");
            string data = "On my way home now. Clyde broke a \n whole carton of eggs so I had to run \n to the supermarket :( \n  Are you coming to the party later?";
            UpdateDisplay("bot", data, "text");
        }

        private IEnumerator CreateService()
        {
            if (string.IsNullOrEmpty(iamApikey))
            {
                throw new IBMException("Plesae provide IAM ApiKey for the service.");
            }

            //  Create credential and instantiate service
            IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

            //  Wait for tokendata
            while (!authenticator.CanAuthenticate())
                yield return null;

            service = new AssistantService(versionDate, authenticator);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                service.SetServiceUrl(serviceUrl);
            }

            Runnable.Run(Examples());
        }

        private IEnumerator Examples()
        {
            Log.Debug("ExampleAssistantV2.RunExample()", "Attempting to CreateSession");
            service.CreateSession(OnCreateSession, assistantId);

            while (!createSessionTested)
            {
                yield return null;
            }





           
                    input.onEndEdit.AddListener(SubmitName); //gets the results from the input field

                    void SubmitName(string arg0)
                    {
                arg0 = "Hi Shelly, what's up?";
                        Debug.Log(arg0);
                        message = input.text;
                        input.text = "";
                        sender = "user";



                        var input1 = new MessageInput()  //sends the input string to the watson assistant service 
                        {
                            Text = message,
                            Options = new MessageInputOptions()
                            {
                                ReturnContext = true
                            }
                        };

                        service.Message(OnMessage1, assistantId, sessionId, input: input1); // sends call to watson assistant
                        UpdateDisplay("user", message, "text"); // calls to update the sender message bubble
                    }

                
            

            Log.Debug("ExampleAssistantV2.Examples()", "Assistant examples complete.");

        }

        private void OnMessage1(DetailedResponse<MessageResponse> response, IBMError error) //results from message
        {
            Log.Debug("ExampleAssistantV2.OnMessage1()", "response: {0}", response.Result.Output.Generic[0].Text);

            messageTested1 = true;

            string data = "";
            data = response.Result.Output.Generic[0].Text.ToString();



           


            if (data != null)
            {
                UpdateDisplay("bot", data, "text");   // calls to update the bot message bubble
            }

        }

        private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
        {
            Log.Debug("ExampleAssistantV2.OnCreateSession()", "Session: {0}", response.Result.SessionId);
            sessionId = response.Result.SessionId;
            createSessionTested = true;

            StartCoroutine(RefreshChatBubblePosition());
        }

        /// <summary>
        /// This method is used to update the display panel with the user's and bot's messages.
        /// </summary>
        /// <param name="sender">The one who wrote this message</param>
        /// <param name="message">The message</param>
        public void UpdateDisplay(string sender, string message, string messageType)
        {
            // Create chat bubble and add components
            GameObject chatBubbleChild = CreateChatBubble(sender);
            AddChatComponent(chatBubbleChild, message, messageType);

            // Set chat bubble position
            StartCoroutine(SetChatBubblePosition(chatBubbleChild.transform.parent.GetComponent<RectTransform>(), sender));
        }



        /// <summary>
        /// Coroutine to set the position of the chat bubble inside the contentDisplayObject.
        /// </summary>
        /// <param name="chatBubblePos">RectTransform of chat bubble</param>
        /// <param name="sender">Sender who sent the message</param>
        private IEnumerator SetChatBubblePosition(RectTransform chatBubblePos, string sender)
        {
            // Wait for end of frame before calculating UI transform
            yield return new WaitForEndOfFrame();

            // get horizontal position based on sender
            int horizontalPos = 0;
            if (sender == "user")
            {
                horizontalPos =  -22;
            }
            else if (sender == "bot")
            {
                horizontalPos =  0;
            }

            // set the vertical position of chat bubble
            allMessagesHeight +=  (int)chatBubblePos.sizeDelta.y;
            chatBubblePos.anchoredPosition3D = new Vector3(horizontalPos, -allMessagesHeight, 0);

            if (allMessagesHeight > 340)
            {
                // update contentDisplayObject hieght
                RectTransform contentRect = contentDisplayObject.GetComponent<RectTransform>();
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, allMessagesHeight + messagePadding);
                contentDisplayObject.transform.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
            }
        }


        /// <summary>
        /// Coroutine to update chat bubble positions based on their size.
        /// </summary>
        public IEnumerator RefreshChatBubblePosition()
        {
            // Wait for end of frame before calculating UI transform
            yield return new WaitForEndOfFrame();

            // refresh position of all gameobjects based on size
            int localAllMessagesHeight = messagePadding;
            foreach (RectTransform chatBubbleRect in contentDisplayObject.GetComponent<RectTransform>())
            {
                if (chatBubbleRect.sizeDelta.y < 35)
                {
                    localAllMessagesHeight += 2 + messagePadding;
                }
                else
                {
                    localAllMessagesHeight += (int)chatBubbleRect.sizeDelta.y + messagePadding;
                }
                chatBubbleRect.anchoredPosition3D =
                        new Vector3(chatBubbleRect.anchoredPosition3D.x, -localAllMessagesHeight, 0);
            }

            // Update global message Height variable
            allMessagesHeight = localAllMessagesHeight;
            if (allMessagesHeight > 340)
            {
                // update contentDisplayObject hieght
                RectTransform contentRect = contentDisplayObject.GetComponent<RectTransform>();
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, allMessagesHeight + messagePadding);
                contentDisplayObject.transform.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
            }
        }


        /// <summary>
        /// This method creates chat bubbles from prefabs and sets their positions.
        /// </summary>
        /// <param name="sender">The sender of message for which bubble is rendered</param>
        /// <returns>Reference to empty gameobject on which message components can be added</returns>
        private GameObject CreateChatBubble(string sender)
        {
            GameObject chat = null;
            if (sender == "user")
            {
                // Create user chat bubble from prefabs and set it's position
                chat = Instantiate(userBubble);
                chat.transform.SetParent(contentDisplayObject.transform, false);
            }
            else if (sender == "bot")
            {
                // Create bot chat bubble from prefabs and set it's position
                chat = Instantiate(botBubble);
                chat.transform.SetParent(contentDisplayObject.transform, false);
            }


            // Add content size fitter
            ContentSizeFitter chatSize = chat.AddComponent<ContentSizeFitter>();
            chatSize.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            chatSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Add vertical layout group
            VerticalLayoutGroup verticalLayout = chat.AddComponent<VerticalLayoutGroup>();
            if (sender == "user")
            {
                verticalLayout.padding = new RectOffset(10, 20, 5, 5);
            }
            else if (sender == "bot")
            {
                verticalLayout.padding = new RectOffset(20, 10, 5, 5);
            }
            verticalLayout.childAlignment = TextAnchor.MiddleCenter;

            // Return empty gameobject on which chat components will be added
            return chat.transform.GetChild(0).gameObject;
        }

        private void AddChatComponent(GameObject chatBubbleObject, string message, string messageType)
        {
            switch (messageType)
            {
                case "text":
                    // Create and init Text component
                    Text chatMessage = chatBubbleObject.AddComponent<Text>();
                    // add font as it is none at times when creating text component from script                             
                    chatMessage.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    chatMessage.fontSize = 12;
                    chatMessage.alignment = TextAnchor.MiddleLeft;
                    chatMessage.text = message;
                    Debug.Log(chatMessage);
                    break;

            }
        }

    }


    
   

    /// <summary>
    /// This class is used to serialize users message into a json
    /// object which can be sent over http request to the bot.
    /// </summary>
   
  

}
