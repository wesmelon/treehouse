let farmRows = 22;
let farmCols = 32;
const viewRows = 8;
const viewCols = 12;
const tileSize = 72;
const growthTimers = { seed: 2200, sprout: 2800 };

const dayLengthMs = 45000;
let dayCount = 1;
let timeMs = 0;
let isNight = false;
let needsSleep = false;

const grid = [];
const groundNoise = [];
let player = { x: farmCols / 2, y: farmRows / 2 };
let playerAvatar = 'raccoon';
const friendTypes = ['bunny', 'puppy', 'kitty'];
const friends = [];
const fruits = [];
const fruitTypes = ['watermelon', 'strawberry', 'banana', 'apple'];
const fruitLabels = {
  watermelon: 'Watermelon',
  strawberry: 'Strawberry',
  banana: 'Banana',
  apple: 'Apple',
};
const decorations = [];
const houses = [
  {
    id: 'cabin',
    name: 'Cozy Cabin',
    door: { x: 7.5, y: 6.5 },
    interior: {
      width: 12,
      height: 8,
      exit: { x: 6, y: 7.2 },
      furniture: [
        { type: 'table', x: 6, y: 4 },
        { type: 'rug', x: 6, y: 5.5 },
        { type: 'plantpot', x: 3, y: 3.5 },
        { type: 'books', x: 9, y: 3.2 },
        { type: 'lamp', x: 2.5, y: 5.8 },
        { type: 'shelf', x: 10.5, y: 2.5 },
        { type: 'couch', x: 6, y: 6.7 },
        { type: 'chest', x: 2.8, y: 6.8 },
      ],
      npcs: [
        { type: 'bunny', name: 'Berry', lines: ['Hi friend!', 'I like carrots.', 'Let\'s play!'], x: 4.5, y: 5 },
      ],
    },
  },
  {
    id: 'barn',
    name: 'Sunny Barn',
    door: { x: 22.5, y: 12.5 },
    interior: {
      width: 14,
      height: 9,
      exit: { x: 7, y: 8 },
      furniture: [
        { type: 'hay', x: 4, y: 4 },
        { type: 'hay', x: 10, y: 4 },
        { type: 'bench', x: 7, y: 5.5 },
        { type: 'bucket', x: 5, y: 6.6 },
        { type: 'lamp', x: 11.5, y: 6.5 },
        { type: 'feedbag', x: 3.5, y: 5.5 },
      ],
      npcs: [
        { type: 'puppy', name: 'Sunny', lines: ['Woof! Welcome!', 'Have you seen any bones?', 'Naps are nice.'], x: 7, y: 4.5 },
        { type: 'kitty', name: 'Momo', lines: ['Meow! Hi.', 'I love baskets.', 'Flowers smell nice.'], x: 9, y: 6.2 },
      ],
    },
  },
];
let currentHouse = null;
let currentArea = 'farm'; // 'farm' or 'house'
const returnPositions = {};
let baskets = 0;
let fruitsCollected = 0;
let moodWords = ['Happy Farm', 'Cozy Garden', 'Sunny Day', 'Gentle Breeze'];
let moodIndex = 0;
let gamepadIndex = null;
const inputState = { up: false, down: false, left: false, right: false };
let gamepadAxes = { x: 0, y: 0 };
let lastActionPressed = false;
let lastActionAt = 0;
const actionCooldown = 260;
let walkCycle = 0;
let isMoving = false;
let friendMoveTimer = 0;
const friendMoveDelay = 1.2;
let fruitTimer = 0;
const fruitDelay = 3.2;
const maxFruits = 4;
let lastFrameTime = performance.now();

const gridEl = document.getElementById('game-canvas');
const ctx = gridEl.getContext('2d');
const moodEl = document.getElementById('mood');
const dayEl = document.getElementById('day-state');
const harvestEl = document.getElementById('harvest-count');
const fruitEl = document.getElementById('fruit-count');

function init() {
  createWorld();
  attachControls();
  setupGamepad();
  spawnFriends();
  placeDecorations();
  bindSaveLoad();
  updateDayState();
  updateHarvest();
  updateFruitCount();
  updateMood();
  resizeCanvas();
  requestAnimationFrame(gameLoop);
}

function createWorld() {
  for (let y = 0; y < farmRows; y++) {
    for (let x = 0; x < farmCols; x++) {
      grid.push({ stage: 'empty', timer: 0 });
      groundNoise.push(Math.random());
    }
  }
}

function resizeCanvas() {
  const rect = gridEl.parentElement.getBoundingClientRect();
  gridEl.width = rect.width;
  gridEl.height = Math.max(420, rect.height);
}

window.addEventListener('resize', resizeCanvas);

function attachControls() {
  window.addEventListener('keydown', (e) => {
    const key = e.key.toLowerCase();
    if (['arrowup', 'arrowdown', 'arrowleft', 'arrowright', ' ', 'w', 'a', 's', 'd'].includes(e.key.toLowerCase())) {
      e.preventDefault();
    }
    setInput(key, true);
  });

  window.addEventListener('keyup', (e) => {
    setInput(e.key.toLowerCase(), false);
  });

  document.querySelectorAll('.control-btn').forEach((btn) => {
    const dir = btn.dataset.dir;
    if (dir === 'action') {
      btn.addEventListener('click', triggerAction);
    } else {
      btn.addEventListener('pointerdown', () => setTouchDir(dir, true));
      btn.addEventListener('pointerup', () => setTouchDir(dir, false));
      btn.addEventListener('pointerleave', () => setTouchDir(dir, false));
      btn.addEventListener('touchend', () => setTouchDir(dir, false));
    }
  });

  document.querySelectorAll('.avatar-btn').forEach((btn) => {
    btn.addEventListener('click', () => {
      playerAvatar = btn.dataset.avatar;
      document.querySelectorAll('.avatar-btn').forEach((b) => b.classList.toggle('selected', b === btn));
      setMood('You picked a new friend!');
    });
    if (btn.dataset.avatar === playerAvatar) {
      btn.classList.add('selected');
    }
  });
}

