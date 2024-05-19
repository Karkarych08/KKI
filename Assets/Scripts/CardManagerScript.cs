using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{

    public enum AbilityType
    {
        NO_ABILITY,
        INSTANTE_ACTIVE,
        DOUBLE_ATTACK,
        SHIELD,
        PROVACATION,
        REGENERATION_EACH_TURN,
        COUNTER_ATTACK
    }

    public enum SpellType
    {
        NO_SPELL,
        HEAL_ALLY_FIELD_CARDS,
        DAMAGE_ENEMY_FIELD_CARDS,
        HEAL_ALLY_HERO,
        DAMAGE_ENEMY_HERO,
        HEAL_ALLY_CARD,
        DAMAGE_ENEMY_CARD,
        SHIELD_ON_ALLY_CARD,
        PROVACATION_ON_ALLY_CARD,
        BUFF_CARD_DAMAGE,
        DEBUFF_CARD_DAMAGE
    }

    public enum TargetType
    {
        NO_TARGET,
        ALLY_CARD_TARGET,
        ENEMY_CARD_TARGET
    }

    public string Name;
    public Sprite Logo;
    public int Attack, Defense, Manacost;
    public bool CanAttack;
    public bool IsPlaced;
    public int TimesDealDamage;
    public List<AbilityType> Abilities;
    public SpellType Spell;
    public TargetType SpellTarget;
    public int SpellValue;

    public bool isAlive
    {
        get 
        { 
           return Defense > 0; 
        }
    }
    public bool HasAbility
    {
        get
        {
            return Abilities.Count > 0;
        }
    }

    public bool isSpell
    {
        get
        {
            return Spell != SpellType.NO_SPELL;
        }
    }

    public bool isProvacation
    {
        get
        {
            return Abilities.Exists(x=>x == AbilityType.PROVACATION);
        }
    }
    public Card(string name, string logoPath, int attack, int defence, int manacost, AbilityType abilityType = 0, SpellType spellType = 0, int spellValue = 0, TargetType targetType = 0)
    {
        Name = name;
        Logo =Resources.Load<Sprite>(logoPath);
        Attack = attack; Defense = defence; Manacost = manacost;
        CanAttack = false;
        IsPlaced = false;
        Abilities = new List<AbilityType>();

        Spell = spellType;
        SpellTarget = targetType;
        SpellValue = spellValue;

        if (abilityType != 0)
         Abilities.Add(abilityType);

        TimesDealDamage = 0;
}


    public void GetDamage(int dmg)
    {
        if (dmg > 0)
            if (Abilities.Exists(x => x == AbilityType.SHIELD))
                Abilities.Remove(AbilityType.SHIELD);
            else
                Defense -= dmg;
    }

    public Card GetCopy()
    {
        Card card = this;
        card.Abilities = new List<AbilityType>(Abilities);
        return card;
    }
}

public static class CardManager
{
    public static List<Card> AllCard = new List<Card>();
}

public class CardManagerScript : MonoBehaviour
{
    public void Awake()
    {
        CardManager.AllCard.Add(new Card("AppleJack", "Sprites/Cards/ponyAvs/AppleJack", 8,10,12));
        CardManager.AllCard.Add(new Card("Rarity", "Sprites/Cards/ponyAvs/Rarity", 4,5,6));
        CardManager.AllCard.Add(new Card("Pinkey Pie", "Sprites/Cards/ponyAvs/Pinkey", 9,9,9));
        CardManager.AllCard.Add(new Card("Rainbow Dash", "Sprites/Cards/ponyAvs/Rainbow", 11,3,9));
        CardManager.AllCard.Add(new Card("Twilight", "Sprites/Cards/ponyAvs/Twilight", 12,14,18));
        CardManager.AllCard.Add(new Card("Fluttershy", "Sprites/Cards/ponyAvs/Fluttershy", 2,4,4));

        CardManager.AllCard.Add(new Card("Shining Armor", "Sprites/Cards/ponyAvs/Shining", 3, 7, 11,
            Card.AbilityType.SHIELD));
        CardManager.AllCard.Add(new Card("Starlight", "Sprites/Cards/ponyAvs/Starlight", 6, 8, 9,
            Card.AbilityType.COUNTER_ATTACK));
        CardManager.AllCard.Add(new Card("Discord", "Sprites/Cards/ponyAvs/Discord", 4, 6, 10,
            Card.AbilityType.DOUBLE_ATTACK));
        CardManager.AllCard.Add(new Card("Cadence", "Sprites/Cards/ponyAvs/Cadence", 6, 5, 9,
            Card.AbilityType.REGENERATION_EACH_TURN));
        CardManager.AllCard.Add(new Card("Trixie", "Sprites/Cards/ponyAvs/Trixie", 7, 4, 13,
            Card.AbilityType.INSTANTE_ACTIVE));
        CardManager.AllCard.Add(new Card("Spike", "Sprites/Cards/ponyAvs/Spike", 4, 9, 14,
            Card.AbilityType.PROVACATION));

        CardManager.AllCard.Add(new Card("Heal All", "Sprites/Cards/SpellIcons/89", 0, 0, 10, 0, 
            Card.SpellType.HEAL_ALLY_FIELD_CARDS,2,Card.TargetType.NO_TARGET));
        CardManager.AllCard.Add(new Card("Damage All", "Sprites/Cards/SpellIcons/52", 0, 0, 10, 0,
            Card.SpellType.DAMAGE_ENEMY_FIELD_CARDS,2, Card.TargetType.NO_TARGET));
        CardManager.AllCard.Add(new Card("Heal Hero", "Sprites/Cards/SpellIcons/60", 0, 0, 12, 0,
            Card.SpellType.HEAL_ALLY_HERO, 4, Card.TargetType.NO_TARGET));
        CardManager.AllCard.Add(new Card("Damage Hero", "Sprites/Cards/SpellIcons/35", 0, 0, 12,0,
            Card.SpellType.DAMAGE_ENEMY_HERO,4, Card.TargetType.NO_TARGET));
        CardManager.AllCard.Add(new Card("Heal Card", "Sprites/Cards/SpellIcons/44", 0, 0, 5, 0,
            Card.SpellType.HEAL_ALLY_CARD, 4, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCard.Add(new Card("Damage card", "Sprites/Cards/SpellIcons/49", 0, 0, 5, 0,
            Card.SpellType.DAMAGE_ENEMY_CARD, 4, Card.TargetType.ENEMY_CARD_TARGET));
        CardManager.AllCard.Add(new Card("Give Shield", "Sprites/Cards/SpellIcons/14", 0, 0, 7, 0,
            Card.SpellType.SHIELD_ON_ALLY_CARD, 1, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCard.Add(new Card("Give Provacation", "Sprites/Cards/SpellIcons/4", 0, 0, 7, 0,
            Card.SpellType.PROVACATION_ON_ALLY_CARD, 1, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCard.Add(new Card("Buff damage", "Sprites/Cards/SpellIcons/98", 0, 0, 7, 0, 
            Card.SpellType.BUFF_CARD_DAMAGE, 3, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCard.Add(new Card("Debuff damage", "Sprites/Cards/SpellIcons/84", 0, 0, 7, 0, 
            Card.SpellType.DEBUFF_CARD_DAMAGE, 3, Card.TargetType.ENEMY_CARD_TARGET));
    }
}
