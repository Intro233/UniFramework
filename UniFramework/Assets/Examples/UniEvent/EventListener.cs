using UniFramework.Event;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    private void Start()
    {
        //监听事件
        UniEvent.AddListener<GameOverMessage>(arg =>
        {
            if (arg is GameOverMessage gameOver)
            {
                Debug.Log($"{gameOver.playerName}:{gameOver.core}分;当前帧{Time.frameCount}");
            }
        });
    }
}

public class GameOverMessage : IEventMessage
{
    public int core;
    public string playerName;

    public GameOverMessage(int core, string playerName)
    {
        this.core = core;
        this.playerName = playerName;
    }
}