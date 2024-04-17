using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.Experimental.Rendering.Universal;

public class Character : MonoBehaviour
{
    // Photon View
    public PhotonView photonView;

    public new Light2D light;

    #region Public Variables

    [Header("Animater")]
    public Animator animator;

    [Header("Character UI")]
    public GameObject characterUI;

    #endregion

    #region Basic Character Stats

    [Header("Max Character Stat")]
    public float MAX_LIGHT_RADIUS;
    public float MAX_FLASH_DURATION;

    [Header("Character Stats")]
    public float movementSpeed;
    public float lightRadius;

    [Header("Flash Light Exposure")]
    public float flashDuration;

    [Header("Character Status")]
    public bool canMove;

    #endregion

    #region Character Handlers

    /// <summary>
    /// This method handles user inputs for player movement.
    /// </summary>
    public void handleMovement()
    {
        if (!canMove)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized * movementSpeed * Time.deltaTime;
        Vector2 position = new Vector2(this.transform.position.x, this.transform.position.y);

        //this.GetComponent<Rigidbody2D>().MovePosition(position + movement);
        this.transform.Translate(new Vector3(movement.x, movement.y, 0));
    }

    /// <summary>
    /// This method handles animations for the explorer.
    /// </summary>
    public void handleAnimationParameters()
    {
        animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
    }

    #endregion
}