function bindSaveLoad() {
  const saveBtn = document.getElementById('save-map');
  const loadInput = document.getElementById('load-map');
  if (saveBtn) {
    saveBtn.addEventListener('click', () => {
      const state = serializeState();
      if (window.MapState?.download) {
        window.MapState.download(state);
        setMood('Saved your map!');
      }
    });
  }
  if (loadInput) {
    loadInput.addEventListener('change', (e) => {
      const file = e.target.files?.[0];
      if (!file) return;
      const reader = new FileReader();
      reader.onload = () => {
        try {
          const data = JSON.parse(reader.result);
          applyState(data);
          setMood('Loaded map file!');
        } catch (err) {
          console.error('Failed to load map', err);
          setMood('Oops, could not load map.');
        }
      };
      reader.readAsText(file);
    });
  }
}

function setTouchDir(dir, on) {
  if (dir === 'up') inputState.up = on;
  if (dir === 'down') inputState.down = on;
  if (dir === 'left') inputState.left = on;
  if (dir === 'right') inputState.right = on;
}

function setInput(key, on) {
  if (key === 'arrowup' || key === 'w') inputState.up = on;
  if (key === 'arrowdown' || key === 's') inputState.down = on;
  if (key === 'arrowleft' || key === 'a') inputState.left = on;
  if (key === 'arrowright' || key === 'd') inputState.right = on;
  if (key === ' ') {
    if (on) triggerAction();
  }
}

function triggerAction() {
  const now = performance.now();
  if (now - lastActionAt < actionCooldown) return;
  lastActionAt = now;
  doAction();
}

function setupGamepad() {
  window.addEventListener('gamepadconnected', (e) => {
    gamepadIndex = e.gamepad.index;
    setMood('Controller ready to play!');
  });
  window.addEventListener('gamepaddisconnected', () => {
    gamepadIndex = null;
    gamepadAxes = { x: 0, y: 0 };
  });
  requestAnimationFrame(readGamepad);
}

function readGamepad() {
  if (gamepadIndex !== null) {
    const gp = navigator.getGamepads()[gamepadIndex];
    if (!gp) {
      gamepadIndex = null;
      gamepadAxes = { x: 0, y: 0 };
    } else {
      handleGamepadInput(gp);
    }
  }
  requestAnimationFrame(readGamepad);
}

function handleGamepadInput(gp) {
  const axisX = gp.axes[0] || 0;
  const axisY = gp.axes[1] || 0;
  let dx = 0;
  let dy = 0;

  if (gp.buttons[12]?.pressed) dy = -1;
  else if (gp.buttons[13]?.pressed) dy = 1;
  if (gp.buttons[14]?.pressed) dx = -1;
  else if (gp.buttons[15]?.pressed) dx = 1;

  if (dx === 0 && dy === 0) {
    if (Math.abs(axisX) > 0.25) dx = axisX;
    if (Math.abs(axisY) > 0.25) dy = axisY;
  }

  gamepadAxes = { x: dx, y: dy };

  const actionPressed =
    gp.buttons[0]?.pressed ||
    gp.buttons[1]?.pressed ||
    gp.buttons[2]?.pressed ||
    gp.buttons[3]?.pressed ||
    gp.buttons[9]?.pressed;

  if (actionPressed && !lastActionPressed) {
    triggerAction();
  }
  lastActionPressed = actionPressed;
}

function gameLoop(timestamp) {
  const dt = Math.min(0.05, (timestamp - lastFrameTime) / 1000);
  lastFrameTime = timestamp;
  update(dt);
  render();
  requestAnimationFrame(gameLoop);
}

function update(dt) {
  const dtMs = dt * 1000;
  advanceTime(dtMs);
  stepGrowth(dtMs);
  updateMovement(dt);
  maybeMoveFriends(dt);
  maybeSpawnFruit(dt);
}

function updateMovement(dt) {
  const speed = 3.2; // cells per second (slower for toddle pace)
  let vx = (inputState.right ? 1 : 0) - (inputState.left ? 1 : 0);
  let vy = (inputState.down ? 1 : 0) - (inputState.up ? 1 : 0);

  if (Math.abs(gamepadAxes.x) > Math.abs(vx)) vx = gamepadAxes.x;
  if (Math.abs(gamepadAxes.y) > Math.abs(vy)) vy = gamepadAxes.y;

  const mag = Math.hypot(vx, vy);
  if (mag > 1) {
    vx /= mag;
    vy /= mag;
  }

  const moving = mag > 0.05;
  if (moving) {
    walkCycle = (walkCycle + dt * 8) % (Math.PI * 2);
  }
  isMoving = moving;

  const bounds = getAreaBounds();
  player.x = clamp(player.x + vx * speed * dt, 0.5, bounds.width - 0.5);
  player.y = clamp(player.y + vy * speed * dt, 0.5, bounds.height - 0.5);
  if (currentArea === 'farm') collectFruitIfHere();
}

function advanceTime(dtMs) {
  if (isNight && needsSleep) return;
  timeMs += dtMs;
  if (!isNight && timeMs >= dayLengthMs) {
    isNight = true;
    needsSleep = true;
    document.body.classList.add('night');
    setMood('Sleepy time! Find the bed.');
    updateDayState();
  }
}

function stepGrowth(dtMs) {
  grid.forEach((cell) => {
    if (cell.stage === 'seed' || cell.stage === 'sprout' || cell.stage === 'harvest') {
      cell.timer -= dtMs;
      if (cell.timer <= 0) {
        advanceStage(cell);
      }
    }
  });
}

function advanceStage(cell) {
  if (cell.stage === 'seed') {
    cell.stage = 'sprout';
    cell.timer = growthTimers.sprout;
  } else if (cell.stage === 'sprout') {
    cell.stage = 'flower';
  } else if (cell.stage === 'harvest') {
    cell.stage = 'empty';
    cell.timer = 0;
  }
}

