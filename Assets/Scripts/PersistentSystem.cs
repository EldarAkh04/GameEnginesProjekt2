using UnityEngine;

public class PersistentSystem : MonoBehaviour
{
    private void Awake()
    {
        // Das sorgt dafür, dass das 'System'-Objekt UND alle seine Kinder 
        // (GameManager, UI, etc.) den Szenenwechsel überleben.
        DontDestroyOnLoad(gameObject);
    }
}