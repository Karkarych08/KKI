using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public Card Card;

    public bool IsPlayerCard;

    public CardInfoScript CardInfo;
    public CardMovementScript CardMovement;

    GameManagerScript GameManager;

    public CardAbility Ability;

    public void Init(Card card, bool isPlayedCard)
    {
        Card = card;
        IsPlayerCard = isPlayedCard;
        GameManager = GameManagerScript.instance;

        if (isPlayedCard)
        {
            CardInfo.ShowCardInfo();
            GetComponent<AttactedCard>().enabled = false;
        }
        else
            CardInfo.HideCardInfo();
    }

    public void CheckForAlive()
    {
        if (Card.isAlive)
            CardInfo.RefreshData();
        else
            DestroyCard();
    }

    public void DestroyCard()
    {
        CardMovement.OnEndDrag(null);

        RemoveCardFromList(GameManager.EnemyFieldCards);
        RemoveCardFromList(GameManager.EnemyHandCards);
        RemoveCardFromList(GameManager.PlayerFieldCards);
        RemoveCardFromList(GameManager.PlayerHandCards);

        Destroy(gameObject);
    }

    public void OnCast()
    {
        if (Card.isSpell && Card.SpellTarget != Card.TargetType.NO_TARGET)
            return;
        if (IsPlayerCard)
        {
            GameManager.PlayerHandCards.Remove(this);
            GameManager.PlayerFieldCards.Add(this);
            GameManager.ReduceMana(true, Card.Manacost);
            GameManager.CheckCardsForManaAvaliability();
        }
        else
        {
            GameManager.EnemyHandCards.Remove(this);
            GameManager.EnemyFieldCards.Add(this);
            GameManager.ReduceMana(false, Card.Manacost);
            CardInfo.ShowCardInfo();
        }
        Card.IsPlaced = true;

        if (Card.HasAbility)
            Ability.OnCast();

        if (Card.isSpell)
            UseSpell(null);
    }

    public void OnTakeDamage(CardController attacker = null)
    {
        CheckForAlive();
        Ability.OnDamageTake(attacker);
    }

    public void OnDamageDeal()
    {
        Card.TimesDealDamage++;
        Card.CanAttack = false;
        CardInfo.HighlightCard(false);
        if (Card.HasAbility)
            Ability.OnDamageDeal();
    }

    public void UseSpell(CardController target)
    {
        switch (Card.Spell)
        {
            case Card.SpellType.DEBUFF_CARD_DAMAGE:
                target.Card.Attack = Mathf.Clamp(target.Card.Attack - Card.SpellValue, 0, int.MaxValue);
                CardInfo.RefreshData();
                break;

            case Card.SpellType.BUFF_CARD_DAMAGE:
                target.Card.Attack += Card.SpellValue;
                CardInfo.RefreshData();
                break;

            case Card.SpellType.DAMAGE_ENEMY_CARD:
                GiveDamageTo(target, Card.SpellValue);
                CardInfo.RefreshData();
                break;

            case Card.SpellType.SHIELD_ON_ALLY_CARD:
                if (target.Card.Abilities.Exists(x => x == Card.AbilityType.SHIELD))
                    target.Card.Abilities.Add(Card.AbilityType.SHIELD);
                CardInfo.RefreshData();
                break;

            case Card.SpellType.HEAL_ALLY_CARD:
                target.Card.Defense -= Card.SpellValue;
                CardInfo.RefreshData();
                break;

            case Card.SpellType.DAMAGE_ENEMY_FIELD_CARDS:
                var enemyCards = IsPlayerCard ? 
                    new List<CardController>(GameManager.EnemyFieldCards) :
                    new List<CardController>(GameManager.PlayerFieldCards);
                foreach (var card in enemyCards)
                    GiveDamageTo(card, Card.SpellValue);
                CardInfo.RefreshData();
                break;

            case Card.SpellType.DAMAGE_ENEMY_HERO:
                if (IsPlayerCard)
                    GameManager.EnemyHP -= Card.SpellValue;
                else
                    GameManager.PlayerHP -= Card.SpellValue;
                GameManager.ShowHP();
                GameManager.CheckForResult();
                break;

            case Card.SpellType.HEAL_ALLY_FIELD_CARDS:
                var allyCards = IsPlayerCard ?
                    GameManager.PlayerFieldCards :
                    GameManager.EnemyFieldCards;
                foreach (var card in allyCards)
                {
                    card.Card.Defense += Card.SpellValue;
                    card.CardInfo.RefreshData();
                }
                break;

            case Card.SpellType.HEAL_ALLY_HERO:
                if (IsPlayerCard)
                    GameManager.PlayerHP += Card.SpellValue;
                else
                    GameManager.EnemyHP += Card.SpellValue;
                GameManager.ShowHP();

                break;
            case Card.SpellType.PROVACATION_ON_ALLY_CARD:
                if (target.Card.Abilities.Exists(x => x == Card.AbilityType.PROVACATION))
                    target.Card.Abilities.Add(Card.AbilityType.PROVACATION);
                CardInfo.RefreshData();
                break;

        }
        if (target)
        {
            target.Ability.OnCast();
            CheckForAlive();
        }

        DestroyCard();
    }

    void GiveDamageTo(CardController card,int damage)
    {
        card.Card.GetDamage(damage);
        card.CheckForAlive();
        card.OnTakeDamage();
    }

    void RemoveCardFromList(List<CardController> list)
    {
        if (list.Exists(x=> x == this))
            list.Remove(this);
    }
}
