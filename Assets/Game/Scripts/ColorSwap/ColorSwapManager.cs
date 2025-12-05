using UnityEngine;
using UnityEngine.Events;

public class ColorSwapManager : MonoBehaviour
{
    public static ColorSwapManager Instance { get; private set; } // Static reference to the single instance

    public UnityEvent OnColorSwap;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this; // Assign this instance as the singleton
            DontDestroyOnLoad(gameObject); // Optional: Persist across scene loads
        }
    }

    public void SwapColor()
    {
        OnColorSwap.Invoke();
    }
}
