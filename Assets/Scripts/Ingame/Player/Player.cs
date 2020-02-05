using UnityEngine;
using System;
using System.Collections;

public class Player : LivingObject
{
    static Player _instance = null;
    public static Player instance
    {
        get
        {
            if (_instance == null)
                _instance = (Player)FindObjectOfType(typeof(Player));

            return _instance;
        }
    }

    public enum PlayerAnimation { Running, HorizontalSlash, VerticalSlash, DiagonalSlash, VerticalSlashTopDown, DoubleSlash, Jump };

    [Serializable]
    public class TouchMembers
    {
        public int raycastCount = 10;
        [HideInInspector]
        public bool started = false;
        [HideInInspector]
        public Vector2 start, end;
        [HideInInspector]
		public float startTime, endTime;
    };

    CharacterController _characterController;
    Vector3 _velocity;
    bool _canJump = true;
	
	[HideInInspector]
	public int score = 0;
	
	int _killCount = 0;
	public int killCount
	{
		get { return _killCount; }
		set
		{
			_killCount = value;
			lastKillTime = Time.time;
			
			ComboManager.instance.SpawnCombo(new Vector2(0.5f, 0.98f), 2, _killCount);
			
			if(_killCount % 10 == 0)
				moveSpeed += 0.1f;				
		}
	}
	
	[HideInInspector] 
	public float previousLastKillTime;
	
	float _lastKillTime	= 0;
	public float lastKillTime	
	{
		get { return _lastKillTime; } 
		set
		{
			previousLastKillTime = _lastKillTime;
			_lastKillTime = value;
		}
	}
	
	float zPosition;
	float oldMoveSpeed;
	
    public float jumpSpeed = 16;
    public float inAirMultiplier = 0.25f;
	
    public TouchMembers touch;
    public Transform[] weapons;
	
	public AudioClip[] slashAudioClips;	
	public PlayerAnimation currentAnimation;	
	
	public bool godMode;
	
	public GameObject particlesObject;

    Transform _leftHandRef, _leftHandWeapon;
    public Transform leftHandWeapon
    {
        get { return _leftHandWeapon; }
        set
        {
            if (_leftHandWeapon != null)
                Destroy(_leftHandWeapon.gameObject);

            _leftHandWeapon = (Transform)Instantiate(value);
            _leftHandWeapon.position = _leftHandRef.position;
            _leftHandWeapon.rotation = _leftHandRef.rotation;
            _leftHandWeapon.parent = _leftHandRef;

            Weapon weapon = _leftHandWeapon.GetComponent<Weapon>();
            weapon.weaponSide = Weapon.WeaponSide.Left;

            foreach (Transform trans in _leftHandWeapon)
            {
                if (trans.collider != null)
                    Physics.IgnoreCollision(collider, trans.collider);
            }
        }
    }

    Transform _rightHandRef, _rightHandWeapon;
    public Transform rightHandWeapon
    {
        get { return _rightHandWeapon; }
        set
        {
            if (_rightHandWeapon != null)
                Destroy(_rightHandWeapon.gameObject);

            _rightHandWeapon = (Transform)Instantiate(value);
            _rightHandWeapon.position = _rightHandRef.position;
            _rightHandWeapon.rotation = _rightHandRef.rotation;
            _rightHandWeapon.parent = _rightHandRef;

            Weapon weapon = _rightHandWeapon.GetComponent<Weapon>();
            weapon.weaponSide = Weapon.WeaponSide.Right;
				
            foreach (Transform trans in _rightHandWeapon)
            {
                if (trans.collider != null)
					Physics.IgnoreCollision(collider, trans.collider);
            }
        }
    }

    public override void Awake()
    {
        base.Awake();

        _characterController = GetComponent<CharacterController>();

        _leftHandRef = Functions.SearchHierarchyForbones(rootObject, "Bip01 L Hand")[0];
        _rightHandRef = Functions.SearchHierarchyForbones(rootObject, "Bip01 R Hand")[0];

        rootObject.animation.Stop();
		
		zPosition = transform.position.z;

        foreach (AnimationState state in rootObject.animation)
        {
            if (!state.name.Contains("Slash"))
                continue;

            state.wrapMode = WrapMode.Once;
            state.blendMode = AnimationBlendMode.Additive; 
            state.layer = 1; 
            state.speed = 1;
        }
    }

    public override void Start()
    {
        base.Start();

        leftHandWeapon = weapons[0];
        rightHandWeapon = weapons[1];
    }

    public override void Update()
    {
        base.Update();
		
		if(GameManager.instance.isPaused)
			return;
		
		GameManager.instance.playerkillCount = killCount;
		GameManager.instance.playerScore = score;		
		if(score > GameManager.instance.playerHighScore)
			GameManager.instance.playerHighScore = score;

		if(health > 0)
		{
			if (Input.GetKeyDown(KeyCode.Keypad0))
				godMode = !godMode;
				
			HandleSlicing();
			HandleMovement();
		}
		
		transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
		if (hit.collider.tag == "Floor")
            return;
        
        if (hit.collider.tag == "Enemy" && !godMode)
            health--;
    }