function doAction() {
  if (isNight && needsSleep) {
    if (onBed()) {
      sleepThroughNight();
    } else {
      setMood('Sleepy! Stand on the bed to sleep.');
    }
    return;
  }

  if (currentArea === 'farm' && tryEnterHouse()) {
    return;
  }

  if (currentArea === 'house') {
    if (tryExitHouse()) return;
    if (tryTalkToNpc()) return;
    setMood('Cozy inside!');
    return;
  }

  const targetX = Math.round(player.x - 0.5);
  const targetY = Math.round(player.y - 0.5);
  const cell = grid[cellIndex(targetX, targetY)];
  if (!cell) return;

  collectFruitIfHere();

  if (cell.stage === 'empty') {
    cell.stage = 'seed';
    cell.timer = growthTimers.seed;
    setMood('Planted a cozy seed!');
  } else if (cell.stage === 'flower') {
    cell.stage = 'harvest';
    cell.timer = 700;
    baskets += 1;
    updateHarvest();
    setMood('Yum! Harvest time!');
  } else if (cell.stage === 'seed' || cell.stage === 'sprout') {
    cell.timer = Math.max(cell.timer - 800, 300);
    setMood('Splash! Helping it grow.');
  }
}

function maybeMoveFriends(dt) {
  if (currentArea !== 'farm') return;
  friendMoveTimer += dt;
  if (friendMoveTimer < friendMoveDelay) return;
  friendMoveTimer = 0;
  friends.forEach((f) => {
    const dirs = [
      [0, 0],
      [1, 0],
      [-1, 0],
      [0, 1],
      [0, -1],
    ];
    const [dx, dy] = dirs[Math.floor(Math.random() * dirs.length)];
    f.x = clamp(f.x + dx, 1, farmCols - 2);
    f.y = clamp(f.y + dy, 1, farmRows - 2);
  });
}

function maybeSpawnFruit(dt) {
  if (currentArea !== 'farm') return;
  fruitTimer += dt;
  if (fruitTimer < fruitDelay || fruits.length >= maxFruits) return;
  fruitTimer = 0;
  for (let attempts = 0; attempts < 20; attempts++) {
    const x = Math.floor(Math.random() * farmCols);
    const y = Math.floor(Math.random() * farmRows);
    const cell = grid[cellIndex(x, y)];
    const blockedDecor = decorations.some((d) => d.x === x && d.y === y);
    if (cell.stage === 'empty' && !blockedDecor && !friends.some((f) => f.x === x && f.y === y)) {
      const type = Math.random() < 0.6 ? 'watermelon' : fruitTypes[Math.floor(Math.random() * fruitTypes.length)];
      fruits.push({ x: x + 0.5, y: y + 0.5, type });
      setMood('A fruit appeared!');
      break;
    }
  }
}

function collectFruitIfHere() {
  const idx = fruits.findIndex((f) => distance(player, f) < 0.45);
  if (idx !== -1) {
    const fruit = fruits[idx];
    fruits.splice(idx, 1);
    fruitsCollected += 1;
    updateFruitCount();
    setMood(`Yum! ${fruitLabels[fruit.type] || 'Fruit'} time!`);
  }
}

function placeDecorations() {
  const centerX = Math.floor(farmCols / 2);
  const centerY = Math.floor(farmRows / 2);
  const spots = [
    { x: centerX, y: centerY - 3, type: '🎄' },
    { x: centerX - 2, y: centerY - 2, type: '🎁' },
    { x: centerX + 2, y: centerY - 2, type: '⭐' },
    { x: centerX - 3, y: centerY + 1, type: '🛏️' },
  ];
  spots.forEach((s) => {
    decorations.push({ x: clamp(s.x, 1, farmCols - 2) + 0.5, y: clamp(s.y, 1, farmRows - 2) + 0.5, type: s.type });
  });
}

function spawnFriends() {
  friendTypes.forEach((avatar, i) => {
    friends.push({
      avatar,
      x: clamp(Math.floor(farmCols / 2) + i * 2 - 2, 1, farmCols - 2) + 0.5,
      y: clamp(Math.floor(farmRows / 2) + i * 2 - 2, 1, farmRows - 2) + 0.5,
    });
  });
}

function onBed() {
  return decorations.some((d) => d.type === '🛏️' && distance(player, d) < 0.7);
}

function sleepThroughNight() {
  isNight = false;
  needsSleep = false;
  timeMs = 0;
  dayCount += 1;
  document.body.classList.remove('night');
  setMood('Good morning! Time to grow more.');
  updateDayState();
}

function render() {
  const viewWidth = viewCols * tileSize;
  const viewHeight = viewRows * tileSize;
  const scale = Math.min(gridEl.width / viewWidth, gridEl.height / viewHeight);
  const bounds = getAreaBounds();
  const camera = {
    x: clamp(player.x - viewCols / 2, 0, bounds.width - viewCols),
    y: clamp(player.y - viewRows / 2, 0, bounds.height - viewRows),
  };

  ctx.clearRect(0, 0, gridEl.width, gridEl.height);
  drawSky(ctx);
  if (currentArea === 'farm') {
    drawGround(ctx, camera, scale);
    drawHouses(ctx, camera, scale);
    drawFruits(ctx, camera, scale);
    drawDecor(ctx, camera, scale);
    drawPlants(ctx, camera, scale);
    drawFriends(ctx, camera, scale);
  } else {
    drawHouseInterior(ctx, camera, scale);
  }
  drawPlayer(ctx, camera, scale);
  if (isNight && currentArea === 'farm') drawNightOverlay(ctx);
}

function drawSky(ctx) {
  const grad = ctx.createLinearGradient(0, 0, 0, gridEl.height);
  if (isNight) {
    grad.addColorStop(0, '#0f1a2b');
    grad.addColorStop(1, '#1e2f44');
  } else {
    grad.addColorStop(0, '#c9f2ff');
    grad.addColorStop(1, '#fafff6');
  }
  ctx.fillStyle = grad;
  ctx.fillRect(0, 0, gridEl.width, gridEl.height);
}

