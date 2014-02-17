using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#region Delegates
public delegate void MobAction(Mob mob);
#endregion

/// <summary>
/// AI Base script. A Rigid body is required to check for collisions. This rigidbody should always be
/// Kinematic however so to not affect the character controller.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(WeaponControl))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Resource))]
[RequireComponent(typeof(DynamicGridObstacle))]
public class Mob : ActiveEntity
{

    #region Events & Delegates
    public event MobAction Killed;
    public MobAction JobTask;
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
    private float _lastActionTime = 0.0f;
    private Transform _healthPivot;
    private House _house;
    private CityManager _cityManager;
    private Gender _gender;

    #endregion

    #region Properties
    public CityManager CityManager
    {
        get { return _cityManager; }
        set { _cityManager = value; }
    }
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
    public House House
    {
        get { return _house; }
        set { _house = value; }
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
            _selectedTransform.gameObject.SetActive(isSelected);
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
        set
        {
            switch (value)
            {
                case ActivityState.Attacking:
                    Animator.SetBool("weaponDrawn", true);
                    break;
                case ActivityState.None:
                    // Reset values
                    ActionEntity = null;
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
                case LivingState.Dead:
                    AIPath.enabled = false;
                    enemies.Clear();
                    FactionFlags = global::FactionFlags.None;
                    EnemyFlags = global::FactionFlags.None;
                    SelectableList.RemoveSelectableEntity(this);
                    isSelected = false;
                    // Fire dead events
                    if (Killed != null)
                        Killed(this);
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

    #region UnityMethodCalls

    protected override void Awake()
    {
        base.Awake();
        skills = new MobSkills(0.5f, 3f, 1, 5f, 0.5f, 1, 0.5f);
        FactionFlags = global::FactionFlags.one;
        _anim = GetComponent<Animator>();
        _weaponcontrol = GetComponent<WeaponControl>();
    }

    protected override void Start()
    {
        base.Start();
        SelectableList.AddSelectableEntity(this);
        if (tag != "Mob")
            Debug.LogWarning(gameObject.ToString() + "'s tag is not set to Mob!");
        if (transform.FindChild("_selectedTransform") == null)
            Debug.LogWarning(gameObject.ToString() + " does not have a select transform!");
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
        }
    }
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
                MobLivingState = LivingState.Dead;
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

    protected virtual void LivingUpdate()
    {
        _attackTime = Math.Max(_attackTime - Time.deltaTime, 0);
        if (ActionEntity == null)
            CurrentActivity = ActivityState.None;
        if (JobTask != null)
            JobTask(this);
        if (ActionEntity != null)
        {
            switch (CurrentActivity)
            {
                case ActivityState.Attacking:
                    if (distanceToTarget() < 10f)
                    {
                        Attack(ActionEntity, skills.attackPower);
                    }
                    break;
                case ActivityState.Supplying:
                    if (distanceToTarget() < 10f)
                    {
                        TryPerformAction(new PerformActionEvent(this, tag), ActionEntity);
                    }
                    break;
                case ActivityState.Woodcutting:
                    if (distanceToTarget() < 10f)
                    {
                        TryPerformAction(new PerformActionEvent(this, tag), ActionEntity);
                        Animator.SetTrigger("chopWood");
                    }
                    break;
                case ActivityState.Retrieving:
                    if (distanceToTarget() < 10f)
                    {
                        TryPerformAction(new PerformActionEvent(this), ActionEntity);
                    }
                    break;
                case ActivityState.Building:
                    if (distanceToTarget() < 10f)
                    {
                        TryPerformAction(new PerformActionEvent(this), ActionEntity);
                    }
                    break;
            }
        }
    }
    protected virtual void DeadUpdate() { }

    /// <summary>
    /// This method essentially calls PerformAction on another ActiveEntity from this current mob.
    /// Unlike accessing PerformAction directly this method takes into account the mobs action speed
    /// i.e how fast a mob can perform an action in succession.
    /// </summary>
    /// <param name="actionEvent"></param>
    private void TryPerformAction(PerformActionEvent actionEvent, ActiveEntity otherEntity)
    {
        if (Time.time - _lastActionTime > skills.actionSpeed)
        {
            _lastActionTime = Time.time;
            otherEntity.PerformAction(actionEvent);
        }
    }

    /// <summary>
    /// Have this mob react to an ActionEvent
    /// </summary>
    /// <param name="actionEvent"></param>
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
                else
                {
                    SetEntityAndFollow(actionEvent.entity);
                }
                break;
            case "Building":
                if (IsEnemey(actionEvent.entity.FactionFlags))
                {
                    CurrentActivity = ActivityState.Attacking;
                    SetEntityAndFollow(actionEvent.entity);
                }
                else
                {
                    actionEvent.entity.PerformAction(new PerformActionEvent(this));
                }
                break;
            case "Tree":
                    CurrentActivity = ActivityState.Woodcutting;
                    SetEntityAndFollow(actionEvent.entity);
                break;
            case "BluePrint":
                    if (!IsEnemey(actionEvent.entity.FactionFlags))
                    {
                        CurrentActivity = ActivityState.Supplying;
                        SetEntityAndFollow(actionEvent.entity);
                    }
                break;
        }
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
    public void SetEntityAndFollow(ActiveEntity entity)
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

    public MobSkills(float actionSpeed, float speed, int attackPower, float attackRange, float attackSpeed, int buildPower, float buildSpeed)
    {
        this.actionSpeed = actionSpeed;
        this.speed = speed;
        this.attackPower = attackPower;
        this.attackRange = attackRange;
        this.attackSpeed = attackSpeed;
        this.buildPower = buildPower;
        this.buildSpeed = buildSpeed;
    }

}

#region States

/// <summary>
/// Activity States. Special refferes to a mob specific special state such as
/// Building, cutting wood, mining etc etc
/// </summary>
public enum ActivityState
{
    None,
    Attacking,
    Supplying,
    Retrieving,
    Mining,
    Woodcutting,
    Building
}

public enum Gender
{
    Male,
    Female
}

public enum LivingState
{
    Alive,
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
    CanSupply,
    CanWoodcut,
    CanAttack,
    CanMine,
}

#endregion

#endregion

public struct MobNeeds
{
    public float hunger;
    public float tiredness;
}
