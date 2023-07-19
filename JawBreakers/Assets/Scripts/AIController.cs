using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIController : MonoBehaviour
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

    [SerializeField]
    bool grounded, attacking, crouching, lookingUp, grabbed, moving;
    bool canUseLevel1, canUseLevel2, canUseLevel3, canUseLevel4, canUseLevel5;

    [SerializeField]
    float healthPercent, sugarRushMeter, caramalMeter;

    [SerializeField]
    int weight, candy;

    float timeStamp, moveEndlag;
    float caramalTimeStamp, maxCaramelEffectTimerMillis;
    float grabTimeStamp, grabRate;
    [SerializeField]
    float moveTimeStamp, maxMoveTimerMillis;

    Rigidbody2D rbody;

    PlayerCombat combat;
    PlayerMovement movement;
    LedgeDetection detection;

    [SerializeField]
    PlayerStates playerState;

    [SerializeField]
    int randomMove;

    private void Start()
    {
        combat = GetComponent<PlayerCombat>();
        movement = GetComponent<PlayerMovement>();
        rbody = GetComponent<Rigidbody2D>();
        detection = ledgeDetection.GetComponent<LedgeDetection>();
        timeStamp = 0;
        caramalTimeStamp = 0;
        grabTimeStamp = 0;
        moveTimeStamp = 0;
        moveEndlag = 2.0f;
        grabRate = 2.0f;
        maxMoveTimerMillis = 5.0f;
        maxCaramelEffectTimerMillis = 5.0f;
        sugarRushMeter = 0;
        caramalMeter = 0;
        grabbed = false;
        candy = 3;
        playerState = PlayerStates.Idle;
    }

    void Update()
    {
        healthText.text = "Health: " + healthPercent + "%";
        sugarRushText.text = "SugarRush: " + sugarRushMeter;
        caramalText.text = "Caramal: " + caramalMeter;
        candyText.text = "Candy: " + candy;

        if (transform.position.x <= leftBlastZone.transform.position.x
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

        if (healthPercent >= 999)
        {
            healthPercent = 999;
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

        if (caramalMeter >= MaxCaramalMeter)
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

        switch (playerState)
        {
            case PlayerStates.Idle:
                movement.Stand();
                movement.LookForward();
                crouching = false;
                lookingUp = false;
                moving = false;
                movement.Stop();

                moveTimeStamp += Time.deltaTime;
                
                if(moveTimeStamp >= maxMoveTimerMillis)
                {
                    randomMove = Random.Range(0, 10);

                    if (randomMove == 0)
                    {
                        playerState = PlayerStates.Move;
                    }
                    else if (randomMove == 1)
                    {
                        playerState = PlayerStates.Crouch;
                    }
                    else if (randomMove == 2)
                    {
                        playerState = PlayerStates.LookUp;
                    }
                    else if (randomMove == 3)
                    {
                        playerState = PlayerStates.Jump;
                    }
                    else if (randomMove == 4)
                    {
                        playerState = PlayerStates.LightAttack;
                    }
                    else if (randomMove == 5)
                    {
                        playerState = PlayerStates.MediumAttack;
                    }
                    else if (randomMove == 6)
                    {
                        playerState = PlayerStates.StrongAttack;
                    }
                    else if (randomMove == 7)
                    {
                        playerState = PlayerStates.Special;
                    }
                    else if (randomMove == 8)
                    {
                        playerState = PlayerStates.Grab;
                    }
                    else if (randomMove == 9)
                    {
                        playerState = PlayerStates.Shield;
                    }
                    moveTimeStamp = 0;
                }
                break;
            case PlayerStates.Move:
                moving = true;
                if (grounded)
                {
                    if(crouching || lookingUp)
                    {
                        movement.Walk();
                    }
                    else
                    {
                        movement.Run();
                    }
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

                movement.Move();

                moveTimeStamp += Time.deltaTime;

                if (moveTimeStamp >= maxMoveTimerMillis)
                {
                    randomMove = Random.Range(0, 10);

                    if (randomMove == 0)
                    {
                        playerState = PlayerStates.Idle;
                    }
                    else if (randomMove == 1)
                    {
                        playerState = PlayerStates.Crouch;
                    }
                    else if (randomMove == 2)
                    {
                        playerState = PlayerStates.LookUp;
                    }
                    else if(randomMove == 3)
                    {
                        playerState = PlayerStates.Jump;
                    }
                    else if (randomMove == 4)
                    {
                        playerState = PlayerStates.LightAttack;
                    }
                    else if (randomMove == 5)
                    {
                        playerState = PlayerStates.MediumAttack;
                    }
                    else if (randomMove == 6)
                    {
                        playerState = PlayerStates.StrongAttack;
                    }
                    else if (randomMove == 7)
                    {
                        playerState = PlayerStates.Special;
                    }
                    else if (randomMove == 8)
                    {
                        playerState = PlayerStates.Grab;
                    }
                    else if (randomMove == 9)
                    {
                        playerState = PlayerStates.Shield;
                    }

                    moveTimeStamp = 0;
                }
                break;
            case PlayerStates.Crouch:
                movement.Crouch();
                movement.LookForward();
                crouching = true;
                lookingUp = false;

                moveTimeStamp += Time.deltaTime;

                if (moveTimeStamp >= maxMoveTimerMillis)
                {
                    randomMove = Random.Range(0, 10);

                    if (randomMove == 0)
                    {
                        playerState = PlayerStates.Idle;
                    }
                    else if (randomMove == 1)
                    {
                        playerState = PlayerStates.Move;
                    }
                    else if (randomMove == 2)
                    {
                        playerState = PlayerStates.LookUp;
                    }
                    else if (randomMove == 3)
                    {
                        playerState = PlayerStates.Jump;
                    }
                    else if (randomMove == 4)
                    {
                        playerState = PlayerStates.LightAttack;
                    }
                    else if (randomMove == 5)
                    {
                        playerState = PlayerStates.MediumAttack;
                    }
                    else if (randomMove == 6)
                    {
                        playerState = PlayerStates.StrongAttack;
                    }
                    else if (randomMove == 7)
                    {
                        playerState = PlayerStates.Special;
                    }
                    else if (randomMove == 8)
                    {
                        playerState = PlayerStates.Grab;
                    }
                    else if (randomMove == 9)
                    {
                        playerState = PlayerStates.Shield;
                    }
                    moveTimeStamp = 0;
                }
                break;
            case PlayerStates.LookUp:
                movement.LookUp();
                movement.Stand();
                crouching = false;
                lookingUp = true;

                moveTimeStamp += Time.deltaTime;

                if (moveTimeStamp >= maxMoveTimerMillis)
                {
                    randomMove = Random.Range(0, 10);

                    if (randomMove == 0)
                    {
                        playerState = PlayerStates.Idle;
                    }
                    else if (randomMove == 1)
                    {
                        playerState = PlayerStates.Move;
                    }
                    else if (randomMove == 2)
                    {
                        playerState = PlayerStates.Crouch;
                    }
                    else if (randomMove == 3)
                    {
                        playerState = PlayerStates.Jump;
                    }
                    else if (randomMove == 4)
                    {
                        playerState = PlayerStates.LightAttack;
                    }
                    else if (randomMove == 5)
                    {
                        playerState = PlayerStates.MediumAttack;
                    }
                    else if (randomMove == 6)
                    {
                        playerState = PlayerStates.StrongAttack;
                    }
                    else if (randomMove == 7)
                    {
                        playerState = PlayerStates.Special;
                    }
                    else if (randomMove == 8)
                    {
                        playerState = PlayerStates.Grab;
                    }
                    else if (randomMove == 9)
                    {
                        playerState = PlayerStates.Shield;
                    }
                    moveTimeStamp = 0;
                }
                break;
            case PlayerStates.Jump:
                if(grounded)
                {
                    movement.GroundJump();

                    int doubleJump = Random.Range(0, 25);

                    if(doubleJump == 0)
                    {
                        movement.DoubleJump();
                    }
                }
                else
                {
                    movement.DoubleJump();
                }

                randomMove = Random.Range(0, 5);

                if(randomMove == 0)
                {
                    playerState = PlayerStates.Move;
                }
                else if (randomMove == 1)
                {
                    playerState = PlayerStates.LightAttack;
                }
                else if (randomMove == 2)
                {
                    playerState = PlayerStates.MediumAttack;
                }
                else if (randomMove == 3)
                {
                    playerState = PlayerStates.StrongAttack;
                }
                else if (randomMove == 4)
                {
                    playerState = PlayerStates.Special;
                }
                break;
            case PlayerStates.LightAttack:
                timeStamp += Time.deltaTime;
                attacking = true;
                if (grounded)
                {
                    combat.GroundLightAttack();
                    CheckDirection();
                }
                else
                {
                    combat.AirLightAttack();
                    CheckDirection();
                }

                if (timeStamp >= moveEndlag)
                {
                    EndMove();
                }
                break;
            case PlayerStates.MediumAttack:
                    timeStamp += Time.deltaTime;
                    attacking = true;
                    if (grounded)
                    {
                        combat.GroundMediumAttack();
                        CheckDirection();
                    }
                    else
                    {
                        combat.AirMediumAttack();
                        CheckDirection();
                    }

                if (timeStamp >= moveEndlag)
                {
                    EndMove();
                }
                break;
            case PlayerStates.StrongAttack:
                timeStamp += Time.deltaTime;
                    attacking = true;
                    if (grounded)
                    {
                        combat.GroundStrongAttack();
                    CheckDirection();
                }
                    else
                    {
                        combat.AirStrongAttack();
                    CheckDirection();
                }

                if (timeStamp >= moveEndlag)
                {
                    EndMove();
                }
                break;
            case PlayerStates.Special:
                timeStamp += Time.deltaTime;
                attacking = true;
                combat.Special();
                CheckDirection();

                if (timeStamp >= moveEndlag)
                {
                    EndMove();
                }
                break;
            case PlayerStates.Grab:
                attacking = true;
                if (grounded)
                {
                    combat.Grab();
                }
                if (grounded)
                {
                    movement.Stop();
                }

                moving = false;

                timeStamp += Time.deltaTime;

                if (timeStamp >= moveEndlag)
                {
                    EndMove();
                }
                break;
            case PlayerStates.Shield:
                combat.Shield();
                movement.Stop();
                moving = false;

                moveTimeStamp += Time.deltaTime;

                if(moveTimeStamp >= maxMoveTimerMillis)
                {
                    playerState = PlayerStates.Idle;
                }
                break;

            default:
                break;
        }
    }

    private void CheckDirection()
    {
        if (moving)
        {
            if (crouching)
            {
                combat.DownAttack();
            }
            else if (lookingUp)
            {
                combat.UpAttack();
            }
            else
            {
                combat.SideAttack();
            }
        }
        else if (crouching)
        {
            combat.DownAttack();
        }
        else if (lookingUp)
        {
            combat.UpAttack();
        }
        else
        {
            combat.NeutralAttack();
        }

        if (grounded)
        {
            movement.Stop();
        }
    }

    private void EndMove()
    {
        if (grounded)
        {
            movement.Run();
        }

        if (!crouching)
        {
            movement.Stand();
            movement.Run();
            crouching = false;
        }

        if (!lookingUp)
        {
            movement.LookForward();
            movement.Run();
            lookingUp = false;
        }

        attacking = false;
        timeStamp = 0;
        combat.EndMove();

        playerState = PlayerStates.Idle;
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
                rbody.velocity = Vector3.zero;
                transform.position = spawnLocation.transform.position;
            }
            else
            {
                gameObject.SetActive(false);
            }
            candy--;
            healthPercent = 0;
            caramalMeter = 0;
            healthText.text = "Health: " + healthPercent + "%";
            sugarRushText.text = "SugarRush: " + sugarRushMeter;
            caramalText.text = "Caramal: " + caramalMeter;
            candyText.text = "Candy: " + candy;
            moveTimeStamp = 0;
            combat.UnShield();
            combat.EndMove();
            movement.Stand();
            crouching = false;
            movement.LookForward();
            movement.Run();
            lookingUp = false;
            attacking = false;
            playerState = PlayerStates.Idle;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            grounded = true;
            movement.RefreshDoubleJump();

            if (attacking)
            {
                movement.Stand();
                crouching = false;
                movement.LookForward();
                movement.Run();
                lookingUp = false;
                attacking = false;
                timeStamp = Time.time;
                combat.EndMove();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            grounded = true;
            movement.RefreshDoubleJump();

            if (attacking)
            {
                movement.Stand();
                crouching = false;
                movement.LookForward();
                movement.Run();
                lookingUp = false;
                attacking = false;
                timeStamp = Time.time;
                combat.EndMove();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
        if (collision.gameObject.CompareTag("Platform"))
        {
            grounded = false;
            collision.gameObject.GetComponent<PlatformEffector2D>().rotationalOffset = 0;
        }
    }
}

[System.Serializable]
public enum PlayerStates
{
    Idle,
    Move,
    Crouch,
    LookUp,
    Jump,
    LightAttack,
    MediumAttack,
    StrongAttack,
    Special,
    Grab,
    Shield
}