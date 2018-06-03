using UnityEngine;

public class MobileTouchUI : MonoBehaviour
{
    public static MobileTouchUI Instance;
    public bool Show;

    void Awake()
    {
        Instance = this;
    }
}