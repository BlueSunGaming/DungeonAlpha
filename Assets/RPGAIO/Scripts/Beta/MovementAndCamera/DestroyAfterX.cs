using UnityEngine;

public class DestroyAfterX : MonoBehaviour
{
    public float DestroyAfterSeconds = 1.0f;

	void Start () {
	    Destroy(gameObject,DestroyAfterSeconds);
	}
}
