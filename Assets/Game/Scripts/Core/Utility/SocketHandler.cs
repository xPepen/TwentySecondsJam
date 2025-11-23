using UnityEngine;

namespace Game.Scripts.Core.Utility
{
    public class SocketHandler : MonoBehaviour
    {
        [SerializeField] private string socketName;

        private void Start()
        {
            ISocket socket = GetComponentInParent<ISocket>();
            if (socket == null)
            {
                Debug.Log("No Socket found in parent hierarchy.");
                return;
            }

            socket.SetSocket(socketName, transform);
        }
    }
}