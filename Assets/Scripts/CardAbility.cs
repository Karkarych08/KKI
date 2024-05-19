using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility : MonoBehaviour
{
    public CardController CC;

    public GameObject Shild, Provacation;

    public void OnCast()
    {
        foreach (var ability in CC.Card.Abilities)
        {
            switch(ability)
            {
                case Card.AbilityType.INSTANTE_ACTIVE:

                    CC.Card.CanAttack = true;

                    if (CC.IsPlayerCard) 
                    {
                        CC.CardInfo.HighlightCard(true);
                    }
                    break;

                case Card.AbilityType.SHIELD:
                    Shild.SetActive(true);
                    break;
                    
                case Card.AbilityType.PROVACATION:
                    Provacation.SetActive(true);
                    break;
            }
        }
    }

    public void OnDamageTake(CardController attacker = null)
    {
        Shild.SetActive(false);

        foreach (var ability in CC.Card.Abilities)
        {
            switch (ability)
            {
                case Card.AbilityType.SHIELD:
                    Shild.SetActive(true);
                    break;
                case Card.AbilityType.COUNTER_ATTACK:
                    if (attacker != null)
                        attacker.Card.GetDamage(CC.Card.Attack);
                    break;
            }
        }
    }

    public void OnDamageDeal()
    {
        foreach (var ability in CC.Card.Abilities)
        {
            switch (ability)
            {
                case Card.AbilityType.DOUBLE_ATTACK: 
                    if (CC.Card.TimesDealDamage == 1)
                    {
                        CC.Card.CanAttack = true;

                        if (CC.IsPlayerCard)
                            CC.CardInfo.HighlightCard(true);
                    }
                    break;
            }
        }
    }

    public void OnNewTurn() 
    {
        CC.Card.TimesDealDamage = 0;
        foreach (var ability in CC.Card.Abilities)
        {
            switch (ability)
            {
                case Card.AbilityType.REGENERATION_EACH_TURN:
                    CC.Card.Defense += 2;
                    CC.CardInfo.RefreshData();
                    break;
            }
        }
    }
}
