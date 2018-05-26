using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResponseModel : MonoBehaviour
{
    public PlayerResponseNode Node;
    public Text ResponseText;

    public void Init(PlayerResponseNode node, string text)
    {
        Node = node;
        ResponseText.text = text;
    }

    public void ChooseResponse()
    {
        DialogHandler.Instance.ChooseResponse(Node);
    }
}