# Little Farm Friend — Design Doc

## Audience & goals
- Audience: toddlers and co-play caregivers.
- Goals: bright, low-pressure play; no failure states; instant feedback; simple inputs; short loops that reward exploration.

## Core loop
1) Move around the farm.  
2) Press Action to plant a seed on an empty tile.  
3) Seeds grow into sprouts, then sparkly flowers.  
4) Action on a flower harvests a basket.  
5) Action on seeds/sprouts waters them (speeds growth).  
6) Repeat anywhere; nothing wilts or is lost.

## World & camera
- Larger 22×32 world rendered as a soft meadow (no visible grid) with smooth camera follow.
- Visible viewport about 8×12 tiles worth of space; camera centers on the player and clamps to edges.
- Ground variation via subtle noise instead of strict tiles.
- Seasonal decor: static cheerful items (🎄, 🎁, ⭐) placed near center; non-blocking and purely visual.
- Bed decor (🛏️) near center for sleeping at night.
- Houses on the farm map with doors to enter interior scenes.

## Characters
- Playable avatars: raccoon, bunny, puppy, kitty; drawn as simple sprites (no emoji).
- Ambient friends: bunny, puppy, kitty wander gently, moving every ~1.2s within bounds.
- No collisions or blocking; purely friendly company.
- Fruits: random drops on empty tiles (watermelon favored) that kids collect by walking over; tracked as a simple count.
- House pals: each house interior includes a couple of NPCs with friendly lines; Action near them shows a short greeting.
- Map state: export/import JSON for persistence outside code.

## Controls & input
- Keyboard: WASD / arrows to move; Space to act.
- Touch: on-screen D-pad + big action button; tap a tile to move/action there.
- Controller: D-pad or left stick to move; A/B/X/Y/Start for action; mild cooldowns prevent rapid repeats.
- Input is always non-destructive; no pause or menu complexity.
- Enter/exit: Action near farm house doors to enter; Action on exit mat in interior returns outside.
- Sleep: when night begins, actions are gated until the player stands on the bed and presses Action, rolling to the next morning/day.

## Feedback & pacing
- Visual: sparkles on grown flowers/harvest-ready tiles; emoji-based plants/characters; status mood text cycles with gentle messages.
- Timing: growth timers ~2–3s; action cooldowns ~200–260ms; friend movement ~1.2s.
- Sound: not included yet (future-friendly for soft pops/chimes).

## UX principles for toddlers
- Big clickable areas; minimal text; high contrast but soft palette.
- No timers to beat; nothing can be lost; every action produces something positive.
- Short distance to reward: planting to flower is quick; harvest count reinforces progress.

## Future ideas (optional)
- Mini-map or edge glow to hint at the larger world.
- Simple quests (“fill 3 baskets”), sticker rewards.
- Gentle audio cues and haptics.
- Seasonal color tints or time-of-day shifts with no impact on pace.
