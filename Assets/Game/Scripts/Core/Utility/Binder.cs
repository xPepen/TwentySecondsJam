using UnityEngine;

namespace Game.Scripts.Core.Utility
{
    [System.Serializable]
    public class Binder<X, Y>
    {
        [field: SerializeField] public X ItemA { get; set; }
        [field: SerializeField] public Y ItemB { get; set; }
    }

    [System.Serializable]
    public class Binder<X, Y, Z> : Binder<X, Y>
    {
        [field: SerializeField] public Z ItemC { get; set; }
    }

    [System.Serializable]
    public class Binder<X, Y, Z, W> : Binder<X, Y, Z>
    {
        [field: SerializeField] public W ItemD { get; set; }
    }

    [System.Serializable]
    public class Binder<X, Y, Z, W, A> : Binder<X, Y, Z, W>
    {
        [field: SerializeField] public A ItemE { get; set; }
    }
}