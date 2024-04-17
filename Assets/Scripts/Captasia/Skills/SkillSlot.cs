using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Image SkillIcon;
    public Image CDShade;
    public GameObject ToolTipPanel;
    public Text ToolTipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipPanel.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ToolTipPanel.SetActive(true);
    }
}
