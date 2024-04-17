using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillManager : MonoBehaviour, IPointerDownHandler
{
    [Header("Captivator UI Reference")]
    public CaptivatorUI UIRef;

    [Header("Skill Slots Panel")]
    public GameObject SkillsPanel;
    public GameObject SlotsPanel;

    [Header("Skills")]
    public List<SkillSlot> skills;

    [Header("Skill Prefabs")]
    public GameObject DarkLanternEffect;

    [Header("Dark Lantern Attributes")]
    public float MAX_DARK_LANTERN_CD;
    public float darkLanternCD;
    public float darkLanternDuration;

    [Header("Shadow Run Attributes")]
    public float MAX_SHADOW_RUN_CD;
    public float shadowRunCD;
    public float shadowRunDuration;
    public float speedIncreaseAmount;

    // Private Variables
    private Camera MapCamera;

    // Start is called before the first frame update
    void Start()
    {
        MapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            DarkLanternSkill();
        } else if (Input.GetButtonDown("Fire2"))
        {
            ShadowRunSkill();
        } else if (Input.GetButtonDown("Fire3"))
        {
        }

        handleDarkLanternCD();
        handleShadowRunCD();

    }

    private Vector3 mapPositionToWorld(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
           UIRef.Map.rectTransform,
           eventData.position,
           Camera.main.GetComponent<Camera>(),
           out localPoint
        );

        var rect = UIRef.Map.rectTransform.rect;
        localPoint.x = (localPoint.x / rect.width) + UIRef.Map.rectTransform.pivot.x;
        localPoint.y = (localPoint.y / rect.height) + UIRef.Map.rectTransform.pivot.y;
        Ray ray = MapCamera.GetComponent<Camera>().ViewportPointToRay(localPoint);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float d;
        plane.Raycast(ray, out d);
        Vector3 hit = ray.GetPoint(d);

        return hit;
    }

    private void handleDarkLanternCD()
    {
        if (darkLanternCD > 0)
        {
            darkLanternCD -= Time.deltaTime;
        } else if (darkLanternCD < 0)
        {
            darkLanternCD = 0;
        }

        skills[0].CDShade.fillAmount = darkLanternCD / MAX_DARK_LANTERN_CD;
    }

    private void handleShadowRunCD()
    {
        if (shadowRunCD > 0)
        {
            shadowRunCD -= Time.deltaTime;
        } else if (shadowRunCD < 0)
        {
            shadowRunCD = 0;
        }

        skills[1].CDShade.fillAmount = shadowRunCD / MAX_SHADOW_RUN_CD;
    }

    #region Skill Button Methods

    /// <summary>
    /// The Dark Lantern Skill will reveal a portion of the map to the captivator based on where they click
    /// </summary>
    public void DarkLanternSkill()
    {
        if (UIRef.MapUIPanel.activeSelf)
        {
            UIRef.MapUIPanel.SetActive(false);
        } else
        {
            UIRef.MapUIPanel.SetActive(true);
        }
    }

    /// <summary>
    /// The Shadow Run skill will make the captivator invisible and increase their movement speed
    /// </summary>
    public void ShadowRunSkill()
    {
        if (shadowRunCD <= 0)
        {
            shadowRunCD = MAX_SHADOW_RUN_CD;
            StartCoroutine(ShadowRunRoutine());
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (UIRef.Map.isActiveAndEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && darkLanternCD <= 0)
            {
                Vector3 hit = mapPositionToWorld(eventData);
                StartCoroutine(DarkLanternRevealArea(new Vector3(hit.x, hit.y, 0)));
            }
        }
    }

    #endregion

    #region Coroutines

    IEnumerator DarkLanternRevealArea(Vector3 position)
    {
        darkLanternCD = MAX_DARK_LANTERN_CD;
        GameObject temp = Instantiate(DarkLanternEffect, position, Quaternion.identity);
        yield return new WaitForSeconds(darkLanternDuration);
        Destroy(temp);
    }

    IEnumerator ShadowRunRoutine()
    {
        UIRef.captivatorRef.shadowRun = true;
        UIRef.captivatorRef.movementSpeed += speedIncreaseAmount;
        yield return new WaitForSeconds(shadowRunDuration);
        UIRef.captivatorRef.movementSpeed = UIRef.captivatorRef.MAX_MOVEMENT_SPEED;
        UIRef.captivatorRef.shadowRun = false;
    }

    #endregion
}
