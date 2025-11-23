using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core.Utility
{
    public interface ISocket
    {
        public Transform GetSocketTransform(string socketName);

        public Dictionary<string, Transform> GetSocketList()
        {
            return null;
        }
        public void SetSocket(string socketName, Transform socketTransform);
    }
}