using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game
{
    public List<Card> EnemyDeck, PlayerDeck;
    public Game()
    {
        EnemyDeck = GiveDeckCard();
        PlayerDeck = GiveDeckCard();
    }

    List<Card> GiveDeckCard()
    {
        List<Card> list = new List<Card>();
        for (int i = 0; i < 20; i++)
        {
            list.Add(CardManager.AllCard[Random.Range(0, CardManager.AllCard.Count)].GetCopy());
        }
        return list;
    }
}

public class GameManagerScript : MonoBehaviour
{
    public Game CurrentGame;
    public Transform EnemyHand, PlayerHand,
                     EnemyField, PlayerField;
    public GameObject CardPref;
    int Turn, TurnTime = 30;
    public TextMeshProUGUI TurnTimeTxt;
    public Button EndTurnButton;
    public int PlayerMana, EnemyMana = 20;
    public int PlayerHP = 30, EnemyHP = 30;
    public TextMeshProUGUI PlayerManaTxt, EnemyManaTxt;
    public TextMeshProUGUI PlayerHPTxt, EnemyHPTxt;
    public GameObject ResualtGO;
    public TextMeshProUGUI ResualtText;
    public AttactedHero EnemyHero, PlayerHero;

    public List<CardController> PlayerHandCards = new List<CardController>(),
                                PlayerFieldCards = new List<CardController>(),
                                EnemyHandCards = new List<CardController>(),
                                EnemyFieldCards = new List<CardController>();

    public static GameManagerScript instance;

    public void Awake()
    {
        if (instance == null)
        instance = this;
    }

    void CreateCardPref(Card card, Transform hand)
    {
        GameObject CardGO = Instantiate(CardPref, hand, false);
        CardController cardC = CardGO.GetComponent<CardController>();
        cardC.Init(card, hand == PlayerHand);

        if (cardC.IsPlayerCard)
            PlayerHandCards.Add(cardC);
        else
            EnemyHandCards.Add(cardC);

    }

