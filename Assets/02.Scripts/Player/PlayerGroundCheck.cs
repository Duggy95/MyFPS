using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    TestPlayerMovement playerMovement;
    void Awake()
    {
        playerMovement = GetComponentInParent<TestPlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject)
            return;
        playerMovement.SetGround(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject)
            return;
        playerMovement.SetGround(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject)
            return;
        playerMovement.SetGround(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerMovement.gameObject)
            return;
        playerMovement.SetGround(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerMovement.gameObject)
            return;
        playerMovement.SetGround(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playerMovement.gameObject)
            return;
        playerMovement.SetGround(true);
    }
}
