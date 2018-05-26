using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using UnityEngine.AI;

namespace Assets.Scripts.Beta.NewImplementation
{
    public class RPGController : MonoBehaviour, IRPGController
    {
        private static IRPGController PlayerInstance;

        public GameObject characterModel;
        public CameraMode cameraMode;

        [SerializeField]
        private RPGControllerState _state;

        public RPGControllerState State { get { return _state; } set { _state = value; } }
        public IRPGAnimation RPGAnimation { get; set; }
        public IRPGCombat RPGCombat { get; set; }
        public RPGPatrol RPGPatrol { get; set; }
        public RPGFollow RPGFollow { get; set; }
        public PetMono PetMono { get; set; }
        public RPGAction CurrentAction { get { return _currentAction; } }
        public RPGActionQueue CurrentQueue { get { return _curQueue; } }
        public BaseCharacterMono CharacterMono { get; private set; }
        public bool Running { get { return _running; } }

        private RPGActionQueue _curQueue;
        private RPGAction _currentAction;

        private RPG_Camera _rpgCamera;
        private BaseCharacter _character;
        private LegacyAnimation _legacyAnimation;
        private NavMeshObstacle _navMeshObstacle;

        Transform _target;
        private bool _stopAIJump;
        private Vector3 _playerDirWorld;
        private Vector3 _rotation = Vector3.zero;
        private bool _tryCancelAction;
        private bool _cancelRepeatQueue;
        private bool _handlingRepeatQueue;
        private bool _running;

        private Animator _characterAnimator;
        private Animation _characterAnimation;

        private Dictionary<Vector3, int> _approachPositions = new Dictionary<Vector3, int>();
        private Vector3 _oldPosition = Vector3.zero;
        private float _moveRight;
        private float _moveForward;
        private bool _handlingCharacterDeath = false;
        private bool _waitingToRevive = false;
        private bool _resumePlayerControl = false;
        private bool _alreadyRetreated = false;
        private bool _regenHealthToMax = false;
        private float _timeInAir = 0;
        private float _timeHoldingClickToMove = 0;
        private float _impactDeteriorateSpeed;
        private bool _showingCastArea;
        private Skill _skillToCast;
        private GameObject _castAreaGameObject;
        private Projector _castAreaProjector;

        private const float JumpThreshold = 0.5f;
        private const float FallingThreshold = 1.0f;
        private const float ImpactSpeed = 3.0f;
        private const float PullSpeed = 3.0f;
        private const float TurnSpeed = 500.0f;
        private const float TooFarFromSpawnDistance = 200.0f;
        private const float TimeToExitCombat = 5.0f;
        private const float TimeBeforeFallDamage = 1.5f;
        private const float ProjectorBaseFieldOfView = 0.6f;
        private const float ProjectorBaseAspectRatio = 1.0f;
        private const float SpawnSkillCastAreaWidth = 2.0f;

        private float previousMobileAngle = 0f;

        public GameObject CharacterModel { get { return characterModel; } }
        public Animation Animation { get { return _characterAnimation; } }
        public Animator Animator { get { return _characterAnimator; } }

        private float AggroRadius
        {
            get
            {
                var cc = Character as CombatCharacter;
                if(cc != null && cc.OverrideAggroRadius)
                {
                    return cc.OverrideAggroRadiusValue;
                }

                return Rm_RPGHandler.Instance.Combat.AggroRadius;
            }
        }


        public BaseCharacter Character
        {
            get { return _character ?? (_character = GetComponent<BaseCharacterMono>().Character); }
        }
        public LegacyAnimation LegacyAnimation
        {
            get { return _legacyAnimation ?? (_legacyAnimation = GetLegacyAnimation()); }
        }
        public Transform Target
        {
            get { return _target; }
            set
            {
                ChangeTarget(value);

                _target = value;
            }   
        }

        public CharacterController CharacterController { get; private set; }
        public NavMeshAgent NavMeshAgent { get; private set; }

        public bool MovingForward { get { return _moveForward > 0; } }
        public bool IsPlayerControlled { get { return (RPGController) PlayerInstance == this; } }
        public bool PlayerCanControl { get { return !ControlledByAI && !Interacting; } }
        public bool ControlledByAI { get; private set; }
        [SerializeField]
        public bool HandlingActions { get; private set; }
        [SerializeField]
        public bool Interacting { get; set; }
        public bool RetreatingToSpawn { get; set; }
        public bool IsCastingSkill { get; set; }
        public Vector3 SpawnPosition { get; set; }

        public bool IsGrounded { get; private set; }
        public bool TargetReached { get; private set; }
        public bool AutoAttack { get; set; }
        public bool InCombat { get; set; }
        public float TimeOutOfCombat { get; set; }
        public bool IsPlayerCharacter { get; set; }

        public float MoveSpeed { 
            get
            {

                if(PetMono != null)
                {
                    return GetObject.PlayerController.MoveSpeed*0.75f;
                }

                var asvt = Rm_RPGHandler.Instance.ASVT;
                var items = Rm_RPGHandler.Instance.Items;
                var baseSpeed = Character.CharacterType == CharacterType.Player ? asvt.BaseMovementSpeed : asvt.BaseNpcMovementSpeed;
                if(asvt.UseStatForMovementSpeed)
                {
                    baseSpeed *= Character.GetStatByID(asvt.StatForMovementID).TotalValue;
                }

                if(items.InventoryUsesWeightSystem && items.AllowOverMax && Character.CharacterType == CharacterType.Player)
                {
                    var player = (PlayerCharacter) Character;
                    if(player.Inventory.OverMaxWeight)
                    {
                        baseSpeed *= items.MoveSpeedOverMax;
                    }
                }


                return baseSpeed;
            } 
        }     
        public float Gravity { get { return 9.8f; } }
        public float JumpHeight { get { return Rm_RPGHandler.Instance.ASVT.JumpHeight; } }
        public float MouseSensitivity { get { return 2.0f; } }
        private float AttackInterval { get { return 1 / Character.AttackSpeed; } }

        public Vector3 Impact { get; private set; }

        private void PullTowards(Vector3 direction, float distance, float speed)
        {
            AddImpact(direction, distance, speed);
        }

        public void PullTo(Vector3 targetPos, float distance = -1)
        {
            var dir = targetPos - transform.position;
            dir.y = transform.position.y;
            
            var dist = Math.Abs(distance - -1) < 0.01f ? Vector3.Distance(transform.position, targetPos) - NavMeshAgent.radius : distance;
            PullTowards(dir, dist, PullSpeed);
        }

        public void AddImpact(Direction direction, float wantedDistance)
        {
            //todo: implement or remove other directions
            Vector3 dir = Vector3.zero;
            switch(direction)
            {
                case Direction.BackWithUp:
                    dir = transform.TransformDirection(Vector3.back * 5 + Vector3.up * 2);
                    dir /= 5;
                    AudioPlayer.Instance.Play(LegacyAnimation.KnockBackAnim.Sound, AudioType.SoundFX, transform.position, transform);
                    break;
                case Direction.Up:
                    dir = transform.TransformDirection(Vector3.up);
                    AudioPlayer.Instance.Play(LegacyAnimation.KnockUpAnim.Sound, AudioType.SoundFX, transform.position, transform);
                    break;
                case Direction.Down:
                    break;
                case Direction.Left:
                    break;
                case Direction.Right:
                    break;
                case Direction.Back:
                    dir = transform.TransformDirection(Vector3.back);
                    AudioPlayer.Instance.Play(LegacyAnimation.KnockBackAnim.Sound, AudioType.SoundFX, transform.position, transform);
                    break;
                case Direction.Forward:
                    break;
            }

            AddImpact(dir, wantedDistance, ImpactSpeed, true);
        }

        private void AddImpact(Vector3 direction, float wantedDistance, float speed, bool counterGravity = false)
        {
            var dir = direction;
            if(dir != Vector3.zero)
            {
                var distance = wantedDistance * speed;
                if (dir.y < 0) dir.y = -dir.y;
                if (counterGravity) distance += Gravity;
                Impact += dir.normalized * distance;
                _impactDeteriorateSpeed = speed;
                WaitForImpactToEnd();
            }
        }

        private void WaitForImpactToEnd()
        {
            ForceStopHandlingActions();

            EnableCharacterController();

            var queue = RPGActionQueue.Create();
            queue.Add(RPGActionFactory.WaitToLand()).WithDefaultAnimations();
            BeginActionQueue(queue);
        }

        public bool BeginActionQueue(RPGActionQueue queue)
        {
            if (!HandlingActions)
            {
                if(queue.Actions.Count > 0)
                {
                    _playerDirWorld = Vector3.zero;

                    if(queue.Actions[0].Type == RPGActionType.RepeatQueue)
                    {
                        StartCoroutine("HandleRepeatQueue", queue);
                    }
                    else
                    {
                        StartCoroutine("HandleActionQueue", queue);
                    }
                    return true;
                }
                
            }

            return false;
        }

        public void Pause()
        {
            ControlledByAI = true;

            //EnableNavMeshAgent();
        }

        public void Resume()
        {
            ControlledByAI = false;

            EnableCharacterController();
        }

