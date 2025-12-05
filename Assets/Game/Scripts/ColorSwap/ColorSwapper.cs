using UnityEngine;

public class ColorSwapper : MonoBehaviour
{
    private ColorSwapManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = ColorSwapManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            manager.SwapColor();
        }
    }
}
