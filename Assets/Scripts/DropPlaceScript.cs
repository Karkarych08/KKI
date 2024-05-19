using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum FieldType
{
    SELF_HAND,
    SELF_FIELD,
    ENEMY_HAND,
    ENEMY_FIELD
}

public class DropPlaceScript : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public FieldType Type;
    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (Type != FieldType.SELF_FIELD)
            return;

        if (card != null && 
            GameManagerScript.instance.IsPlayerTurn &&
            GameManagerScript.instance.PlayerMana >= card.Card.Manacost &&
            !card.Card.IsPlaced) 
        {
            if (!card.Card.isSpell)
                card.CardMovement.DefaultParent = transform;
            card.OnCast();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag==null|| Type == FieldType.ENEMY_FIELD || Type == FieldType.ENEMY_HAND || Type == FieldType.SELF_HAND)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>(); 
        if (card != null) 
            card.DefaultGameCardParent = transform; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>();
        if (card != null && card.DefaultGameCardParent == transform)
            card.DefaultGameCardParent = card.DefaultParent;
    }
}