function drawGround(ctx, camera, scale) {
  // Base meadow fill
  const baseGrad = ctx.createLinearGradient(0, 0, 0, gridEl.height);
  if (isNight) {
    baseGrad.addColorStop(0, '#15231a');
    baseGrad.addColorStop(1, '#203525');
  } else {
    baseGrad.addColorStop(0, '#c8f0c2');
    baseGrad.addColorStop(1, '#a6d9a0');
  }
  ctx.fillStyle = baseGrad;
  ctx.fillRect(0, 0, gridEl.width, gridEl.height);

  const startX = Math.floor(camera.x);
  const startY = Math.floor(camera.y);
  const endX = Math.ceil(camera.x + viewCols);
  const endY = Math.ceil(camera.y + viewRows);

  for (let y = startY; y < endY; y++) {
    for (let x = startX; x < endX; x++) {
      const idx = cellIndex(x, y);
      const noise = groundNoise[idx];
      const lush = isNight ? '#22442c' : '#8ecf7f';
      const meadow = isNight ? '#1b3526' : '#a5d89d';
      const mix = noise > 0.55 ? lush : meadow;
      const [sx, sy] = worldToScreen(x + 0.5, y + 0.5, camera, scale);
      const patchW = tileSize * 1.05 * scale;
      const patchH = tileSize * 1.05 * scale;
      ctx.fillStyle = mix;
      drawRoundedRect(ctx, sx - patchW / 2, sy - patchH / 2, patchW, patchH, 12 * scale);

      if (noise < 0.18) {
        ctx.fillStyle = isNight ? 'rgba(76,56,45,0.7)' : 'rgba(160,115,76,0.35)';
        ctx.beginPath();
        ctx.ellipse(sx, sy, patchW * 0.25, patchH * 0.18, 0.2, 0, Math.PI * 2);
        ctx.fill();
      }
    }
  }
}

function drawPlants(ctx, camera, scale) {
  const startX = Math.floor(camera.x);
  const startY = Math.floor(camera.y);
  const endX = Math.ceil(camera.x + viewCols);
  const endY = Math.ceil(camera.y + viewRows);

  for (let y = startY; y < endY; y++) {
    for (let x = startX; x < endX; x++) {
      const cell = grid[cellIndex(x, y)];
      if (!cell || cell.stage === 'empty') continue;
      const [sx, sy] = worldToScreen(x + 0.5, y + 0.5, camera, scale);
      drawPlantSprite(ctx, sx, sy, scale, cell.stage);
    }
  }
}

function drawFriends(ctx, camera, scale) {
  friends.forEach((f) => {
    const [sx, sy] = worldToScreen(f.x, f.y, camera, scale);
    drawCritterSprite(ctx, sx, sy, scale, f.avatar, false, 0);
  });
}

function drawDecor(ctx, camera, scale) {
  decorations.forEach((d) => {
    const [sx, sy] = worldToScreen(d.x, d.y, camera, scale);
    drawDecorSprite(ctx, sx, sy, scale, d.type);
  });
}

function drawFruits(ctx, camera, scale) {
  fruits.forEach((fruit) => {
    const [sx, sy] = worldToScreen(fruit.x, fruit.y, camera, scale);
    drawFruitSprite(ctx, sx, sy, scale, fruit.type);
  });
}

function drawPlayer(ctx, camera, scale) {
  const [sx, sy] = worldToScreen(player.x, player.y, camera, scale);
  drawCritterSprite(ctx, sx, sy, scale, playerAvatar, true, walkCycle, isMoving);
}

function drawNightOverlay(ctx) {
  ctx.save();
  ctx.fillStyle = 'rgba(10, 15, 35, 0.35)';
  ctx.fillRect(0, 0, gridEl.width, gridEl.height);
  ctx.restore();
}

function worldToScreen(wx, wy, camera, scale) {
  const x = (wx - camera.x) * tileSize * scale;
  const y = (wy - camera.y) * tileSize * scale;
  const offsetX = (gridEl.width - viewCols * tileSize * scale) / 2;
  const offsetY = (gridEl.height - viewRows * tileSize * scale) / 2;
  return [x + offsetX, y + offsetY];
}

function drawPlantSprite(ctx, sx, sy, scale, stage) {
  const radius = tileSize * 0.35 * scale;
  ctx.save();
  const colors = {
    seed: '#f3e0c2',
    sprout: '#b7e9b0',
    flower: '#ffe3f5',
    harvest: '#ffe8c0',
  };
  ctx.fillStyle = colors[stage] || '#fff';
  ctx.shadowColor = stage === 'flower' ? 'rgba(255,95,178,0.35)' : stage === 'harvest' ? 'rgba(255,180,0,0.5)' : 'rgba(0,0,0,0.1)';
  ctx.shadowBlur = stage === 'flower' || stage === 'harvest' ? 18 : 8;
  ctx.beginPath();
  ctx.arc(sx, sy, radius, 0, Math.PI * 2);
  ctx.fill();

  // stem detail
  ctx.shadowBlur = 0;
  ctx.strokeStyle = '#4a7c4c';
  ctx.lineWidth = Math.max(2, 4 * scale);
  ctx.beginPath();
  ctx.moveTo(sx, sy + radius * 0.6);
  ctx.lineTo(sx, sy - radius * 0.4);
  ctx.stroke();

  // leaf/bloom
  ctx.fillStyle = stage === 'flower' ? '#ff6faf' : '#4dbb6a';
  ctx.beginPath();
  ctx.ellipse(sx - radius * 0.3, sy - radius * 0.2, radius * 0.25, radius * 0.18, -0.3, 0, Math.PI * 2);
  ctx.fill();
  if (stage === 'flower' || stage === 'harvest') {
    ctx.fillStyle = '#ffd54f';
    ctx.beginPath();
    ctx.arc(sx, sy - radius * 0.45, radius * 0.2, 0, Math.PI * 2);
    ctx.fill();
  }
  ctx.restore();
}

