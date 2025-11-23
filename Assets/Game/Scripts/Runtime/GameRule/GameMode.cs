using Game.Scripts.Core.Classes;
using Game.Scripts.Core.HUD;
using Game.Scripts.Runtime.PlayerCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Runtime.GameRule
{

    public class GameMode : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private Transform PlayerStart;

        [Header("Player Gameplay Core")] [SerializeField]
        private GameObject PlayerPrefab;

        [SerializeField] private GameObject PlayerControllerPrefab;
        [SerializeField] private GameObject HUDPrefab;

        [SerializeField] private string FisrtScene;

        public PlayerCharacter PlayerCharacter { get; private set; }
        public PlayerController PlayerController { get; private set; }
        public HUDCore HUD { get; private set; }

        private void Awake()
        {
            if (!this.GetInternalComponent<PlayerCharacter>(PlayerPrefab))
            {
                Debug.LogError("PlayerCharacter doesn't have a PlayerCharacter");
                return;
            }

            if (!this.GetInternalComponent<HUDCore>(HUDPrefab))
            {
                Debug.LogError("PlayerCharacter doesn't have a PlayerCharacter");
                return;
            }
        }

        private void Start()
        {
            SceneManager.LoadScene(FisrtScene, LoadSceneMode.Additive);
            
            //todo : add init function to have a better initialization flow

            //Setup Player Controller
            PlayerController = InstantiateObject<PlayerController>(PlayerControllerPrefab, Vector3.zero, Quaternion.identity, transform);
            PlayerController.OnInitialize();

            //Spawn Player Character
            PlayerCharacter = InstantiateObject<PlayerCharacter>(PlayerPrefab,PlayerStart.position, Quaternion.identity, null);
            PlayerCharacter.OnInitialize();

            PlayerController.PossesPlayer(PlayerCharacter);

            //Setup HUD creation
            GameObject hudObject = new GameObject("HUD_ROOT");
            HUD =InstantiateObject<HUDCore>(HUDPrefab, Vector3.zero, Quaternion.identity, hudObject.transform);

            PlayerController.SetHUD(HUD);
            HUD.OnInitialize(PlayerController);
            
            
        }

        public virtual T InstantiateObject<T>(GameObject obj, Vector3 location, Quaternion rotation = default,
            Transform root = null) where T : Component
        {
            if (!obj)
            {
                return null;
            }

            GameObject objInstance = Instantiate(obj, location, rotation, root ? root : null);
            T component = this.GetInternalComponent<T>(objInstance);

            return component ? component : null;
        }
    }
}