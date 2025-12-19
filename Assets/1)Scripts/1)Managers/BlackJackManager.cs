using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BlackJackManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int deckCount = 1;
    [SerializeField] private int minPoints = 17;
    [SerializeField] private float cardSpacing = 0.3f;
    [SerializeField] private int reshuffleThreshold = 15;

    [Header("References")]
    [SerializeField] private Transform dealerCardSpawnPoint;
    [SerializeField] private TMP_Text dealerPointText;
    [SerializeField] private Transform deckTop;
    [SerializeField] private Transform playerCardSpawnPoint;
    [SerializeField] private TMP_Text playerPointText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject NewGameButton;

    private readonly List<(eCardRank, eCardSuit)> gameDeck = new();
    private Queue<(eCardRank, eCardSuit)> playDeck = new();

    private readonly List<Card> dealerHand = new();
    private readonly List<Card> playerHand = new();

    private int dealerHandPoint;
    private int playerHandPoint;

    private Card dealerSecretCard;

    private void Awake()
    {
        InitDeck();
    }

    private void InitDeck()
    {
        gameDeck.Clear();
        for (int d = 0; d < deckCount; d++)
        {
            for (int i = 1; i <= 13; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    gameDeck.Add(((eCardRank)i, (eCardSuit)j));
                }
            }
        }
        ResetPlayDeck();
    }

    private void ResetPlayDeck()
    {
        ShuffleDeck();
        playDeck.Clear();
        playDeck = new Queue<(eCardRank, eCardSuit)>(gameDeck);
    }

    private void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = gameDeck.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (gameDeck[n], gameDeck[k]) = (gameDeck[k], gameDeck[n]);
        }
    }

    #region ButtonFuncs

    public void StartRound()
    {
        StartCoroutine(StartRoundCoroutine());
    }

    public void Hit()
    {
        DealCard(eDealType.ToPlayer);
    }

    public void Stand()
    {
        StartCoroutine(StandCoroutine());
    }

    public void NewGame()
    {
        CheckDeckStatus();
        
        foreach (var card in dealerHand) DeckPool.instance.ReturnToPool(card);
        dealerHand.Clear();
        dealerHandPoint = 0;
        dealerPointText.text = "0";

        foreach (var card in playerHand) DeckPool.instance.ReturnToPool(card);
        playerHand.Clear();
        playerHandPoint = 0;
        playerPointText.text = "0";

        resultText.transform.parent.gameObject.SetActive(false);
        NewGameButton.SetActive(false);
        
        StartRound();
    }

    #endregion

    private void CheckDeckStatus()
    {
        if (playDeck.Count < reshuffleThreshold)
        {
            ResetPlayDeck();
        }
    }

    private IEnumerator StandCoroutine()
    {
        bool isFlipDone = false;
        var cardInfo = playDeck.Dequeue();

        dealerSecretCard.transform.DORotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() =>
        {
            Vector3 currentPos = dealerSecretCard.transform.position;
            
            DeckPool.instance.ReturnToPool(dealerSecretCard);
            dealerHand.Remove(dealerSecretCard);
            
            dealerSecretCard = DeckPool.instance.GetFromPool(cardInfo, currentPos, transform);
            dealerHand.Add(dealerSecretCard);
            
            dealerSecretCard.transform.rotation = Quaternion.Euler(0, 90, 0);
            dealerSecretCard.transform.DORotate(new Vector3(0, 0, 0), 0.25f).OnComplete(() => {
                isFlipDone = true;
            });

            dealerHandPoint = CalculateHandPoint(dealerHand);
            UpdatePointText(eDealType.ToDealer);
        });

        yield return new WaitUntil(() => isFlipDone);
        yield return new WaitForSeconds(0.3f);

        while (dealerHandPoint < minPoints)
        {
            DealCard(eDealType.ToDealer);
            yield return new WaitForSeconds(0.8f);
        }

        if (CheckCrash(eDealType.ToDealer)) yield break;
        CheckComparison();
    }

    private IEnumerator StartRoundCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            DealCard(eDealType.ToPlayer);
            yield return new WaitForSeconds(0.6f);

            (eCardRank, eCardSuit) dealerCardInfo = (i == 0) ? playDeck.Dequeue() : (eCardRank.CardBack, eCardSuit.CardBack);
            OpenCardWithAnim(dealerCardInfo, eDealType.ToDealer);
            yield return new WaitForSeconds(0.6f);
        }
    }

    private void DealCard(eDealType dealType)
    {
        if (playDeck.Count == 0) ResetPlayDeck();
        (eCardRank, eCardSuit) cardInfo = playDeck.Dequeue();
        OpenCardWithAnim(cardInfo, dealType);
    }

    private void OpenCardWithAnim((eCardRank, eCardSuit) cardInfo, eDealType dealType)
    {
        Vector3 initialDeckPos = deckTop.position;
        Transform spawnPoint = (dealType == eDealType.ToDealer) ? dealerCardSpawnPoint : playerCardSpawnPoint;
        int cardCount = (dealType == eDealType.ToDealer) ? dealerHand.Count : playerHand.Count;

        Vector3 targetPos = spawnPoint.position + new Vector3(cardSpacing * cardCount, 0, -0.01f * cardCount);

        deckTop.DOMove(targetPos, 0.5f).OnComplete(() =>
        {
            Card card = DeckPool.instance.GetFromPool(cardInfo, targetPos, transform);

            if (dealType == eDealType.ToDealer) dealerHand.Add(card);
            else playerHand.Add(card);

            deckTop.position = initialDeckPos;
            UpdatePoint(card, dealType);
        });
    }

    private void UpdatePoint(Card card, eDealType dealType)
    {
        if (card.GetRank() == eCardRank.CardBack)
        {
            dealerSecretCard = card;
            return;
        }

        if (dealType == eDealType.ToDealer)
            dealerHandPoint = CalculateHandPoint(dealerHand);
        else
            playerHandPoint = CalculateHandPoint(playerHand);

        UpdatePointText(dealType);
        CheckCrash(dealType);
    }

    private int CalculateHandPoint(List<Card> hand)
    {
        int total = 0;
        int aceCount = 0;

        foreach (var card in hand)
        {
            eCardRank rank = card.GetRank();
            if (rank == eCardRank.CardBack) continue;

            if (rank == eCardRank.Ace) aceCount++;
            else if ((int)rank >= 10) total += 10;
            else total += (int)rank;
        }

        for (int i = 0; i < aceCount; i++)
        {
            if (total + 11 <= 21) total += 11;
            else total += 1;
        }
        return total;
    }

    private void UpdatePointText(eDealType dealType)
    {
        if (dealType == eDealType.ToDealer)
            dealerPointText.text = dealerHandPoint.ToString();
        else
            playerPointText.text = playerHandPoint.ToString();
    }

    private bool CheckCrash(eDealType dealType)
    {
        int point = (dealType == eDealType.ToDealer) ? dealerHandPoint : playerHandPoint;
        if (point > 21)
        {
            SetWinner(dealType == eDealType.ToDealer ? eDealType.ToPlayer : eDealType.ToDealer);
            return true;
        }
        return false;
    }

    private void CheckComparison()
    {
        if (playerHandPoint > dealerHandPoint) SetWinner(eDealType.ToPlayer);
        else if (playerHandPoint < dealerHandPoint) SetWinner(eDealType.ToDealer);
        else SetWinner(eDealType.Nobody);
    }

    private void SetWinner(eDealType winnerType)
    {
        string msg = winnerType == eDealType.ToDealer ? "Dealer Wins!" :
                     winnerType == eDealType.ToPlayer ? "Player Wins!" : "Push!";
        
        resultText.text = msg;
        resultText.transform.parent.gameObject.SetActive(true);
        NewGameButton.SetActive(true);
    }
}
public enum eDealType
{
    ToDealer,
    ToPlayer,
    Nobody,
}

