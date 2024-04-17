using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explorer : Character, IPunObservable
{
    #region Public Static Variables

    public const float shakeSpeed = 1f;
    public const float shakeAmount = 2f;
    public const float timeTilCompletelyCapsuled = 100f;
    public const float defaultSurvivalGaugeFillAmount = 2f;

    #endregion

    #region Public Variables

    [Header("Found Interactables")]
    public GameObject foundRitual;
    public GameObject foundItem;
    public GameObject foundChest;
    public GameObject foundExplorer;

    [Header("Performing Ritual")]
    public GameObject performingRitual;

    [Header("FlashLight")]
    public FlashLight flashLight;

    [Header("Explorer Audio Manager")]
    public ExplorerAudioManager audioManager;

    [Header("Captivator Reference")]
    public Captivator captivatorRef;

    #endregion

    #region Player Stats / Status Variables

    [Header("MAX STAT")]
    public int MAX_HP;
    public float MAX_MOVEMENT_SPEED;

    // Lower is better for these gauges
    public float MAX_ESCAPE_GAUGE;
    public float MAX_RESCUE_GAUGE;

    // Higer is better for this gauge
    public float MAX_SURVIVAL_GAUGE;

    [Header("Explorer Specific Stats")]
    public int hp;
    public float escapeGauge;
    public float survivalGauge;
    public float survivalGaugeFillAmount;
    public float rescueGauge;
    public float lanternDuration;
    public int gender;
    public float terrorRadius;

    [Header("Explorer Status")]
    public bool capsuled;
    public bool onPodium;
    public bool completelyCapsuled;
    public bool isInvulnerable;

    #endregion

    #region Unity Update/Start

    void Start()
    {
        photonView = this.GetComponent<PhotonView>();
        characterUI = GameObject.FindGameObjectWithTag("CharacterUI");
        completelyCapsuled = false;
        capsuled = false;
        survivalGauge = MAX_SURVIVAL_GAUGE;
        hp = MAX_HP;

        PhotonNetwork.AddCallbackTarget(this);

        if (photonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.TEAM_KEY] = "Explorer";
            PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.CHARACTER_VIEW_KEY] = photonView.ViewID;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
    }

    void Update()
    {
        updateOnAllClients();

        if (!photonView.IsMine)
            return;

        // Find Captivator Reference
        findCaptivatorReference();

        handleAnimationParameters();

        // Handler Explorer Properties
        handleExplorerRitualProps();
        handleExplorerPodiumProps();
        handleLightRadius();
        handleFlashExposure();
        handleHeartBeat();

        // Handler Explorer State and Gauges
        handleExplorerState();
        handleEscapeGauge();
        handleSurvivalGauge();
        handleRescueGauge();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        handleMovement();
    }

    private void updateOnAllClients()
    {
    }

    #endregion

    private void findCaptivatorReference()
    {
        if (captivatorRef == null)
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in playerList)
            {
                if (player.GetComponent<Captivator>() != null)
                {
                    captivatorRef = player.GetComponent<Captivator>();
                    return;
                }
            }
        }
    }

    #region Handler Methods

    private void handleExplorerRitualProps()
    {
        int performingRitualNumber = (int)PhotonNetwork.LocalPlayer
            .CustomProperties[CustomProperties.PERFORMING_RITUAL_NUM_KEY];

        if (performingRitual != null)
        {
            // If another player uses a key when you're doing a ritual, close it
            if (performingRitual.GetComponent<RitualPanel>().ritual.GetComponent<Ritual>().completed)
            {
                Destroy(performingRitual);
                return;
            }

            if (performingRitualNumber == performingRitual.GetComponent<RitualPanel>().ritual.GetComponent<Ritual>().ritualNumber)
                return;

            PhotonNetwork.LocalPlayer
                .CustomProperties[CustomProperties.PERFORMING_RITUAL_NUM_KEY] = performingRitual.GetComponent<RitualPanel>().ritual.GetComponent<Ritual>().ritualNumber;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
        else
        {


            if (performingRitualNumber == -1)
                return;

            PhotonNetwork.LocalPlayer
                .CustomProperties[CustomProperties.PERFORMING_RITUAL_NUM_KEY] = -1;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
    }

    private void handleExplorerPodiumProps()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomProperties.EXPLORER_PODIUM_KEY, out object num) &&
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomProperties.RESCUED_KEY, out object rescued))
        {
            if ((int)num >= 0 && (bool)rescued && !completelyCapsuled)
            {
                hp = 1;
                PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.EXPLORER_PODIUM_KEY] = -1;
                PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.RESCUED_KEY] = false;
                PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
            }
        }
    }

    private void handleExplorerState()
    {
        if (performingRitual || animator.GetCurrentAnimatorStateInfo(0).IsName("Capsuled") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("CompletelyCapsuled"))
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }

        animator.SetInteger("Hp", hp);

        if (hp <= 0)
        {
            capsuled = true;
        }
        else
        {
            capsuled = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Capsuled") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("CompletelyCapsuled"))
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] podiumList = GameObject.FindGameObjectsWithTag("Podium");

            // Find Captivator to follow
            foreach (GameObject player in playerList)
            {
                if (player.GetComponent<Captivator>() != null)
                {
                    PhotonView targetView = player.GetComponent<PhotonView>();

                    if (PhotonNetwork.CurrentRoom.Players[targetView.ControllerActorNr].CustomProperties
                        .TryGetValue(CustomProperties.CAPTIVE_KEY, out object num))
                    {
                        if ((int)num == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            this.transform.position = targetView.gameObject.transform.position;
                        }
                    }
                }
            }

            // Find podium to attach too
            foreach (GameObject podium in podiumList)
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(
                    CustomProperties.EXPLORER_PODIUM_KEY, out object num))
                {
                    PhotonView targetView = podium.GetComponent<PhotonView>();
                    Podium podiumScript = podium.GetComponent<Podium>();
                    if ((int)num == podiumScript.podiumNumber)
                    {
                        if (!targetView.IsMine)
                        {
                            targetView.TransferOwnership(PhotonNetwork.LocalPlayer);
                        }
                        else
                        {
                            podiumScript.occupied = true;
                        }

                        this.transform.position = podiumScript.capsulePlacementSpot.transform.position;
                    }
                    else
                    {
                        if (targetView.IsMine)
                        {
                            podiumScript.occupied = false;
                        }
                    }
                }
            }
        }
    }

    private void handleEscapeGauge()
    {
        // Check if player is on a podium
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomProperties.EXPLORER_PODIUM_KEY, out object num))
        {
            if ((int)num >= 0)
            {
                onPodium = true;
            }
            else
            {
                onPodium = false;
            }
        }

        if (escapeGauge >= MAX_ESCAPE_GAUGE && !onPodium)
        {
            hp = 1;
            capsuled = false;
            escapeGauge = 0;
            StartCoroutine(afterEscapeMovementBoost());
        }

        if (hp <= 0 && !onPodium)
        {
            escapeGauge += Time.deltaTime;
        }
    }

    private void handleSurvivalGauge()
    {
        if (onPodium && !completelyCapsuled)
        {
            survivalGauge -= survivalGaugeFillAmount * Time.deltaTime;
            survivalGaugeFillAmount = defaultSurvivalGaugeFillAmount;
        }

        if (survivalGauge <= 0)
        {
            completelyCapsuled = true;
        }

        animator.SetBool("CompletelyCapsuled", completelyCapsuled);

        if (completelyCapsuled)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(
                CustomProperties.EXPLORER_COMPLETELY_CAPSULED_KEY, out object capsuled))
            {
                if (!(bool)capsuled)
                {
                    PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.EXPLORER_COMPLETELY_CAPSULED_KEY] = true;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
                }
            }
        }
    }

    private void handleRescueGauge()
    {
        if (foundExplorer == null)
        {
            rescueGauge = 0;
            return;
        }

        if (rescueGauge >= MAX_RESCUE_GAUGE)
        {
            PhotonView targetView = foundExplorer.GetComponent<PhotonView>();

            targetView.Controller.CustomProperties[CustomProperties.RESCUED_KEY] = true;
            targetView.Controller.SetCustomProperties(targetView.Controller.CustomProperties);

            rescueGauge = 0;
        }
    }

    private void handleLightRadius()
    {
        if (lanternDuration <= 0)
        {
            lightRadius = MAX_LIGHT_RADIUS;
            lanternDuration = 0;
        }
        else
        {
            lanternDuration -= Time.deltaTime;

            if (lanternDuration < 5)
            {
                lightRadius = MAX_LIGHT_RADIUS + (2 * (lanternDuration / 5.0f));
            }
        }

        light.pointLightOuterRadius = lightRadius;
    }

    private void handleFlashExposure()
    {
        light.intensity = 1 + flashDuration;

        if (flashDuration > 0)
        {
            flashDuration -= 8 * Time.deltaTime;
        }

        if (flashDuration < 0)
        {
            flashDuration = 0;
        }
    }

    private void handleHeartBeat()
    {
        if (captivatorRef == null)
            return;

        float volume;
        float heartBeatSensorRadius = lightRadius + terrorRadius;
        float distance = Vector2.Distance(this.transform.position, captivatorRef.transform.position);

        volume = (heartBeatSensorRadius - distance) / heartBeatSensorRadius;

        if (volume > 0f)
        {
            audioManager.heartBeat.volume = volume;
            audioManager.heartBeat.gameObject.SetActive(true);
        }
        else
        {
            audioManager.heartBeat.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Coroutines

    public IEnumerator afterHitMovementBoost()
    {
        PhotonView.Get(this).RPC("PlayExplorerHitSounds", RpcTarget.All);
        movementSpeed = MAX_MOVEMENT_SPEED * 2;
        isInvulnerable = true;
        yield return new WaitForSeconds(2);
        movementSpeed = MAX_MOVEMENT_SPEED;
        isInvulnerable = false;
    }

    public IEnumerator afterEscapeMovementBoost()
    {
        movementSpeed = MAX_MOVEMENT_SPEED * 1.5f;
        isInvulnerable = true;
        yield return new WaitForSeconds(1.5f);
        movementSpeed = MAX_MOVEMENT_SPEED;
        isInvulnerable = false;
    }

    #endregion

    #region Sound Effects Methods

    public void playFootstepOne()
    {
        audioManager.footstepOne.Play();
    }

    public void playFootstepTwo()
    {
        audioManager.footstepTwo.Play();
    }

    public void playFootstepThree()
    {
        audioManager.footstepThree.Play();
    }

    public void playFemaleScream()
    {
        audioManager.FemaleScream.Play();
    }

    public void playMaleGrunt()
    {
        audioManager.MaleGrunt.Play();
    }

    public void playHitSound()
    {
        audioManager.HitSound.Play();
    }

    #endregion

    #region RPC Functions

    [PunRPC]
    public void PlayExplorerHitSounds()
    {
        playHitSound();

        if (gender == 0)
        {
            playMaleGrunt();
        } else if (gender == 1)
        {
            playFemaleScream();
        }
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(movementSpeed);
            stream.SendNext(canMove);
            stream.SendNext(hp);
            stream.SendNext(onPodium);
            stream.SendNext(capsuled);
            stream.SendNext(completelyCapsuled);
            stream.SendNext(lightRadius);
            stream.SendNext(lanternDuration);
            stream.SendNext(flashDuration);
            stream.SendNext(isInvulnerable);
        }
        else
        {
            movementSpeed = (float)stream.ReceiveNext();
            canMove = (bool)stream.ReceiveNext();
            hp = (int)stream.ReceiveNext();
            onPodium = (bool)stream.ReceiveNext();
            capsuled = (bool)stream.ReceiveNext();
            completelyCapsuled = (bool)stream.ReceiveNext();
            lightRadius = (float)stream.ReceiveNext();
            lanternDuration = (float)stream.ReceiveNext();
            flashDuration = (float)stream.ReceiveNext();
            isInvulnerable = (bool)stream.ReceiveNext();
        }
    }
}
