using System.Collections.Generic;
using Game.Scripts.Runtime.PlayerCore;

namespace Game.Scripts.Core.Classes
{
    public static class GameInstance
    {
        private static readonly List<PlayerController > ListOfPlayers = new ();
        
        public static void RegisterPlayer(PlayerController player)
        {
            if (!ListOfPlayers.Contains(player))
            {
                ListOfPlayers.Add(player);
            }
        }
        
        public static void UnregisterPlayer(PlayerController player)
        {
            if (ListOfPlayers.Contains(player))
            {
                ListOfPlayers.Remove(player);
            }
        }
        
        public static List<PlayerController> GetAllPlayers()
        {
            return ListOfPlayers;
        }
        
        public static PlayerController GetPlayerByIndex(int index)
        {
            if (index >= 0 && index < ListOfPlayers.Count)
            {
                return ListOfPlayers[index];
            }
            return null;
        }
    }
}