        public void ToggleAI(bool onOff)
        {
            if(onOff)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        public Vector3 GetApproachPosition(Vector3 approachPosition, float stopRange)
        {
            //todo: implement stoprange
            //note: maybe should be approach direction? or take direction * stoprange or something :D

            if(_approachPositions.Count == 0)
            {
                SetApproachPositions();
            }

            var minSlots = _approachPositions.Min(p => p.Value); //e.g. 0

            var closest = _approachPositions.OrderBy(p => Vector3.Distance(approachPosition, p.Key)).ToArray();

            var pos = closest.First(p => p.Value == minSlots);
            _approachPositions[pos.Key] = _approachPositions[pos.Key] + 1;
            return pos.Key;
        }

        public void SetPlayerControl(GameObject combatant)
        {
            PlayerInstance = combatant.GetComponent<RPGController>();
            GetObject.RPGCamera.SwitchToCharacter(combatant.transform);
            _playerDirWorld = Vector3.zero;
            ForceStopHandlingActions();
        }

        private void ChangeTarget(Transform newTarget)
        {
            if (!IsPlayerControlled || !Rm_RPGHandler.Instance.Combat.ShowSelected) return;

            if (newTarget != Target)
            {
                if(Target != null)
                {
                    var currentTargetMono = Target.GetComponent<BaseCharacterMono>();
                    if (currentTargetMono != null)  
                    {
                        var lockHandler = currentTargetMono.GetComponent<TargetLockHandler>();
                        if(lockHandler != null)
                            lockHandler.ChangeState(TargetLockState.Unselected);
                    }
                }

                if(newTarget != null)
                {
                    var newTargetMono = newTarget.GetComponent<BaseCharacterMono>();
                    if (newTargetMono != null)
                    {
                        var lockHandler = newTargetMono.GetComponent<TargetLockHandler>();
                        if (lockHandler != null)
                        {
                            var isInCombat = newTargetMono.Controller.InCombat && newTargetMono.Controller.Target == transform;
                            lockHandler.ChangeState(isInCombat ? TargetLockState.InCombat : TargetLockState.Selected);
                        }
                    }
                }
            }
            else
            {
                if (Target != null)
                {
                    var currentTargetMono = Target.GetComponent<BaseCharacterMono>();
                    if (currentTargetMono != null)
                    {
                        var lockHandler = currentTargetMono.GetComponent<TargetLockHandler>();
                        if (lockHandler != null)
                        {
                            var isInCombat = currentTargetMono.Controller.InCombat && currentTargetMono.Controller.Target == transform;
                            lockHandler.ChangeState(isInCombat ? TargetLockState.InCombat : TargetLockState.Selected);
                        }
                            
                    }
                }
            }
        }

        public void ForceStopHandlingActions()
        {
            
            var s = new Stopwatch();
            //todo: impact cancels this can cause problem maybe??
            StopCoroutine("HandleActionQueue");
            _tryCancelAction = false;
            _curQueue = null;
            _currentAction = null;
            if(NavMeshAgent.enabled && NavMeshAgent.isOnNavMesh)
            {
                NavMeshAgent.destination = transform.position;
                NavMeshAgent.updatePosition = false;
                NavMeshAgent.isStopped = true;
            }

            if (_handlingRepeatQueue)
            {
                _cancelRepeatQueue = true;    
            }

            if (_resumePlayerControl)
            {
                Resume();
            }
            //RPGAnimation.CrossfadeAnimation(InCombat ? LegacyAnimation.IdleAnim : LegacyAnimation.CombatIdleAnim);
            HandlingActions = false;

            IsCastingSkill = false;

            if(!IsPlayerControlled)
            {
                EnableNavMeshAgent();
            }

            if(s.ElapsedMilliseconds > 5)
                Debug.Log("Force stop:" + s.ElapsedMilliseconds.ToString());
        }

        private void TryCancelAction()
        {
            if (Impact != Vector3.zero) return;

            Debug.Log("___action canceled____");
            AutoAttack = false;
            _tryCancelAction = true;
        }

        private IEnumerator HandleRepeatQueue(RPGActionQueue queue)
        {
            _handlingRepeatQueue = true;
            var repeatAction = queue.Actions[0];
            while(!_cancelRepeatQueue)
            {
                var repeatqueue = (RPGActionQueue)repeatAction.Params["Queue"];
                yield return StartCoroutine(HandleActionQueue(repeatqueue));
            }
            _cancelRepeatQueue = false;
            _handlingRepeatQueue = false;
        }

        private IEnumerator HandleActionQueue(RPGActionQueue queue)
        {
            _curQueue = queue;
            HandlingActions = true;
            //var startTime = Time.time;
            _resumePlayerControl = !ControlledByAI;
            if (!ControlledByAI)
            {
                Pause();
            }

            if (_tryCancelAction)
            {
                Debug.Log("Cancelling action.");
                ForceStopHandlingActions();
                yield break;
            }

            for (int index = 0; index < queue.Actions.Count; index++)
            {
                if(queue.SkillId != null)
                {
                    if(queue.HasTarget && queue.Target == null)
                    {
                        ForceStopHandlingActions();
                    }
                }

                var action = queue.Actions[index];
                _currentAction = action;
                Vector3 targetPos = Vector3.zero;
                Vector3 targetToLookAt = Vector3.zero;

                if (action.Params.ContainsKey("Combatant"))
                {
                    var combatantObj = action.Params["Combatant"];

                    var goParam = (BaseCharacterMono) combatantObj;

                    var stopRange = action.Params.ContainsKey("StopRange") ? (float) action.Params["StopRange"] : 0;
                    
                    if(goParam != null)
                    {
                        IRPGController controller = goParam.GetComponent<RPGController>();
                        targetPos = controller.GetApproachPosition(transform.position, stopRange);
                        targetToLookAt = goParam.transform.position;
                    }
                    else
                    {
                        //Debug.Log("GoParam is null : ActionType:" + _currentAction.Type + " Params:" +
                         //   string.Join("\n",_currentAction.Params.Select(p => p.Key + " " + p.Value + "\n").ToArray()));

                        //ForceStopHandlingActions();
                    }
                    //yield return new WaitForEndOfFrame();

                    //var targetRadius = goParam.GetComponent<NavMeshAgent>().radius;
                    if(NavMeshAgent.enabled)
                    {
                        if (Math.Abs(stopRange - -1f) > 0.01f)
                        {
                            NavMeshAgent.stoppingDistance = 0;
                        }
                        else
                        {
                            NavMeshAgent.stoppingDistance = 0;
                            //NavMeshAgent.stoppingDistance = BaseStoppingDistance + targetRadius;
                        }
                    }
                    //Debug.Log("moving to combatant!");
                }
                else if (action.Params.ContainsKey("Position"))
                {
                    if (action.Params.ContainsKey("StopRange"))
                    {
                        var stopRange = (float) action.Params["StopRange"];
                        NavMeshAgent.stoppingDistance = stopRange == -1f ? 0 : stopRange;
                    }

                    targetPos = (Vector3) action.Params["Position"];
                    targetToLookAt = targetPos;
                    //Debug.Log("type: " + action.Type + "   we have pos");
                }

                if (action.Animation != null)
                {
                    if (new[] {RPGActionType.MoveToPosition}.All(a => a != action.Type))
                        RPGAnimation.CrossfadeAnimation(action.Animation);
                }

                if (action.Sound != null)
                {
                    if (new[] {RPGActionType.MoveToPosition}.All(a => a != action.Type))
                    {

                        if (action.WhileActionIsActive)
                        {
                            var sound = GetObject.AudioPlayer.PlayForever(action.Sound.Audio, AudioType.SoundFX, transform.position, action.ID);
                            if(sound != null)
                            {
                                sound.AddComponent<DestroyHelper>().Init(DestroyCondition.ActionNotPlaying, this, action.ID);    
                            }
                            //Debug.Log("play while action active");
                        }
                        else
                        {
                            GetObject.AudioPlayer.Play(action.Sound.Audio, AudioType.SoundFX, transform.position,null, action.ID);
                        }
                    }
                        
                }

                switch (action.Type)
                {
                        #region WaitForSeconds

                    case RPGActionType.WaitForSeconds:
                        if (action.FaceQueueTarget || action.Params.ContainsKey("Position") || action.Params.ContainsKey("Combatant"))
                        {
                            var timeToWait = (float) action.Params["Time"];
                            var timeWaited = 0f;
                            var lookAtCombatant = action.Params.ContainsKey("Combatant");
                            var combatant = lookAtCombatant ? (BaseCharacterMono)action.Params["Combatant"] : null;
                            var pos = !lookAtCombatant && action.Params.ContainsKey("Position") ? (Vector3)action.Params["Position"] : Vector3.zero;

                            while(timeWaited < timeToWait)
                            {
                                if (action.FaceQueueTarget)
                                {
                                    targetToLookAt = queue.QueueTarget;
                                }
                                else
                                {
                                    targetToLookAt = lookAtCombatant && combatant != null ? combatant.transform.position : pos;
                                }

                                if(targetToLookAt != Vector3.zero)
                                {
                                   //Debug.Log(targetToLookAt);
                                    transform.LookAt(new Vector3(targetToLookAt.x, transform.position.y, targetToLookAt.z));    
                                }
                                timeWaited += Time.deltaTime;
                                yield return null;
                            }
                        }
                        else
                        {
                            yield return new WaitForSeconds((float) action.Params["Time"]);
                        }
                        //Debug.Log("Waited " + (float)action.Params["Time"] + " seconds!");
                        break;

                        #endregion

                        #region MoveToPosition

                    case RPGActionType.MoveToPosition:
                        {
                            EnableNavMeshAgent();

                            TargetReached = false;
                            NavMeshAgent.speed = action.Params.ContainsKey("MoveSpeed") ? (float) action.Params["MoveSpeed"] : MoveSpeed;
                            NavMeshAgent.acceleration = 1000;
                            NavMeshAgent.destination = targetPos;
                            bool arrived = false;
                            var stopRange = (float) action.Params["StopRange"];
                            while (!arrived && NavMeshAgent.enabled)
                            {

                                if (_tryCancelAction)
                                {
                                    Debug.Log("Cancelling move action.");
                                    ForceStopHandlingActions();
                                    yield break;
                                }

                                object combatantObj = null;
                                if (action.Params.TryGetValue("Combatant", out combatantObj))
                                {
                                    var combatantTransform = (BaseCharacterMono) combatantObj;
                                    if (combatantTransform != null)
                                    {
                                        targetPos = combatantTransform.transform.position;
                                        targetToLookAt = targetPos;
                                    }
                                    else
                                    {
                                        arrived = true;
                                        break;
                                    }
                                    
                                }

                                //transform.LookAt(new Vector3(targetPos.x,transform.position.y,targetPos.z));

                                transform.LookAt(new Vector3(NavMeshAgent.steeringTarget.x, transform.position.y, NavMeshAgent.steeringTarget.z));

                                if (Vector3.Distance(transform.position, targetPos) < stopRange)
                                {
                                    arrived = true;
                                    break;
                                }
                                //Debug.Log("Moving to target: " + targetPos + " " + targetToLookAt);
                                //var x = transform.position;
                                var navMeshAgent = this.NavMeshAgent;
                                navMeshAgent.destination = targetPos;
                                navMeshAgent.isStopped = false;
                                //var v = Vector3.Distance(transform.position, targetPos);

                                while (navMeshAgent.pathPending)
                                {
                                    yield return null;
                                }


                                if (navMeshAgent.enabled)
                                {

                                    if (navMeshAgent.remainingDistance <= navMeshAgent.radius + 0.1F ||
                                        (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
                                                                                                    (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)))
                                    {
                                        if (Vector3.Distance(transform.position, targetPos) < stopRange)
                                        {
                                            arrived = true;
                                        }
                                    }
                                }
                                

                                if (!arrived)
                                {
                                    if (action.Animation != null)
                                    {
                                        RPGAnimation.CrossfadeAnimation(action.Animation);
                                    }

                                    if (action.Sound != null)
                                    {
                                        GetObject.AudioPlayer.Play(action.Sound.Audio, AudioType.SoundFX,transform.position, transform, action.ID);
                                    }
                                }
                                yield return null;
                            }

                            if(NavMeshAgent.enabled)
                            {
                                NavMeshAgent.isStopped = true;
                                NavMeshAgent.SetDestination(transform.position);
                            }
                            //Debug.Log("Reached position!");
                            TargetReached = true;
                        }
                        break;

                        #endregion

                        #region SpawnPrefab

                    case RPGActionType.SpawnPrefab:
                        {
                            var prefabPath = (string) action.Params["PrefabPath"];
                            var position = action.UseQueuePosition ? _curQueue.TargetPos : (Vector3) action.Params["Position"];
                            if(action.UseQueuePosition && queue.HasTarget)
                            {
                                position = queue.Target != null ? queue.Target.transform.Center() : position;
                            }

                            var rotation = (Quaternion) action.Params["Rotation"];
                            var parentTransform = (Transform) action.Params["ParentTransform"];
                            var withParent = (bool) action.Params["WithParent"];
                            var doAction = (Action<GameObject>) action.Params["DoAction"];


                            var canSpawn = !withParent || parentTransform != null;
                            

                            if (canSpawn)
                            {
                                var prefab = Resources.Load(prefabPath) as GameObject;
                                if(prefab != null)
                                {
                                    var spawnedPrefab = Instantiate(prefab,position,Quaternion.identity) as GameObject;
                                    if(rotation != Quaternion.identity)
                                    {
                                        spawnedPrefab.transform.eulerAngles = new Vector3(spawnedPrefab.transform.eulerAngles.x,
                                            rotation.eulerAngles.y,
                                            spawnedPrefab.transform.eulerAngles.z);
                                    }
                                    else
                                    {
                                        spawnedPrefab.transform.rotation = rotation;    
                                    }
                                    
                                    if (withParent)
                                    {
                                        spawnedPrefab.transform.parent = parentTransform;
                                        spawnedPrefab.transform.position = position;
                                    }
                                    doAction(spawnedPrefab);
                                }
                            }
                            else
                            {
                                Debug.Log("Failed to spawn prefab, parent transform null");
                            }
                        }
                        break;

                        #endregion

                        #region BasicJump

                    case RPGActionType.BasicJump:
                        {
                            TargetReached = false;
                            EnableCharacterController();
                            //playerDirWorld = Vector3.zero;
                            var jumpHeight = (float) action.Params["Height"];
                            var completion = (float) action.Params["Completion"];
                            BasicJump(jumpHeight);
                            //playerdir.y = jumpheight = start,   playerdir.y = 0 = halfway , playerdir.y = -jumpheight = full
                            //completion 0.10 => 90% of jumpheight (18)
                            //completion 0.5 => 0% of jumpheight
                            //completion 0.7 => -70% of jumpheight (-14)
                            //Debug.Log(_playerDirWorld);

                            while (!IsGrounded || _playerDirWorld.y >= 0)
                            {
                                if (completion > 0.5f)
                                {
                                    var amt = completion -= 0.5f;
                                    var x = amt*(-jumpHeight);
                                    if (_playerDirWorld.y <= x)
                                    {
                                        Debug.Log("Done jump! (>0.5)");
                                        break;
                                    }
                                }
                                else if (Math.Abs(completion - 0.5f) < 0.1f)
                                {
                                    if (Math.Abs(_playerDirWorld.y - 0) < 0.01f)
                                    {
                                        Debug.Log("Done jump! (==0.5)");
                                        break;
                                    }
                                }
                                else
                                {
                                    var x = completion*(jumpHeight*2);
                                    if (_playerDirWorld.y <= jumpHeight - x)
                                    {
                                        Debug.Log("Done jump! (<0.5)");
                                        break;
                                    }
                                }

                                //Debug.Log(_playerDirWorld);

                                yield return null;
                            }
                            Debug.Log("Finished Jump!");
                            TargetReached = true;
                        }
                        break;

                        #endregion

                        #region WaitToLand

                    case RPGActionType.WaitToLand:
                        {
                            EnableCharacterController();
                            while (!CharacterController.isGrounded || Impact != Vector3.zero)
                            {
                                Debug.Log("Waiting to land / lose impact force...");
                                yield return null;
                            }

                            Debug.Log("Landed!");
                        }
                        break;

                        #endregion

                        #region JumpToPosition

                    case RPGActionType.JumpToPosition:
                        {
                            TargetReached = false;
                            EnableCharacterController();
                            var jumpHeight = (float) action.Params["Height"];
                            //var firstPos = (targetPos + transform.position)/2;
                            var firstPos = Vector3.Lerp(transform.position, targetPos, 0.25f);
                            //var speed = (firstPos - transform.position).magnitude / 0.2f;
                            var speed = action.Params.ContainsKey("MoveSpeed") ? (float) action.Params["MoveSpeed"] : (firstPos - transform.position).magnitude/0.2f;
                            if (action.Params.ContainsKey("Combatant"))
                            {
                                var combatant = (BaseCharacterMono)action.Params["Combatant"];
                                targetPos += combatant.transform.forward*1.1f;
                            }

                            var magnitudeMargin = .5f;
                            if(speed > 75)
                            {
                                magnitudeMargin = 2.0f;
                            }

                            firstPos.y = transform.position.y + jumpHeight;
                            while (Vector3.Distance(firstPos, transform.position) > magnitudeMargin)
                            {
                                if (_stopAIJump)
                                {
                                    break;
                                }

                                if (action.Sound != null)
                                {
                                    GetObject.AudioPlayer.StopSoundById(action.ID);
                                }


                                Debug.Log("Moving!");
                                _playerDirWorld = (firstPos - transform.position).normalized*speed;
                                yield return null;
                            }

                            while (Vector3.Distance(targetPos, transform.position) > magnitudeMargin)
                            {
                                if (_stopAIJump || IsGrounded)
                                {
                                    break;
                                }

                                if (action.Sound != null)
                                {
                                    GetObject.AudioPlayer.StopSoundById(action.ID);
                                }
                                
                                Debug.Log("Moving!");
                                _playerDirWorld = (targetPos - transform.position).normalized*speed;
                                yield return null;
                            }
                            _stopAIJump = false;

                            if(queue.HasTargetPos)
                            {
                                if(Vector3.Distance(transform.position, queue.TargetPos) > 2)
                                {
                                    queue.TargetPos = transform.position;    
                                }
                            }
                                

                            Debug.Log("REEACCHHED!");
                            _playerDirWorld = Vector3.zero;
                            TargetReached = true;
                        }
                        break;

                        #endregion

                        #region WarpToPosition

                    case RPGActionType.WarpToPosition:
                        //EnableNavMeshAgent();
                        //NavMeshAgent.Warp(targetPos);
                        EnableCharacterController();
                        transform.position = targetPos + (transform.up);
                        Debug.Log("Warped to position:" + targetPos);
                        break;

                        #endregion

                        #region PlayAnimation

                    case RPGActionType.PlayAnimation:
                        {
                            if (action.Params.ContainsKey("AnimDef"))
                            {
                                var animDef = (AnimationDefinition) action.Params["AnimDef"];
                                RPGAnimation.CrossfadeAnimation(animDef);
                            }
                            else
                            {
                                var animToPlay = (string) action.Params["Animation"];
                                var speed = action.Params.ContainsKey("Speed") ? (float) action.Params["Speed"] : 1;
                                var wrapMode = action.Params.ContainsKey("WrapMode") ? (WrapMode) action.Params["WrapMode"] : WrapMode.Loop;
                                var backwards = action.Params.ContainsKey("Backwards") && (bool) action.Params["Backwards"];
                                RPGAnimation.CrossfadeAnimation(animToPlay, speed, wrapMode, backwards);
                            }
                            //Debug.Log("Played animation!");
                        }
                        break;

                        #endregion

                        #region PlaySound

                    case RPGActionType.PlaySound:
                        {
                            var audioContainer = (AudioContainer)action.Params["AudioContainer"]; // 2seconds
                            var audioType = (AudioType)action.Params["AudioType"];
                            var duration = action.Params.ContainsKey("Duration") ? (float)action.Params["Duration"] : -1;

                            if(duration != -1)
                            {
                                GetObject.AudioPlayer.Play(audioContainer.Audio, audioType, transform.position,null, action.ID, duration);    
                            }
                            else
                            {
                                GetObject.AudioPlayer.Play(audioContainer.Audio, audioType,transform.position,null, action.ID);    
                            }
                            
                            //Debug.Log("Played sound!");
                        }
                        break;

                        #endregion

                        #region KnockBack

                    case RPGActionType.KnockBack:
                        {
                            var target = (BaseCharacterMono)action.Params["Target"];
                            var direction = (Direction)action.Params["Direction"];
                            var distance = (float)action.Params["Distance"];
                            
                            target.Controller.AddImpact(direction, distance);
                        }
                        break;

                        #endregion

                        #region PullTowards

                        case RPGActionType.PullTowards:
                        {
                            var target = (BaseCharacterMono)action.Params["Target"];
                            var targetCombatant = action.Params.ContainsKey("Combatant") ? (BaseCharacterMono)action.Params["Combatant"] : null;
                            var targetPosition = action.Params.ContainsKey("TargetPosition") ? (Vector3)action.Params["TargetPosition"] : Vector3.zero;
                            var distance = (float)action.Params["Distance"];
                            
                            if(target == null)
                            {
                                break;
                            }

                            if(action.UseQueuePosition && queue.HasTargetPos)
                            {
                                targetPosition = queue.TargetPos;
                                target.Controller.PullTo(targetPosition, distance);
                            }
                            else if(targetPosition != Vector3.zero)
                            {
                                target.Controller.PullTo(targetPosition, distance);
                            }
                            else if(target.Character.Alive && targetCombatant != null)
                            {
                                target.Controller.PullTo(targetCombatant.transform.position,distance);
                            }
                            else
                            {
                                Debug.LogWarning("Action requires queue target pos, but Queue does not have a target position.");
                                break;
                            }
                        }
                        break;

                        #endregion

                        #region SetLastAttackTime

                        case RPGActionType.SetLastAttackTime:
                        {
                            RPGCombat.LastAttackTime = Time.time;
                        }
                        break;

                        #endregion

                        #region DealBonusTaunt

                        case RPGActionType.DealBonusTaunt:
                        {
                            var targetToDmg = (BaseCharacterMono)action.Params["Target"];
                            var bonusTaunt = (int)action.Params["BonusTaunt"];

                            var cc = targetToDmg.Character as CombatCharacter;
                            if(cc != null)
                            {
                                var targetTaunt = cc.TauntHandler;
                                if (!targetTaunt.Tracker.ContainsKey(CharacterMono))
                                {
                                    targetTaunt.Tracker.Add(CharacterMono, 1);
                                }

                                targetTaunt.Tracker[CharacterMono] += bonusTaunt;
                            }
                        }
                        break;

                        #endregion

                        #region DamageTarget

                    case RPGActionType.DamageTarget:
                        {
                            var targetToDmg = (BaseCharacterMono) action.Params["Target"];
                            var damageToDeal = (Damage) action.Params["DamageToDeal"];

                            var outcome = RPGCombat.DamageTarget(targetToDmg, damageToDeal);
                            var skillRef = action.Params.ContainsKey("SkillRef") ? (Skill)action.Params["SkillRef"] : null;

                            if(outcome != null)
                            {
                                if (outcome.AttackOutcome == AttackOutcome.Success || outcome.AttackOutcome == AttackOutcome.Critical)
                                {
                                    if (skillRef != null)
                                    {
                                        if(IsPlayerCharacter)
                                        {
                                            var player = (PlayerCharacter) Character;
                                            var skill = player.SkillHandler.AvailableSkills.FirstOrDefault(s => s.ID == skillRef.ID);
                                            if(skill != null)
                                            {
                                                skill.TimesUsed += 1;
                                                if (skill.ProcEffect != null)
                                                {
                                                    skill.ProcEffect.ParameterCounter += 1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //npc skills
                                        }
                                    }
                                }

                                var critHitActions = (List<RPGAction>)action.Params["CritHitActions"];
                                var hitActions = (List<RPGAction>)action.Params["HitActions"];

                                var procs = Character.ProcEffects;
                                foreach (var procEffect in procs)
                                {
                                    var procactions = RPGCombat.GetProcActions(queue, procEffect, targetToDmg, skillRef);
                                    if (procEffect.ProcCondition == Rm_ProcCondition.On_Hit ||
                                        procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Hit)
                                    {
                                        hitActions.AddRange(procactions);
                                    }
                                    else if (procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Critical_Hit)
                                    {
                                        critHitActions.AddRange(procactions);
                                    }
                                    else if (procEffect.ProcCondition == Rm_ProcCondition.Every_N_Hits)
                                    {
                                        hitActions.AddRange(procactions);
                                    }
                                }

                                if (outcome.AttackOutcome == AttackOutcome.Critical)
                                {
                                    if (critHitActions.Count > 0)
                                    {
                                        _curQueue.Actions.InsertRange(_curQueue.Actions.IndexOf(_currentAction) + 1, critHitActions);
                                    }
                                }

                                if (outcome.AttackOutcome == AttackOutcome.Success || outcome.AttackOutcome == AttackOutcome.Critical)
                                {
                                    if (hitActions.Count > 0)
                                    {
                                        _curQueue.Actions.InsertRange(_curQueue.Actions.IndexOf(_currentAction) + 1, hitActions);
                                    }
                                }
                            }
                        }
                        break;

                        #endregion

                        #region DamageTargetMelee

                    case RPGActionType.DamageTargetMelee:
                        {
                            var targetToDmg = (BaseCharacterMono)action.Params["Target"];
                            var targetPosition = (Vector3)action.Params["TargetPos"];
                            var damageToDeal = (Damage)action.Params["DamageToDeal"];

                            if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget && targetPosition == Vector3.zero)
                            {
                                var pos = targetToDmg.GetComponent<CapsuleCollider>().bounds.center;
                                targetPosition = targetPosition == Vector3.zero ? pos : targetPosition;
                            }

                            //todo:change to rm_rpghandler...
                            var prefabPath = "RPGMakerAssets/DefaultMeleePrefab";

                            var prefab = Resources.Load(prefabPath) as GameObject;
                            var spawnedPrefab = Instantiate(prefab);
                            Vector3 newTargetPos = transform.position + transform.forward;
                            spawnedPrefab.transform.position = newTargetPos;

                            var projHandler = spawnedPrefab.GetComponent<MeleeAutoAttackHandler>();
                            projHandler.Init(CharacterMono, damageToDeal, targetPosition, targetToDmg);

                            spawnedPrefab.transform.Translate(0.001f, 0.001f, 0.001f);
                        }
                        break;

                        #endregion

                        #region AutoAttack

                    case RPGActionType.AutoAttack:
                        {
                            var targetToDmg = (BaseCharacterMono) action.Params["Target"];
                            var targetPosition = (Vector3) action.Params["TargetPos"];
                            var damageToDeal = (Damage) action.Params["DamageToDeal"];

                            if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget && targetPosition == Vector3.zero)
                            {
                                Vector3 pos = Vector3.zero;
                                if(targetToDmg != null)
                                {
                                    pos = targetToDmg.transform.Center();
                                }

                                targetPosition = targetPosition == Vector3.zero ? pos : targetPosition;
                            }

                            var prefabPath = "";
                            var attackStyle = Character.AttackStyle;
                            if (Character.CharacterType == CharacterType.Player)
                            {
                                var player = (PlayerCharacter)Character;
                                var hasWeapon = !player.Equipment.Unarmed;
                                var weapon = (player.Equipment.EquippedWeapon ?? player.Equipment.EquippedOffHand) as Weapon;

                                if (hasWeapon)
                                {
                                    var wepDef = Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == weapon.WeaponTypeID);
                                    attackStyle = wepDef.AttackStyle;
                                }
                            }

                            if (attackStyle == AttackStyle.Ranged)
                            {
                                prefabPath = Character.CharacterType == CharacterType.Player
                                ? Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == ((PlayerCharacter)Character).PlayerCharacterID).AutoAttackPrefabPath
                                : ((CombatCharacter)Character).AutoAttackPrefabPath;

                                if (Character.CharacterType == CharacterType.Player)
                                {
                                    var player = (PlayerCharacter)Character;
                                    var weapon = player.Equipment.EquippedWeapon as Weapon;
                                    weapon = weapon ?? player.Equipment.EquippedOffHand as Weapon;
                                    if (weapon != null)
                                    {
                                        var wepDef = Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == weapon.WeaponTypeID);
                                        if (wepDef.AttackStyle == AttackStyle.Ranged)
                                        {
                                            prefabPath = wepDef.AutoAttackPrefabPath;
                                        }
                                    }
                                }
                            }

                            if (attackStyle == AttackStyle.Ranged)
                            {
                                if (string.IsNullOrEmpty(prefabPath))
                                {
                                    prefabPath = Rm_RPGHandler.Instance.Combat.DefaultProjectilePrefabPath;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(prefabPath))
                                {
                                    prefabPath = "RPGMakerAssets/DefaultMeleePrefab";
                                }
                            }

                            var prefab = Resources.Load(prefabPath) as GameObject;
                            var spawnedPrefab = Instantiate(prefab);
                            Vector3 newTargetPos;

                            if (attackStyle == AttackStyle.Melee)
                            {
                                newTargetPos = transform.position +  transform.forward;
                                spawnedPrefab.transform.position = newTargetPos;

                                var projHandler = spawnedPrefab.GetComponent<MeleeAutoAttackHandler>();
                                projHandler.Init(CharacterMono, damageToDeal, targetPosition, targetToDmg);
                            }
                            else
                            {
                                newTargetPos = transform.Center() + transform.forward;
                                spawnedPrefab.transform.position = newTargetPos;
                                var projHandler = spawnedPrefab.GetComponent<ProjectileAutoAttackHandler>();
                                if(Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget)
                                {
                                    projHandler.Init(CharacterMono, damageToDeal, null, targetPosition);
                                }
                                else
                                {
                                    projHandler.Init(CharacterMono, damageToDeal, targetToDmg);
                                }
                            }

                            if (attackStyle == AttackStyle.Melee)
                            {
                                spawnedPrefab.transform.Translate(0.001f,0.001f,0.001f);
                            }
                        }
                        break;

                        #endregion

                        #region RemoveCombatant

                    case RPGActionType.RemoveCombatant:
                        {


                            var combantantToRemove = (Transform) action.Params["CombatantTransform"];

                            Destroy(combantantToRemove.gameObject, 5);

                            if(Rm_RPGHandler.Instance.Combat.ShadersToLerp.Any())
                            {
                                //get the material[s] from the object+children
                                var allMats = DeathBehaviours.GetAllMaterials(combantantToRemove);

                                var matDoneDict = new Dictionary<UnityEngine.Material, Dictionary<string, bool>>();
                                for (int i = 0; i < allMats.Count; i++)
                                {
                                    var mat = allMats[i];
                                    var propDict = new Dictionary<string, bool>();
                                    var shaderLerpInfo = Rm_RPGHandler.Instance.Combat.ShadersToLerp.FirstOrDefault(s => s.ShaderName == mat.shader.name);
                                    
                                    if(shaderLerpInfo != null)
                                    {
                                        var propsCount = shaderLerpInfo.PropsToLerp.Count;
                                        for (int x = 0; x < propsCount; x++)
                                        {
                                            var prop = shaderLerpInfo.PropsToLerp[x];
                                            if (prop.LerpThisProperty)
                                            {
                                                propDict.Add(prop.PropName, false);
                                            }
                                        }
                                    }
                                    
                                    matDoneDict.Add(mat, propDict);
                                }

                                while (matDoneDict.Any(d => d.Value.Any(e => e.Value != true)))
                                {
                                    foreach(var mat in allMats)
                                    {
                                        var shaderLerpInfo = Rm_RPGHandler.Instance.Combat.ShadersToLerp.FirstOrDefault(s => s.ShaderName == mat.shader.name);
                                        var propsCount = shaderLerpInfo.PropsToLerp.Count;

                                        for (int i = 0; i < propsCount; i++)
                                        {
                                            var propToLerp = shaderLerpInfo.PropsToLerp[i];
                                            if (!propToLerp.LerpThisProperty)
                                            {
                                                continue;
                                            }

                                            switch(propToLerp.PropType)
                                            {
                                                case ShaderType.Color:
                                                    if(propToLerp.OnlyLerpAlpha)
                                                    {
                                                        var c = mat.GetColor(propToLerp.PropName);
                                                        var toColor = new Color(c.r, c.g, c.b, propToLerp.LerpTo);
                                                        mat.SetColor(propToLerp.PropName, Color.Lerp(mat.GetColor(propToLerp.PropName), toColor, 1.4f * Time.deltaTime));
                                                        if (mat.GetColor(propToLerp.PropName).IsCloseTo(propToLerp.LerpToColor))
                                                        {
                                                            matDoneDict[mat][propToLerp.PropName] = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        mat.SetColor(propToLerp.PropName, Color.Lerp(mat.GetColor(propToLerp.PropName), propToLerp.LerpToColor, 1.4f * Time.deltaTime));
                                                        if (mat.GetColor(propToLerp.PropName).IsCloseTo(propToLerp.LerpToColor))
                                                        {
                                                            matDoneDict[mat][propToLerp.PropName] = true;
                                                        }
                                                    }
                                                    break;
                                                case ShaderType.Float:
                                                    mat.SetFloat(propToLerp.PropName, Mathf.Lerp(mat.GetFloat(propToLerp.PropName),propToLerp.LerpTo,Time.deltaTime));
                                                    if (Math.Abs(mat.GetFloat(propToLerp.PropName) - propToLerp.LerpTo) < 0.05f)
                                                    {
                                                        matDoneDict[mat][propToLerp.PropName] = true;
                                                    }
                                                    break;
                                                case ShaderType.Range:
                                                    mat.SetFloat(propToLerp.PropName, Mathf.Lerp(mat.GetFloat(propToLerp.PropName), propToLerp.LerpTo, Time.deltaTime));
                                                    if (Math.Abs(mat.GetFloat(propToLerp.PropName) - propToLerp.LerpTo) < 0.05f)
                                                    {
                                                        matDoneDict[mat][propToLerp.PropName] = true;
                                                    }
                                                    break;
                                            }
                                        }
                                    }

                                    yield return null;
                                }
                                
                                Destroy(combantantToRemove.gameObject);
                            }

                        }
                        break;

                        #endregion

                        #region WaitForNextAttack

                    case RPGActionType.WaitForNextAttack:
                        {
                            var attackInterval = 1/Character.AttackSpeed;
                            var timeToNextAttack = attackInterval - (Time.time - RPGCombat.LastAttackTime);
                            if (timeToNextAttack > 0)
                            {
                                RPGAnimation.CrossfadeAnimation(LegacyAnimation.CombatIdleAnim);
                                yield return new WaitForSeconds(timeToNextAttack);
                            }
                        }
                        break;

                        #endregion

                        #region GivePlayerItem

                    case RPGActionType.GivePlayerItem:
                        {
                            var itemGroup = (ItemGroup)action.Params["ItemGroup"];
                            var itemId = (string)action.Params["ItemId"];
                            var quantity = (int)action.Params["Quantity"];
                            var forceAdd = (bool)action.Params["ForceAdd"];
                            Item itemToAdd;

                            switch(itemGroup)
                            {
                                case ItemGroup.Normal:
                                    itemToAdd = Rm_RPGHandler.Instance.Repositories.Items.Get(itemId);
                                    break;
                                case ItemGroup.Craftable:
                                    itemToAdd = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(itemId);
                                    break;
                                case ItemGroup.Quest:
                                    itemToAdd = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(itemId);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            if(itemToAdd != null)
                            {
                                var stack = itemToAdd as IStackable;
                                if(stack != null)
                                {
                                    stack.CurrentStacks = quantity;
                                }

                                if(forceAdd)
                                {
                                    GetObject.PlayerCharacter.Inventory.AllItems.Add(itemToAdd); 
                                }
                                else if (GetObject.PlayerCharacter.Inventory.CanAddItem(itemToAdd))
                                {
                                    GetObject.PlayerCharacter.Inventory.AddItem(itemToAdd);
                                }
                                else
                                {
                                    Debug.Log("Inventory too full to add skill items");
                                }
                            }

                        }
                        break;

                        #endregion

                        #region RespawnPlayer

                    case RPGActionType.RespawnPlayer:
                        {
                            EnableCharacterController();
                            yield return new WaitForSeconds(3.0f);

                            transform.position = GetObject.SpawnPositionTransform.transform.position;

                            if (GetObject.PlayerCharacter.CurrentPet != null)
                            {
                                PetMono.SpawnPet(GetObject.PlayerSave.CurrentPet, transform.position - transform.forward);
                            }
                            
                            Character.VitalHandler.Health.CurrentValue = (int) (Character.VitalHandler.Health.MaxValue*0.5f);
                            if (Character.VitalHandler.Health.CurrentValue == 0) Character.VitalHandler.Health.CurrentValue += 1;
                            InCombat = false;
                            Character.Alive = true;
                            if(Character.AnimationType == RPGAnimationType.Mecanim)
                            {
                                Animator.SetBool("isDead", false);    
                            }

                            foreach(var vital in GetObject.PlayerCharacter.Vitals)
                            {
                                if(vital.AlwaysStartsAtZero)
                                {
                                    vital.CurrentValue = 0;
                                }
                            }
                            Character.FullUpdateStats();
                            _handlingCharacterDeath = false;
                        }
                        break;

                        #endregion

                        #region RespawnNPC

                    case RPGActionType.RespawnNPC:
                        {
                            var health = Character.VitalHandler.Health;
                            var t = 0f;
                            while (health.CurrentValue < health.MaxValue)
                            {
                                t += Time.deltaTime;
                                health.CurrentValue = (int)Mathf.Lerp(0, health.MaxValue + 1, t/3);
                                RPGAnimation.CrossfadeAnimation(LegacyAnimation.DeathAnim);
                                yield return null;
                            }
                            Character.Alive = true;
                            _handlingCharacterDeath = false;
                        }
                        break;

                        #endregion

                        #region DoAction

                    case RPGActionType.DoAction:
                        {
                            var controller = (IRPGController)action.Params["Controller"];
                            var doAction = (Action<IRPGController>)action.Params["Action"];
                            doAction(controller);
                        }
                        break;

                        #endregion

                        #region AddDamageOverTime

                    case RPGActionType.AddDamageOverTime:
                        {
                            var target = (BaseCharacterMono) action.Params["Target"];
                            var dot = (DamageOverTime) action.Params["DamageOverTime"];

                            dot = GeneralMethods.CopyObject(dot);

                            var existingDot = target.Character.CurrentDoTs.FirstOrDefault(d => d.ID == dot.ID);
                            if (existingDot != null)
                            {
                                target.Character.CurrentDoTs.Remove(existingDot);
                            }

                            target.Character.AddDoT(dot);
                            Debug.Log("DoT added.");
                        }
                        break;

                        #endregion

                        #region AddEffect

                    case RPGActionType.AddEffect:
                        {
                            var target = (BaseCharacterMono) action.Params["Target"];

                            var auraSkill = action.Params.ContainsKey("AuraSkill") ? (AuraSkill) action.Params["AuraSkill"] : null;
                            if (auraSkill != null)
                            {
                                target.Character.ToggleAura(auraSkill, true);
                                break;
                            }

                            var timedPassiveEffect = action.Params.ContainsKey("TimedPassiveEffect") ? (TimedPassiveEffect) action.Params["TimedPassiveEffect"] : null;
                            if (timedPassiveEffect != null)
                            {
                                var existingEffect = target.Character.TimedPassiveEffects.FirstOrDefault(d => d.ID == timedPassiveEffect.ID);
                                if (existingEffect != null)
                                {
                                    target.Character.RemoveTimedPassiveEffect(existingEffect);
                                }

                                var effect = GeneralMethods.CopyObject(timedPassiveEffect);
                                target.Character.AddTimedPassiveEffect(effect);
                                break;
                            }

                            var statusEffect = action.Params.ContainsKey("StatusEffect") ? (StatusEffect) action.Params["StatusEffect"] : null;
                            if (statusEffect != null)
                            {
                                var existingEffect = target.Character.StatusEffects.FirstOrDefault(d => d.ID == statusEffect.ID);
                                if (existingEffect != null)
                                {
                                    target.Character.RemoveStatusEffect(existingEffect);
                                }

                                var effect = GeneralMethods.CopyObject(statusEffect);
                                if(action.Params.ContainsKey("WithDuration"))
                                {
                                    var duration = (float)action.Params["Duration"];
                                    var withDuration = (bool)action.Params["WithDuration"];
                                    if (withDuration)
                                    {
                                        effect.Effect.HasDuration = true;
                                        effect.Effect.Duration = duration;
                                    }
                                    else
                                    {
                                        effect.Effect.HasDuration = false;
                                    }
                                }

                                target.Character.AddStatusEffect(effect);
                                break;
                            }

                            var restoration = action.Params.ContainsKey("Restoration") ? (Restoration) action.Params["Restoration"] : null;
                            if (restoration != null)
                            {
                                var effect = GeneralMethods.CopyObject(restoration);
                                target.Character.AddRestoration(effect);
                                break;
                            }
                        }
                        break;

                        #endregion

                        #region RemoveStatusEffect

                    case RPGActionType.RemoveStatusEffect:
                        {
                            var target = (BaseCharacterMono) action.Params["Target"];
                            var statusEffectId = (string) action.Params["StatusEffectToRemove"];
                            target.Character.RemoveStatusEffect(statusEffectId);
                        }
                        break;

                        #endregion

                        #region RunEvent

                    case RPGActionType.RunEvent:
                        {
                            var eventId = (string) action.Params["EventID"];
                            //var async = (bool) action.Params["Async"];
                            GetObject.EventHandler.RunEvent(eventId);
                        }
                        break;

                        #endregion

                        #region FaceTarget

                    case RPGActionType.FaceTarget:
                        {
                            var target = (Vector3)action.Params["Target"];
                            yield return StartCoroutine(FaceTarget(target));
                        }
                        break;

                        #endregion

                        #region StartMecanimCasting

                        case RPGActionType.StartMecanimCasting:
                            {
                                IsCastingSkill = true;
                            }
                            break;

                        #endregion

                        #region EndMecanimCasting

                        case RPGActionType.EndMecanimCasting:
                            {
                                IsCastingSkill = false;
                            }
                            break;

                        #endregion

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if(NavMeshAgent.enabled)
                {
                    NavMeshAgent.isStopped = true;
                    NavMeshAgent.updateRotation = false;
                }

                if (action.Sound != null)
                {
                    GetObject.AudioPlayer.StopSoundById(action.ID);
                }
            }
            if (_resumePlayerControl)
            {
                Resume();
            }
            HandlingActions = false;
            _curQueue = null;
            _currentAction = null;
            //Debug.Log("Queue from: " + Character.Name + " completed in " + (Time.time - startTime));
           

            yield return null;
        }

        private IEnumerator FaceTarget(Vector3 targetPos)
        {
            yield return null;
        }

        private LegacyAnimation GetLegacyAnimation()
        {
            switch (Character.CharacterType)
            {
                case CharacterType.Player:
                    var playerChar = Character as PlayerCharacter;
                    if (playerChar != null)
                    {
                        return Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == playerChar.PlayerCharacterID).LegacyAnimations;
                    }
                    break;
                case CharacterType.NPC:
                    var npcChar = Character as CombatCharacter;
                    if (npcChar != null)
                    {
                        return Rm_RPGHandler.Instance.Repositories.Interactable.AllNpcs.First(e => e.ID == npcChar.ID).LegacyAnimations;
                    }
                    break;
                case CharacterType.Enemy:
                    var combatChar = Character as CombatCharacter;
                    if (combatChar != null)
                    {
                        return Rm_RPGHandler.Instance.Repositories.Enemies.AllEnemies.First(e => e.ID == combatChar.ID).LegacyAnimations;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }

        private bool LookingAtTarget(Vector3 position)
        {
            var cubeDir = transform.position - position;
            var angle = Vector3.Angle(cubeDir, position);

            return angle < 8;


            //var relativePoint = transform.InverseTransformPoint(position);
            //return Math.Abs(relativePoint.x - 0) > 0.1f;
        }

        //For mobile jump
        public void TryJump()
        {
            //Copy from jump code below
            var canJump = _playerDirWorld.y <= 0;
            if (IsGrounded && canJump)
            {
                BasicJump(JumpHeight);
                AudioPlayer.Instance.Play(LegacyAnimation.JumpAnim.Sound, AudioType.SoundFX, transform.position, null, "anim_Core" + LegacyAnimation.JumpAnim.Name);
            }
        }

        private void MovePlayer()
        {
            if (Impact.magnitude > 0.2F)
            {
                if(CharacterController.enabled)
                {
                    CharacterController.Move(Impact * Time.deltaTime);    
                }
                Impact = Vector3.Lerp(Impact, Vector3.zero, _impactDeteriorateSpeed * Time.deltaTime);
            }
            else
            {
                Impact = Vector3.zero;
            }

            if (CharacterController.enabled)
            {
                if (IsPlayerControlled || !IsGrounded)
                {
                    _playerDirWorld.y -= Gravity * Time.deltaTime;
                }

                if(!IsPlayerControlled && IsGrounded)
                {
                    _playerDirWorld = Vector3.zero;
                }

                CharacterController.Move(_playerDirWorld * Time.deltaTime);

            }
        }

        private void MovePlayerMobile()
        {
            if (Impact.magnitude > 0.2F)
            {
                if(CharacterController.enabled)
                {
                    CharacterController.Move(Impact * Time.deltaTime);    
                }
                Impact = Vector3.Lerp(Impact, Vector3.zero, _impactDeteriorateSpeed * Time.deltaTime);
            }
            else
            {
                Impact = Vector3.zero;
            }

            if (CharacterController.enabled)
            {
                if (IsPlayerControlled || !IsGrounded)
                {
                    _playerDirWorld.y -= Gravity * Time.deltaTime;
                }

                if(!IsPlayerControlled && IsGrounded)
                {
                    _playerDirWorld = Vector3.zero;
                }

                CharacterController.Move(_playerDirWorld * Time.deltaTime);
            }
        }

        private void GetInput()
        {
            if(!IsPlayerControlled)
            {
                Debug.Log("wtf");
                return;
            }

            var canJump = _playerDirWorld.y <= 0;
            _moveRight = 0f;
            _moveForward = 0f;

            //todo: change to RPG.INPUT.GETKEY + add runbutton in controls
            if (Rm_RPGHandler.Instance.DefaultSettings.HoldRunKeyToRun)
            {
                _running =  RPG.Input.GetKey(RPG.Input.Sprint);
            }
            else
            {
                if(RPG.Input.GetKeyUp(RPG.Input.Sprint))
                {
                    _running = !_running;
                }
            }

            if (_rpgCamera.cameraMode == CameraMode.Standard)
            {
               _moveRight = RPG.Input.GetAxis(RPG.Input.StrafeAxis) == 0 
                   ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
                   : RPG.Input.GetAxis(RPG.Input.StrafeAxis);

               // _moveRight = RPG.Input.GetAxis(RPG.Input.StrafeAxis);
            }
            else if (_rpgCamera.cameraMode == CameraMode.FirstPerson)
            {
                _moveRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis) == 0 
                    ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
                    : RPG.Input.GetAxis(RPG.Input.HorizontalAxis);
                //todo: run key
                
                transform.Rotate(0, Input.GetAxis("Mouse X") * MouseSensitivity, 0);
                var cam = _rpgCamera.transform;
                var camRotation = cam.rotation;
                cam.eulerAngles = new Vector3(camRotation.eulerAngles.x, transform.eulerAngles.y, camRotation.eulerAngles.z);
            }

            _moveForward = RPG.Input.GetAxis(RPG.Input.VerticalAxis) == 0 
                ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.VerticalAxis : 0)
                : RPG.Input.GetAxis(RPG.Input.VerticalAxis);

#if (!UNITY_IOS && !UNITY_ANDROID)

            if(Rm_RPGHandler.Instance.Customise.PressBothMouseButtonsToMove)
            {
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                {
                    transform.rotation = Quaternion.Euler(0, _rpgCamera.transform.eulerAngles.y, 0);

                    _moveForward = 1f;
                }    
            }
#endif
            
            if(_moveForward == 1f)
            {
                //todo: camera rotate to back of player
            }

            if(_playerDirWorld.y <= 1)
            {
                if (CharacterController.enabled && _rpgCamera.cameraMode != CameraMode.FirstPerson && _rpgCamera.cameraMode != CameraMode.Manual)
                {

//                    var rotateRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis) == 0 
//                                        ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
//                                        : RPG.Input.GetAxis(RPG.Input.HorizontalAxis);
                    
                    var rotateRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis);


                    var cameraRight = RPG.Input.GetAxis(RPG.Input.CameraHorizontalAxis);

                    var playerRot = rotateRight * 0.4f * TurnSpeed;

                    if(Target != null && Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget && 
                        Rm_RPGHandler.Instance.DefaultSettings.EnableTargetLock)
                    {
//                        var horiRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis) == 0 
//                                        ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
//                                        : RPG.Input.GetAxis(RPG.Input.HorizontalAxis);

                        var horiRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis);

                        //var strafRight = RPG.Input.GetAxis(RPG.Input.StrafeAxis);

                        var strafRight = RPG.Input.GetAxis(RPG.Input.StrafeAxis) == 0
                                        ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
                                        : RPG.Input.GetAxis(RPG.Input.StrafeAxis);


                        _moveRight = horiRight != 0 ? horiRight : strafRight;
                    }
                    else
                    {
                        if (_moveForward > 0)
                        {
                            _rotation.y = (cameraRight + rotateRight) * 0.4f * TurnSpeed;
                            transform.Rotate(_rotation * Time.deltaTime);
                        }
                        else
                        {
                            _rotation.y = playerRot;
                            transform.Rotate(_rotation * Time.deltaTime);
                        }
#if (!UNITY_IOS && !UNITY_ANDROID)

                        if(Rm_RPGHandler.Instance.Customise.RotateCameraWithPlayer)
                        {
                            _rpgCamera.RotateWithCharacter(_rotation.y * Time.deltaTime);    
                        }
#endif
                    }
                }
            }

