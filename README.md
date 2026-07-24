# Sora

A character mod for **Slay the Spire 2**, built on [BaseLib](https://www.nuget.org/packages/Alchyr.Sts2.BaseLib).

Sora fights with the Keyblade, building **Situation Points (SP)** to unleash Situation Commands, calling on **Links** with his friends, and transforming into his **Ultimate Form**.

Values shown in brackets are the card's upgraded value, e.g. `5(8)` means 5 damage, 8 when upgraded.

---

## Mechanics

- **Situation Points (SP)** — A resource tracked by Sora's Keyblade relic. It is gained from attacking and at the start of your turn, and is spent on **Situation Commands** (special generated cards). Several cards and relics grant bonus SP.
- **Links** — Timed buffs representing Sora's bonds: **Riku's Link**, **Kairi's Link**, **Cloud's Link**, **Mickey's Link**, and **Yen Sid's Link**. Each lasts a number of turns, and some cards scale with the number of **active Links** you currently have.
- **Ultimate Form** — A duration buff. While in Ultimate Form, Sora builds toward Ultimate Finishers. Cards that grant it can extend its remaining turns if you are already transformed.
- **Companion cards** — Cards that call an ally into battle. A handful of powers trigger whenever you play one.

---

## Cards

### Common

| Card | Cost | Type | Effect |
|------|------|------|--------|
| **Aerial Sweep** | 1 | Attack | Deal 5(8) damage to ALL enemies. Gain 1 SP for each enemy hit. |
| **Slapshot** | 1 | Attack | Deal 5(8) damage to ALL enemies. Gain 2(3) Block for each enemy hit. |
| **Damage Control** | 1 | Skill | Gain 5 Block. Heal HP equal to 20%(30%) of your missing HP. |
| **Treasure Hunter** | 1 | Skill | Draw 1(2) cards. Gain 2(3) SP. |

### Uncommon

| Card | Cost | Type | Effect |
|------|------|------|--------|
| **Situation Boost** | 1 | Power · Innate | Whenever you play a card that costs 2 or more Energy, gain 5 SP. |
| **Bond of Light** | 2(1) | Power | Each time you play a Companion card, draw 1 card. |
| **Yen Sid's Blessing** | 2(1) | Power | At the start of your turn, upgrade a random card in your hand. |

### Rare

| Card | Cost | Type | Effect |
|------|------|------|--------|
| **One Heart** | 1 | Skill | Gain 5(6) Block, and 5(6) Block for each active Link. |
| **Ultimate Combo** | 1 | Attack | Deal 13(18) damage. Gain Ultimate Form for 2 turns. If already in Ultimate Form, extend it by 1 turn instead. |
| **Heart Recovery** | 1 | Skill · Exhaust *(no Exhaust when upgraded)* | Gain Kairi's Link for 3 turns. All Skills in your hand cost 0 this turn. |
| **Power Of Waking** | 1 | Power *(Innate when upgraded)* | At the start of your turn, return a random Attack card from your discard pile to your hand. |
| **King's Guidance** | 2(1) | Power | Gain Mickey's Link. Gain 2 SP. *(Mickey's Link stacks, granting extra Energy and SP each turn.)* |
| **Negative Combo** | 2 | Power | Reduce SP gained per turn by 1. Gain 2(3) Strength each turn. |
| **Seven Lights** | 2(1) | Power | The first time you play a Companion card each turn, gain 1 Energy. *(Stacks.)* |
| **Synch Blade** | 2 | Power | Whenever you play an Attack, deal 5(7) damage to a random enemy. |
| **Finish Boost** | 2 | Power | The third Attack you play each turn deals 50%(75%) more damage. |
| **Dual Wield** | 3(2) | Power | Each turn, the first Attack card you play is played an additional time. |

### Ancient

| Card | Cost | Type | Effect |
|------|------|------|--------|
| **Ultimate Form** | 0 | Skill | Gain Ultimate Form for 4(5) turns. |
| **Formchange** | 2 | Skill · Exhaust | Gain 8(12) Block. Gain Ultimate Form for 3 turns. If already in Ultimate Form, extend it by 2 turns instead. |

---

## Relics

### Common

| Relic | Effect |
|-------|--------|
| **Starlight** | Start each combat with 5 SP. |

### Uncommon

| Relic | Effect |
|-------|--------|
| **Braveheart** | At the start of your turn, if you have Riku's Link, gain 4 Vigor. |
| **Destiny's Embrace** | At the start of your turn, if you have Kairi's Link, heal 2 HP. |
| **Wayfinder** | Situation Commands cost 0 and are upgraded. |

### Rare

| Relic | Effect |
|-------|--------|
| **Fenrir** | At the start of combat, gain Cloud's Link. |
| **Oblivion** | At the start of combat, gain Riku's Link. |
| **Oathkeeper** | At the start of combat, gain Kairi's Link. |

---

## Potions

| Potion | Rarity | Effect |
|--------|--------|--------|
| **Paopu Fruit** | Rare | Gain 30 SP. |
