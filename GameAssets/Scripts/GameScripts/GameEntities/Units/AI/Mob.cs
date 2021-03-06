﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// AI Base script. A Rigid body is required to check for collisions. This rigidbody should always be
/// Kinematic however so to not affect the character controller.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(WeaponControl))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Resource))]
[RequireComponent(typeof(DynamicGridObstacle))]
public abstract class Mob : ActiveEntity
{

    #region Events
    public event EventHandler Killed;
    #endregion

    #region Fields
    public int maxHp = 0;
    public int Hp = 0;
    public GameObject prefab;

    private LivingState livingState = LivingState.Alive;
    protected MobSkills skills;

    /// <summary>
    /// The faction flags this mob belongs to
    /// </summary>
    private FactionFlags _factionFlags = FactionFlags.None;
    /// <summary>
    /// The flags belonging to this mob's enemies
    /// </summary>
    private FactionFlags _enemyFlags = FactionFlags.None;
    private AIPath _aiPath;
    /// <summary>
    /// The transform that indicates the mob is selected. This is visual. Not to be confused
    /// with the transform the mob is interacting with
    /// </summary>
    private Transform _selectedTransform;
    /// <summary>
    /// Mob Skill flags. I.E what abilities this mob can perform
    /// </summary>
    private MobFlags _mobFlags = MobFlags.None;
    /// <summary>
    /// What this mob is currently doing. Eg idle, moving, attacking etc
    /// </summary>
    private ActivityState _activityState = ActivityState.None;
    /// <summary>
    /// The resources this mob is currently holding
    /// </summary>
    private Resource _resource;
    /// <summary>
    /// The transform the mob is interacting with based on the current activity. 
    /// Eg if it is set to an enemy and it is within range it could be attacking this.
    /// </summary>
    private Transform _actionTransform;
    private ActiveEntity _actionEntity;
    private Animator _anim;
    private WeaponControl _weaponcontrol;
    /// <summary>
    /// If 0 the mob can attack if not it has to tick down until the mob can attack again.
    /// Influenced by attackSpeed mobskill.
    /// </summary>
    private float _attackTime = 0f;
    private string _mobName = "undefined";
    private LocomotionState _locomotionState = LocomotionState.Idle;
    /// <summary>
    /// Time it takes to recognize a mob is dead. I.e the dying state.
    /// </summary>
    protected float deathTime = 0.45f;
    private float _dyingTime = 0.0f;
    private float _lastActionTime = 0.0f;
    private Transform _healthPivot;

    #endregion

    #region Properties
    public override FactionFlags FactionFlags
    {
        get
        {
            return _factionFlags;
        }
        set
        {
            _factionFlags = value;
        }
    }
    public Transform HealthPivot
    {
        get { return _healthPivot; }
    }
    public override FactionFlags EnemyFlags
    {
        get
        {
            return _enemyFlags;
        }
        set
        {
            _enemyFlags = value;
        }
    }

    public AIPath AIPath
    {
        get { return _aiPath; }
    }
    public Animator Animator
    {
        get { return _anim; }
    }
    public WeaponControl Weapon
    {
        get { return _weaponcontrol; }
    }

    /// <summary>
    /// If this mob has been selected by the unit select controller.
    /// </summary>
    public override bool isSelected
    {
        get
        {
            return base.isSelected;
        }
        set
        {
            base.isSelected = value;
            _selectedTransform.gameObject.SetActive(value);
        }
    }
    public MobSkills Skills
    {
        get { return skills; }
    }
    public MobFlags MobAbiltiyFlags
    {
        get { return _mobFlags; }
        protected set
        {
            _mobFlags = value;
        }
    }
    public ActivityState CurrentActivity
    {
        get { return _activityState; }
        protected set
        {
            switch (value)
            {
                case ActivityState.Attacking:
                    Animator.SetBool("weaponDrawn", true);
                    break;
                case ActivityState.None:
                    // Reset values
                    Animator.SetBool("weaponDrawn", false);
                    break;
            }
            _activityState = value;
        }
    }
    /// <summary>
    /// The cached transform of the entity we are interacting with
    /// </summary>
    public Transform ActionTransform
    {
        get { return _actionTransform; }
        set { _actionTransform = value; }
    }
    /// <summary>
    /// The ActiveEntity we are currently interacting with
    /// </summary>
    public ActiveEntity ActionEntity
    {
        get { return _actionEntity; }
        set { _actionEntity = value; }
    }
    public Resource Resource
    {
        get { return _resource; }
    }
    /// <summary>
    /// Checks to see if the mobs attack timer has reached 0. Everytime the mob attacks
    /// it resets the attack timer to the attack speed of the mob and counts back down to 0
    /// allowing it to attack once again.
    /// </summary>
    public bool CanAttack
    {
        get { return _attackTime == 0; }
    }
    public LivingState MobLivingState
    {
        get
        {
            return livingState;
        }
        set
        {
            switch (value)
            {
                case LivingState.Dying:
                    //Disable AI Path to prevent movement
                    AIPath.enabled = false;
                    //Remove all enemies and reset entity flags
                    enemies.Clear();
                    FactionFlags = global::FactionFlags.None;
                    EnemyFlags = global::FactionFlags.None;
                    SelectableList.RemoveSelectableEntity(this);
                    isSelected = false;
                    break;
                case LivingState.Dead:
                    // Fire dead events
                    if (Killed != null)
                        Killed(this, new EventArgs());
                    MobKilled();
                    break;
            }
            livingState = value;
        }
    }
    public string MobName
    {
        get { return _mobName; }
        set { _mobName = value; }
    }
    public LocomotionState MobLocoMotionState
    {
        get { return _locomotionState; }
        set { _locomotionState = value; }
    }
    #endregion

