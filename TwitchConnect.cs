using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;
using System.IO;

public class TwitchConnect : MonoBehaviour
{
    public Spawner spawner;
    public UnityEvent<string, string> OnMessageReceived;
    TcpClient Twitch;
    StreamReader Reader;
    StreamWriter Writer;

    const string URL = "irc.chat.twitch.tv";
    const int PORT = 6667;

    string Username = "justinfan98124";
    string Password = "Kappa123";
    string Channel = "leyohen";

    // Kappa123
    // justinfan98124
    float PingCounter = 0;


    private void ConnectToTwitch(string channelToConnect)
    {

        Twitch = new TcpClient(URL, PORT);
        Reader = new StreamReader(Twitch.GetStream());
        Writer = new StreamWriter(Twitch.GetStream());
        
        Writer.WriteLine("PASS oauth:" + Password+ "\r");
        Writer.WriteLine("NICK " + Username.ToLower());
        Writer.WriteLine("JOIN #" + channelToConnect.ToLower());
        print("Connection to Twitch to channel: " + channelToConnect);
        Writer.WriteLine("CAP REQ :twitch.tv/tags");
        Writer.Flush();
    }

    private void Awake()
    {
        ConnectToTwitch(PlayerPrefs.GetString("username").Trim().ToLower().Replace("\u200b", ""));
    }


    void Update()
    {
     
        PingCounter += Time.deltaTime;
        if(PingCounter >= 60)
        {
            Writer.WriteLine("PING :tmi.twitch.tv");
            Writer.Flush();
            PingCounter = 0;
        }

        if(!Twitch.Connected)
        {
            ConnectToTwitch(PlayerPrefs.GetString("username").Trim().ToLower().Replace("\u200b", ""));
        }

        if(Twitch.Available > 0)
        {
            var message = Reader.ReadLine();
            // print(message);

            if(message.Contains("PRIVMSG"))
            {

            // @badge-info=subscriber/52;badges=broadcaster/1,subscriber/3000,glhf-pledge/1;client-nonce=da244b53124f4a63dc76ee0540592880;color=#9BA3E0;display-name=LeYohen;emotes=;first-msg=0;flags=;id=af491b55-4c72-404c-8443-fd6873189c88;mod=0;returning-chatter=0;room-id=129964618;subscriber=1;tmi-sent-ts=1666088345297;turbo=0;user-id=129964618;user-type= :leyohen!leyohen@leyohen.tmi.twitch.tv PRIVMSG #leyohen :test

            string userID = message.Substring(message.IndexOf("user-id="));
            userID = userID.Substring(8, userID.IndexOf(";") - 8); // 8 is the length of "user-id="

            string chatMessage = message.Substring(message.IndexOf("PRIVMSG"));
            chatMessage = chatMessage.Substring(chatMessage.IndexOf(":") + 1);
            
            string chatter = message.Substring(message.IndexOf("display-name="));
            chatter = chatter.Substring(13, chatter.IndexOf(";") - 13); // 13 is the length of "display-name="
            
            Debug.Log(chatMessage);

                if(chatMessage == "!spawn")
                {
                    spawner.SpawnObject(int.Parse(userID), chatter);
                }
            
            }   

        }

    }

}