            if (IsGrounded && canJump)
            {
                

                var playerDir = _moveRight * Vector3.right + _moveForward * Vector3.forward;
                _playerDirWorld = transform.TransformDirection(playerDir);

                if (Mathf.Abs(playerDir.x) + Mathf.Abs(playerDir.z) > 1)
                    _playerDirWorld.Normalize();

                
                _playerDirWorld *= _running ? MoveSpeed : MoveSpeed * 0.4f;

                if (RPG.Input.GetKeyDown(RPG.Input.Jump))
                {
                    BasicJump(JumpHeight);
                    AudioPlayer.Instance.Play(LegacyAnimation.JumpAnim.Sound, AudioType.SoundFX, transform.position,null, "anim_Core" + LegacyAnimation.JumpAnim.Name);
                }
            }
            else
            {
                var playerDir = _moveRight * Vector3.right * 0.1f + _moveForward * Vector3.forward * 0.1f;
                var newDir = transform.TransformDirection(playerDir);
                _playerDirWorld += newDir * 0.25f;

                if (Mathf.Abs(playerDir.x) + Mathf.Abs(playerDir.z) > 1)
                    _playerDirWorld.Normalize();
            }

            if (RPG.Input.GetKey("Reset_Camera"))
            {
                _rpgCamera.ResetCanera();
            }