    #region States

    /// <summary>
    /// Activity States. Special refferes to a mob specific special state such as
    /// Building, cutting wood, mining etc etc
    /// </summary>
    public enum ActivityState
    {
        None,
        Attacking,
        Collecting,
        Mining,
        Woodcutting,
        Building
    }

    public enum LivingState
    {
        Alive,
        Dying,
        Dead
    }

    public enum LocomotionState
    {
        Idle,
        Walking,
        Running
    }

    #region Flags

    [Flags]
    public enum MobFlags
    {
        None = 0x0,
        CanBuild,
        CanCollect,
        CanWoodcut,
        CanAttack,
        CanMine,
    }

    #endregion

    #endregion

    #region UnityMethodCalls

    protected override void Awake()
    {
        base.Awake();
        skills = new MobSkills();
        _anim = GetComponent<Animator>();
        _weaponcontrol = GetComponent<WeaponControl>();
    }

    protected override void Start()
    {
        base.Start();
        SelectableList.AddSelectableEntity(this);
        if (tag != "Mob")
            Debug.LogWarning(gameObject.ToString() + "'s tag is not set to Mob!");
        if (gameObject.layer != 11)
            Debug.LogWarning(gameObject.ToString() + "'s layer is not set to Object!");
        if (transform.FindChild("_lineOfSight") == null)
            Debug.LogWarning(gameObject.ToString() + "'s does not have a Line of Sight transform!");
        _healthPivot = transform.FindChild("_healthPivot");
        if (_healthPivot == null)
            Debug.LogWarning(gameObject.ToString() + "'s does not have a '_healthPivot' transform!");

        if (HUDRoot.go == null)
        {
            Debug.LogWarning("Hud Root is null!");
        }
        else
        {
            GameObject child = NGUITools.AddChild(HUDRoot.go, prefab);
            // Make the UI follow the target
            child.AddComponent<UIFollowTarget>().target = _healthPivot;
        }

        rigidbody.isKinematic = true;
        Hp = maxHp;
        _aiPath = GetComponent<AIPath>();
        _selectedTransform = transform.FindChild("_selectedTransform");
        if (_selectedTransform == null)
            Debug.LogWarning(gameObject.ToString() + "does not have a select transform, don't forget to add one!");
        SelectableList.AddSelectableEntity(this);
        _resource = GetComponent<Resource>();

        // Set Speed
        AIPath.speed = skills.speed;
        skills.actionSpeed = 0.5f;

    }

    protected virtual void Update()
    {
        switch (livingState)
        {
            case LivingState.Alive:
                LivingUpdate();
                break;
            case LivingState.Dead:
                DeadUpdate();
                break;
            case LivingState.Dying:
                DyingUpdate();
                break;
        }
    }

    protected virtual void LivingUpdate()
    {
        _attackTime = Math.Max(_attackTime - Time.deltaTime, 0);

    }
    protected virtual void DyingUpdate()
    {
        _dyingTime = Mathf.Min(_dyingTime + Time.deltaTime, deathTime);
        if (_dyingTime == deathTime)
        {
            MobLivingState = LivingState.Dead;
            return;
        }
    }
    protected virtual void DeadUpdate() { }

    #endregion

    #region MobLogic

    public void Attack(IDamageable target, int damage)
    {
        if (CanAttack)
        {
            Animator.SetTrigger("combo");
            target.Damage(damage);
            _attackTime = skills.attackSpeed;
        }
    }

