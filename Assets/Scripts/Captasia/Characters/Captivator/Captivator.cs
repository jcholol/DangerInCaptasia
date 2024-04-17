using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Captivator : Character, IPunObservable
{
    #region Public Variables

    [Header("Captured Explorer")]
    public GameObject captive;

    [Header("Found PickUps")]
    public GameObject foundCapsuledExplorer;
    public GameObject foundPodium;

    [Header("Captivator Audio Manager")]
    public CaptivatorAudioManager audioManager;

    #endregion

    #region Captivator Stats

    [Header("MAX STAT")]
    public float MAX_MOVEMENT_SPEED;
    public float ATTACK_MAX_COOLDOWN;

    [Header("Cooldowns")]
    public float attackCooldown;

    [Header("Stunned Timer")]
    public float stunnedDuration;

    [Header("Shadow Run Boolean")]
    public bool shadowRun;

    private bool captiveLock = false;
    private bool podiumIndicatorLock = false;

    #endregion

    #region Private Lists

    // List for rituals
    private GameObject[] rituals;
    private bool[] spawnedIndicatorList;

    #endregion

    #region Previous States

    public GameObject previous_captive_state;

    #endregion

    void Start()
    {
        photonView = this.GetComponent<PhotonView>();
        characterUI = GameObject.FindGameObjectWithTag("CharacterUI");

        if (photonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.TEAM_KEY] = "Captivator";
            PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.CHARACTER_VIEW_KEY] = photonView.ViewID;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

            rituals = GameObject.FindGameObjectsWithTag("Ritual");
            spawnedIndicatorList = new bool[rituals.Length];
        }
    }

    void Update()
    {
        handleShadowRun();

        if (!photonView.IsMine)
        {
            return;
        }
            

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            PhotonView targetView = PhotonView.Find((int)player.CustomProperties[CustomProperties.CHARACTER_VIEW_KEY]);

            if (targetView != null && targetView.GetComponent<Explorer>() != null)
            {
                Explorer explorer = targetView.GetComponent<Explorer>();
                
                if (Vector2.Distance(this.transform.position, explorer.flashLight.transform.position) >= lightRadius)
                {
                    explorer.flashLight.light.color = Color.black;
                } else
                {
                    explorer.flashLight.light.color = Color.white;
                }
            }
        }

        handleAnimationParameters();
        handleCaptivatorState();
        handleLightRadius();
        handleFlashExposure();
        handleCaptive();
        handleMovementSpeedChanges();
        handleExplorerStruggledOut();

        // Ritual Indicator
        handleRitualIndicator();
        handlePodiumIndicator();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        handleMovement();
    }

    private void handleCaptivatorState()
    {
        if (attackCooldown < ATTACK_MAX_COOLDOWN)
        {
            attackCooldown += Time.deltaTime;
        }

        if (stunnedDuration > 0)
        {
            canMove = false;
            stunnedDuration -= Time.deltaTime;
            animator.SetBool("Stunned", true);
        } else
        {
            animator.SetBool("Stunned", false);
            canMove = true;
        }
    }

    private void handleCaptive()
    {
        if (captive != null)
        {
            if (captive.GetComponent<Explorer>().hp > 0)
            {
                captive = null;
                PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.CAPTIVE_KEY] = -1;
                PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
                return;
            }

            int actorNumber = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.CAPTIVE_KEY];

            if (actorNumber != captive.GetComponent<PhotonView>().Controller.ActorNumber)
            {
                PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.CAPTIVE_KEY] = captive.GetComponent<PhotonView>().Controller.ActorNumber;
                PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
            }
        }

        if (captive == null)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomProperties.CAPTIVE_KEY, out object num)) 
            {
                if ((int) num >= 0)
                {
                    PhotonNetwork.LocalPlayer.CustomProperties[CustomProperties.CAPTIVE_KEY] = -1;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
                }
            }
        }
    }

    private void handleLightRadius()
    {
        light.pointLightOuterRadius = lightRadius;
    }

    private void handleFlashExposure()
    {
        light.intensity = 1 + flashDuration;

        if (flashDuration >= MAX_FLASH_DURATION)
        {
            stunnedDuration = 3;
            flashDuration = MAX_FLASH_DURATION;
        }

        if (flashDuration > 0)
        {
            flashDuration -= 8 * Time.deltaTime;
        }

        if (flashDuration < 0)
        {
            flashDuration = 0;
        }
    }

    private void handleMovementSpeedChanges()
    {
        if (captive != null && !captiveLock)
        {
            StartCoroutine(captiveMovementSpeedReduction());
        }
    }

    private void handleRitualIndicator()
    {
        for (int i = 0; i < rituals.Length; i++)
        {
            if (rituals[i].GetComponent<Ritual>().completed &&
                spawnedIndicatorList[i] == false)
            {
                StartCoroutine(completedRitualIndicator(rituals[i]));
                spawnedIndicatorList[i] = true;
            }
        }
    }

    private void handlePodiumIndicator()
    {
        if (podiumIndicatorLock) 
        {
            return;
        }

        GameObject[] podiums = GameObject.FindGameObjectsWithTag("Podium");

        GameObject nearestPodium = null;
        
        for (int i = 0; i < podiums.Length; i++)
        {
            if (nearestPodium == null)
            {
                nearestPodium = podiums[i];
                continue;
            }

            if (Vector3.Distance(this.transform.position, nearestPodium.transform.position)
                > Vector3.Distance(this.transform.position, podiums[i].transform.position))
            {
                if (!podiums[i].GetComponent<Podium>().occupied)
                {
                    nearestPodium = podiums[i];
                }
            }
        }

        if (captive != null)
        {
            StartCoroutine(nearestPodiumIndicator(nearestPodium));
        }
    }

    private void handleExplorerStruggledOut()
    {
        if (previous_captive_state != null && captive == null)
        {
            stunnedDuration = 2f;
        }

        previous_captive_state = captive;
    }

    private void handleShadowRun()
    {
        if (!photonView.IsMine)
        {
            if (shadowRun)
            {
                this.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().enabled = true;
            }
        } else
        {
            if (shadowRun)
            {
                this.GetComponent<SpriteRenderer>().color = Color.black;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    #region Coroutines
    public IEnumerator captiveMovementSpeedReduction()
    {
        captiveLock = true;

        while (captive != null)
        {
            movementSpeed = MAX_MOVEMENT_SPEED / 2.0f;
            yield return null;
        }

        movementSpeed = MAX_MOVEMENT_SPEED;
        captiveLock = false;

        yield return null;
    }

    public IEnumerator attackSpeedReduction()
    {
        movementSpeed = 0.2f;
        yield return new WaitForSeconds(2);
        movementSpeed = MAX_MOVEMENT_SPEED;
    }

    public IEnumerator completedRitualIndicator(GameObject ritual)
    {
        GameObject indicatorRef = Instantiate(CaptasiaResources.Instance.INDICATOR, 
            this.transform.position, Quaternion.identity, this.transform);

        indicatorRef.GetComponent<Indicator>().pointToObject = ritual;
        indicatorRef.GetComponent<SpriteRenderer>().color = new Color(255, 255, 0);

        yield return new WaitForSeconds(8);

        Destroy(indicatorRef);
    }

    public IEnumerator nearestPodiumIndicator(GameObject podium)
    {
        podiumIndicatorLock = true;

        GameObject indicatorRef = Instantiate(CaptasiaResources.Instance.INDICATOR,
            this.transform.position, Quaternion.identity, this.transform);

        indicatorRef.GetComponent<Indicator>().pointToObject = podium;
        indicatorRef.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);

        yield return new WaitUntil(() => captive == null);

        Destroy(indicatorRef.gameObject);
        podiumIndicatorLock = false;
    }

    #endregion

    #region Sound Effect Methods

    public void playFootstepOne()
    {
        audioManager.footstepOne.Play();
    }

    public void playFootstepTwo()
    {
        audioManager.footstepTwo.Play();
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(attackCooldown);
            stream.SendNext(stunnedDuration);
            stream.SendNext(shadowRun);
        } else
        {
            attackCooldown = (float)stream.ReceiveNext();
            stunnedDuration = (float)stream.ReceiveNext();
            shadowRun = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void RPC_Attack()
    {
        if (attackCooldown >= ATTACK_MAX_COOLDOWN)
        {
            animator.SetTrigger("Attack");
            StartCoroutine(attackSpeedReduction());
        }
    }
}
