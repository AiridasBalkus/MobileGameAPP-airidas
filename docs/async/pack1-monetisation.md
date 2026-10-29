# Async Pack 1: Monetisation Models & Ethics: Mino
**Module:** Mobile Game Development (A12581) - **Student:** Airidas Balkus (20107228) - **Build:** v0.4.0

## 1. Economy loop
- Players earn 5 coins for every pickup collected during a run the pickup lines are placed next to hazards, so earning carries risk.
- Coins are spent on a single upgrade from the in-run HUD, priced on a 100 * 1.5^level curve (100, 150, 225, 337, 506..).
- Each level makes lane changes 35% faster, which players feel directly because lane change is Mino's only verb. On-device telemetry (`upgrade_purchased`) puts the first upgrade at 0:36 and earning at roughly 175 coins per minute.

## 2. Monetisation model if published
**Free to play with cosmetic-only IAP**, sold as **non-consumable** products: runner skins, trail colours and an underwater lane theme (the theme planned in my MDA). 
- I choose non-consumables because they are bought once and restored on reinstall. 
- Consumables, by contrast, are where currency packs and leftover-balance tricks live (Reading 1.1, 1.4). 
- Skins are priced directly in euro, with no gem currency in between. Coins stay earn-only. The Week 6 "Buy 500 coins" stub is replaced by the skin shop, so the speed upgrade can never be bought with money. Selling coins would have made Mino pay-to-win, which my MDA rules out.

- There are no paid random items, which keeps Mino clear of the PEGI 16 "paid random items" rating applied from June 2026 (Reading 2.3). That suits the intended teen and adult audience (PEGI 12+), with the "In-game Purchases" notice shown on the store page.

## 3. Where Mino could drift, and the guardrail I keep
- **Nagging after failure (interface trick, 3.5):** the Result screen appears straight after a crash, which is exactly where "continue?" offers are pushed. *Guardrail:* the shop is a static button that the player taps, never a pop-up. Skins move to a Menu, and there is no paid continue after death.
- **Pay-to-win (monetary, 3.3):** the coin stub sold the same coins that buy lane speed. *Guardrail:* coins are only ever earned; nothing that changes play is for sale.
- **Grinding (temporal, 3.2):** on the 1.5 * curve, level 6 costs 759 and level 7 costs 1,139, so later upgrades slow down sharply. With coins for sale, that grind would make paying look reasonable. *Guardrail:* I will have to tune the curve for fun using telemetry in Week 9, and cap the upgrade levels instead of stretching them out.
- **Premium-currency obfuscation and decoy bundles (monetary, 3.3):** gem packs that never match prices, or a huge "best value" anchor. *Guardrail:* every price is shown in real euro; items are sold one at a time, with no bundles.
- **Fake urgency and streak pressure (temporal and monetary, 3.2 - 3.3):** rotating "24 hrs only" skins or daily rewards that reset. *Guardrail:* every skin stays permanently available; there are no countdowns and no penalty for missing a day.

## The Reflection
Building the stub before doing this reading changed drastically how I see my own loop, I was tempted by a "new skin every 24hrs" or even "7 days remaining" offer, but ultimately that is the fake urgency pattern from my own guardrail list. However a solution I could go with is keep releasing new skins on a schedule that never leave the shop, so that would make the something for the player to come back to, with no countdown and nothing to lose buy missing it. In Week 6 the "Buy 500 coins" button felt harmless, because it sat on the Result screen as the lab asked. The reading showed me that it combined two of the patterns listed above: paid power and an offer placed right after failure, this button will be available only for Development builds (it gets the DebugOnly component). The telemerty says the pacing targets are met, but for me late in a run the coins feel very scarce, because the difficulty ramps up makes pickup lines rarer. That is exactly the kind of slowdown monetised games sell a fix for, so in week 9 I will fix it by raising the late-run earn rate, but not by selling coins: money can change how Mino looks, never how it plays.

**Sources:** Google Play Payments policy (randomised items, s.7); PEGI interactive risk criteria (March 2026); CPC Network Key Principles on in-game virtual currencies (2025, via Bird & Bird); Zagal, Björk and Lewis (2013), *Dark Patterns in the Design of Games*.