    /// <summary>
    /// This method essentially calls PerformAction on another ActiveEntity from this current mob.
    /// Unlike accessing PerformAction directly this method takes into account the mobs action speed
    /// i.e how fast a mob can perform an action in succession.
    /// </summary>
    /// <param name="actionEvent"></param>
    public void TryPerformAction(PerformActionEvent actionEvent, ActiveEntity otherEntity)
    {
        if (Time.time - _lastActionTime > skills.actionSpeed)
        {
            _lastActionTime = Time.time;
            otherEntity.PerformAction(actionEvent);
        }
    }

    /// <summary>
    /// Damages the mob with the specified amount
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>Returns true if the mob was damages otherwise it returns false. 
    /// Returning false could indicate the mob is in a state where it can't be damaged
    /// such as dead
    /// </returns>
    public override bool Damage(int damage)
    {
        if (MobLivingState == LivingState.Alive)
        {
            Hp = Math.Max(Hp - damage, 0);
            if (Hp == 0)
            {
                MobLivingState = LivingState.Dying;
                return false;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// An internal method that is called when the mob dies. Mobs inherting this class that wish to handle
    /// the mob death can just override this method.
    /// </summary>
    protected virtual void MobKilled()
    {
        // Set Killed Animation
        Animator.SetTrigger("Died");
        
    }

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);

        // Avoid trying to set commands on self
        if (actionEvent.entity == this)
            return;
        switch (actionEvent.tag)
        {
            case "Ground":
                CurrentActivity = ActivityState.None;
                AIPath.targetCoord = actionEvent.vector3Args[0];
                break;
            case "Mob":
                if (IsEnemey(actionEvent.entity.FactionFlags))
                {
                    // The action is an enemy. Set mode to attacking and move to position
                    // Note that the pathfinding AI determains the attack distance before attacking events are fired
                    CurrentActivity = ActivityState.Attacking;
                    SetEntityAndFollow(actionEvent.entity);
                }
                break;
            case "Building":
                if (IsEnemey(actionEvent.entity.FactionFlags))
                {
                    CurrentActivity = ActivityState.Attacking;
                    SetEntityAndFollow(actionEvent.entity);
                }
                break;
            case "BluePrint":
                if (!IsEnemey(actionEvent.entity.FactionFlags) && (MobAbiltiyFlags & MobFlags.CanBuild) == MobFlags.CanBuild)
                {
                    CurrentActivity = ActivityState.Building;
                    SetEntityAndFollow(actionEvent.entity);
                }
                break;
            case "Tree":
                if ((MobAbiltiyFlags & MobFlags.CanWoodcut) == MobFlags.CanWoodcut)
                {
                    CurrentActivity = ActivityState.Woodcutting;
                    SetEntityAndFollow(actionEvent.entity);
                }
                break;
        }
    }

    /// <summary>
    /// Mob specific method which executes a given action only after the elapsed amount of time relative to the mobs
    /// action speed has passed since a last action was executed. This method may be subject to change but in the games current
    /// state this will remain mob specific
     /// </summary>
    protected bool ExecuteAction(Action action)
    {
        if (Time.time - _lastActionTime > skills.actionSpeed)
        {
            _lastActionTime = Time.time;
            action();
            return true;
        }
        return false;
    }

    #endregion


    #region Helpers

    public bool IsEnemey(FactionFlags flags)
    {
        return (EnemyFlags & flags) != global::FactionFlags.None;
    }

    public float distanceToTarget()
    {
        return Vector3.Distance(transform.position, ActionTransform.position);
    }
    protected void SetEntityAndFollow(ActiveEntity entity)
    {
        ActionEntity = entity;
        ActionTransform = entity.transform;
        AIPath.target = entity.transform;
    }

    #endregion

    #region Triggers

    protected override void OnTriggerLOSEnter(TriggerData data)
    {
        base.OnTriggerLOSEnter(data);
        if (IsEnemey(data.entity.GetComponent<ActiveEntity>().FactionFlags))
            enemies.Add(data.entity.GetComponent<ActiveEntity>());
    }

    protected override void OnChaseLOSExit(TriggerData data)
    {
        base.OnChaseLOSExit(data);
        enemies.Remove(data.entity.GetComponent<ActiveEntity>());
    }

#endregion

}

public struct MobSkills
{
    public float actionSpeed;

    public float speed;
    public int attackPower;
    public float attackRange;
    public float attackSpeed;

    public int buildPower;
    public float buildSpeed;

}
