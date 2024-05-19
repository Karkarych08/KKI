using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttactedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManagerScript.instance.IsPlayerTurn)
            return;

        CardController attacker = eventData.pointerDrag.GetComponent<CardController>(),
                       defender = GetComponent<CardController>();
        if (attacker &&
            attacker.Card.CanAttack &&
            defender.Card.IsPlaced) 
        {
            if (GameManagerScript.instance.EnemyFieldCards.Exists(x => x.Card.isProvacation) &&
                !defender.Card.isProvacation)
                return; 
            if (attacker.IsPlayerCard)
                attacker.CardInfo.HighlightCard(false);

            GameManagerScript.instance.CardsFight(attacker, defender);
        }
    }
}