            if(_moveRight != 0 || _moveForward != 0 || RPG.Input.GetKey(RPG.Input.Jump))
            {
                AutoAttack = false;
                if(_curQueue != null && _currentAction.Cancellable)
                {
                    ForceStopHandlingActions();
                }
            }
        }

        private void GetMobileInput()
        {
            if(!IsPlayerControlled)
            {
                Debug.Log("wtf");
                return;
            }

            var canJump = _playerDirWorld.y <= 0;
            _moveRight = 0f;
            _moveForward = 0f;

            _running = true;

            if (_rpgCamera.cameraMode == CameraMode.Standard || (_rpgCamera.cameraMode == CameraMode.TopDown && !Rm_RPGHandler.Instance.DefaultSettings.EnableClickToMove))
            {
                _moveRight = RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0;
            }
            else if (_rpgCamera.cameraMode == CameraMode.FirstPerson)
            {
                _moveRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis) == 0 
                    ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
                    : RPG.Input.GetAxis(RPG.Input.HorizontalAxis);
                //todo: run key
                
                transform.Rotate(0, Input.GetAxis("Mouse X") * MouseSensitivity, 0);
                var cam = _rpgCamera.transform;
                var camRotation = cam.rotation;
                cam.eulerAngles = new Vector3(camRotation.eulerAngles.x, transform.eulerAngles.y, camRotation.eulerAngles.z);
            }

            _moveForward = RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.VerticalAxis : 0;
            
            //if(_playerDirWorld.y <= 1)
            //{

                //Vector3 movement = new Vector3(_moveRight, 0.0f, _moveForward);

            if(!InCombat)
            {
                if(_moveForward == 0 && _moveRight == 0)
                {
                    transform.eulerAngles = new Vector3(0, previousMobileAngle, 0);
                }
                else
                {
                    float newAngle = Mathf.Atan2(_moveRight, _moveForward) * Mathf.Rad2Deg;
                    transform.eulerAngles = new Vector3(0, newAngle, 0);
                    previousMobileAngle = newAngle;
                }
            }
                