function drawFruitSprite(ctx, sx, sy, scale, type) {
  const radius = tileSize * 0.3 * scale;
  ctx.save();
  const palette = {
    watermelon: ['#ff6b7a', '#2fbf71'],
    strawberry: ['#ff5e5b', '#2fa64a'],
    banana: ['#ffd166', '#f2b705'],
    apple: ['#ff3b30', '#2fa64a'],
  };
  const [base, accent] = palette[type] || ['#ffd166', '#2fa64a'];
  ctx.fillStyle = base;
  ctx.shadowColor = 'rgba(0,0,0,0.18)';
  ctx.shadowBlur = 10;
  ctx.beginPath();
  ctx.arc(sx, sy, radius, 0, Math.PI * 2);
  ctx.fill();

  // stem
  ctx.shadowBlur = 0;
  ctx.strokeStyle = accent;
  ctx.lineWidth = Math.max(2, 3 * scale);
  ctx.beginPath();
  ctx.moveTo(sx, sy - radius * 0.9);
  ctx.lineTo(sx, sy - radius * 1.2);
  ctx.stroke();

  // leaf
  ctx.fillStyle = accent;
  ctx.beginPath();
  ctx.ellipse(sx + radius * 0.2, sy - radius * 1.05, radius * 0.25, radius * 0.12, 0.4, 0, Math.PI * 2);
  ctx.fill();

  // stripes for watermelon
  if (type === 'watermelon') {
    ctx.strokeStyle = 'rgba(0,0,0,0.1)';
    ctx.lineWidth = 2 * scale;
    for (let i = -2; i <= 2; i++) {
      ctx.beginPath();
      ctx.arc(sx, sy, radius * (0.5 + i * 0.05), Math.PI * 0.1, Math.PI * 1.2);
      ctx.stroke();
    }
  }
  ctx.restore();
}

function drawDecorSprite(ctx, sx, sy, scale, type) {
  const baseShadow = 'rgba(0,0,0,0.2)';
  ctx.save();
  if (type === '🎄') {
    const h = tileSize * 0.9 * scale;
    ctx.shadowColor = baseShadow;
    ctx.shadowBlur = 12;
    ctx.fillStyle = '#2fa064';
    ctx.beginPath();
    ctx.moveTo(sx, sy - h * 0.5);
    ctx.lineTo(sx - h * 0.35, sy + h * 0.15);
    ctx.lineTo(sx + h * 0.35, sy + h * 0.15);
    ctx.closePath();
    ctx.fill();
    ctx.fillStyle = '#8b5a2b';
    ctx.fillRect(sx - h * 0.08, sy + h * 0.15, h * 0.16, h * 0.2);
  } else if (type === '🎁') {
    const size = tileSize * 0.7 * scale;
    ctx.shadowColor = baseShadow;
    ctx.shadowBlur = 10;
    ctx.fillStyle = '#ff9e9d';
    ctx.fillRect(sx - size / 2, sy - size / 2, size, size);
    ctx.fillStyle = '#ffd166';
    ctx.fillRect(sx - size / 8, sy - size / 2, size / 4, size);
    ctx.fillRect(sx - size / 2, sy - size / 8, size, size / 4);
  } else if (type === '⭐') {
    const r = tileSize * 0.38 * scale;
    ctx.shadowColor = baseShadow;
    ctx.shadowBlur = 12;
    ctx.fillStyle = '#ffd54f';
    ctx.beginPath();
    for (let i = 0; i < 5; i++) {
      const angle = (Math.PI / 2.5) * i - Math.PI / 2;
      const x1 = sx + Math.cos(angle) * r;
      const y1 = sy + Math.sin(angle) * r;
      ctx.lineTo(x1, y1);
      const angle2 = angle + Math.PI / 5;
      const x2 = sx + Math.cos(angle2) * r * 0.5;
      const y2 = sy + Math.sin(angle2) * r * 0.5;
      ctx.lineTo(x2, y2);
    }
    ctx.closePath();
    ctx.fill();
  } else if (type === '🛏️') {
    const w = tileSize * 0.9 * scale;
    const h = tileSize * 0.5 * scale;
    ctx.shadowColor = baseShadow;
    ctx.shadowBlur = 10;
    ctx.fillStyle = '#8fb2ff';
    ctx.fillRect(sx - w / 2, sy - h / 2, w, h);
    ctx.fillStyle = '#e6f0ff';
    ctx.fillRect(sx + w * 0.05, sy - h / 2, w * 0.35, h * 0.5);
    ctx.fillStyle = '#5f6b7a';
    ctx.fillRect(sx - w / 2, sy + h * 0.1, w, h * 0.18);
  }
  ctx.restore();
}

