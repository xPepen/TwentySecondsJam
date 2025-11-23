using System;

namespace Game.Scripts.Core.TimerManager
{
    public class TimerHandle
    {
        private readonly Guid _handle = Guid.NewGuid();

        public bool IsValid()
        {
            return _handle != Guid.Empty;
        }

        public Guid GetHandle()
        {
            return _handle;
        }
    }

    public class TimerData
    {
        public TimerHandle Handle;
        public float Counter;
        public float Duration;
        public bool Looping;
        public Action Callback;
        public bool IsDirty;
    }
}