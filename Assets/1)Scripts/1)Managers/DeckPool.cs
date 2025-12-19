using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DeckPool : MonoBehaviour
{
    public static DeckPool instance;
    [Header("PoolSettings")]
    [Tooltip("The initial size of the pool.")]
    [SerializeField] private int basePoolSize = 10;
    [Tooltip("If pool doesn't have the card create this much instantly")]
    [SerializeField] private int creationSize = 4;
    
    
    [SerializeField] private Card[] CardGOs;
    [SerializeField] private Card cardBack;
    private Dictionary<(eCardRank,eCardSuit),Card> GoDict = new();
    
    private Dictionary<(eCardRank,eCardSuit),Queue<Card>> PoolDict = new();

    private void Awake()
    {
        instance = this;
        InitializeGoDict();
        InitializePool();
    }

    private void InitializeGoDict()
    {
        for (int i = 0; i < 13; i++)
        {   
            for (int j = 0; j < 4; j++)
            {
                GoDict.Add((    (eCardRank)i+1, (eCardSuit) j+1    ),CardGOs[4*i+j]);
            }
        }
        GoDict.Add((eCardRank.CardBack,eCardSuit.CardBack),cardBack);
    }
    
    private void InitializePool()
    {
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Queue<Card> pool = new Queue<Card>();
                (eCardRank, eCardSuit) cardInfo = ((eCardRank)i + 1, (eCardSuit)j + 1);
                PoolDict.Add(cardInfo,pool);

                for (int k = 0; k < basePoolSize; k++)
                {
                    InstantiateCard(cardInfo);
                    
                }
                
                
            }  
        }
        //Card Back
        Queue<Card> backPool = new Queue<Card>();
        (eCardRank, eCardSuit) backInfo = (eCardRank.CardBack, eCardSuit.CardBack);
        PoolDict.Add(backInfo,backPool);
        for (int k = 0; k < basePoolSize; k++)
        {
            InstantiateCard(backInfo);
                    
        }
    }

    #region PoolFuncs

    private Card InstantiateCard((eCardRank,eCardSuit) cardInfo)
    {
        Card card = Instantiate(GoDict[cardInfo], transform);
        card.Init(cardInfo.Item1, cardInfo.Item2);
        
        ReturnToPool(card);


        return card;
    }
    
    public Card GetFromPool((eCardRank,eCardSuit) cardInfo,Vector3 pos,Transform tr)
    {
        Card card = PoolDict[cardInfo].Dequeue();
        card.gameObject.SetActive(true);
        card.transform.SetParent(tr);
        card.transform.position = pos;
        return card;
    }

    public void ReturnToPool(Card card)
    {   
        card.transform.SetParent(transform);
        PoolDict[(card.GetRank(),card.GetSuit())].Enqueue(card);
        card.gameObject.SetActive(false);
    }
    
    

    #endregion
    
    
    
    
    
    
}

public enum eCardRank
{   
    CardBack =0,
    Ace = 1, // As
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11, // Vale (J)
    Queen = 12, // Kız (Q)
    King = 13 // Papaz (K)
}

public enum eCardSuit
{   
    CardBack=0,
    Clubs = 1,
    Diamonds = 2,
    Hearts = 3,
    Spades = 4
}