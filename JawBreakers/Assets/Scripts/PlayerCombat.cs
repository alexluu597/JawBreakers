using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    GameObject shield, neutralMove, sideMove, upMove, downMove, grabMove, sugarRushMove;

    [SerializeField]
    Material[] groundMaterials, airMaterials, sugarRushMaterials;

    [SerializeField]
    Material specialMaterial, grabMaterial;

    Rigidbody2D rbody;

    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        neutralMove.SetActive(false);
        sideMove.SetActive(false);
        upMove.SetActive(false);
        downMove.SetActive(false);
        shield.SetActive(false);
        grabMove.SetActive(false);
        sugarRushMove.SetActive(false);
    }

    public void EndMove()
    {
        neutralMove.SetActive(false);
        sideMove.SetActive(false);
        upMove.SetActive(false);
        downMove.SetActive(false);
        grabMove.SetActive(false);
        sugarRushMove.SetActive(false);
    }

    public void NeutralAttack()
    {
        neutralMove.SetActive(true);
    }

    public void SideAttack()
    {
        sideMove.SetActive(true);
    }

    public void UpAttack()
    {
        upMove.SetActive(true);
    }

    public void DownAttack()
    {
        downMove.SetActive(true);
    }

    public void GroundLightAttack()
    {
        neutralMove.GetComponent<MeshRenderer>().material = groundMaterials[0];
        sideMove.GetComponent<MeshRenderer>().material = groundMaterials[0];
        upMove.GetComponent<MeshRenderer>().material = groundMaterials[0];
        downMove.GetComponent<MeshRenderer>().material = groundMaterials[0];
    }

    public void GroundMediumAttack()
    {
        neutralMove.GetComponent<MeshRenderer>().material = groundMaterials[1];
        sideMove.GetComponent<MeshRenderer>().material = groundMaterials[1];
        upMove.GetComponent<MeshRenderer>().material = groundMaterials[1];
        downMove.GetComponent<MeshRenderer>().material = groundMaterials[1];
    }

    public void GroundStrongAttack()
    {
        neutralMove.GetComponent<MeshRenderer>().material = groundMaterials[2];
        sideMove.GetComponent<MeshRenderer>().material = groundMaterials[2];
        upMove.GetComponent<MeshRenderer>().material = groundMaterials[2];
        downMove.GetComponent<MeshRenderer>().material = groundMaterials[2];
    }

    public void AirLightAttack()
    {
        neutralMove.GetComponent<MeshRenderer>().material = airMaterials[0];
        sideMove.GetComponent<MeshRenderer>().material = airMaterials[0];
        upMove.GetComponent<MeshRenderer>().material = airMaterials[0];
        downMove.GetComponent<MeshRenderer>().material = airMaterials[0];
    }

    public void AirMediumAttack()
    {
        neutralMove.GetComponent<MeshRenderer>().material = airMaterials[1];
        sideMove.GetComponent<MeshRenderer>().material = airMaterials[1];
        upMove.GetComponent<MeshRenderer>().material = airMaterials[1];
        downMove.GetComponent<MeshRenderer>().material = airMaterials[1];
    }

    public void AirStrongAttack()
    {
        neutralMove.GetComponent<MeshRenderer>().material = airMaterials[2];
        sideMove.GetComponent<MeshRenderer>().material = airMaterials[2];
        upMove.GetComponent<MeshRenderer>().material = airMaterials[2];
        downMove.GetComponent<MeshRenderer>().material = airMaterials[2];
    }

    public void Special()
    {
        neutralMove.GetComponent<MeshRenderer>().material = specialMaterial;
        sideMove.GetComponent<MeshRenderer>().material = specialMaterial;
        upMove.GetComponent<MeshRenderer>().material = specialMaterial;
        downMove.GetComponent<MeshRenderer>().material = specialMaterial;
    }

    public void Shield()
    {
        shield.SetActive(true);
    }

    public void UnShield()
    {
        shield.SetActive(false);
    }

    public void AirDash()
    {
        rbody.velocity = Vector3.zero;
        if(transform.localEulerAngles.y == 0)
        {
            rbody.AddForce(new Vector3(5, 2.5f, 0), ForceMode2D.Impulse);
        }
        else if (transform.localEulerAngles.y == 180)
        {
            rbody.AddForce(new Vector3(-5, 2.5f, 0), ForceMode2D.Impulse);
        }
    }

    public void Grab()
    {
        grabMove.SetActive(true);
        grabMove.GetComponent<MeshRenderer>().material = grabMaterial;
    }

    public void Level1SugarRush()
    {
        sugarRushMove.SetActive(true);
        sugarRushMove.GetComponent<MeshRenderer>().material = sugarRushMaterials[0];
    }

    public void Level2SugarRush()
    {
        sugarRushMove.SetActive(true);
        sugarRushMove.GetComponent<MeshRenderer>().material = sugarRushMaterials[1];
    }

    public void Level3SugarRush()
    {
        sugarRushMove.SetActive(true);
        sugarRushMove.GetComponent<MeshRenderer>().material = sugarRushMaterials[2];
    }

    public void Level4SugarRush()
    {
        sugarRushMove.SetActive(true);
        sugarRushMove.GetComponent<MeshRenderer>().material = sugarRushMaterials[3];
    }
    public void Level5SugarRush()
    {
        sugarRushMove.SetActive(true);
        sugarRushMove.GetComponent<MeshRenderer>().material = sugarRushMaterials[4];
    }
}