    public bool IsPlayerTurn
    {
        get
        {
            return Turn % 2 == 0;  
        }
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Turn = 0;
        EndTurnButton.interactable =true;
        CurrentGame = new Game();
        GiveHandCards(CurrentGame.EnemyDeck, EnemyHand);
        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);
        PlayerMana = EnemyMana = 20;
        PlayerHP = EnemyHP = 30;
        ShowHP();
        ShowMana();
        ResualtGO.SetActive(false);
    StartCoroutine(TurnFunc());
    }

    void GiveHandCards(List<Card> deck, Transform hand)
    {
        int i = 0;
        while (i++ < 4)
            GiveCardsToHand(deck, hand);
    }

    void GiveCardsToHand(List<Card> deck, Transform hand)
    {
        if (deck.Count == 0)
            return;

        Card card = deck[0];

        CreateCardPref(deck[0], hand);

        deck.RemoveAt(0);
    }

    IEnumerator TurnFunc()
    {
        TurnTime = 30;
        TurnTimeTxt.text = TurnTime.ToString();
        foreach (var card in PlayerFieldCards)
        {
            card.CardInfo.HighlightCard(false);
        }

        CheckCardsForManaAvaliability();

        if (IsPlayerTurn)
        { 
            foreach (var card in PlayerFieldCards)
            {
                card.Card.CanAttack = true;
                card.CardInfo.HighlightCard(true);
                card.Ability.OnNewTurn();
            }

            while (TurnTime-->0)
            {
                TurnTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
            ChangeTurn();
        }
        else
        {
            foreach (var card in EnemyFieldCards)
            {
                card.Card.CanAttack = true;
                card.Ability.OnNewTurn();
            }
            StartCoroutine(EnemyTurn(EnemyHandCards));
          
        }
    }

    IEnumerator EnemyTurn(List<CardController> cards)
    {
        yield return new WaitForSeconds(1);
        int count = cards.Count == 1 ? 1:
            Random.Range(0, cards.Count);

        for (int i = 0; i < count; i++)
        {
            if (EnemyFieldCards.Count > 5 ||
                EnemyMana == 0 ||
                EnemyHandCards.Count == 0)
                break;

            List<CardController> cardList = cards.FindAll(x => x.Card.Manacost <= EnemyMana &&
                                                              !x.Card.isSpell);

            if (cardList.Count == 0)
                break;

            cardList[0].GetComponent<CardMovementScript>().MoveToField(EnemyField);

            yield return new WaitForSeconds(1);

            cardList[0].transform.SetParent(EnemyField);
            cardList[0].OnCast();
        }

        yield return new WaitForSeconds(1);

        while (EnemyFieldCards.Exists(x=>x.Card.CanAttack))
        {
            var activeCard = EnemyFieldCards.FindAll(x => x.Card.CanAttack)[0];
            bool hasProvacation = PlayerFieldCards.Exists(x => x.Card.isProvacation);
            if (hasProvacation ||
                (Random.Range(0, 2) == 0 &&
                PlayerFieldCards.Count > 0))
            {
                CardController enemy;

                if (hasProvacation)
                    enemy = PlayerFieldCards.Find(x => x.Card.isProvacation);
                else
                    enemy = PlayerFieldCards[Random.Range(0, PlayerFieldCards.Count)];




                Debug.Log(activeCard.Card.Name + " (" + activeCard.Card.Attack + ";" + activeCard.Card.Defense + ") -->" +
                          enemy.Card.Name + " (" + enemy.Card.Attack + ";" + enemy.Card.Defense + ")");


                activeCard.CardMovement.MoveToTarget(enemy.transform);

                yield return new WaitForSeconds(.75f);

                CardsFight(enemy, activeCard);
            }
            else
            {
                Debug.Log(activeCard.Card.Name + " (" + activeCard.Card.Attack + ";" + activeCard.Card.Defense + " Attacked Hero");
                activeCard.Card.CanAttack = false;
                activeCard.GetComponent<CardMovementScript>().MoveToTarget(PlayerHero.transform);
                yield return new WaitForSeconds(.75f);
                DamageHero(activeCard, false);
            }
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForSeconds(1);
        ChangeTurn();
    }

    public void ChangeTurn()
    {
        StopAllCoroutines();
        Turn++;

        EndTurnButton.interactable = IsPlayerTurn;
        
        if (IsPlayerTurn == false)
        {
            GiveNewCards();

            PlayerMana = EnemyMana = 20;
            ShowMana();
        }
        StartCoroutine(TurnFunc());
    }

    void GiveNewCards()
    {
        GiveCardsToHand(CurrentGame.EnemyDeck, EnemyHand);
        GiveCardsToHand(CurrentGame.PlayerDeck, PlayerHand);
    }

    public void CardsFight(CardController attacker, CardController defender)
    {
        defender.Card.GetDamage(attacker.Card.Attack);

        attacker.OnDamageDeal();
        defender.OnTakeDamage(attacker);
        attacker.Card.GetDamage(defender.Card.Attack);
        attacker.OnTakeDamage();


        attacker.CheckForAlive();
        defender.CheckForAlive();
    }

    void ShowMana()
    {
        PlayerManaTxt.text=PlayerMana.ToString();
        EnemyManaTxt.text=EnemyMana.ToString();
    } 
    
    public void ShowHP()
    {
        PlayerHPTxt.text= "HP " + PlayerHP.ToString();
        EnemyHPTxt.text= "HP " + EnemyHP.ToString();
    }

    public void ReduceMana(bool playerMana, int manacost) 
    {
        if (playerMana)
            PlayerMana = Mathf.Clamp(PlayerMana - manacost, 0, int.MaxValue);
        else
            EnemyMana = Mathf.Clamp(EnemyMana - manacost, 0, int.MaxValue);
        ShowMana();
    }

    public void DamageHero(CardController card, bool isEnemyAttacked)
    {
        if (isEnemyAttacked)
            EnemyHP = Mathf.Clamp(EnemyHP - card.Card.Attack, 0, int.MaxValue);
        else
            PlayerHP = Mathf.Clamp(PlayerHP - card.Card.Attack, 0, int.MaxValue);
        ShowHP();
        card.OnDamageDeal();
        CheckForResult();
    }

    public void CheckForResult()
    {
        if (EnemyHP == 0 || PlayerHP == 0)
        {
            ResualtGO.SetActive(true);
            if (EnemyHP == 0)
                ResualtText.text = "Winner";
            else ResualtText.text = "Loser";
            StopAllCoroutines();
        }
    }

    public void CheckCardsForManaAvaliability()
    {
        foreach (var card in PlayerHandCards)
        {
            card.CardInfo.HighlightManaAvailability(PlayerMana);
        }
    }

    public void HighlightAsTarget (CardController attacker, bool highlighted)
    {
        List<CardController> targets = new List<CardController>();

        if (attacker.Card.isSpell)
        {
            if (attacker.Card.SpellTarget == Card.TargetType.NO_TARGET)
                targets = new List<CardController>();
            else if (attacker.Card.SpellTarget == Card.TargetType.ALLY_CARD_TARGET)
                targets = PlayerFieldCards;
            else
                targets = EnemyFieldCards;

        }
        else
        {

            if (EnemyFieldCards.Exists(x => x.Card.isProvacation))
                targets = EnemyFieldCards.FindAll(x => x.Card.isProvacation);
            else
            {
                targets = EnemyFieldCards;
                EnemyHero.HighlightAsTarget(highlighted);
            }

            foreach (var card in targets)
            {
                if (attacker.Card.isSpell)
                    card.CardInfo.HighlightAsSpellTarget(highlighted);
                else
                    card.CardInfo.HighlightAsTarget(highlighted);
            }
        }
    }

    public void RestartGame()
    {
        StopAllCoroutines();

        foreach (var card in PlayerFieldCards)
        {
            Destroy(card.gameObject);
        }
        foreach (var card in PlayerHandCards)
        {
            Destroy(card.gameObject);
        }
        foreach (var card in EnemyFieldCards)
        {
            Destroy(card.gameObject);
        }
        foreach (var card in EnemyHandCards)
        {
            Destroy(card.gameObject);
        }

        PlayerFieldCards.Clear();
        EnemyFieldCards.Clear();
        PlayerHandCards.Clear();
        EnemyHandCards.Clear();

        StartGame();
    }
}
