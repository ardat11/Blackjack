using UnityEngine;

public class Card : MonoBehaviour
{
    private eCardRank rank;
    private eCardSuit suit;
    
    public eCardRank GetRank() => rank;
    public eCardSuit GetSuit() => suit;
    
    public void Init(eCardRank Rank,eCardSuit Suit)
    {
        rank = Rank;
        suit = Suit;
    }

}