function drawCritterSprite(ctx, sx, sy, scale, type, highlight = false, walkPhase = 0, moving = false) {
  const palettes = {
    raccoon: { body: '#55463f', accent: '#7d6f68', mask: '#3a312d', belly: '#f1e7dc' },
    bunny: { body: '#f5d8e3', accent: '#ffdce8', mask: '#f7ebf1', belly: '#fff8fc' },
    puppy: { body: '#d6a16a', accent: '#f2c387', mask: '#b78454', belly: '#f7e3c7' },
    kitty: { body: '#f2d37f', accent: '#ffe7a8', mask: '#d9ba6a', belly: '#fff6db' },
  };
  const palette = palettes[type] || { body: '#7b5de7', accent: '#a38bf2', mask: '#6049c1', belly: '#ede6ff' };
  const r = tileSize * 0.32 * scale;
  const bob = moving ? Math.sin(walkPhase * 2) * r * 0.08 : 0;
  const legSwing = moving ? Math.sin(walkPhase * 2) * r * 0.15 : 0;
  ctx.save();
  ctx.shadowColor = highlight ? 'rgba(0,0,0,0.28)' : 'rgba(0,0,0,0.18)';
  ctx.shadowBlur = highlight ? 14 : 10;

  // Body
  ctx.fillStyle = palette.body;
  ctx.beginPath();
  ctx.ellipse(sx, sy + r * 0.25 + bob, r * 1.1, r * 1.1, 0, 0, Math.PI * 2);
  ctx.fill();

  // Belly
  ctx.fillStyle = palette.belly;
  ctx.beginPath();
  ctx.ellipse(sx, sy + r * 0.4 + bob, r * 0.65, r * 0.5, 0, 0, Math.PI * 2);
  ctx.fill();

  // Head
  ctx.fillStyle = palette.body;
  ctx.beginPath();
  ctx.ellipse(sx, sy - r * 0.15 + bob, r * 0.95, r * 0.9, 0, 0, Math.PI * 2);
  ctx.fill();

  // Ears per type
  ctx.fillStyle = palette.accent;
  if (type === 'bunny') {
    drawEar(ctx, sx - r * 0.55, sy - r * 0.8 + bob, r * 0.32, r * 0.9);
    drawEar(ctx, sx + r * 0.55, sy - r * 0.8 + bob, r * 0.32, r * 0.9);
  } else if (type === 'kitty') {
    drawTriangleEar(ctx, sx - r * 0.65, sy - r * 0.7 + bob, r * 0.45);
    drawTriangleEar(ctx, sx + r * 0.65, sy - r * 0.7 + bob, r * 0.45);
  } else if (type === 'puppy') {
    drawFlopEar(ctx, sx - r * 0.75, sy - r * 0.3 + bob, r * 0.35, r * 0.6);
    drawFlopEar(ctx, sx + r * 0.75, sy - r * 0.3 + bob, r * 0.35, r * 0.6);
  } else {
    // raccoon/other round ears
    ctx.beginPath();
    ctx.arc(sx - r * 0.7, sy - r * 0.5 + bob, r * 0.35, 0, Math.PI * 2);
    ctx.arc(sx + r * 0.7, sy - r * 0.5 + bob, r * 0.35, 0, Math.PI * 2);
    ctx.fill();
  }

  // Face mask and muzzle
  ctx.shadowBlur = 0;
  if (type === 'raccoon') {
    ctx.fillStyle = palette.mask;
    ctx.beginPath();
    ctx.ellipse(sx, sy - r * 0.1 + bob, r * 1.05, r * 0.65, 0, 0, Math.PI * 2);
    ctx.fill();
    ctx.fillStyle = palette.belly;
    ctx.beginPath();
    ctx.ellipse(sx, sy + r * 0.15 + bob, r * 0.65, r * 0.55, 0, 0, Math.PI * 2);
    ctx.fill();
  } else {
    ctx.fillStyle = palette.belly;
    ctx.beginPath();
    ctx.ellipse(sx, sy + r * 0.1 + bob, r * 0.65, r * 0.55, 0, 0, Math.PI * 2);
    ctx.fill();
  }

  // Eyes
  ctx.fillStyle = '#2f2f2f';
  const eyeOffset = r * 0.35;
  ctx.beginPath();
  ctx.arc(sx - eyeOffset, sy - r * 0.05 + bob, r * 0.12, 0, Math.PI * 2);
  ctx.arc(sx + eyeOffset, sy - r * 0.05 + bob, r * 0.12, 0, Math.PI * 2);
  ctx.fill();

  // Nose
  ctx.fillStyle = '#e67a7a';
  ctx.beginPath();
  ctx.arc(sx, sy + r * 0.18 + bob, r * 0.12, 0, Math.PI * 2);
  ctx.fill();

  // Whiskers for kitty/bunny
  if (type === 'kitty' || type === 'bunny') {
    ctx.strokeStyle = 'rgba(0,0,0,0.35)';
    ctx.lineWidth = 2 * scale;
    ctx.beginPath();
    ctx.moveTo(sx - r * 0.5, sy + r * 0.18 + bob);
    ctx.lineTo(sx - r * 0.9, sy + r * 0.1 + bob);
    ctx.moveTo(sx - r * 0.5, sy + r * 0.25 + bob);
    ctx.lineTo(sx - r * 0.9, sy + r * 0.3 + bob);
    ctx.moveTo(sx + r * 0.5, sy + r * 0.18 + bob);
    ctx.lineTo(sx + r * 0.9, sy + r * 0.1 + bob);
    ctx.moveTo(sx + r * 0.5, sy + r * 0.25 + bob);
    ctx.lineTo(sx + r * 0.9, sy + r * 0.3 + bob);
    ctx.stroke();
  }

  // Tail hints
  ctx.fillStyle = palette.accent;
  if (type === 'raccoon') {
    ctx.beginPath();
    ctx.ellipse(sx + r * 0.95, sy + r * 0.5 + bob, r * 0.4, r * 0.2, 0.4, 0, Math.PI * 2);
    ctx.fill();
    ctx.fillStyle = palette.mask;
    ctx.fillRect(sx + r * 0.7, sy + r * 0.45 + bob, r * 0.2, r * 0.1);
  } else if (type === 'puppy') {
    ctx.beginPath();
    ctx.ellipse(sx + r * 0.9, sy + r * 0.4 + bob, r * 0.35, r * 0.18, -0.4, 0, Math.PI * 2);
    ctx.fill();
  } else if (type === 'kitty') {
    ctx.beginPath();
    ctx.ellipse(sx + r * 0.9, sy + r * 0.45 + bob, r * 0.32, r * 0.16, 0.2, 0, Math.PI * 2);
    ctx.fill();
  }

  // Simple leg swings
  const legY = sy + r * 0.9 + bob;
  ctx.fillStyle = palette.mask;
  const legW = r * 0.18;
  const legH = r * 0.32;
  ctx.save();
  ctx.translate(sx, legY);
  ctx.rotate(legSwing * 0.05);
  ctx.fillRect(-r * 0.5 - legW, -legH * 0.2, legW, legH);
  ctx.rotate(-legSwing * 0.1);
  ctx.fillRect(r * 0.5, -legH * 0.2, legW, legH);
  ctx.restore();

  ctx.fillStyle = palette.belly;
  ctx.fillRect(sx - r * 0.4, legY - legH * 0.2, r * 0.8, legH * 0.35);
  ctx.restore();
}

function drawRoundedRect(ctx, x, y, w, h, r) {
  ctx.beginPath();
  ctx.moveTo(x + r, y);
  ctx.lineTo(x + w - r, y);
  ctx.quadraticCurveTo(x + w, y, x + w, y + r);
  ctx.lineTo(x + w, y + h - r);
  ctx.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
  ctx.lineTo(x + r, y + h);
  ctx.quadraticCurveTo(x, y + h, x, y + h - r);
  ctx.lineTo(x, y + r);
  ctx.quadraticCurveTo(x, y, x + r, y);
  ctx.closePath();
  ctx.fill();
}

function drawEar(ctx, x, y, rx, ry) {
  ctx.beginPath();
  ctx.ellipse(x, y, rx, ry, 0, 0, Math.PI * 2);
  ctx.fill();
}

function drawTriangleEar(ctx, x, y, size) {
  ctx.beginPath();
  ctx.moveTo(x, y - size);
  ctx.lineTo(x - size * 0.8, y + size * 0.6);
  ctx.lineTo(x + size * 0.8, y + size * 0.6);
  ctx.closePath();
  ctx.fill();
}