//                if (CharacterController.enabled && _rpgCamera.cameraMode != CameraMode.FirstPerson && _rpgCamera.cameraMode != CameraMode.Manual)
//                {
//
////                    var rotateRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis) == 0 
////                                        ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
////                                        : RPG.Input.GetAxis(RPG.Input.HorizontalAxis);
//                    
//                    var rotateRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis);
//
//
//                    var cameraRight = RPG.Input.GetAxis(RPG.Input.CameraHorizontalAxis);
//
//                    var playerRot = rotateRight * 0.4f * TurnSpeed;
//
//                    if(Target != null && Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget && 
//                        Rm_RPGHandler.Instance.DefaultSettings.EnableTargetLock)
//                    {
////                        var horiRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis) == 0 
////                                        ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
////                                        : RPG.Input.GetAxis(RPG.Input.HorizontalAxis);
//
//                        var horiRight = RPG.Input.GetAxis(RPG.Input.HorizontalAxis);
//
//                        //var strafRight = RPG.Input.GetAxis(RPG.Input.StrafeAxis);
//
//                        var strafRight = RPG.Input.GetAxis(RPG.Input.StrafeAxis) == 0
//                                        ? (RPGAIO_MobileControls.Instance != null ? RPGAIO_MobileControls.Instance.HorizontalAxis : 0)
//                                        : RPG.Input.GetAxis(RPG.Input.StrafeAxis);
//
//
//                        _moveRight = horiRight != 0 ? horiRight : strafRight;
//                    }
//                    else
//                    {
//                        if (_moveForward > 0)
//                        {
//                            _rotation.y = (cameraRight + rotateRight) * 0.4f * TurnSpeed;
//                            transform.Rotate(_rotation * Time.deltaTime);
//                        }
//                        else
//                        {
//                            _rotation.y = playerRot;
//                            transform.Rotate(_rotation * Time.deltaTime);
//                        }
//#if (!UNITY_IOS && !UNITY_ANDROID)
//
//                        if(Rm_RPGHandler.Instance.Customise.RotateCameraWithPlayer)
//                        {
//                            _rpgCamera.RotateWithCharacter(_rotation.y * Time.deltaTime);    
//                        }
//#endif
//                    }
//                }
            //}

            if (IsGrounded && canJump)
            {
                

                var playerDir = _moveRight * Vector3.right + _moveForward * Vector3.forward;
                //_playerDirWorld = transform.TransformDirection(playerDir);
                _playerDirWorld = GetObject.RPGCamera.transform.TransformDirection(playerDir);

                if (Mathf.Abs(playerDir.x) + Mathf.Abs(playerDir.z) > 1)
                    _playerDirWorld.Normalize();

                
                _playerDirWorld *= _running ? MoveSpeed : MoveSpeed * 0.4f;

                if (RPG.Input.GetKeyDown(RPG.Input.Jump))
                {
                    BasicJump(JumpHeight);
                    AudioPlayer.Instance.Play(LegacyAnimation.JumpAnim.Sound, AudioType.SoundFX, transform.position,null, "anim_Core" + LegacyAnimation.JumpAnim.Name);
                }
            }
            else
            {
                var playerDir = _moveRight * Vector3.right * 0.1f + _moveForward * Vector3.forward * 0.1f;
                var newDir = transform.TransformDirection(playerDir);
                _playerDirWorld += newDir * 0.25f;

                if (Mathf.Abs(playerDir.x) + Mathf.Abs(playerDir.z) > 1)
                    _playerDirWorld.Normalize();
            }

            if (RPG.Input.GetKey("Reset_Camera"))
            {
                _rpgCamera.ResetCanera();
            }

            if(_moveRight != 0 || _moveForward != 0 || RPG.Input.GetKey(RPG.Input.Jump))
            {
                AutoAttack = false;
                if(_curQueue != null && _currentAction.Cancellable)
                {
                    ForceStopHandlingActions();
                }
            }
        }

        private void GetClickInput()
        {
            if(!IsPlayerControlled)
            {
                Debug.Log("Not a player");
                return;
            }

            //Ignore if moving with leftclick+rightclick
            if(Rm_RPGHandler.Instance.Customise.PressBothMouseButtonsToMove)
            {
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                {
                    return;
                }
            }
            

            //Left click selects
#if (UNITY_IOS || UNITY_ANDROID)
            if (_showingCastArea && (Input.touchCount > 0 || RPG.Input.GetKey(RPG.Input.ConfirmCast)))
            {
                //var mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                var mousePos = GetPositionAtMousePosition();
#else
            if (_showingCastArea && RPG.Input.GetKey(RPG.Input.ConfirmCast))
            {
                var mousePos = GetPositionAtMousePosition();
#endif

                if (_currentAction != null && _currentAction.Cancellable)
                {
                    ForceStopHandlingActions();
                }

                var rotation = _rpgCamera.transform.rotation;   
                rotation.eulerAngles = new Vector3(0, _rpgCamera.transform.eulerAngles.y, 0);

                RPGCombat.UseSkill(_skillToCast, mousePos, rotation);
                EndCastArea();
            }
#if (UNITY_IOS || UNITY_ANDROID)
            else if (Input.touchCount > 0 || RPG.Input.GetKey(RPG.Input.SelectTarget))
            {
                //var mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                var foundTarget = GetTargetOnMousePosition();
#else
            else if (RPG.Input.GetKey(RPG.Input.SelectTarget))
            {
                var foundTarget = GetTargetOnMousePosition();
#endif
                if (foundTarget != null)
                {
                    if (foundTarget != CharacterMono)
                        Target = foundTarget.transform;
                }
            }

            //Right click attacks

            if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget)
            {
                if (Rm_RPGHandler.Instance.DefaultSettings.CanAttackOnSpot && (RPG.Input.GetKey(RPG.Input.AttackInPlaceKey) && RPG.Input.GetKey(RPG.Input.Attack)) || RPG.Input.GetGamePadInputDown(RPG.Input.Attack))
                {
                    var foundTarget = GetTargetOnMousePosition();
                    foundTarget = PlayerCanAttack(foundTarget) ? foundTarget : null;
                    if (foundTarget != null)
                    {

                        if (_currentAction != null && !AutoAttack && _currentAction.Cancellable)
                        {
                            ForceStopHandlingActions();
                        }

                        if (!IsFriendly(foundTarget))
                        {
                            Target = foundTarget.transform;
                            if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
                            {
                                AutoAttack = true;
                            }
                            else
                            {
                                RPGCombat.Attack(Target);
                                return;
                            }
                        }
                    }   
                    else
                    {
                        var targetPos = GetPositionAtMousePosition();
                        targetPos.y = transform.Center().y;
                        RPGCombat.Attack(null, targetPos);
                        return; 
                    }
                }


#if (UNITY_IOS || UNITY_ANDROID)
                else if (Input.touchCount > 0 || RPG.Input.GetKey(RPG.Input.Attack))
                {
                    var mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    var foundTarget = GetTargetOnMousePosition();
#else
                else if (RPG.Input.GetKey(RPG.Input.Attack))
                {
                    var foundTarget = GetTargetOnMousePosition();
#endif


                    foundTarget = PlayerCanAttack(foundTarget) ? foundTarget : null;
                    if (foundTarget != null)
                    {
                        if (_currentAction != null && !AutoAttack && _currentAction.Cancellable)
                        {
                            ForceStopHandlingActions();
                        }

                        if (!IsFriendly(foundTarget))
                        {
                            Target = foundTarget.transform;
                            if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
                            {
                                AutoAttack = true;
                            }
                            else
                            {
                                RPGCombat.Attack(Target);
                            }
                        }
                    }
                }
            }
            else
            {

#if (UNITY_IOS || UNITY_ANDROID)
                if ((Input.touchCount > 0 || RPG.Input.GetKey(RPG.Input.Attack))) //first have a target before attacking
                {
                    //var mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    //var mousePos = Input.mousePosition;
                    //var foundTarget = GetTargetOnMousePosition() ?? (Target != null ? Target.GetComponent<BaseCharacterMono>() : null);
                    var foundTarget = GetTargetOnMousePosition();

                    //if (foundTarget != null || foundTarget.transform != Target) return;
#else
                if (RPG.Input.GetKey(RPG.Input.Attack))
                {
                    var foundTarget = GetTargetOnMousePosition() ?? (Target != null ? Target.GetComponent<BaseCharacterMono>() : null);
#endif

                    foundTarget = PlayerCanAttack(foundTarget) ? foundTarget : null;

                    if (foundTarget != null)
                    {

                        if (_currentAction != null && !AutoAttack && _currentAction.Cancellable)
                        {
                            ForceStopHandlingActions();
                        }

                        if (!IsFriendly(foundTarget))
                        {

                            Target = foundTarget.transform;
                            if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
                            {
                                AutoAttack = true;
                            }
                            else
                            {
                                RPGCombat.Attack(Target);
                            }
                        }
                    }
                }
            }

            if (Rm_RPGHandler.Instance.Combat.TargetStyle != TargetStyle.TargetLock)
            {
                AutoAttack = false;
            }

            if (Rm_RPGHandler.Instance.DefaultSettings.EnableClickToMove)
            {

#if (UNITY_IOS || UNITY_ANDROID)
                if (!UIHandler.MouseOnUI && !UIHandler.MouseOnPlayer && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
                {
                    //var mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    var mousePos = Input.mousePosition;
#else
                if(!UIHandler.MouseOnUI && !UIHandler.MouseOnPlayer && Input.GetMouseButtonDown(0))
                {
                    var mousePos = Input.mousePosition;
#endif
                    var ray = GetObject.RPGCamera.GetComponent<Camera>().ScreenPointToRay(mousePos);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, 1000, ClickToMoveLayers.LayerMask))
                    {
                        EnableNavMeshAgent();


                        var wantedPos = hit.point;
                        wantedPos.y = transform.position.y;
                        transform.LookAt(wantedPos);
                        var q = new RPGActionQueue();
                        q.Add(RPGActionFactory.MoveToPosition(wantedPos, 1, _running ? MoveSpeed : MoveSpeed * 0.4f).WithCancellable());
                        q.Add(RPGActionFactory.DoAction(this, controller => ((RPGController)controller).EnableCharacterController()));
                        //var _playerDir = wantedPos - transform.position;
                        //
                        //if (Mathf.Abs(_playerDir.x) + Mathf.Abs(_playerDir.z) > 1)
                        //    _playerDir.Normalize();
                        //
                        //_playerDir *= MoveSpeed;
                        //_playerDir.y = 0;
                        //_playerDirWorld = new Vector3(_playerDir.x, _playerDirWorld.y, _playerDir.z);
                        //
                        if (_currentAction != null && _currentAction.Cancellable)
                        {
                            ForceStopHandlingActions();
                        }
                        BeginActionQueue(q);
                        RPGAnimation.CrossfadeAnimation(_running ? LegacyAnimation.RunAnim : LegacyAnimation.WalkAnim);
                    }
                }
                else
                {
                    _timeHoldingClickToMove = 0;
                }
                
            }
        }

        private bool PlayerCanAttack(BaseCharacterMono baseMono)
        {
            if (baseMono == null) return false;

            if (baseMono.GetComponent<PetMono>()) return false;

            if (baseMono.Character.CharacterType == CharacterType.NPC)
            {
                if (!Rm_RPGHandler.Instance.Combat.NPCsCanFight)
                {
                    return false;
                }
                else
                {
                    if (!Rm_RPGHandler.Instance.Combat.CanAttackNPcs)
                    {
                        return false;
                    }
                    else
                    {
                        var npcChar = baseMono.Character as NonPlayerCharacter;
                        if (npcChar.CanBeKilled && !Rm_RPGHandler.Instance.Combat.CanAttackUnkillableNPCs)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void BasicJump(float jumpHeight)
        {
            //Debug.Log("Jumping a distance of " + jumpHeight);
            _playerDirWorld.x *= 1.2f;
            _playerDirWorld.z *= 1.2f;
            _playerDirWorld.y += jumpHeight;
        }

        private void SetApproachPositions()
        {
            //todo: not every frame (we move)
            if(transform.position != _oldPosition || _approachPositions.Count == 0)
            {
                _approachPositions = new Dictionary<Vector3, int>();
                _oldPosition = transform.position;
                const int spotsAvailable = 12;
                for (var i = 0; i < spotsAvailable; i++)
                {
                    var newPosition = (transform.position - transform.up) + Quaternion.AngleAxis(i * (360 / spotsAvailable), Vector3.up) * Vector3.forward * 1.5f;
                    _approachPositions.Add(newPosition, 0);
                }
            }
        }

        private void EnableCharacterController()
        {
            CharacterController.enabled = true;
            NavMeshAgent.enabled = false;
        }

        private void EnableNavMeshAgent()
        {
            _playerDirWorld = Vector3.zero;
            CharacterController.enabled = false;
            if(_navMeshObstacle != null)
            {
                _navMeshObstacle.enabled = false;    
            }
            NavMeshAgent.updatePosition = true;
            NavMeshAgent.enabled = true;
            NavMeshAgent.acceleration = 10;
            NavMeshAgent.autoBraking = true;
            NavMeshAgent.updateRotation = true;
        }

        private void GetAIActions()
        {
            if(Target != null)
            {
                var charTarget = Target.GetComponent<BaseCharacterMono>();
                if(charTarget != null && !charTarget.Character.Alive)
                {
                    Target = null;
                }
            }

            if (Character.Stunned)
            {
                _playerDirWorld = new Vector3(0, _playerDirWorld.y,0);

                if (Character.StunFreeze)
                {
                    Animation.Stop();
                }
                ForceStopHandlingActions();
                return;
            }

            if (Character.Retreating)
            {
                _playerDirWorld = new Vector3(0, _playerDirWorld.y, 0);
                if (HandlingActions && _curQueue.Identifier != "Retreat")
                {
                    ForceStopHandlingActions();
                }
                var q = new RPGActionQueue() { Identifier = "Retreat" };
                q.Add(RPGActionFactory.MoveToPosition(transform.position - (transform.forward * 20.0f))).WithAnimation(LegacyAnimation.RunAnim);
                BeginActionQueue(q);
            }
            else
            {
                if (HandlingActions && _curQueue.Identifier == "Retreat")
                {
                    ForceStopHandlingActions();
                }
            }

            //Debug.Log("TIME OUT OF COMBAT:" + TimeOutOfCombat);
            var oldState = _state;
            var combatChar = (CombatCharacter) Character;

            //If at spawn unattacked for 25 secs then regen health, if unattacked for 60 we can retreat again
            if (Vector3.Distance(transform.position, SpawnPosition) < 2.0F)
            {
                if (TimeOutOfCombat > 25 && Character.VitalHandler.Health.CurrentValue < Character.VitalHandler.Health.MaxValue)
                {
                    Character.VitalHandler.Health.CurrentValue += (int)((Character.VitalHandler.Health.MaxValue * 0.25f) * Time.deltaTime);
                }
                else if(TimeOutOfCombat > 60 )
                {
                    _alreadyRetreated = false;
                }
            }
            


            //Behaviours:::::
            //If we are too far from spawn pos retreat back to spawn and then patrol/idle
            if (RetreatingToSpawn && Vector3.Distance(transform.position, SpawnPosition) < (NavMeshAgent.radius + 2.0f))
            {
                Debug.Log("reached spawn");
                RetreatingToSpawn = false;
                _state = RPGControllerState.Idle;
            }

            //Pets never retreat
            if(PetMono != null)
            {
                RetreatingToSpawn = false;
            }

            if(RetreatingToSpawn)
            {
                Target = null;
            }
            else if (PetMono == null && Vector3.Distance(transform.position, SpawnPosition) > TooFarFromSpawnDistance + NavMeshAgent.radius) //If we are too far away from spawnPos
            {
                ForceStopHandlingActions();
                _state = RPGControllerState.ReturnToSpawn;
                RetreatingToSpawn = true;
                InCombat = false;
                Target = null;
            }
            //If we are about to die and this enemy retreats, then retreat till at spawn and then patrol/idle, mark retreated as true so we can't retreat again
            else if (PetMono == null && combatChar.RetreatsWhenLow && !_alreadyRetreated && Character.VitalHandler.Health.CurrentValue < (Character.VitalHandler.Health.MaxValue * 0.1f)) //If we're about to die retreat //todo: check if this char retreats
            {
                if(Vector3.Distance(transform.position, SpawnPosition) > 5.0f)
                {
                    ForceStopHandlingActions();
                    _state = RPGControllerState.ReturnToSpawn;
                    InCombat = false;
                    RetreatingToSpawn = true;
                }

                _alreadyRetreated = true;
                Target = null;

            }
            //If we are in combat then fight
            else if(InCombat)
            {
                if(Rm_RPGHandler.Instance.Combat.EnableTauntSystem)
                {
                    var primeTarget = combatChar.TauntHandler.GetTarget();
                    if (primeTarget != null)
                    {
                        Target = primeTarget.transform;
                    }
                }
                

                if(Target == null || !Target.gameObject.activeInHierarchy)
                {
                    InCombat = false;
                }
                _state = RPGControllerState.Attacking;
            }
            //Patrol/Follow, if we are aggresive (and haven't retreated) then actively search for targets, if we are supposed to follow but no target, then idle, if follower in combat fight with him
            else
            {
                //patrol/follow
                 if(RPGFollow != null && RPGFollow.FollowTarget && RPGFollow.TargetToFollow != null 
                     && RPGFollow.TargetToFollow.isActiveAndEnabled && RPGFollow.TargetToFollow.Character.Alive)
                 {
                     if(RPGFollow.TargetToFollow.Controller.InCombat)
                     {
                         //Only assist as a pet if aggresive/assisting
                         if(PetMono == null || PetMono.PetData.CurrentBehaviour != PetBehaviour.PetOnly)
                         {
                             Target = RPGFollow.TargetToFollow.Controller.Target;
                             InCombat = true;
                         }
                     }
                     else
                     {
                         _state = RPGControllerState.Follow;
                     }
                 }
                 else if(RPGFollow != null && RPGFollow.FollowTarget && RPGFollow.TargetToFollow != null
                     && RPGFollow.TargetToFollow.isActiveAndEnabled && RPGFollow.TargetToFollow.Controller.Target != null)
                 {
                     Target = RPGFollow.TargetToFollow.Controller.Target;
                     InCombat = true;
                 }
                 else if(RPGPatrol != null && RPGPatrol.ShouldPatrol)
                 {
                     _state = RPGControllerState.Patrol;
                 }
                 else
                 {
                     _state = RPGControllerState.Idle;     
                 }
                
                //search
                if(combatChar.IsAggressive && !_alreadyRetreated)
                {
                    Target = GetNearestEnemy();

                    if(Target != null)
                    {
                        InCombat = true;
                        _state = RPGControllerState.Attacking;
                    }
                }
            }

            ////////////////////////////////////////////////##########################################################
            var queue = RPGActionQueue.Create();

            if(oldState != _state)
            {
                ForceStopHandlingActions();
            }

            //Act on state
            if (_state == RPGControllerState.ReturnToSpawn)     //If we're about to die retreat
            {
                //Debug.Log("Retreating")
                queue.Add(RPGActionFactory.MoveToPosition(SpawnPosition)).WithAnimation(LegacyAnimation.RunAnim);
            }
            else if (_state == RPGControllerState.Follow)   
            {
                NavMeshAgent.avoidancePriority = 1000;
                var targetToFollow = RPGFollow.TargetToFollow;
                var targetToFollowPos = RPGFollow.TargetToFollow.transform.position;
                
                ForceStopHandlingActions();

                transform.LookAt(new Vector3(targetToFollowPos.x, transform.position.y,targetToFollowPos.z));
                if (Vector3.Distance(transform.position, targetToFollow.transform.position) < 2.0f + NavMeshAgent.radius * transform.localScale.x)
                {
                    RPGAnimation.CrossfadeAnimation(LegacyAnimation.IdleAnim);
                }
                else
                {                    
                    var action = RPGActionFactory.MoveToPosition(targetToFollow, 1.4f + NavMeshAgent.radius * transform.localScale.x);
                    action.ID = "FollowMove";

                    var anim = GetObject.PlayerController.Running ? LegacyAnimation.RunAnim : LegacyAnimation.WalkAnim;
                    if(string.IsNullOrEmpty(anim.Animation))
                    {
                        anim = new[] {LegacyAnimation.RunAnim, LegacyAnimation.WalkAnim}.FirstOrDefault(s => !string.IsNullOrEmpty(s.Animation));
                    }

                    queue.Add(action.WithAnimation(anim));
                }
            }
            else if (_state == RPGControllerState.Attacking)
            {
                if(Target != null)
                {
                    var targetAsMono = Target.GetComponent<BaseCharacterMono>();
                    if ((targetAsMono != null && !targetAsMono.Character.Alive))
                    {
                        _state = RPGControllerState.Idle;
                    }
                }
                else
                {
                    _state = RPGControllerState.Idle;
                }
                

                var timeToNextAttack = AttackInterval - (Time.time - RPGCombat.LastAttackTime);
                if (timeToNextAttack <= 0)
                {
                    var targetPos = Vector3.zero;
                    BaseCharacterMono skillTarget = null;
                    Rm_NPCSkill skillToUse = GetAISkillToUse(out skillTarget, out targetPos);
                    if(skillToUse == null)
                    {
                        RPGCombat.Attack(Target);
                        var cc = (CombatCharacter) Character;
                        cc.AttackCounter++;
                    }
                    else
                    {
                        var skill = skillToUse.SkillRef;

                        if(targetPos == Vector3.zero)
                        {
                            var targetMono = skillTarget.GetComponent<BaseCharacterMono>();
                            RPGCombat.UseSkill(skill, targetMono);
                            skillToUse.Paramater++;
                        }
                        else
                        {
                            RPGCombat.UseSkill(skill, targetPos);
                            skillToUse.Paramater++;
                        }
                    }
                }
                else
                {
                    if(!HandlingActions)
                    {
                        RPGAnimation.CrossfadeAnimation(LegacyAnimation.CombatIdleAnim);
                    }
                }
            }
            else if (_state == RPGControllerState.Idle)
            {   
                //Debug.Log("Idle!");
                //queue.Add(RPGActionFactory.PlayAnimation(LegacyAnimation.IdleAnim));
                RPGAnimation.CrossfadeAnimation(LegacyAnimation.IdleAnim);
            }
            else if (_state == RPGControllerState.Patrol)   
            {
                var animToUse = !string.IsNullOrEmpty(LegacyAnimation.WalkAnim.Animation) ? LegacyAnimation.WalkAnim : LegacyAnimation.RunAnim;

                //Debug.Log("Patrolling!");
                if(HandlingActions && _currentAction.Cancellable) //If we are moving
                {
                    RPGAnimation.CrossfadeAnimation(animToUse);
                }
                
                var currentTarget = RPGPatrol.CurrentWaypoint;
                if (Vector3.Distance(transform.position, currentTarget.position) < 1.0f +  NavMeshAgent.radius * transform.localScale.x)
                {
                    ForceStopHandlingActions();
                    RPGPatrol.SetNextWayPoint();
                    if(RPGPatrol.MinWaitTimeBetweenPoints > 0)
                    {
                        queue.Add(RPGActionFactory.WaitForSeconds(Random.Range(RPGPatrol.MinWaitTimeBetweenPoints, RPGPatrol.MaxWaitTimeBetweenPoints))).WithAnimation(LegacyAnimation.IdleAnim);
                    }
                }
                var moveSpeed = !string.IsNullOrEmpty(LegacyAnimation.WalkAnim.Animation) ? MoveSpeed/1.5f : MoveSpeed;
                queue.Add(RPGActionFactory.MoveToPosition(RPGPatrol.CurrentWaypoint.position, 0.2f, moveSpeed).WithAnimation(animToUse)).WithCancellable();
                
            }
            
            if(queue.Actions.Count > 0)
            {
                BeginActionQueue(queue);
            }
        }

        private Rm_NPCSkill GetAISkillToUse(out BaseCharacterMono skillTarget, out Vector3 targetPos)
        {
            if(Character.Silenced)
            {
                skillTarget = null;
                targetPos = Vector3.zero;
                return null;
            }

            //if we have skill and target, check we can cast it (proc counters, cooldowns, etc)
            var cc = (CombatCharacter)Character;
            var usableSkills = new List<Rm_NPCSkill>();
            //Skill skillToUse = null;
            Rm_NPCSkill npcSkill = null;

            foreach(var skill in cc.EnemySkills)
            {
                switch (skill.CastType)
                {
                    case Rm_EnemySkillCastType.WhenOffCooldown:
                        if(skill.SkillRef.OffCooldown) //todo: implement cooldowns
                        {
                            usableSkills.Add(skill);
                        }
                        break;
                    case Rm_EnemySkillCastType.EveryNthAttack:
                        if (cc.AttackCounter % (int)skill.Paramater == 0)
                        {
                            usableSkills.Add(skill);
                        }
                        break;
                    case Rm_EnemySkillCastType.EveryNthAttackIfOffCooldown:
                        if (cc.AttackCounter % (int)skill.Paramater == 0 && skill.SkillRef.OffCooldown)
                        {
                            usableSkills.Add(skill);
                        }
                        break;
                    case Rm_EnemySkillCastType.EveryNthSeconds:
                        if (skill.NthSecondsTimer <= 0)
                        {
                            usableSkills.Add(skill);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //check which skills we can get a target for (e.g. damage is the target, heal try find ally/self) => findnearestenemy etc
            for (int i = 0; i < usableSkills.Count; i++)
            {
                var skill = usableSkills[i];
                bool canUse = false;

                if(skill.SkillRef.Targetable)
                {
                    switch(skill.SkillRef.TargetType)
                    {
                        case TargetType.Self:
                        case TargetType.Any:
                        case TargetType.SelfOrAlly:
                            canUse = true;
                            break;
                        case TargetType.Ally:
                            canUse = GetNearbyAllies(skill.SkillRef.CastRange).Any(c => Vector3.Distance(transform.position, c.transform.position) > skill.SkillRef.MinCastRange);
                            break;
                        case TargetType.Enemy:
                            canUse = GetNearbyEnemies(skill.SkillRef.CastRange).Any(c => Vector3.Distance(transform.position, c.transform.position) > skill.SkillRef.MinCastRange);
                            break;
                        case TargetType.NotSelf:
                            canUse = GetNearbyTargets(skill.SkillRef.CastRange).Any(c => Vector3.Distance(transform.position, c.transform.position) > skill.SkillRef.MinCastRange);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if(!canUse)
                {
                    usableSkills.RemoveAt(i);
                    i--;
                }
            }

            //if one is available, select the first
            if (usableSkills.Any())
            {   
                //Choose the skill to cast
                //todo: extension method, one day :)
                //also: max is: .Aggregate((i1, i2) => i1.Priority > i2.Priority ? i1 : i2) -- notice the > instead of <
                npcSkill = usableSkills.Aggregate((i1, i2) => i1.Priority < i2.Priority ? i1 : i2);


                var potentialTargets = new List<BaseCharacterMono>();
                
                switch (npcSkill.SkillRef.TargetType)
                {
                    case TargetType.Self:
                        potentialTargets.Add(CharacterMono);
                        break;
                    case TargetType.Any:
                        potentialTargets.AddRange(GetNearbyAllies(npcSkill.SkillRef.CastRange).Where(c => Vector3.Distance(transform.position, c.transform.position) > npcSkill.SkillRef.MinCastRange));
                        potentialTargets.AddRange(GetNearbyEnemies(npcSkill.SkillRef.CastRange).Where(c => Vector3.Distance(transform.position, c.transform.position) > npcSkill.SkillRef.MinCastRange));
                        potentialTargets.Add(CharacterMono);
                        break;
                    case TargetType.SelfOrAlly:
                        potentialTargets.AddRange(GetNearbyAllies(npcSkill.SkillRef.CastRange).Where(c => Vector3.Distance(transform.position, c.transform.position) > npcSkill.SkillRef.MinCastRange));
                        potentialTargets.Add(CharacterMono);
                        break;
                    case TargetType.Ally:
                        potentialTargets.AddRange(GetNearbyAllies(npcSkill.SkillRef.CastRange).Where(c => Vector3.Distance(transform.position, c.transform.position) > npcSkill.SkillRef.MinCastRange));
                        break;
                    case TargetType.Enemy:
                        potentialTargets.AddRange(GetNearbyEnemies(npcSkill.SkillRef.CastRange).Where(c => Vector3.Distance(transform.position, c.transform.position) > npcSkill.SkillRef.MinCastRange));
                        break;
                    case TargetType.NotSelf:
                        potentialTargets.AddRange(GetNearbyAllies(npcSkill.SkillRef.CastRange).Where(c => Vector3.Distance(transform.position, c.transform.position) > npcSkill.SkillRef.MinCastRange));
                        potentialTargets.AddRange(GetNearbyEnemies(npcSkill.SkillRef.CastRange).Where(c => Vector3.Distance(transform.position, c.transform.position) > npcSkill.SkillRef.MinCastRange));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if(!potentialTargets.Any())
                {
                    Debug.Log("eye");
                }




                //We have a skill with a target
                var target = potentialTargets[Random.Range(0, potentialTargets.Count)];

                //get the actual target (or set targetPos here)
                if(npcSkill.SkillRef.SkillType != SkillType.Area_Of_Effect && npcSkill.SkillRef.SkillType != SkillType.Spawn)
                {
                    if(Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
                    {
                        skillTarget = target;
                        targetPos = Vector3.zero;
                    }
                    else
                    {
                        if(npcSkill.SkillRef.Targetable)
                        {
                            skillTarget = target;
                            targetPos = Vector3.zero;
                        }
                        else
                        {
                            skillTarget = null;
                            targetPos = target.transform.position;
                        }
                    }
                }
                else
                {
                    skillTarget = null;
                    targetPos = target.transform.position;
                    if(npcSkill.SkillRef.SkillType == SkillType.Spawn)
                    {
                        targetPos += new Vector3(Random.Range(0, NavMeshAgent.radius + 2), 0, Random.Range(0, NavMeshAgent.radius + 2));
                    }
                }
            }
            else
            {
                skillTarget = null;
                targetPos = Vector3.zero;
            }

            //if all good return the skill, and if it's by position then set targetpos before return
            return npcSkill;
        }

        public List<BaseCharacterMono> GetNearbyTargets(float radius)
        {
            var targets = new List<BaseCharacterMono>();
            var cols = Physics.OverlapSphere(transform.position, radius + 5);
            foreach (var hit in cols)
            {
                //TODO: REPUTATION AGGRO
                if (!hit) continue;
                var mono = hit.GetComponent<BaseCharacterMono>();
                if (mono == null || mono.Character == null) continue;
                if(hit.transform == transform) continue;
                if (mono == CharacterMono) continue;
                if (!mono.Character.Alive) continue;
                if (mono.Character.CharacterType == CharacterType.NPC)
                {
                    if(!Rm_RPGHandler.Instance.Combat.NPCsCanFight)
                    {
                        continue;
                    }

                    var npcChar = mono.Character as NonPlayerCharacter;
                    if (!npcChar.CanFight) continue;
                }

                if(Vector3.Distance(transform.position, mono.transform.position) <= radius)
                {
                    targets.Add(mono);
                }
            }
            return targets;
        }

        public Transform GetNearestEnemy()
        {
            var targets = GetNearbyTargets(AggroRadius).Where(e => !IsFriendly(e)).ToList();
            if(targets.Count > 0)
            {
                return targets.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).First().transform;    
            }

            return null;
        }

        public Transform GetNearestAlly()
        {
            var targets = GetNearbyTargets(AggroRadius).Where( IsFriendly ).ToList();
            if (targets.Count > 0)
            {
                return targets.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).First().transform;    
            }

            return null;
        }

        public List<BaseCharacterMono> GetNearbyAllies(float distance)
        {
            var targets = GetNearbyTargets(distance).Where(IsFriendly).ToList();
            return targets;
        }

        public List<BaseCharacterMono> GetNearbyEnemies(float distance)
        {
            var targets = GetNearbyTargets(distance).Where(p => !IsFriendly(p)).ToList();
            return targets;
        }

        public bool IsFriendly(BaseCharacterMono target)
        {
            return Character.IsFriendly(target.Character);
        }

        private void GetActions()
        {
            if (Interacting)
            {
                _playerDirWorld = Vector3.zero;
                return;
            }

            if(Character.Stunned)
            {
                _playerDirWorld = new Vector3(0, _playerDirWorld.y, 0);
                if(Character.StunFreeze)
                {
                    Animation.Stop();
                }
                ForceStopHandlingActions();
                return;
            }
            
            if (Character.Retreating)
            {
                _playerDirWorld = new Vector3(0, _playerDirWorld.y, 0);
                if(HandlingActions && _curQueue.Identifier != "Retreat")
                {
                    ForceStopHandlingActions();
                }
                var q = new RPGActionQueue() { Identifier = "Retreat" };
                q.Add(RPGActionFactory.MoveToPosition(transform.position - (transform.forward * 20.0f))).WithAnimation(LegacyAnimation.RunAnim);
                BeginActionQueue(q);
            }

            if(AutoAttack && !HandlingActions)
            {
                RPGCombat.Attack(Target);
            }

            if (!HandlingActions || (_currentAction != null && _currentAction.Cancellable) || _curQueue.Identifier == "RPG_Combat")
            {
                if(GameMaster.isMobile)
                {
                    GetMobileInput();
                }
                else
                {
                    GetInput();   
                }
                GetClickInput();                  
            }

            if(RPG.Input.GetKey(RPG.Input.MoveForward) || RPG.Input.GetKey(RPG.Input.MoveBackward) ||RPG.Input.GetKey(RPG.Input.StrafeLeft) ||RPG.Input.GetKey(RPG.Input.StrafeRight) ||RPG.Input.GetKey(RPG.Input.Jump))
            {
                if(AutoAttack)
                {
                    AutoAttack = false;
                }

                if(HandlingActions && _currentAction.Cancellable)
                {
                    ForceStopHandlingActions();
                }
            }
        }

        private void CheckTargetStatus()
        {
            if(HandlingActions && Target == null && _curQueue == null)
            {
                ForceStopHandlingActions();
            }
        }

        private void DoAnimations()
        {
            //If we are not moving, then we are idle
            if (_playerDirWorld == Vector3.zero && Impact == Vector3.zero)
            {
                if (_rotation == Vector3.zero)
                {
                    //Normal idle or combat idle depending on if in combat
                    RPGAnimation.CrossfadeAnimation(InCombat ? LegacyAnimation.CombatIdleAnim : LegacyAnimation.IdleAnim);
                }
                else
                {
                    RPGAnimation.CrossfadeAnimation(_rotation.y > 0 ? LegacyAnimation.TurnRightAnim : LegacyAnimation.TurnLeftAnim);
                }
            }
            //We are moving
            else
            {
                //Check if we are taking impact forces
                if (Impact != Vector3.zero)
                {
                    if (Impact.y > 0)
                    {
                        Debug.Log("Impact up anim?");
                        RPGAnimation.CrossfadeAnimation(LegacyAnimation.KnockUpAnim);
                    }
                    else
                    {
                        Debug.Log("Impact back anim?");
                        RPGAnimation.CrossfadeAnimation(LegacyAnimation.KnockBackAnim);
                    }
                }
                //Check if we are in the air first
                else if (Math.Abs(_playerDirWorld.y - 0) > 0.01f) //we're in the air
                {
                    if(_timeInAir > TimeBeforeFallDamage) //we're falling
                    {
                        RPGAnimation.CrossfadeAnimation(LegacyAnimation.FallAnim);
                    }
                    else if (_playerDirWorld.y > JumpThreshold) //we're ascending
                    {
                        RPGAnimation.CrossfadeAnimation(LegacyAnimation.JumpAnim);
                    }
                    else if (_playerDirWorld.y < -(JumpHeight + FallingThreshold)) //we're falling
                    {
                        RPGAnimation.CrossfadeAnimation(LegacyAnimation.FallAnim);
                    }

                }
                //We're moving but not in the air, so we are walking/running
                else
                {
                    if (_moveForward != 0 || RPG.Input.GetKey(RPG.Input.MoveForward) || RPG.Input.GetKey(RPG.Input.MoveBackward))
                    {
                        if (_running && _moveForward > 0)
                        {
                            RPGAnimation.CrossfadeAnimation(LegacyAnimation.RunAnim);
                        }
                        else if(Input.GetMouseButton(0) && Input.GetMouseButton(1))
                        {
                            RPGAnimation.CrossfadeAnimation(_running ? LegacyAnimation.RunAnim : LegacyAnimation.WalkAnim);
                        }
                        else
                        {
                            RPGAnimation.CrossfadeAnimation(RPG.Input.GetKey(RPG.Input.MoveForward) 
                                ? LegacyAnimation.WalkAnim
                                : !string.IsNullOrEmpty(LegacyAnimation.WalkBackAnim.Animation) ? LegacyAnimation.WalkBackAnim : LegacyAnimation.WalkAnim);
                        }
                    }
                    else if (RPG.Input.GetKey(RPG.Input.StrafeRight) || RPG.Input.GetKey(RPG.Input.StrafeLeft))
                    {
                        RPGAnimation.CrossfadeAnimation(RPG.Input.GetKey(RPG.Input.StrafeRight) ? LegacyAnimation.StrafeRightAnim : LegacyAnimation.StrafeLeftAnim);
                    }
                    else if(_moveRight != 0)
                    {
                        RPGAnimation.CrossfadeAnimation(_moveRight > 0 ? LegacyAnimation.StrafeRightAnim : LegacyAnimation.StrafeLeftAnim);
                    }
                }
            }
        }

        private void HandleKeybinds()
        {
            var stop = new Stopwatch();
            stop.Start();
            

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown(RPG.Gamepad.GamepadStart))  
            {
                //if target not null and not action
                //if action and cancellable
                if (_showingCastArea)
                {
                    EndCastArea();
                }
                else if (!HandlingActions && Target != null)
                {
                    Target = null;
                }
                else if (HandlingActions && _currentAction.Cancellable)
                {
                    ForceStopHandlingActions();
                }
                else
                {
                    RPG.Events.OnMenuOpened(new RPGEvents.OpenMenuEventArgs());
                }
            }

            if (Interacting) return;

            if(Input.GetKeyDown(KeyCode.F1)) //todo: Rm_RPGHandle.input.targetselfbutton
            {
                if (Rm_RPGHandler.Instance.Combat.TargetStyle != TargetStyle.ManualTarget)
                {
                    Target = transform;
                }
            }

            if(RPG.Input.GetKeyDown(RPG.Input.TargetNearest)) //todo: Rm_RPGHandle.input.targetselfbutton
            {
                if(Target == null)
                {
                    //todo: cycle targets
                    var foundTarget = GetNearestEnemy();
                    Target = foundTarget;

#if (!UNITY_IOS && !UNITY_ANDROID)
                    if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget && foundTarget != null
                        && Rm_RPGHandler.Instance.DefaultSettings.EnableTargetLock)
                    {
                        var pos = Target.position;
                        transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                        _rpgCamera.ResetCanera();
                    } 
#endif
                }
                else
                {
                    Target = null;
                }
            }

#if (!UNITY_IOS && !UNITY_ANDROID)

            if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget && Target != null
                && Rm_RPGHandler.Instance.DefaultSettings.EnableTargetLock)
            {
                var pos = Target.position;
                transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                _rpgCamera.ResetCanera();
            }
#endif

            //todo: use current settings no RPGMaker ones
            var customControls = Rm_RPGHandler.Instance.DefaultSettings.DefaultControls.ControlDefinitions.Where(c => !c.IsRequiredControl).ToList();
            for (int i = 0; i < customControls.Count; i++)
            {
                var control = customControls[i];
                if(control.CustomAction == CustomControlAction.Begin_Event)
                {
                    if(RPG.Input.GetKeyDown(control.ID))
                    {
                        if(!GetObject.EventHandler.EventIsRunning(control.StringParameter))
                        {
                            GetObject.EventHandler.RunEvent(control.StringParameter);    
                        }
                    }
                }
            }

            if (!Character.Silenced)
            {
                for (int i = 1; i <= Rm_RPGHandler.Instance.Combat.SkillBarSlots; i++)
                {
                    if (RPG.Input.GetKey("Skill_" + i))
                    {
                        UseSkill(i);
                    }
                }
            }

            //if(stop.ElapsedMilliseconds > 50)
            //    Debug.Log("SPIKE " + stop.ElapsedMilliseconds);
        }

        public void UseSkill(int i)
        {
            var slotIndex = i - 1;

            if (slotIndex > GetObject.PlayerCharacter.SkillHandler.Slots.Length - 1)
                return;

            var slot = GetObject.PlayerCharacter.SkillHandler.Slots[slotIndex];
            if (slot.Usable)
            {
                if (slot.IsItem)
                {
                    slot.Use();
                }
                else
                {
                    UseRefSkill(slot.Skill);
                }
            }
            else
            {
                Debug.Log("Tried to use unusable skill bar slot");
            }
        }

        public void UseRefSkill(Skill skill)
        {
            var skillToUse = skill;
            var notCurrentSkill = CurrentQueue == null || CurrentQueue.SkillId == null || CurrentQueue.SkillId != skillToUse.ID;
            var targetable = new[] { SkillType.Aura, SkillType.Ability, SkillType.Restoration, SkillType.Projectile }.Any(s => s == skillToUse.SkillType);

            if (!notCurrentSkill) return;
            if (!skillToUse.CanCastSkill(true)) return;

            bool skillNeedsTarget;
            if (skillToUse.SkillType == SkillType.Area_Of_Effect || skillToUse.SkillType == SkillType.Spawn)
            {
                if (Rm_RPGHandler.Instance.Combat.SmartCastSkills || !ControllerChecker.UsingKeyboardMouse)
                {
                    if (_currentAction != null && _currentAction.Cancellable)
                    {
                        ForceStopHandlingActions();
                    }

                    var rotation = _rpgCamera.transform.rotation;
                    rotation.eulerAngles = new Vector3(0, _rpgCamera.transform.eulerAngles.y, 0);
                    RPGCombat.UseSkill(skillToUse, GetPositionAtMousePosition(), rotation);
                }
                else
                {
                    BeginCastArea(skillToUse);
                }
                return;
            }

            if (skillToUse.AlwaysRequireTarget || Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
            {
                skillNeedsTarget = true;
            }
            else if (skillToUse.SkillType == SkillType.Aura || skillToUse.SkillType == SkillType.Restoration || skillToUse.SkillType == SkillType.Melee)
            {
                skillNeedsTarget = true;
            }
            else
            {
                skillNeedsTarget = false;
            }

            //let's try get a target if we can:
            BaseCharacterMono target = null;
            var foundTarget = false;
            if (skillNeedsTarget || skillToUse.SkillType == SkillType.Ability)
            {
                if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
                {
                    target = Target != null ? Target.GetComponent<BaseCharacterMono>() : null;
                }
                else
                {
                    target = GetTargetOnMousePosition();
                }

                if (target == null && targetable)
                {
                    switch (skillToUse.TargetType)
                    {
                        case TargetType.Self:
                        case TargetType.SelfOrAlly:
                        case TargetType.Any:
                            target = CharacterMono;
                            break;
                    }
                }

                //if targeting enemy and spell has self

                if (skillToUse.TargetType == TargetType.Self)
                {
                    target = CharacterMono;
                }
                else if (target != null && (!IsFriendly(target)) && skillToUse.TargetType == TargetType.SelfOrAlly)
                {
                    target = CharacterMono;
                }

                foundTarget = target != null;

                if (target != null && target.Character.CharacterType == CharacterType.NPC)
                {
                    if (!Rm_RPGHandler.Instance.Combat.NPCsCanFight)
                    {
                        target = null;
                    }
                    else
                    {
                        if (!Rm_RPGHandler.Instance.Combat.CanAttackNPcs)
                        {
                            target = null;
                        }
                        else
                        {
                            var npcChar = target.Character as NonPlayerCharacter;
                            if (!npcChar.CanBeKilled && !Rm_RPGHandler.Instance.Combat.CanAttackUnkillableNPCs)
                            {
                                target = null;
                            }

                            if (!npcChar.CanFight)
                            {
                                target = null;
                            }
                        }
                    }
                }

                if (target != null)
                {
                    if (targetable &&
                        ((skillToUse.TargetType == TargetType.Self && target != CharacterMono) ||
                         (skillToUse.TargetType == TargetType.SelfOrAlly && !IsFriendly(target) && target != CharacterMono) ||
                         (skillToUse.TargetType == TargetType.Ally && !IsFriendly(target)) ||
                         (skillToUse.TargetType == TargetType.Enemy && IsFriendly(target)) ||
                         (skillToUse.TargetType == TargetType.NotSelf && target == CharacterMono) ||
                         (skillToUse.TargetType == TargetType.Any && target == null)))
                    {
                        foundTarget = false;
                    }
                }
            }

            if (foundTarget)
            {
                foundTarget = target.Character.Alive;
            }

            //cast:
            if ((skillNeedsTarget && foundTarget) || !skillNeedsTarget)
            {
                if (_currentAction != null && _currentAction.Cancellable)
                {
                    ForceStopHandlingActions();
                }

                if (foundTarget)
                {
                    RPGCombat.UseSkill(skillToUse, target);
                    if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock && !IsFriendly(target))
                    {
                        AutoAttack = true;
                    }
                }
                else if (Rm_RPGHandler.Instance.Combat.TargetStyle != TargetStyle.TargetLock)
                {
                    var targetPos = target == null ? GetPositionAtMousePosition() : target.transform.position;
                    RPGCombat.UseSkill(skillToUse, targetPos);
                }
            }
        }

        private void BeginCastArea(Skill skillToCast)
        {
            if(_showingCastArea)
            {
                EndCastArea();
            }

            _skillToCast = skillToCast;
            _showingCastArea = true;
            _castAreaGameObject.SetActive(true);

            if (skillToCast.SkillType == SkillType.Area_Of_Effect)
            {
                var aoeSkill = (AreaOfEffectSkill)skillToCast;
                if (aoeSkill.Shape == AOEShape.Sphere)
                {
                    _castAreaProjector.fieldOfView = ProjectorBaseFieldOfView * aoeSkill.Diameter;
                    _castAreaProjector.aspectRatio = ProjectorBaseAspectRatio;
                }
                else
                {
                    _castAreaProjector.fieldOfView = ProjectorBaseFieldOfView * aoeSkill.Length;
                    _castAreaProjector.aspectRatio = ProjectorBaseAspectRatio / aoeSkill.Length;

                    _castAreaProjector.aspectRatio = _castAreaProjector.aspectRatio * aoeSkill.Width;
                }
            }
            else
            {
                _castAreaProjector.fieldOfView = ProjectorBaseFieldOfView * SpawnSkillCastAreaWidth;
                _castAreaProjector.aspectRatio = ProjectorBaseAspectRatio;
            }
        }

        private void EndCastArea()
        {
            if (!IsPlayerControlled) return;
            _showingCastArea = false;
            _skillToCast = null;
            _castAreaGameObject.SetActive(false);
        }

        public Vector3 GetPositionAtMousePosition()
        {
            //Using controller
            if (!ControllerChecker.UsingKeyboardMouse)
            {
                //var leftPos = Input.GetAxis(RPG.Gamepad.LeftStickHori);
                var forwardPos = Input.GetAxis(RPG.Gamepad.LeftStickVert);

                if(forwardPos > 0 && forwardPos < 0.2f)
                {   
                    forwardPos = 1.5f;
                }

                if(forwardPos == 0)
                {
                    forwardPos = 2.0f;
                }

                var targetPos = transform.position + new Vector3(0,0.1f,0) + (transform.forward * forwardPos);
                return targetPos;
            }

            //Using Kb/m
            var ray = GetObject.RPGCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000, ~(_castAreaProjector.ignoreLayers));

            if (hit.point != Vector3.zero)
            {
                return hit.point;
            }

            return Vector3.zero;
        }

        //note: move to interface
        private BaseCharacterMono GetTargetOnMousePosition()
        {
            BaseCharacterMono foundTarget = null;


            //Using controller
            if(!ControllerChecker.UsingKeyboardMouse)
            {
                if(Target != null)
                {
                    foundTarget = Target.GetComponent<BaseCharacterMono>();
                    return foundTarget;
                }
                else
                {
                    return null;
                }
            }

            //Using Kb/M
            var ray = GetObject.RPGCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000, ClickAttackLayers.LayerMask);
            if (hit.transform != null)
            {
                var isTarget = hit.transform.GetComponent<BaseCharacterMono>();
                if (isTarget && isTarget.Character.Alive)
                {
                    foundTarget = isTarget;    
                    
                    if (isTarget.Character.CharacterType == CharacterType.NPC)
                    {
                        if (!Rm_RPGHandler.Instance.Combat.NPCsCanFight)
                        {
                            foundTarget = null;
                        }
                        else
                        {
                            if (!Rm_RPGHandler.Instance.Combat.CanAttackNPcs)
                            {
                                foundTarget = null;
                            }
                            else
                            {
                                var npcChar = isTarget.Character as NonPlayerCharacter;
                                if (!npcChar.CanBeKilled && !Rm_RPGHandler.Instance.Combat.CanAttackUnkillableNPCs)
                                {
                                    foundTarget = null;
                                }

                                if(!npcChar.CanFight)
                                {
                                    foundTarget = null;
                                }
                            }
                        }
                    }
                }
            }

            return foundTarget;
        }

        private BaseCharacterMono GetTargetOnPosition(Vector3 position)
        {
            BaseCharacterMono foundTarget = null;


            //Using controller
            if(!ControllerChecker.UsingKeyboardMouse)
            {
                if(Target != null)
                {
                    foundTarget = Target.GetComponent<BaseCharacterMono>();
                    return foundTarget;
                }
                else
                {
                    return null;
                }
            }

            //Using Kb/M
            var ray = GetObject.RPGCamera.GetComponent<Camera>().ScreenPointToRay(position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000, ClickAttackLayers.LayerMask);
            if (hit.transform != null)
            {
                var isTarget = hit.transform.GetComponent<BaseCharacterMono>();
                if (isTarget && isTarget.Character.Alive)
                {
                    foundTarget = isTarget;    
                    
                    if (isTarget.Character.CharacterType == CharacterType.NPC)
                    {
                        if (!Rm_RPGHandler.Instance.Combat.NPCsCanFight)
                        {
                            foundTarget = null;
                        }
                        else
                        {
                            if (!Rm_RPGHandler.Instance.Combat.CanAttackNPcs)
                            {
                                foundTarget = null;
                            }
                            else
                            {
                                var npcChar = isTarget.Character as NonPlayerCharacter;
                                if (!npcChar.CanBeKilled && !Rm_RPGHandler.Instance.Combat.CanAttackUnkillableNPCs)
                                {
                                    foundTarget = null;
                                }

                                if(!npcChar.CanFight)
                                {
                                    foundTarget = null;
                                }
                            }
                        }
                    }
                }
            }

            return foundTarget;
        }

        private void TryToExitCombat()
        {
            if(Character.CharacterType != CharacterType.Player)
            {
                if(Target != null)
                {
                    TimeOutOfCombat = 0;
                }
            }

            if (TimeOutOfCombat < TimeToExitCombat)
            {
                TimeOutOfCombat += Time.deltaTime;
            }
            else
            {
                InCombat = false;
            }
        }

        #region "Unity Functions"

        private void Awake()
        {
            ControlledByAI = false;
            Interacting = false;
            HandlingActions = false;
            RetreatingToSpawn = false;
            SpawnPosition = transform.position;

            CharacterController = GetComponent<CharacterController>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            RPGAnimation = GetComponent<RPGAnimation>();
            RPGCombat = GetComponent<RPGCombat>();
            RPGPatrol = GetComponent<RPGPatrol>();
            RPGFollow = GetComponent<RPGFollow>();

            CharacterController.enabled = true;
            NavMeshAgent.enabled = false;

            if(characterModel != null)
            {
                _characterAnimation = CharacterModel.GetComponent<Animation>();    
                _characterAnimator = CharacterModel.GetComponent<Animator>();    
            }

            _state = RPGControllerState.Patrol;

            _character = null;
            _legacyAnimation = null;
            _rpgCamera = GetObject.RPGCamera;   

            SceneManager.sceneLoaded += (arg0, mode) => OnLevelLoad();
        }

        private void Start()
        {
            Impact = Vector3.zero;
            _playerDirWorld = Vector3.zero;
            CharacterMono = GetComponent<BaseCharacterMono>();
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            PetMono = GetComponent<PetMono>();
            _impactDeteriorateSpeed = ImpactSpeed;
            _rpgCamera.cameraMode = Rm_RPGHandler.Instance.DefaultSettings.DefaultCameraMode;
            Interacting = false;


            if (Character.AttackStyle == AttackStyle.Melee)
            {
                NavMeshAgent.avoidancePriority = Random.Range(5, 30);
            }
            else
            {
                NavMeshAgent.avoidancePriority = Random.Range(30, 55);
            }

            if(IsPlayerControlled)
            {
                _state = RPGControllerState.PlayerControlled;

                _castAreaGameObject = GameObject.FindGameObjectWithTag("CastAreaProjector");
                _castAreaProjector = _castAreaGameObject.GetComponent<Projector>(); ;
                _castAreaProjector.gameObject.SetActive(false);
            }
            else
            {
                EnableNavMeshAgent();
            }


        }

        private void OnLevelLoad()
        {
            GetObject.ClearReferences();
            _rpgCamera = GetObject.RPGCamera;
        }

        private void Update()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();  
            if (GameMaster.CutsceneActive)
            {
                EnableCharacterController();
                return;
            }

            if(Character.CharacterType == CharacterType.NPC && !Rm_RPGHandler.Instance.Combat.NPCsCanFight) return;

            if (Character == null) return;

            if (!CharacterMono.Initialised)
            {
                return;
            }

            if(IsPlayerCharacter)
            {
                _navMeshObstacle.enabled = !NavMeshAgent.enabled;
            }

            if(Vector3.Distance(transform.position,GetObject.PlayerMonoGameObject.transform.position) > 100.0f)
            {
                if(HandlingActions)
                {
                    ForceStopHandlingActions();    
                }
                return;
            }

            _rotation = Vector3.zero;

            if (stopwatch.ElapsedMilliseconds > 10)
                Debug.Log("1 Spike!" + stopwatch.ElapsedMilliseconds.ToString());

            if(_showingCastArea)
            {
                var ray = GetObject.RPGCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 1000, ~(_castAreaProjector.ignoreLayers));

                if(hit.point != Vector3.zero)
                {
                    _castAreaProjector.transform.position = hit.point;
                    _castAreaProjector.transform.SetY(100);
                }
            }



            if (CharacterController.enabled)
            {
                if (!IsGrounded)
                {
                    if(CurrentAction == null || CurrentAction.Type != RPGActionType.JumpToPosition)
                    {
                        _timeInAir += Time.deltaTime;
                    }
                }

                IsGrounded = CharacterController.isGrounded;
                if(IsGrounded)
                {
                    if (Rm_RPGHandler.Instance.Combat.EnableFallDamage)
                    {
                        if (_timeInAir > TimeBeforeFallDamage)
                        {
                            Character.VitalHandler.TakeFallDamage(_timeInAir);
                        }
                    }
                    _timeInAir = 0;
                }
            }

            if (!Character.Alive) return;

            //todo:remove testing
            {
                if (IsPlayerControlled)
                {
                    HandleKeybinds();
                    //if (stopwatch.ElapsedMilliseconds > 10)
                    //    Debug.Log("Keybind Spike!" + stopwatch.ElapsedMilliseconds.ToString());
                }
            }
            
            CheckTargetStatus();
            //if (stopwatch.ElapsedMilliseconds > 10)
            //    Debug.Log("3 Spike!" + stopwatch.ElapsedMilliseconds.ToString());

            if(IsPlayerControlled)
            {
                GetActions();
                //if (stopwatch.ElapsedMilliseconds > 10)
                //    Debug.Log("PLAYER Spike!" + stopwatch.ElapsedMilliseconds.ToString());
            }
            else
            {
                GetAIActions();
                //if (stopwatch.ElapsedMilliseconds > 10)
                //    Debug.Log("AI Spike!" + stopwatch.ElapsedMilliseconds.ToString());
            }

            //if (stopwatch.ElapsedMilliseconds > 10)
            //    Debug.Log("UPDATE Spike!" + stopwatch.ElapsedMilliseconds.ToString());
        }

        private void LateUpdate()
        {
            if (Character == null) return;
            if (GameMaster.CutsceneActive) return;

            if (_waitingToRevive)
            {
                if(!GetObject.PlayerController.InCombat)
                {
                    if(Target == null || !Target.GetComponent<RPGController>().Character.Alive)
                    {
                        var playerPos = GetObject.PlayerMonoGameObject.transform;
                        PetMono.SpawnPet(GetObject.PlayerSave.CurrentPet, playerPos.position - playerPos.forward);
                        _waitingToRevive = false;
                    }
                }
            }

            if(!_handlingCharacterDeath && !Character.Alive)
            {
                if(Character.AnimationType == RPGAnimationType.Mecanim)
                {
                    Animator.SetBool("isDead", true);
                }
                _playerDirWorld = Vector3.zero;
                if(NavMeshAgent.enabled)
                {
                    NavMeshAgent.destination = transform.position;    
                }

                _handlingCharacterDeath = true;
                
                if(PetMono != null)
                {
                    InCombat = false;
                    ForceStopHandlingActions();
                    var petQueue = RPGActionQueue.Create();
                    var petDeathAnim = LegacyAnimation.DeathAnim.Animation;
                    var petAnimLength = !string.IsNullOrEmpty(petDeathAnim) ? CharacterModel.gameObject.GetComponent<Animation>()[petDeathAnim].length : 1.0f;
                    if (!string.IsNullOrEmpty(petDeathAnim) || Character.AnimationType == RPGAnimationType.Mecanim)
                    {
                        petQueue.Add(RPGActionFactory.PlayAnimation(LegacyAnimation.DeathAnim));
                        if (LegacyAnimation.DeathAnim.Sound != null)
                        {
                            petQueue.Add(RPGActionFactory.PlaySound(new AudioContainer() { AudioPath = LegacyAnimation.DeathAnim.SoundPath }, AudioType.SoundFX));
                        }
                    }
                    BeginActionQueue(petQueue);
                    _waitingToRevive = true;
                    return;
                }

                if(Character.CharacterType != CharacterType.Player)
                {
                    if(Character.CharacterType == CharacterType.Enemy)
                    {
                        Destroy(gameObject,5);
                    }
                    else
                    {
                        //todo: check NPC can die
                    }
                }

                InCombat = false;
                ForceStopHandlingActions();
                EndCastArea();
                Character.ClearStats();

                var deathAnim = LegacyAnimation.DeathAnim.Animation;
                var animLength = !string.IsNullOrEmpty(deathAnim) ? CharacterModel.gameObject.GetComponent<Animation>()[deathAnim].length : 1.0f;
                

                var queue = RPGActionQueue.Create();
                var cc = Character as CombatCharacter;
                if(cc != null && !string.IsNullOrEmpty(cc.PrefabReplacementOnDeath))
                {

                    //var charMono = CharacterMono;
                    var charModel = CharacterModel.transform;
                    var ragdoll = GeneralMethods.SpawnPrefab(cc.PrefabReplacementOnDeath,
                                                charModel.position,
                                                charModel.rotation,
                                                null);
                    ragdoll.transform.localScale = transform.localScale;

                    ragdoll.transform.parent = transform;

                    var myCollider = transform.GetComponent<BoxCollider>();
                    Destroy(myCollider);
                    Transform[] ragdollJoints = ragdoll.GetComponentsInChildren<Transform>();
                    Transform[] currentJoints = charModel.GetComponentsInChildren<Transform>();

                    for (int i = 0; i < ragdollJoints.Length; i++)
                    {
                        for (int q = 0; q < currentJoints.Length; q++)
                        {
                            if (currentJoints[q].name == ragdollJoints[i].name)
                            {
                                ragdollJoints[i].position = currentJoints[q].position;
                                ragdollJoints[i].rotation = currentJoints[q].rotation;
                                break;
                            }
                        }
                    }

                    CharacterModel.SetActive(false);
                }
                else
                {

                    if (!string.IsNullOrEmpty(deathAnim) || Character.AnimationType == RPGAnimationType.Mecanim)
                    {
                        queue.Add(RPGActionFactory.PlayAnimation(LegacyAnimation.DeathAnim));
                        if(LegacyAnimation.DeathAnim.Sound != null)
                        {
                            queue.Add(RPGActionFactory.PlaySound(new AudioContainer() { AudioPath = LegacyAnimation.DeathAnim.SoundPath }, AudioType.SoundFX));    
                        }
                    }
                }
                
                
                queue.Add(RPGActionFactory.WaitForSeconds(animLength - 0.1f));
                if(IsPlayerCharacter)
                {
                    Target = null;
                    AutoAttack = false;
                    queue.Add(RPGActionFactory.RespawnPlayer());    
                }
                else
                {

                    if(IsPlayerControlled)
                    {
                        SetPlayerControl(GetObject.PlayerMonoGameObject);
                    }

                    if(Character.CharacterType == CharacterType.Enemy)
                    {
                        var myCollider = transform.GetComponent<BoxCollider>();
                        Destroy(myCollider);
                        queue.Add(RPGActionFactory.RemoveCombatant(transform));        
                    }
                    else
                    {
                        var npcChar = Character as NonPlayerCharacter;
                        if(npcChar.CanBeKilled)
                        {
                            var myCollider = transform.GetComponent<BoxCollider>();
                            Destroy(myCollider);
                            queue.Add(RPGActionFactory.RemoveCombatant(transform));        
                        }
                        else
                        {
                            queue.Add(RPGActionFactory.RespawnNpc());
                        }
                    }
                    
                }
                
                BeginActionQueue(queue);
            }

            TryToExitCombat();

            SetApproachPositions();

            if(!ControlledByAI || (_currentAction != null && _currentAction.UseDefaultAnimations))
                DoAnimations();
        }

        private void FixedUpdate()
        {
            if (Character == null) return;
            if (GameMaster.CutsceneActive) return;

            if(Character.AnimationType == RPGAnimationType.Mecanim)
            {
                Animator.SetBool("jumping", !IsGrounded);
            }
            //Debug.Log("IsGrounded: " + IsGrounded);

            if(GameMaster.isMobile)
            {
                MovePlayerMobile();
            }
            else
            {
                MovePlayer();    
            }
        }

        void OnCollisionStay(Collision collisionInfo)
        {
            if (CharacterController.enabled) return;
            IsGrounded = true;
        }

        void OnCollisionExit(Collision collisionInfo)
        {
            if (CharacterController.enabled) return;

            
            IsGrounded = false;
        }

        void OnColliisionEnter(Collision col)
        {
            if (_currentAction != null && _currentAction.Type == RPGActionType.JumpToPosition && _playerDirWorld.y > 0)
            {
                Debug.Log("Stopped 'JumpToPosition' action and modified queue");

                var actionIndex = _curQueue.Actions.IndexOf(_currentAction);
                var newQueueActions = _curQueue.Actions.GetRange(actionIndex + 1, _curQueue.Actions.Count - (actionIndex + 1));
                newQueueActions.Insert(0, RPGActionFactory.WaitToLand());

                var queue = RPGActionQueue.Create(newQueueActions);
                queue.HasTarget = _curQueue.HasTarget;
                queue.HasTargetPos = _curQueue.HasTargetPos;
                queue.Target = _curQueue.Target;
                queue.TargetPos = transform.position;
                queue.SkillId = _curQueue.SkillId;


                //Cancel Jump
                _stopAIJump = false;
                Impact = Vector3.zero;
                _playerDirWorld = Vector3.zero;
                TargetReached = true;

                ForceStopHandlingActions();
                BeginActionQueue(queue);
            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _stopAIJump = !CharacterController.isGrounded;
        }

        #endregion
    }
}