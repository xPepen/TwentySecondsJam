using UnityEngine;
using UnityEngine.Events;

public class ColorSwapEntity : MonoBehaviour
{
    public Material[] lightMat;
    public Material[] darkMat;

    public bool usingLight = true;

    public UnityEvent<bool> OnEntityColorSwapped;

    private Renderer rend;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorSwapManager.Instance.OnColorSwap.AddListener(DoSwap);
        rend = GetComponent<Renderer>();
        if (usingLight)
            ApplyLight();
        else
            ApplyDark();
    }

    private void DoSwap()
    {
        usingLight = !usingLight;
        if (usingLight)
            ApplyLight();
        else
            ApplyDark();

        OnEntityColorSwapped.Invoke(usingLight);
    }

    private void ApplyLight()
    {
        if(rend != null)
            rend.materials = lightMat;
    }

    private void ApplyDark()
    {
        if (rend != null)
            rend.materials = darkMat;
    }
}
