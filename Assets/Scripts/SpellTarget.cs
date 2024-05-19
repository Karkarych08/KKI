using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManagerScript.instance.IsPlayerTurn)
            return;

        CardController spell = eventData.pointerDrag.GetComponent<CardController>(),
                       target = GetComponent<CardController>();
        if (spell &&
            spell.IsPlayerCard &&
            target.Card.IsPlaced &&
            GameManagerScript.instance.PlayerMana >= spell.Card.Manacost)
        {
            if ((spell.Card.SpellTarget == Card.TargetType.ALLY_CARD_TARGET &&
                target.IsPlayerCard) ||
                (spell.Card.SpellTarget == Card.TargetType.ENEMY_CARD_TARGET &&
                !target.IsPlayerCard))
            {
                GameManagerScript.instance.ReduceMana(true, spell.Card.Manacost);
                spell.UseSpell(target);
                GameManagerScript.instance.CheckCardsForManaAvaliability();
            }
        }

    }
}