    void HandleSlicing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touch.start = Input.mousePosition;
			touch.startTime = Time.time;
            touch.started = true;
        }

        if (Input.GetMouseButtonUp(0) && touch.started)
        {
            touch.end = Input.mousePosition;
			touch.endTime = Time.time;
			
            if (Vector3.Distance(touch.start, touch.end) > 50)
            {
				float angle = Mathf.Rad2Deg * Mathf.Atan2(touch.end.x - touch.start.x, touch.end.y - touch.start.y);
				if (angle < 0)
					angle += 360f;
				
				float z = Camera.main.transform.position.z - transform.position.z;				
				float distance = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(touch.end.x, touch.end.y, z)), 
					Camera.main.ScreenToWorldPoint(new Vector3(touch.start.x, touch.start.y, z))); // in meters..
					
				float speed = distance / (touch.endTime - touch.startTime); // in m/s
				//
				
				speed /= 15f;
				speed = Mathf.Clamp(speed, 1, 2);
				
				//HorizontalSlash, VerticalSlash, DiagonalSlash, VerticalSlashTopDown, DoubleSlash
				if(angle >= 60f && angle <= 120f || angle >= 240f && angle <= 300f) // beide kanten op
					currentAnimation = PlayerAnimation.HorizontalSlash;
				else if(angle >= 330f || angle >= 0 && angle <= 30)
					currentAnimation = PlayerAnimation.VerticalSlash;
				else if(angle >= 30f && angle <= 90f || angle >= 300 && angle <= 330)
					currentAnimation = PlayerAnimation.DiagonalSlash;				
				else if(angle >= 150f && angle <= 210f || angle >= 90 && angle <= 150)
					currentAnimation = PlayerAnimation.VerticalSlashTopDown;
				
				rootObject.animation[currentAnimation.ToString()].speed = speed;
				rootObject.animation.Play(currentAnimation.ToString());
				
				Functions.PlayAudioClip(slashAudioClips[UnityEngine.Random.Range(0, slashAudioClips.Length)], transform.position, 0.8f);
				
				_rightHandWeapon.GetComponent<Weapon>().InstantiateTrailRenderer(rootObject.animation[currentAnimation.ToString()].length);
				
                SliceManager.instance.SpawnSlice(new Vector2(touch.start.x, Screen.height - touch.start.y),
                        new Vector2(touch.end.x, Screen.height - touch.end.y), 3);
            }

            touch.started = false;
        }
    }

    void HandleMovement()
    {
        Vector3 movement = transform.forward * moveSpeed;
        if (_characterController.isGrounded)
        {
            /*bool jump = false;
            if (touch.jumpTouchPad != null)
            {
                if (!touch.jumpTouchPad.IsFingerDown())
                    _canJump = true;

                if (_canJump && touch.jumpTouchPad.IsFingerDown())
                {
                    jump = true;
                    _canJump = false;
                }
            }

            if (jump)
            {
                _velocity = _characterController.velocity;
                _velocity.y = jumpSpeed;

                rootObject.animation.CrossFade("Jump");
            }
            else*/ if (moveSpeed > 0)
                rootObject.animation.CrossFade("Running");
            else
				rootObject.animation.Stop("Running");	
        }
        else
        {
            _velocity.y += Physics.gravity.y * Time.deltaTime;
            movement.x *= inAirMultiplier;
        }

        movement += _velocity;
        movement += Physics.gravity;
        movement *= Time.deltaTime;

        _characterController.Move(movement);

        if (_characterController.isGrounded)           
            _velocity = Vector3.zero;
    }
	
	void OnFadeOutCompleted()
	{
		Application.LoadLevel("GameOver_Scene");
	}

	public void Pause(bool isPaused)
	{
		if(isPaused)
		{
			godMode = true;
			particlesObject.SetActiveRecursively(false);
			
			oldMoveSpeed = moveSpeed;
			moveSpeed = 0;
		}
		else
		{
			godMode = false;
			particlesObject.SetActiveRecursively(true);
			
			moveSpeed = oldMoveSpeed;
		}
	}
	
    public override void Die()
    {				
        base.Die();
		
		moveSpeed = 0;
		
		if(GameObject.Find("InGameGUI_Prefab") != null)
			GameObject.Find("InGameGUI_Prefab").GetComponent<InGameGUI>().gameOverGUITexture.active = true;
		
		CameraFade cameraFade = Camera.main.GetComponent<CameraFade>();		
		cameraFade.OnFadeOutCompleted += OnFadeOutCompleted;
		cameraFade.FadeOut();
    }
}