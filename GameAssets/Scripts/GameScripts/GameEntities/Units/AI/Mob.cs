using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

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
public class Mob : ActiveEntity, ISelectable, IResource, IUnitName, ICitymanager, IFactionFlag, ICurrencyContainer
{

    #region Events & Delegates
    public event MobAction Killed;
    public MobAction JobTask;
    #endregion

    #region Fields
    private LivingState livingState = LivingState.Alive;
    protected MobSkills skills;

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
    private float _lastActionTime = 0.0f;
    private Transform _healthPivot;
    private House _house;
    private JobBuilding _jobBuilding;
    private PerformActionVariables _performActionVariables;

    #endregion

    #region Properties
    public Gender Gender { get; set; }
    public PerformActionVariables PerformActionVariables
    {
        get { return _performActionVariables; }
        set { _performActionVariables = value; }
    }
    public JobBuilding JobBuilding
    {
        get { return _jobBuilding; }
        set
        {
            if (value == null)
                CityManager.UnemployedCitizens.Add(this);
            else
                CityManager.UnemployedCitizens.Remove(this);
            _jobBuilding = value;
        }
    }
    public int Currency
    {
        get;
        set;
    }
    public bool HasJobBuilding
    {
        get { return _jobBuilding != null; }
    }
    public bool HasJobTask
    {
        get { return JobTask == null; }
    }
    public CityManager CityManager
    {
        get;
        set;
    }
    public FactionFlags FactionFlags
    {
        get;
        set;
    }
    public Transform HealthPivot
    {
        get { return _healthPivot; }
    }
    public FactionFlags EnemyFlags
    {
        get;
        set;
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

    public string UnitName
    {
        get;
        set;
    }

    bool _isSelected;
    /// <summary>
    /// If this mob has been selected by the unit select controller.
    /// </summary>
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;
            _selectedTransform.gameObject.SetActive(IsSelected);
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
                    AIPath.target = null;
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
        get;
        set;
    }
    /// <summary>
    /// Checks to see if the mobs attack timer has reached 0. Everytime the mob attacks
    /// it resets the attack timer to the attack speed of the mob and counts back down to 0
    /// allowing it to attack once again.
    /// </summary>
    private bool CanAttack
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
                    FactionFlags = global::FactionFlags.None;
                    EnemyFlags = global::FactionFlags.None;
                    IsSelected = false;
                    // Fire dead events
                    if (Killed != null)
                        Killed(this);
                    MobKilled();
                    break;
            }
            livingState = value;
        }
    }
    #endregion

    #region UnityMethodCalls

    public override void Awake()
    {
        base.Awake();
        skills = new MobSkills(2f, Random.Range(1.25f, 1.75f), 1, 5f, 0.5f, 1, 0.5f);
        FactionFlags = global::FactionFlags.one;
        _anim = GetComponent<Animator>();
        _weaponcontrol = GetComponent<WeaponControl>();
    }

    public override void Start()
    {
        base.Start();
        // How much Currency this mob brings initially
        Currency = 1000;
        // Set Gender
        if (Gender == global::Gender.NotSet)
        {
            Gender = UnityEngine.Random.Range(0, 1) == 0 ? Gender.Male : Gender.Female;
        }
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
        /*
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
         */

        rigidbody.isKinematic = true;
        _aiPath = GetComponent<AIPath>();
        _selectedTransform = transform.FindChild("_selectedTransform");
        if (_selectedTransform == null)
            Debug.LogWarning(gameObject.ToString() + "does not have a select transform, don't forget to add one!");
        Resource = GetComponent<Resource>();

        // Set Speed
        AIPath.speed = skills.speed;

        UnitName = "No Name";

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
        _attackTime = System.Math.Max(_attackTime - Time.deltaTime, 0);
        if (ActionEntity == null && CurrentActivity != ActivityState.None)
        {
            CurrentActivity = ActivityState.None;
            Debug.LogWarning("Action Entity was null, resetting CurrentActivity");
        }
        if (JobTask != null)
            JobTask(this);
        if (ActionEntity != null)
        {
            switch (CurrentActivity)
            {
                case ActivityState.Attacking:
                    if (distanceToTarget() < 10f)
                    {
                        // Attacking not yet supported
                    }
                    break;
                case ActivityState.Supplying:
                    if (distanceToTarget() < 10f)
                    {
                        TryPerformAction(PerformActionVariables, ActionEntity);
                    }
                    break;
                case ActivityState.Woodcutting:
                    if (distanceToTarget() < 10f)
                    {
                        TryPerformAction(PerformActionVariables, ActionEntity);
                        Animator.SetTrigger("chopWood");
                    }
                    break;
                case ActivityState.Retrieving:
                    if (distanceToTarget() < 10f)
                    {
                        TryPerformAction(PerformActionVariables, ActionEntity);
                    }
                    break;
                case ActivityState.Building:
                    if (distanceToTarget() < 10f)
                    {
                        TryPerformAction(PerformActionVariables, ActionEntity);
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
    private void TryPerformAction(PerformActionVariables actionEvent, ActiveEntity otherEntity)
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
    public override void PerformAction(PerformActionVariables actionEvent)
    {
        base.PerformAction(actionEvent);
        // Avoid trying to set commands on self
        if (actionEvent.entity == this)
            return;
        // Null if not applicable. If this is accessed it is only because the referenced tags assumes this interface
        // has been implemented
        IFactionFlag otherFlags = this;
        switch (actionEvent.tag)
        {
            case "Ground":
                CurrentActivity = ActivityState.None;
                AIPath.targetCoord = actionEvent.vector3Args[0];
                break;
            case "Mob":
                if (IsEnemey(otherFlags.FactionFlags))
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
                if (IsEnemey(otherFlags.FactionFlags))
                {
                    CurrentActivity = ActivityState.Attacking;
                    SetEntityAndFollow(actionEvent.entity);
                }
                else
                {
                    actionEvent.entity.PerformAction(new PerformActionVariables(this));
                }
                break;
            case "Tree":
                CurrentActivity = ActivityState.Woodcutting;
                SetEntityAndFollow(actionEvent.entity);
                break;
            case "BluePrint":
                if (IsEnemey(otherFlags.FactionFlags))
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
    NotSet,
    Male,
    Female
}

public enum LivingState
{
    Alive,
    Dead
}

#region Flags

[System.Flags]
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