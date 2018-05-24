using UnityEngine;
using System.Collections;

public class damageTextController : MonoBehaviour
{
    private static popUpText popupText;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("Canvas");
        if (!popupText)
            popupText = Resources.Load<popUpText>("UI/Canvas/damageTextParent");
    }

    public static void CreateFloatingText(string text, Transform location)
    {
        popUpText instance = Instantiate(popupText);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(new Vector2(location.position.x + Random.Range(-.2f, .2f), location.position.y + Random.Range(-.2f, .2f)));

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPosition;
        instance.SetText(text);
    }
}