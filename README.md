# Little Farm Friend

A toddler-friendly farming mini-game inspired by cozy farming sims. Everything is bright, gentle, and low-pressure—no losing, only planting, watering, and harvesting sparkly flowers.

## Game description

Play as a friendly critter (raccoon, bunny, puppy, or kitty) tending a larger scrolling farm. Wander smoothly around a soft meadow, plant seeds, watch sprouts pop, harvest sparkly flowers into baskets, find fruits like juicy 🍉 to collect, visit houses with friendly characters and simple furniture, and wave to wandering pals. Simple inputs and a gentle pace keep it toddler-appropriate.

## Play it

- Open `index.html` in a browser (double-click or `python -m http.server` and visit `http://localhost:8000`).
- Works with keyboard or touch.

## Controls

- Move: Arrow keys or WASD.
- Action: Space to plant/water/harvest the tile you stand on.
- Touch: Use the on-screen arrows and the pink **Plant / Harvest** button.
- Controller: D-pad or left stick to move; A/B/X/Y or Start to plant/harvest.
- Click/tap a tile: walk there and try planting/harvesting.
- Camera: The view scrolls to keep your critter near the center on the larger farm.
- Characters: Tap a critter button (raccoon, bunny, puppy, kitty) to play as them; friendly pals wander the farm. Sprites are drawn, not emoji.
- Fruits: Watermelons and friends appear on empty tiles—walk over them to collect as drawn fruit sprites.
- Day/Night: Time passes; when it’s night, stand on the bed and press Action to sleep and start the next morning.
- Seasonal: Festive touches like a cozy 🎄 and presents hang out near the center.
- Houses: Stand at a door and press Action to enter; talk to friends inside or step on the exit mat to return outside.

## Loop

- Empty tile → Action plants a seed.
- Seeds sprout and become sparkly flowers.
- Action on a flower harvests a basket (score).
- Action on seeds/sprouts gently waters them to speed growth.
- Random fruits (often 🍉) appear on empty tiles—collect them by moving onto them; tracked separately from baskets.
- Seasonal decorations are just for cheer; they don’t block planting or movement.
- When night falls, other actions pause until you sleep in the bed; sleeping resets time to morning and bumps the day count.
- Houses contain NPC pals and simple furniture; Action near a door enters, Action near a pal shows a friendly line.

## Design for toddlers

- No failure states; only positive feedback and sparkles.
- Big, high-contrast buttons and chunky grid tiles.
- Short growth times so payoff is quick.
- Soft status text rotates through cozy moods.

## Files

- `index.html` — layout and UI.
- `styles.css` — colors, layout, and friendly visuals.
- `game.js` — simple grid logic, growth, movement, and rendering.
