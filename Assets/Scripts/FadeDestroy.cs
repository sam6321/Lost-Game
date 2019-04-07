
using UnityEngine;

public class FadeDestroy : MonoBehaviour
{
    public Vector2 Direction { set; get; }
    public float FadeTime { set; get; }

    void Update()
    {
        if(Time.time >= FadeTime)
        {
            Destroy(gameObject);
        }
    }
}
