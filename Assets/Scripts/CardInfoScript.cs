using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

public class CardInfoScript : MonoBehaviour
{
    public CardController CC;

    public Image Logo;
    public TextMeshProUGUI Name, Attack, Defense, Manacost;
    public GameObject HideObj, HilightedObj;
    public Color NormalCol, TargetCol, SpellTargetCol;

    public void HideCardInfo()
    {
        HideObj.SetActive(true);
        Manacost.text = "";
    }

    public void ShowCardInfo()
    {
        HideObj.SetActive(false);
        Logo.sprite = CC.Card.Logo;
        Logo.preserveAspect = true;
        Name.text = CC.Card.Name;
        Attack.text = CC.Card.Attack.ToString();
        Defense.text = CC.Card.Defense.ToString();
        Manacost.text = CC.Card.Manacost.ToString();
    }

    public void RefreshData()
    {
        Attack.text = CC.Card.Attack.ToString();
        Defense.text = CC.Card.Defense.ToString();
        Manacost.text= CC.Card.Manacost.ToString();
    }

    public void HighlightCard(bool highlighted)
    {
        HilightedObj.SetActive(highlighted);
    }


    public void HighlightManaAvailability(int currentMana) 
    {
        GetComponent<CanvasGroup>().alpha= currentMana >= CC.Card.Manacost ? 1 : .5f;
    }

    public void HighlightAsTarget (bool Highlighted)
    {
        GetComponent<Image>().color = Highlighted ? TargetCol : NormalCol;
    }

    public void HighlightAsSpellTarget(bool Highlighted)
    {
        GetComponent<Image>().color = Highlighted ? SpellTargetCol : NormalCol;
    }
}
