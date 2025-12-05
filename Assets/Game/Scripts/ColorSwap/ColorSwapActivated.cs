using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ColorSwapEntity))]
public class ColorSwapActivated : MonoBehaviour
{

    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;
    public bool ActivateOnLight = true;

    private ColorSwapEntity entity;

    void Start()
    {
        entity = GetComponent<ColorSwapEntity>();

        entity.OnEntityColorSwapped.AddListener(ActivateDesactivate);
    }

    private void ActivateDesactivate(bool isLight)
    {
        // If ActivateOnLight == true, then isLight means activate.
        // If ActivateOnLight == false, then NOT isLight means activate.
        bool shouldActivate = (ActivateOnLight && isLight) || (!ActivateOnLight && !isLight);

        if (shouldActivate)
            Activate();
        else
            Desactivate();
    }

    private void Activate()
    {
        OnActivate?.Invoke();
    }

    private void Desactivate()
    {
        OnDeactivate?.Invoke();
    }

}
