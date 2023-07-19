using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField]
    bool detectLedge;

    public bool DetectLedge { get => detectLedge; }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ledge"))
        {
            detectLedge = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ledge"))
        {
            detectLedge = false;
        }
    }
}