function drawFlopEar(ctx, x, y, rx, ry) {
  ctx.beginPath();
  ctx.ellipse(x, y, rx, ry, 0.2, 0, Math.PI * 2);
  ctx.fill();
}

function drawHouses(ctx, camera, scale) {
  houses.forEach((house) => {
    const [sx, sy] = worldToScreen(house.door.x, house.door.y, camera, scale);
    drawHouseExterior(ctx, sx, sy, scale, house.id);
  });
}

function drawHouseExterior(ctx, sx, sy, scale, style = 'cabin') {
  const w = tileSize * 1.6 * scale;
  const h = tileSize * 1.2 * scale;
  ctx.save();
  ctx.shadowColor = 'rgba(0,0,0,0.22)';
  ctx.shadowBlur = 16;
  ctx.fillStyle = style === 'barn' ? '#f6c172' : '#d9b48f';
  drawRoundedRect(ctx, sx - w / 2, sy - h, w, h, 10 * scale);
  ctx.fillStyle = style === 'barn' ? '#d86a5f' : '#b57d5b';
  ctx.fillRect(sx - w / 2, sy - h, w, h * 0.25);

  // Roof
  ctx.fillStyle = style === 'barn' ? '#c44a46' : '#8c5c3e';
  ctx.beginPath();
  ctx.moveTo(sx - w / 2 - 6 * scale, sy - h);
  ctx.lineTo(sx, sy - h - h * 0.45);
  ctx.lineTo(sx + w / 2 + 6 * scale, sy - h);
  ctx.closePath();
  ctx.fill();

  // Door
  const doorW = w * 0.25;
  const doorH = h * 0.4;
  ctx.fillStyle = '#6b4a32';
  ctx.fillRect(sx - doorW / 2, sy - doorH, doorW, doorH);
  ctx.restore();
}

function drawHouseInterior(ctx, camera, scale) {
  if (!currentHouse) return;
  const area = currentHouse.interior;
  const [ox, oy] = worldToScreen(area.width / 2, area.height / 2, camera, scale);
  const w = area.width * tileSize * scale;
  const h = area.height * tileSize * scale;
  ctx.save();
  ctx.fillStyle = isNight ? '#2b2c32' : '#f5e6d8';
  drawRoundedRect(ctx, ox - w / 2, oy - h / 2, w, h, 16 * scale);

  // Floor planks
  ctx.strokeStyle = 'rgba(0,0,0,0.06)';
  ctx.lineWidth = 2 * scale;
  for (let i = 0; i < area.width; i++) {
    const x = ox - w / 2 + i * tileSize * scale;
    ctx.beginPath();
    ctx.moveTo(x, oy - h / 2);
    ctx.lineTo(x, oy + h / 2);
    ctx.stroke();
  }

  // Furniture
  area.furniture.forEach((f) => {
    const [sx, sy] = worldToScreen(f.x, f.y, camera, scale);
    drawFurniture(ctx, sx, sy, scale, f.type);
  });

  // NPCs
  area.npcs.forEach((npc) => {
    const [sx, sy] = worldToScreen(npc.x, npc.y, camera, scale);
    drawCritterSprite(ctx, sx, sy, scale, npc.type);
  });

  // Exit mat
  const [ex, ey] = worldToScreen(area.exit.x, area.exit.y, camera, scale);
  ctx.fillStyle = '#9bc3ff';
  drawRoundedRect(ctx, ex - tileSize * 0.4 * scale, ey - tileSize * 0.15 * scale, tileSize * 0.8 * scale, tileSize * 0.3 * scale, 6 * scale);

  ctx.restore();
}

function drawFurniture(ctx, sx, sy, scale, type) {
  ctx.save();
  if (type === 'table') {
    ctx.fillStyle = '#c8a16b';
    drawRoundedRect(ctx, sx - 50 * scale, sy - 30 * scale, 100 * scale, 60 * scale, 8 * scale);
  } else if (type === 'rug') {
    ctx.fillStyle = '#fdd274';
    drawRoundedRect(ctx, sx - 70 * scale, sy - 26 * scale, 140 * scale, 52 * scale, 12 * scale);
  } else if (type === 'plantpot') {
    ctx.fillStyle = '#9bc3ff';
    drawRoundedRect(ctx, sx - 16 * scale, sy - 24 * scale, 32 * scale, 48 * scale, 8 * scale);
    ctx.fillStyle = '#58b56b';
    ctx.beginPath();
    ctx.arc(sx, sy - 30 * scale, 20 * scale, 0, Math.PI * 2);
    ctx.fill();
  } else if (type === 'books') {
    ctx.fillStyle = '#6a74d9';
    drawRoundedRect(ctx, sx - 20 * scale, sy - 20 * scale, 40 * scale, 12 * scale, 4 * scale);
    ctx.fillStyle = '#ff9e9d';
    drawRoundedRect(ctx, sx - 18 * scale, sy - 6 * scale, 36 * scale, 12 * scale, 4 * scale);
  } else if (type === 'hay') {
    ctx.fillStyle = '#f1d27a';
    drawRoundedRect(ctx, sx - 40 * scale, sy - 20 * scale, 80 * scale, 40 * scale, 10 * scale);
  } else if (type === 'bench') {
    ctx.fillStyle = '#b38b6d';
    drawRoundedRect(ctx, sx - 50 * scale, sy - 16 * scale, 100 * scale, 32 * scale, 8 * scale);
  } else if (type === 'lamp') {
    ctx.fillStyle = '#d8b377';
    drawRoundedRect(ctx, sx - 6 * scale, sy - 30 * scale, 12 * scale, 60 * scale, 6 * scale);
    ctx.fillStyle = '#ffd166';
    ctx.beginPath();
    ctx.arc(sx, sy - 32 * scale, 16 * scale, 0, Math.PI * 2);
    ctx.fill();
  } else if (type === 'shelf') {
    ctx.fillStyle = '#b07a4f';
    drawRoundedRect(ctx, sx - 40 * scale, sy - 20 * scale, 80 * scale, 12 * scale, 6 * scale);
    ctx.fillStyle = '#6a74d9';
    drawRoundedRect(ctx, sx - 30 * scale, sy - 30 * scale, 14 * scale, 10 * scale, 3 * scale);
    ctx.fillStyle = '#ff9e9d';
    drawRoundedRect(ctx, sx - 12 * scale, sy - 30 * scale, 14 * scale, 10 * scale, 3 * scale);
    ctx.fillStyle = '#8fda89';
    drawRoundedRect(ctx, sx + 6 * scale, sy - 30 * scale, 14 * scale, 10 * scale, 3 * scale);
  } else if (type === 'couch') {
    ctx.fillStyle = '#9fc1ff';
    drawRoundedRect(ctx, sx - 70 * scale, sy - 18 * scale, 140 * scale, 36 * scale, 12 * scale);
    ctx.fillStyle = '#7aa6f5';
    drawRoundedRect(ctx, sx - 50 * scale, sy - 26 * scale, 100 * scale, 12 * scale, 8 * scale);
  } else if (type === 'chest') {
    ctx.fillStyle = '#c48a51';
    drawRoundedRect(ctx, sx - 32 * scale, sy - 18 * scale, 64 * scale, 36 * scale, 8 * scale);
    ctx.fillStyle = '#8a5a2b';
    ctx.fillRect(sx - 32 * scale, sy - 4 * scale, 64 * scale, 6 * scale);
  } else if (type === 'bucket') {
    ctx.fillStyle = '#c9d6e3';
    drawRoundedRect(ctx, sx - 16 * scale, sy - 20 * scale, 32 * scale, 40 * scale, 10 * scale);
    ctx.fillStyle = '#9bb5cb';
    ctx.fillRect(sx - 16 * scale, sy - 8 * scale, 32 * scale, 6 * scale);
  } else if (type === 'feedbag') {
    ctx.fillStyle = '#f7d27d';
    drawRoundedRect(ctx, sx - 26 * scale, sy - 24 * scale, 52 * scale, 48 * scale, 10 * scale);
    ctx.fillStyle = '#d9b35a';
    ctx.fillRect(sx - 26 * scale, sy - 6 * scale, 52 * scale, 8 * scale);
  }
  ctx.restore();
}

