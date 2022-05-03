using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    public float moveSpeed = 5f;
    public PlayerInputActions playerControls;
    public Rigidbody2D rb;
    public Animator animator;
    Vector2 movement;                                                                    //INPUT CONTAINER, CONAINS VALUE CONTAINERS FOR AXIS X AND Y IN THE SCENE
    Vector2 mousePos;
    public Camera cam;
    private InputAction move;
    private InputAction fire;
    private InputAction lunarSlash;
    private InputAction dash;
    private InputAction one;
    private InputAction groundTarget;
    public Transform firePoint;
    public GameObject fireBulletPrefab;
    public GameObject lunarSlashPrefab;
    public float fireBulletForce = 20f;
    public float lunarSlashForce = 15f;

    //indicator
    private bool FOrbPending;
    public GameObject FOrbIndicator;
    private bool groundTargetPending;
    public Texture2D cursorForGroundTarget;
    public Texture2D cursorDefault;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;



    //public int indicatorCooldown = 2;


    //fireball
    public GameObject fireballBulletPrefab;
    private float FireballCoolCounter;
    public float FireballCooldown;
    public float fireballBulletForce = 20f;

    //health
    public float maxHealth = 100f;
    private float currentHealth;
    public HealthBar healthBarOverhead;
    public HealthBar healthBar;
    public float regenAmount;
    private float healthCoolCounter;
    public float healthRegenCooldown;

    //ground target
    public GameObject go;
    private float groundTargetCoolCounter;
    public float groundTargetCooldown;


    //lunar slash
    private float lunarCoolCounter;
    public float lunarCooldown = 1f;

    //Frozen Orb
    private float FireCoolCounter;
    public float FireCooldown;

    //dash 
    private float activeMoveSpeed;
    public float dashSpeed;
    public float dashLength = .5f, dashCooldown = 1f;
    private float dashCounter;
    private float dashCoolCounter;
    private bool isInvincible = false;

    private void Awake()
    {
        Cursor.SetCursor(cursorDefault, hotSpot, cursorMode);
        Instance = this;
        playerControls = new PlayerInputActions();                               //PlayerInputActions = script generated by NEW INPUT SYSTEM when the c# option is checked for the PlayerInputActions file, we have to create a script alongside the input system file to be able to reference it in other scripts like this one. we then store the information that is collected by the PlayerInputActions script in the playerControls variable to use later
        activeMoveSpeed = moveSpeed; //this is for dash
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);//this is for health
    }

    // WHEN YOU PRESS A BUTTON    
    private void OnEnable()
    {
        move = playerControls.Player.Move;                                              //Grabbing our input values that are being generated by PlayerInputActions script from the container we are storing them in (playerControls) and designating what value we wan't to use 
        move.Enable();                                                                  // Enabling that information to be accessed by the UPDATE method when the key is pressed down (thats what the OnEnable function is for in the first place)

        fire = playerControls.Player.Fire;
        fire.Enable();
        //fire.performed += Fire;

        lunarSlash = playerControls.Player.LunarSlash;
        lunarSlash.Enable();
        //lunarSlash.performed += LunarSlash;        --- moved this to update so i could add a cooldown.

        dash = playerControls.Player.Dash;
        dash.Enable();

        one = playerControls.Player.One;
        one.Enable();

        groundTarget = playerControls.Player.GroundTarget;
        groundTarget.Enable();
    }

    //WHEN YOU RELEASE IT
    private void OnDisable()
    {
        move.Disable();                                                                 //tells the UPDATE method below that there is no more input once the key is released
        fire.Disable();
        lunarSlash.Disable();
        dash.Disable();
    }

    // Update is called once per frame (use this for input)
    void Update()
    {
        //movement.x = Input.GetAxisRaw("Horizontal");                                  //OLD INPUT system
        //movement.y = Input.GetAxisRaw("Vertical");                                    //OLD INPUT system
        movement = move.ReadValue<Vector2>();                                           //NEW INPUT system ---- updating our vector2(information type) movement container with the values stored in the move container defined above, which gets information from PlayerInputActions. this information is updated once per frame, so we don't want to put the equation that tells the engine to move here, because it would change based on the users framerate, so we will call upon this data in FixedUpdate when we perform the equation caluclating our movespeed
        animator.SetFloat("Horizontal", movement.x);                                    // INPUT FOR ANIMATIONS (TELLS THE ANIMATOR WHICH ANIMATION TO USE FOR EACH DIRECTION
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);                              //TELLS THE ANIMATOR IF WE ARE MOVING
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());   // updates mouse position of the user in NEW INPUT SYSTEM
        //DASH
        if (playerControls.Player.Dash.triggered)
        {
            if (dashCoolCounter <= 0 && dashCounter <= 0)
            {
                activeMoveSpeed = dashSpeed;
                isInvincible = true;
                dashCounter = dashLength;
            }

            if (isInvincible == true)
            {
                //Debug.Log("Player is INVINCIBLE!");
            }
        }

        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;

            if (dashCounter <= 0)
            {
                activeMoveSpeed = moveSpeed;
                isInvincible = false;
                dashCoolCounter = dashCooldown;
                //Debug.Log("Player is VULNERABLE!");
            }
        }

        if (dashCoolCounter > 0)
        {
            dashCoolCounter -= Time.deltaTime;
        }

        //LUNARSLASH
        /*if (playerControls.Player.LunarSlash.triggered)
        {
            if (lunarCoolCounter <= 0)
            {
                GameObject bullet = Instantiate(lunarSlashPrefab, firePoint.position, firePoint.rotation);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(firePoint.up * lunarSlashForce, ForceMode2D.Impulse);
                lunarCoolCounter = lunarCooldown;
            }
        }

        if (lunarCoolCounter > 0)
        {
            lunarCoolCounter -= Time.deltaTime;
        }*/

        //Frozen Orb
        /*if (playerControls.Player.Fire.triggered)
        {
            if (FireCoolCounter <= 0)
            {
                GameObject bullet = Instantiate(fireBulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(firePoint.up * fireBulletForce, ForceMode2D.Impulse);
                FireCoolCounter = FireCooldown;
            }
        }

        if (FireCoolCounter > 0)
        {
            FireCoolCounter -= Time.deltaTime;
        }*/


        //Fireball
        if (playerControls.Player.One.triggered)
        {
            if (FireballCoolCounter <= 0)
            {
                GameObject bullet = Instantiate(fireballBulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(firePoint.up * fireballBulletForce, ForceMode2D.Impulse);
                FireballCoolCounter = FireballCooldown;
            }
        }

        if (FireballCoolCounter > 0)
        {
            FireballCoolCounter -= Time.deltaTime;
        }

        //Passive Regen
        if (currentHealth < maxHealth)
        {
            if (healthCoolCounter <= 0)
            {
                UpdateHealth(+regenAmount);
                healthCoolCounter = healthRegenCooldown;
            }
        }

        if (healthCoolCounter > 0)
        {
            healthCoolCounter -= Time.deltaTime;
        }

        //groundtarget
        /*if (playerControls.Player.GroundTarget.WasPressedThisFrame())
        {
            releasedbutton = false;
            canplace = true;
        }
        if (playerControls.Player.GroundTarget.WasReleasedThisFrame())
        {
            releasedbutton = true;
            canplace = false;
        }

        if (releasedbutton == false && canplace)
        {            
            GameObject newEnemy = Instantiate(go, mousePos , Quaternion.identity);
            Destroy(newEnemy, 2f);           
            canplace = false;
        }*/

        //First Indicator Test using frozen orb




        //Frozen Orb
        if (playerControls.Player.GroundTarget.triggered)
        {            
            if (FireCoolCounter <= 0)
            {
                Debug.Log("AIM INDICATOR FOR FROZEN ORB SPAWNED");
                FOrbIndicator.SetActive(true);
                FOrbPending = true;
            }
               
        }

        if (playerControls.Player.Fire.triggered && FOrbPending)
        {
            Debug.Log("FrozenOrb Casted");
            GameObject bullet = Instantiate(fireBulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * fireBulletForce, ForceMode2D.Impulse);
            FireCoolCounter = FireCooldown;
            FOrbIndicator.SetActive(false);
            FOrbPending = false;
        }

        if (FireCoolCounter > 0)
        {
            FireCoolCounter -= Time.deltaTime;
        }




        //ground target aoe
        if (playerControls.Player.LunarSlash.triggered)
        {
            if (groundTargetCoolCounter <= 0)
            {
                Cursor.SetCursor(cursorForGroundTarget, hotSpot, cursorMode);
                Debug.Log("AIM INDICATOR FOR GROUND TARGET SPAWNED");
                groundTargetPending = true;
            }

        }

        if (playerControls.Player.Fire.triggered && groundTargetPending)
        {
            Debug.Log("Ground Target Skill Casted");
            GameObject newEnemy = Instantiate(go, mousePos, Quaternion.identity);
            Destroy(newEnemy, 4f);
            groundTargetCoolCounter = groundTargetCooldown;
            Cursor.SetCursor(cursorDefault, hotSpot, cursorMode);
            groundTargetPending = false;
        }

        if (groundTargetCoolCounter > 0)
        {
            groundTargetCoolCounter -= Time.deltaTime;
        }

































    }

    //called 50 times per second     (use this for output)
    void FixedUpdate()
    {                                           //|NEWDASH BELOW|
        rb.MovePosition(rb.position + movement * activeMoveSpeed * Time.fixedDeltaTime);    //accessing our "moveSpeed" float(data container) which we set at the top, and accessing our "movement" data which we collect from the user once per frame(look in the void Update method above), so that we can perform the equation for movement direction and speed. (our Vector2 x,y value container "movement" for direction) (our float container "moveSpeed" for speed), and send the result of the equation off to the rigidbody2d that we named rb at the top, and using the MovePosition function possessed by the game engine (unity) to move our player's rigidbody that we assigned it in the engine.
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg -90f;
        rb.rotation = angle;    
    }

    public void UpdateHealth(float damage)
    {
        if (isInvincible) return;
        currentHealth += damage;

        healthBar.SetHealth(currentHealth);
        healthBarOverhead.SetHealth(currentHealth);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            
            FindObjectOfType<LevelManager>().Restart();
        }
    }
}