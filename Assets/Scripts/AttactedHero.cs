using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttactedHero : MonoBehaviour, IDropHandler
{
 public enum HeroType
    {
        PLAYER,
        ENEMY
    }

    public HeroType Type;

    public Color NormalCol, TargetCol;

    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManagerScript.instance.IsPlayerTurn)
            return;

        CardController card = eventData.pointerDrag.GetComponent<CardController>();

        if (card != null &&
            card.Card.CanAttack &&
            Type == HeroType.ENEMY &&
            !GameManagerScript.instance.EnemyFieldCards.Exists(x => x.Card.isProvacation))
        GameManagerScript.instance.DamageHero(card,true);
    }

    public void HighlightAsTarget(bool Highlighted)
    {
        GetComponent<Image>().color = Highlighted ? TargetCol : NormalCol;
    }
}
