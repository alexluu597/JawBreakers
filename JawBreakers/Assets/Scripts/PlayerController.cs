using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    const float MaxLevel1SugarRush = 50.0f;
    const float MaxLevel2SugarRush = 110.0f;
    const float MaxLevel3SugarRush = 170.0f;
    const float MaxLevel4SugarRush = 230.0f;
    const float MaxLevel5SugarRush = 290.0f;
    const float MaxCaramalMeter = 100.0f;

    [SerializeField]
    GameObject head, spawnLocation, ledgeDetection, leftBlastZone, rightBlastZone, topBlastZone, bottomBlastZone;

    [SerializeField]
    Material caramalMaterial, playerMaterial;

    [SerializeField]
    Text healthText, sugarRushText, caramalText, candyText;

    bool grounded, attacking, crouching, lookingUp, grabbed, shield;
    bool canUseLevel1, canUseLevel2, canUseLevel3, canUseLevel4, canUseLevel5;

    [SerializeField]
    float healthPercent, sugarRushMeter, caramalMeter;

    [SerializeField]
    int weight, candy;

    [SerializeField]
    int playerControllerNumber;
    [SerializeField]
    int airDashCount;

    float timeStamp, moveEndlag;
    float caramalTimeStamp, maxCaramelEffectTimerMillis;
    float grabTimeStamp, grabRate;

    Rigidbody2D rbody;

    PlayerCombat combat;
    PlayerMovement movement;
    LedgeDetection detection;

    PlayerInput player;
    Gamepad pGamepad;

    private void Start()
    {
        combat = GetComponent<PlayerCombat>();
        movement = GetComponent<PlayerMovement>();
        rbody = GetComponent<Rigidbody2D>();
        detection = ledgeDetection.GetComponent<LedgeDetection>();
        timeStamp = 0;
        caramalTimeStamp = 0;
        grabTimeStamp = 0;
        moveEndlag = 1.0f;
        grabRate = 2.0f;
        maxCaramelEffectTimerMillis = 5.0f;
        sugarRushMeter = 0;
        caramalMeter = 0;
        grabbed = false;
        shield = false;
        candy = 3;
        player = GetComponent<PlayerInput>();
        if(player.currentControlScheme == "Gamepad")
        {
            pGamepad = player.devices[0] as Gamepad;
        }
    }

    void Update()
    {
        healthText.text = "Health: " + healthPercent + "%";
        sugarRushText.text = "SugarRush: " + sugarRushMeter;
        caramalText.text = "Caramal: " + caramalMeter;
        candyText.text = "Candy: " + candy;

        if(transform.position.x <= leftBlastZone.transform.position.x
            || transform.position.x >= rightBlastZone.transform.position.x
            || transform.position.y >= topBlastZone.transform.position.y 
            || transform.position.y <= bottomBlastZone.transform.position.y)
        {
            if (candy > 1)
            {
                transform.position = spawnLocation.transform.position;
                rbody.velocity = Vector3.zero;
            }
            else
            {
                gameObject.SetActive(false);
            }
            candy--;
            healthPercent = 0;
            caramalMeter = 0;
        }

        if(healthPercent >= 999)
        {
            healthPercent = 999;
        }

        if (shield)
        {
            movement.Stop();
        }
        else
        {
            if (grounded)
            {
                movement.Run();
            }
            else
            {
                if (detection.DetectLedge == true)
                {
                    movement.Stop();
                    rbody.bodyType = RigidbodyType2D.Static;
                }
                else
                {
                    movement.Air();
                    rbody.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }

        if (sugarRushMeter >= MaxLevel1SugarRush)
        {
            canUseLevel1 = true;
        }
        else
        {
            canUseLevel1 = false;
        }

        if (sugarRushMeter >= MaxLevel2SugarRush)
        {
            canUseLevel2 = true;
        }
        else
        {
            canUseLevel2 = false;
        }

        if (sugarRushMeter >= MaxLevel3SugarRush)
        {
            canUseLevel3 = true;
        }
        else
        {
            canUseLevel3 = false;
        }

        if (sugarRushMeter >= MaxLevel4SugarRush)
        {
            canUseLevel4 = true;
        }
        else
        {
            canUseLevel4 = false;
        }

        if (sugarRushMeter >= MaxLevel5SugarRush)
        {
            canUseLevel5 = true;
            sugarRushMeter = MaxLevel5SugarRush;
        }
        else
        {
            canUseLevel5 = false;
        }

        if(caramalMeter >= MaxCaramalMeter)
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().material = caramalMaterial;
            caramalTimeStamp += Time.deltaTime;
            if (caramalTimeStamp >= maxCaramelEffectTimerMillis)
            {
                transform.GetChild(0).GetComponent<MeshRenderer>().material = playerMaterial;
                caramalMeter = 0;
                caramalTimeStamp = 0;
            }
        }

        if (attacking)
        {
            if (!grabbed)
            {
                if (grounded)
                {
                    movement.Stop();
                }

                if (Time.time > timeStamp + moveEndlag)
                {
                    if (grounded)
                    {
                        movement.Run();
                    }

                    if(player.currentControlScheme == "Gamepad")
                    {
                        if (!pGamepad.leftStick.down.isPressed)
                        {
                            movement.Stand();
                            movement.Run();
                            crouching = false;
                        }

                        if (!pGamepad.leftStick.down.isPressed)
                        {
                            movement.LookForward();
                            movement.Run();
                            lookingUp = false;
                        }
                    }
                    else
                    {
                        if (!Input.GetKey(KeyCode.S))
                        {
                            movement.Stand();
                            movement.Run();
                            crouching = false;
                        }

                        if (!Input.GetKey(KeyCode.W))
                        {
                            movement.LookForward();
                            movement.Run();
                            lookingUp = false;
                        }
                    }
                    
                    attacking = false;
                    timeStamp = Time.time;
                    combat.EndMove();
                }
            }
            else
            {
                if (player.currentControlScheme == "Gamepad")
                {
                    if (!pGamepad.leftStick.down.isPressed)
                    {
                        movement.Stand();
                        crouching = false;
                    }

                    if (!pGamepad.leftStick.up.isPressed)
                    {
                        movement.LookForward();
                        lookingUp = false;
                    }
                }
                else
                {
                    if (!Input.GetKey(KeyCode.S))
                    {
                        movement.Stand();
                        crouching = false;
                    }

                    if (!Input.GetKey(KeyCode.W))
                    {
                        movement.LookForward();
                        lookingUp = false;
                    }
                }
                movement.Stop();
                attacking = false;
                timeStamp = Time.time;
                combat.EndMove();

                if (Time.time > grabTimeStamp + grabRate)
                {
                    grabbed = false;
                }
            }
        }
        else
        {
            if (!grabbed)
            {
                movement.Turn();

                if (player.currentControlScheme == "Gamepad")
                {
                    if (pGamepad.leftStick.down.wasReleasedThisFrame)
                    {
                        movement.Stand();
                        movement.Run();
                        crouching = false;
                    }

                    if (pGamepad.leftStick.up.wasReleasedThisFrame)
                    {
                        movement.LookForward();
                        movement.Run();
                        lookingUp = false;
                    }

                    if (pGamepad.leftShoulder.wasReleasedThisFrame)
                    {
                        combat.UnShield();
                        shield = false;
                    }
                }
                else
                {
                    if (Input.GetKeyUp(KeyCode.S))
                    {
                        movement.Stand();
                        movement.Run();
                        crouching = false;
                    }

                    if (Input.GetKeyUp(KeyCode.W))
                    {
                        movement.LookForward();
                        movement.Run();
                        lookingUp = false;
                    }

                    if (Input.GetKeyUp(KeyCode.U))
                    {
                        combat.UnShield();
                        shield = false;
                    }
                }
            }
            else
            {
                if (player.currentControlScheme == "Gamepad")
                {
                    if (!pGamepad.leftStick.down.isPressed)
                    {
                        movement.Stand();
                        crouching = false;
                    }

                    if (!pGamepad.leftStick.up.isPressed)
                    {
                        movement.LookForward();
                        lookingUp = false;
                    }
                }
                else
                {
                    if (!Input.GetKey(KeyCode.S))
                    {
                        movement.Stand();
                        crouching = false;
                    }

                    if (!Input.GetKey(KeyCode.W))
                    {
                        movement.LookForward();
                        lookingUp = false;
                    }
                }
                movement.Stop();

                if (Time.time > grabTimeStamp + grabRate)
                {
                    grabbed = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hitbox"))
        {
            healthPercent += collision.GetComponent<Hitbox>().DamagePercent;
            sugarRushMeter += 10;
            rbody.AddForce(collision.GetComponent<Hitbox>().ApplyKnockback(healthPercent, weight), ForceMode2D.Force);
            //rbody.AddForce(new Vector3(150, 150, 0), ForceMode2D.Force);
            if (collision.GetComponent<Hitbox>().Effect == SpecialEffects.Caramal)
            {
                caramalMeter += 10;
                if (caramalMeter >= MaxCaramalMeter)
                {
                    caramalMeter = MaxCaramalMeter;
                }
            }
        }
        if (collision.CompareTag("GrabBox"))
        {
            grabbed = true;
            transform.position = collision.transform.position;
            grabTimeStamp = Time.time;
        }
        if (collision.CompareTag("Blastzone"))
        {
            if (candy > 1)
            {
                transform.position = spawnLocation.transform.position;
                rbody.velocity = Vector3.zero;
            }
            else 
            {
                gameObject.SetActive(false);
            }
            candy--;
            healthPercent = 0;
            caramalMeter = 0;
            combat.UnShield();
            healthText.text = "Health: " + healthPercent + "%";
            sugarRushText.text = "SugarRush: " + sugarRushMeter;
            caramalText.text = "Caramal: " + caramalMeter;
            candyText.text = "Candy: " + candy;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            grounded = true;
            movement.RefreshDoubleJump();
            combat.UnShield();
            airDashCount = 0;

            if (attacking)
            {
                if (player.currentControlScheme == "Gamepad")
                {
                    if (!pGamepad.leftStick.down.isPressed)
                    {
                        movement.Stand();
                        movement.Run();
                        crouching = false;
                    }

                    if (!pGamepad.leftStick.up.isPressed)
                    {
                        movement.LookForward();
                        movement.Run();
                        lookingUp = false;
                    }
                }
                else
                {
                    if (!Input.GetKey(KeyCode.S))
                    {
                        movement.Stand();
                        movement.Run();
                        crouching = false;
                    }
                    if (!Input.GetKey(KeyCode.W))
                    {
                        movement.LookForward();
                        movement.Run();
                        lookingUp = false;
                    }
                }
                attacking = false;
                timeStamp = Time.time;
                combat.EndMove();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            if (player.currentControlScheme == "Gamepad")
            {
                if (pGamepad.leftStick.down.isPressed)
                {
                    collision.gameObject.GetComponent<PlatformEffector2D>().rotationalOffset = 180;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.S))
                {
                    collision.gameObject.GetComponent<PlatformEffector2D>().rotationalOffset = 180;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
        if(collision.gameObject.CompareTag("Platform"))
        {
            grounded = false;
            collision.gameObject.GetComponent<PlatformEffector2D>().rotationalOffset = 0;
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if(pGamepad.leftStick.down.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (!lookingUp)
                    {
                        movement.Walk();
                        movement.Crouch();
                        crouching = true;
                    }
                }
            }
        }
        else
        {
            if(Input.GetKey(KeyCode.S))
            {
                if (!grabbed && !attacking)
                {
                    if (!lookingUp)
                    {
                        movement.Walk();
                        movement.Crouch();
                        crouching = true;
                    }
                }
            }
        }
    }

    public void LookUp(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if(pGamepad.leftStick.up.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (!crouching)
                    {
                        if (grounded)
                        {
                            movement.Walk();
                        }
                        else
                        {
                            movement.Air();
                        }

                        movement.LookUp();
                        lookingUp = true;
                    }
                }
            }
        }
        else
        {
            if(Input.GetKey(KeyCode.W))
            {
                if (!grabbed && !attacking)
                {
                    if (!crouching)
                    {
                        if (grounded)
                        {
                            movement.Walk();
                        }
                        else
                        {
                            movement.Air();
                        }

                        movement.LookUp();
                        lookingUp = true;
                    }
                }
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.buttonNorth.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (grounded)
                    {
                        movement.GroundJump();
                    }
                    else
                    {
                        movement.DoubleJump();
                    }
                }
            }
        }
        else
        {
            if(Input.GetKey(KeyCode.Space))
            {
                if (!grabbed && !attacking)
                {
                    if (grounded)
                    {
                        movement.GroundJump();
                    }
                    else
                    {
                        movement.DoubleJump();
                    }
                }
            }
        }
    }

    public void LightAttack(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.buttonEast.isPressed)
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        if (grounded)
                        {
                            combat.GroundLightAttack();
                            CheckAttackDirection();
                        }
                        else
                        {
                            combat.AirLightAttack();
                            CheckAttackDirection();
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.J))
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        if (grounded)
                        {
                            combat.GroundLightAttack();
                            CheckAttackDirection();
                        }
                        else
                        {
                            combat.AirLightAttack();
                            CheckAttackDirection();
                        }
                    }
                }
            }
        }
    }

    public void MediumAttack(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.buttonWest.isPressed)
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        if (grounded)
                        {
                            combat.GroundMediumAttack();
                            CheckAttackDirection();
                        }
                        else
                        {
                            combat.AirMediumAttack();
                            CheckAttackDirection();
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.K))
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        if (grounded)
                        {
                            combat.GroundMediumAttack();
                            CheckAttackDirection();
                        }
                        else
                        {
                            combat.AirMediumAttack();
                            CheckAttackDirection();
                        }
                    }
                }
            }
        }
    }

    public void StrongAttack(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.buttonSouth.isPressed)
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        if (grounded)
                        {
                            combat.GroundStrongAttack();
                            CheckAttackDirection();
                        }
                        else
                        {
                            combat.AirStrongAttack();
                            CheckAttackDirection();
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.L))
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        if (grounded)
                        {
                            combat.GroundStrongAttack();
                            CheckAttackDirection();
                        }
                        else
                        {
                            combat.AirStrongAttack();
                            CheckAttackDirection();
                        }
                    }
                }
            }
        }
    }

    public void Special(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.leftTrigger.isPressed)
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Special();
                        CheckAttackDirection();
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.I))
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Special();
                        CheckAttackDirection();
                    }
                }
            }
        }
    }

    private void CheckAttackDirection()
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.leftStick.left.isPressed || pGamepad.leftStick.right.isPressed)
            {
                if (crouching && !lookingUp)
                {
                    combat.DownAttack();
                }
                else if (lookingUp && !crouching)
                {
                    combat.UpAttack();
                }
                else if(!crouching && !lookingUp)
                {
                    combat.SideAttack();
                }
            }
            else if (crouching && !lookingUp)
            {
                combat.DownAttack();
            }
            else if (lookingUp && !crouching)
            {
                combat.UpAttack();
            }
            else if(!lookingUp && !crouching)
            {
                combat.NeutralAttack();
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (crouching && !lookingUp)
                {
                    combat.DownAttack();
                }
                else if (!crouching && lookingUp)
                {
                    combat.UpAttack();
                }
                else if(!crouching && !lookingUp)
                {
                    combat.SideAttack();
                }
            }
            else if (crouching && !lookingUp)
            {
                combat.DownAttack();
            }
            else if (!crouching && lookingUp)
            {
                combat.UpAttack();
            }
            else if(!crouching && !lookingUp)
            {
                combat.NeutralAttack();
            }
        }
    }

    public void Shield(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.leftShoulder.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (grounded)
                    {
                        combat.Shield();
                        shield = true;
                    }
                    else
                    {
                        if (airDashCount < 1)
                        {
                            airDashCount++;
                            combat.AirDash();
                        }
                    }
                }
            }
        }
        else
        {
            if(Input.GetKey(KeyCode.U))
            {
                if (!grabbed && !attacking)
                {
                    if (grounded)
                    {
                        combat.Shield();
                        shield = true;
                    }
                    else
                    {
                        if (airDashCount < 1)
                        {
                            airDashCount++;
                            combat.AirDash();
                        }
                    }
                }
            }
        }
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.rightShoulder.isPressed)
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        if (grounded)
                        {
                            combat.Grab();
                        }
                    }
                }
            }
        }
        else
        {
            if(Input.GetKey(KeyCode.O))
            {
                if (!grabbed)
                {
                    if (!attacking)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        if (grounded)
                        {
                            combat.Grab();
                        }
                    }
                }
            }
        }
    }

    public void SugarRush01(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.dpad.down.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel1)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level1SugarRush();
                        sugarRushMeter -= MaxLevel1SugarRush;
                    }
                }
            }
        }
        else
        {
            if(Input.GetKey(KeyCode.Alpha1))
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel1)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level1SugarRush();
                        sugarRushMeter -= MaxLevel1SugarRush;
                    }
                }
            }
        }
    }

    public void SugarRush02(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.dpad.left.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel2)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level2SugarRush();
                        sugarRushMeter -= MaxLevel2SugarRush;
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Alpha2))
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel2)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level2SugarRush();
                        sugarRushMeter -= MaxLevel2SugarRush;
                    }
                }
            }
        }
    }

    public void SugarRush03(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.dpad.right.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel3)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level3SugarRush();
                        sugarRushMeter -= MaxLevel3SugarRush;
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel3)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level3SugarRush();
                        sugarRushMeter -= MaxLevel3SugarRush;
                    }
                }
            }
        }
    }

    public void SugarRush04(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.dpad.up.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel4)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level4SugarRush();
                        sugarRushMeter -= MaxLevel4SugarRush;
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel4)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level4SugarRush();
                        sugarRushMeter -= MaxLevel4SugarRush;
                    }
                }
            }
        }
    }

    public void SugarRush05(InputAction.CallbackContext context)
    {
        if (player.currentControlScheme == "Gamepad")
        {
            if (pGamepad.rightTrigger.isPressed)
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel5)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level5SugarRush();
                        sugarRushMeter -= MaxLevel5SugarRush;
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (!grabbed && !attacking)
                {
                    if (canUseLevel5)
                    {
                        timeStamp = Time.time;
                        attacking = true;
                        combat.Level5SugarRush();
                        sugarRushMeter -= MaxLevel5SugarRush;
                    }
                }
            }
        }
    }

    public void RestartGame(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}