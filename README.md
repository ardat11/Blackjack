# Blackjack Unity

This project is an object-oriented Blackjack (21) game developed using the Unity engine. It features optimized memory management through **Object Pooling** and a polished user experience powered by **DOTween** animations.

## 🚀 Features

- **Object Pooling:** To enhance performance, cards are managed via `DeckPool.cs`. Instead of constant instantiation and destruction, cards are recycled from a pre-allocated pool.
- **Flexible Deck System:** The game allows configuration of the number of decks used and a reshuffle threshold, ensuring a customizable gameplay experience.
- **Animated Gameplay:** Card movements, dealer reveals, and UI transitions are handled using **DOTween** for a smooth and professional feel.
- **Smart Score Calculation:** The Ace logic is implemented to automatically calculate its value as 1 or 11 based on what benefits the hand most.

## 🛠 Technologies Used

- **Unity 2022+**
- **C#** (Monobehaviour-based logic)
- **DOTween:** For card movement and rotation animations.
- **TextMesh Pro:** For dynamic and high-quality UI text rendering.

## 📁 Project Structure

- **BlackJackManager.cs:** The core engine that manages the game loop, card dealing, point calculation, and win/loss conditions.
- **DeckPool.cs:** Pre-loads all card variations (Clubs, Diamonds, Hearts, Spades) and manages their lifecycle in memory.
- **Card.cs:** A base class that stores the rank and suit of an individual card.

## 🎮 How to Play

1. **Start Round:** Initializes the game; the player and dealer are dealt two cards each (one of the dealer's cards remains face down).
2. **Hit:** The player requests an additional card to increase their total.
3. **Stand:** The player ends their turn. The dealer reveals the hidden card and continues drawing until reaching at least 17 points.
4. **Bust:** If any side exceeds 21 points, they automatically lose the round.
5. **New Game:** Clears the table, checks the deck status, and prepares for a new round.

## ⚙️ Settings

You can adjust the following parameters within the `BlackJackManager` component:
* **Deck Count**: Number of decks used in the game.
* **Min Points**: The minimum score the dealer must reach before they stop drawing cards (Default: 17).
* **Reshuffle Threshold**: The deck is automatically reset when the remaining card count falls below this value.
