using System;
using UniFramework.Event;
using UnityEngine;

public class EventSender : MonoBehaviour
{
    private void Start()
    {
        UniEvent.Initalize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //延迟发送事件
            GameOverMessage gameOverMessageDelay = new GameOverMessage(66, "张三");
            UniEvent.PostMessage(gameOverMessageDelay);

            //发送事件
            GameOverMessage gameOverMessage = new GameOverMessage(90, "李华");
            UniEvent.SendMessage(gameOverMessage);
        }
    }

    private void OnDestroy()
    {
        UniEvent.Destroy();
    }
}