function tryEnterHouse() {
  for (const house of houses) {
    if (distance(player, house.door) < 0.75) {
      enterHouse(house);
      return true;
    }
  }
  return false;
}

function tryExitHouse() {
  if (!currentHouse) return false;
  const exit = currentHouse.interior.exit;
  if (distance(player, exit) < 0.7) {
    exitHouse();
    return true;
  }
  return false;
}

function tryTalkToNpc() {
  if (!currentHouse) return false;
  const npc = currentHouse.interior.npcs.find((n) => distance(player, n) < 0.75);
  if (npc) {
    const line = npc.lines[Math.floor(Math.random() * npc.lines.length)];
    setMood(`${npc.name}: ${line}`);
    return true;
  }
  return false;
}

function enterHouse(house) {
  currentHouse = house;
  currentArea = 'house';
  returnPositions[house.id] = { x: player.x, y: player.y };
  player.x = house.interior.exit.x;
  player.y = house.interior.exit.y;
  setMood(`Welcome to the ${house.name}!`);
}

function exitHouse() {
  if (!currentHouse) return;
  const back = returnPositions[currentHouse.id] || { x: currentHouse.door.x, y: currentHouse.door.y + 0.6 };
  currentArea = 'farm';
  player = { x: back.x, y: back.y };
  setMood('Back to the farm!');
  currentHouse = null;
}

function getAreaBounds() {
  if (currentArea === 'house' && currentHouse) {
    return { width: currentHouse.interior.width, height: currentHouse.interior.height };
  }
  return { width: farmCols, height: farmRows };
}

function serializeState() {
  return {
    farmRows,
    farmCols,
    grid,
    fruits,
    decorations,
    houses,
    friends,
    player,
    baskets,
    fruitsCollected,
    dayCount,
    timeMs,
    isNight,
    needsSleep,
    currentHouseId: currentHouse?.id || null,
    currentArea,
    returnPositions,
  };
}

function applyState(state) {
  if (!state || !state.grid || !state.farmRows || !state.farmCols) return;
  farmRows = state.farmRows;
  farmCols = state.farmCols;
  grid.length = 0;
  groundNoise.length = 0;
  for (let y = 0; y < farmRows; y++) {
    for (let x = 0; x < farmCols; x++) {
      grid.push(state.grid[y * farmCols + x] || { stage: 'empty', timer: 0 });
      groundNoise.push(Math.random());
    }
  }
  fruits.length = 0;
  state.fruits?.forEach((f) => fruits.push(f));
  decorations.length = 0;
  state.decorations?.forEach((d) => decorations.push(d));
  friends.length = 0;
  state.friends?.forEach((f) => friends.push(f));
  player = state.player || player;
  baskets = state.baskets || 0;
  fruitsCollected = state.fruitsCollected || 0;
  dayCount = state.dayCount || 1;
  timeMs = state.timeMs || 0;
  isNight = !!state.isNight;
  needsSleep = !!state.needsSleep;
  currentArea = state.currentArea || 'farm';
  currentHouse = state.currentHouseId ? houses.find((h) => h.id === state.currentHouseId) || null : null;
  Object.assign(returnPositions, state.returnPositions || {});
  resizeCanvas();
}

function updateHarvest() {
  harvestEl.textContent = `Baskets: ${baskets}`;
}

function updateFruitCount() {
  fruitEl.textContent = `Fruits: ${fruitsCollected}`;
}

function updateDayState() {
  const icon = isNight ? '🌙' : '☀️';
  const label = isNight ? 'Night' : 'Day';
  dayEl.textContent = `${label} ${dayCount} ${icon}`;
}

function updateMood() {
  moodIndex = (moodIndex + 1) % moodWords.length;
  moodEl.textContent = `😊 ${moodWords[moodIndex]}`;
}

function setMood(text) {
  moodEl.textContent = `😊 ${text}`;
  setTimeout(updateMood, 2200);
}

function clamp(num, min, max) {
  return Math.min(Math.max(num, min), max);
}

function cellIndex(x, y) {
  return y * farmCols + x;
}

function distance(a, b) {
  return Math.hypot(a.x - b.x, a.y - b.y);
}

init();
