using UnityEngine;

public class Exit : MonoBehaviour
{
    private bool fading = false;
    public bool canExit = false;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(canExit && collider.gameObject.layer == LayerMask.NameToLayer("Player") && !fading)
        {
            collider.GetComponent<PlayerMovement>().enabled = false;
            GameObject gameManager = GameObject.Find("GameManager");
            gameManager.GetComponent<FadeManager>().StartFade(() =>
            {
                gameManager.GetComponent<BoardCreator>().CreateLevel();
            });
        }
    }
}
