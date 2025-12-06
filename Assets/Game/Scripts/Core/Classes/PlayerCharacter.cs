using System.Collections.Generic;
using Game.Scripts.Core.Utility;
using Game.Scripts.Runtime.GameRule;
using Game.Scripts.Runtime.PlayerCore;
using Game.Scripts.Runtime.Weapon.Base;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.Core.Classes
{
    public class PlayerCharacter : Character, ISocket, IWeaponCarried
    {
        public MovementComponent MovementComponent { get; private set; }
        public InputComponent InputComponent { get; private set; }

        private Transform _socket;
        private WeaponBase _Weapon;

        //Cached Tags
        private GameplayTag JumpTag;
        private GameplayTag DashTag;
        private GameplayTag ColorAbility;
        private GameplayTag LevelTag;

        private PlayerController PlayerController;

        protected override void OnAwake()
        {
            base.OnAwake();

            MovementComponent = GetComponent<MovementComponent>();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            //Cached internal frequently used tags 
            JumpTag = GameplayTag.RequestTag("Ability.Jump");
            DashTag = GameplayTag.RequestTag("Ability.Dash");
            ColorAbility = GameplayTag.RequestTag("Ability.ChangeColor");
            
            if (Animator)
            {
                Animator.SetInteger(WalkingState, _currentState);
            }

            if (GameplayAbilityComponent)
            {
                if (MovementComponent)
                {
                    MovementComponent.MoveSpeed =
                        GlobalGameplayTags.GetVariable(GlobalGameplayTags.Speed, GameplayAbilityComponent);

                    MovementComponent.MaxMoveSpeed =
                        GlobalGameplayTags.GetVariable(GlobalGameplayTags.MaxSpeed, GameplayAbilityComponent);
                }

                LevelTag = GameplayTag.CreateTag("Stat.Level");
                GameplayAbilityComponent.AddVariable(LevelTag, 1);
            }
        }

        public override void OnPossessed(Controller newController)
        {
            base.OnPossessed(newController);

            PlayerController = (newController as PlayerController);
            if (!PlayerController)
            {
                Debug.LogError("PlayerCharacter possessed by non PlayerController");
                return;
            }

            InputComponent = PlayerController.InputComponent;

            if (InputComponent)
            {
                InputComponent.OnJumpAction += OnCharacterJump;
                InputComponent.OnDashAction += OnCharacterDash;
                InputComponent.OnFireAction += OnCharacterAttack;
                InputComponent.OnMoveAction += OnCharacterMove;
                InputComponent.OnChangeColorAction += OnChangeColor;
            }
        }

        void OnChangeColor()
        {
            if (GameplayAbilityComponent)
            {
                GameplayAbilityComponent.PerformAbility(ColorAbility);
            }
        }
        
        public override void UnPossessed()
        {
            if (InputComponent)
            {
                InputComponent.OnJumpAction -= OnCharacterJump;
                InputComponent.OnDashAction -= OnCharacterDash;
                InputComponent.OnFireAction -= OnCharacterAttack;
                InputComponent.OnMoveAction -= OnCharacterMove;
                InputComponent.OnChangeColorAction -= OnChangeColor;
            }

            base.UnPossessed();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (Animator)
            {
                Animator.SetFloat(SpeedParam, InputComponent.InputMovement.sqrMagnitude, 0.01f, Time.fixedDeltaTime);
                Animator.SetFloat(VerticalVelo, MovementComponent.CharacterController.velocity.y);
                Animator.SetBool(IsOnGround, IsGrounded());
            }
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (InputComponent)
            {
                if (MovementComponent)
                {
                    if (IsMovingOnGround() && InputComponent.InputMovement.magnitude > 0.1f)
                    {
                        GameplayAbilityComponent.ApplyFeedBack(GameplayTag.RequestTag("Action.Move"),
                            transform.position);
                    }

                    MovementComponent.MoveWithCam(InputComponent.InputMovement);
                    MovementComponent.Look(InputComponent.InputLook);

                    MovementComponent.ApplyMovement();
                }
            }
        }

        private void OnCharacterMove(Vector3 dir)
        {
            // if (IsMovingOnGround() && dir.magnitude > 0.1f)
            // {
            //     GameplayAbilityComponent.ApplyFeedBack(GameplayTag.RequestTag("Action.Move"), transform.position);
            // }

            // else
            // {
            //     GameplayAbilityComponent.StopFeedBack(GameplayTag.RequestTag("Action.Move"));
            // }
        }

        protected override void OnTriggerEnterVirtual(Collider other)
        {
            base.OnTriggerEnterVirtual(other);

            var obj = other.gameObject;
            if (obj)
            {
                //later on would be nice to have a input to equip the weapon
                if (obj.IsA<WeaponBase>(out var weaponBase))
                {
                    SetWeapon(weaponBase);
                }
            }
        }

        protected override void OnDestroyVirtual()
        {
            base.OnDestroyVirtual();
            UnPossessed();
        }

        // --- Getter  ---
        public CharacterController GetCharacterController()
        {
            if (MovementComponent)
            {
                return MovementComponent.CharacterController;
            }

            return null;
        }

        public bool IsMovingOnGround()
        {
            return IsGrounded() && InputComponent && InputComponent.InputMovement.sqrMagnitude > 0f;
        }

        public bool IsGrounded()
        {
            if (MovementComponent)
            {
                return MovementComponent.IsGrounded;
            }

            return false;
        }

        public override float GetHeight()
        {
            CharacterController characterController = GetCharacterController();
            if (characterController)
            {
                return characterController.height;
            }

            return base.GetHeight();
        }

        public WeaponBase GetWeapon()
        {
            return _Weapon ? _Weapon : null;
        }

        public void SetWeapon(WeaponBase newWeapon)
        {
            if (_Weapon)
            {
                _Weapon.OnDetach();
            }

            _Weapon = newWeapon; // @ WeaponStats is dump need to rework this...
            _Weapon.Init(this, GameplayAbilityComponent, new WeaponStats(10, 10));

            Transform socket = GetSocketTransform("WeaponSocket");
            _Weapon.OnAttach(socket);
        }

        public float GetExperience()
        {
            if (GameplayAbilityComponent)
            {
                return GameplayAbilityComponent.GetVariable(GlobalGameplayTags.Experience);
            }

            return -1f;
        }

        public float GetMaxExperience()
        {
            if (GameplayAbilityComponent)
            {
                return GameplayAbilityComponent.GetVariable(GlobalGameplayTags.MaxExperience);
            }

            return -1f;
        }

        //do to need to add a curve to know what multiplier to use for newLevel
        //for when we recieve xp amount and when we level up so 2 curves
        public void AddExperience(float amount)
        {
            if (GameplayAbilityComponent)
            {
                float currentExp = GetExperience();


                if (currentExp + amount >= GetMaxExperience())
                {
                    //level up logic can go here
                    var overExp = currentExp + amount - GetMaxExperience();
                    var newMaxExp = GetMaxExperience() * 2f;
                    GameplayAbilityComponent.SetVariableOverride(GlobalGameplayTags.Experience, overExp);
                    GameplayAbilityComponent.SetVariableOverride(GlobalGameplayTags.MaxExperience, newMaxExp);
                    GameplayAbilityComponent.SetVariableAmountChange(LevelTag, (int)1);

                    print("Level Up! ," + "Current XP :" + GetExperience() + " New Max Exp :" + GetMaxExperience());
                    return;
                }

                GameplayAbilityComponent.SetVariableAmountChange(GlobalGameplayTags.Experience, amount);
            }
        }

        public int GetCurrentLevel()
        {
            if (GameplayAbilityComponent)
            {
                return (int)GameplayAbilityComponent.GetVariable(LevelTag).Value;
            }

            return (int)-1;
        }

        // --- Input Callbacks ---

        protected void OnCharacterJump()
        {
            if (GameplayAbilityComponent)
            {
                GameplayAbilityComponent.PerformAbility(JumpTag);
                Animator.SetTrigger(JumpingTrigger);
            }
        }

        protected void OnCharacterDash()
        {
            if (GameplayAbilityComponent)
            {
                GameplayAbilityComponent.PerformAbility(DashTag);
            }
        }

        protected void OnCharacterAttack(bool isPressed)
        {
            if ( !_Weapon)
            {
                return;
            }

            if (MovementComponent)
            {
                if (GameplayAbilityComponent)
                {
                    _Weapon.Attack();
                }
            }
        }

        protected override void OnHealthChanged(float oldValue, float newValue)
        {
            base.OnHealthChanged(oldValue, newValue);

            var maxHealthVar = GetMaxHealth();
            int NewState = -1;
            float healthPercent = Mathf.Clamp01(newValue / maxHealthVar);
            if (healthPercent > 0.66f)
            {
                //happy state
                NewState = HappyState;
            }
            else if (healthPercent <= 0.66f && healthPercent > 0.33f)
            {
                //normal state
                NewState = NormalState;
            }
            else if (healthPercent <= 0.33f)
            {
                //angry state
                NewState = AngryState;
            }

            if (_currentState != NewState)
            {
                _currentState = NewState;
                Animator.SetInteger(WalkingState, NewState);
            }
        }

        public override void OnDeadFinish()
        {
            //todo :add anim and add event to notify when anim finished
        }


        public Transform GetSocketTransform(string socketName)
        {
            return _sockets[socketName];
            ;
        }

        public void SetSocket(string socketName, Transform socketTransform)
        {
            _sockets.TryAdd(socketName, socketTransform);
        }

        private static readonly string SpeedParam = "Blend";
        private static readonly string VerticalVelo = "VerticalVelo";
        private static readonly string JumpingTrigger = "Jumping";
        private static readonly string WalkingState = "WalkingState";
        private static readonly string IsOnGround = "IsGrounded";

        //Animator WalkingStates triggers variables 
        private static readonly int HappyState = 0;
        private static readonly int NormalState = 1;
        private static readonly int AngryState = 2;
        private int _currentState = 0;

        private Dictionary<string, Transform> _sockets = new();
    }
}