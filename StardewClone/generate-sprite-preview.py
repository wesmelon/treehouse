#!/usr/bin/env python3
"""Generate sprite preview image matching the game's sprites"""

from PIL import Image, ImageDraw, ImageFont
import os

# Constants
SPRITE_SIZE = 16
SCALE = 3
ATLAS_SIZE = 256

# Color definitions matching C# code
colors = {
    # Player
    'skin': (255, 220, 177),
    'shirt': (65, 105, 225),  # Royal blue
    'pants': (101, 67, 33),
    'hair': (139, 69, 19),
    'outline': (30, 30, 30),
    'eyes': (50, 50, 50),

    # Crop colors (harvestable stage)
    'parsnip': (255, 248, 220),
    'cauliflower': (255, 255, 240),
    'potato': (222, 184, 135),
    'tomato': (255, 69, 0),
    'corn': (255, 215, 0),
    'pumpkin': (255, 117, 24),
    'wheat': (245, 222, 179),

    'stem': (34, 139, 34),
    'darkGreen': (0, 100, 0),
    'soil': (101, 67, 33),

    # NPCs
    'npcSkin': (255, 220, 177),
    'pierreShirt': (34, 139, 34),
    'pierreHair': (85, 107, 47),
    'emilyDress': (147, 112, 219),
    'emilyHair': (255, 69, 0),
    'shaneShirt': (105, 105, 105),
    'shaneHair': (47, 79, 79),
    'npcPants': (60, 60, 60),

    'bg': (40, 40, 40)
}

def set_pixel(img, x, y, color):
    """Set a pixel in the image"""
    if 0 <= x < img.width and 0 <= y < img.height:
        img.putpixel((x, y), color)

def draw_player_down(img, x, y):
    """Draw player facing down"""
    c = colors

    # Head
    for py in range(4, 8):
        for px in range(5, 11):
            set_pixel(img, x + px, y + py, c['skin'])

    # Hair
    for i in range(5, 11):
        set_pixel(img, x + i, y + 4, c['hair'])

    # Eyes
    set_pixel(img, x + 6, y + 6, c['eyes'])
    set_pixel(img, x + 9, y + 6, c['eyes'])

    # Body - blue shirt
    for py in range(8, 12):
        for px in range(5, 11):
            set_pixel(img, x + px, y + py, c['shirt'])

    # Pants
    for py in range(12, 15):
        for px in range(6, 10):
            set_pixel(img, x + px, y + py, c['pants'])

def draw_crop_stage(img, x, y, stage, crop_color):
    """Draw crop at specific growth stage"""
    c = colors

    if stage == 0:  # Seed
        for py in range(12, 14):
            for px in range(7, 9):
                set_pixel(img, x + px, y + py, crop_color)
    elif stage == 4:  # Harvestable
        # Main crop body
        for py in range(10, 14):
            for px in range(6, 11):
                set_pixel(img, x + px, y + py, crop_color)
        # Leaves/stem
        for py in range(8, 11):
            for px in range(6, 11):
                set_pixel(img, x + px, y + py, c['darkGreen'])
    else:  # Growing stages
        height = 4 + stage * 2
        for py in range(height):
            for px in range(7, 9):
                set_pixel(img, x + px, y + 13 - py, c['darkGreen'])

def draw_npc(img, x, y, clothing_color, hair_color):
    """Draw NPC sprite"""
    c = colors

    # Head
    for py in range(4, 8):
        for px in range(5, 11):
            set_pixel(img, x + px, y + py, c['npcSkin'])

    # Hair
    for i in range(5, 11):
        set_pixel(img, x + i, y + 4, hair_color)

    # Eyes
    set_pixel(img, x + 6, y + 6, c['outline'])
    set_pixel(img, x + 9, y + 6, c['outline'])

    # Body
    for py in range(8, 12):
        for px in range(5, 11):
            set_pixel(img, x + px, y + py, clothing_color)

    # Pants
    for py in range(12, 15):
        for px in range(6, 10):
            set_pixel(img, x + px, y + py, c['npcPants'])

def create_atlas():
    """Create the full sprite atlas"""
    img = Image.new('RGB', (ATLAS_SIZE, ATLAS_SIZE), colors['bg'])

    # Player sprites - 4 directions (only drawing down for now)
    for direction in range(4):
        for frame in range(4):
            x = frame * SPRITE_SIZE
            y = direction * SPRITE_SIZE
            if direction == 0:  # Only down direction implemented
                draw_player_down(img, x, y)

    # Crop sprites - 7 types, 5 stages
    crop_colors = [
        colors['parsnip'], colors['cauliflower'], colors['potato'],
        colors['tomato'], colors['corn'], colors['pumpkin'], colors['wheat']
    ]

    for crop_idx in range(7):
        for stage in range(5):
            x = stage * SPRITE_SIZE
            y = (4 + crop_idx) * SPRITE_SIZE
            draw_crop_stage(img, x, y, stage, crop_colors[crop_idx])

    # NPC sprites
    npc_configs = [
        (colors['pierreShirt'], colors['pierreHair']),
        (colors['emilyDress'], colors['emilyHair']),
        (colors['shaneShirt'], colors['shaneHair'])
    ]

    for i, (clothing, hair) in enumerate(npc_configs):
        x = 0
        y = (11 + i) * SPRITE_SIZE
        draw_npc(img, x, y, clothing, hair)

    return img

def create_labeled_preview():
    """Create a full preview image with labels and scaled sprites"""
    # Create atlas
    atlas = create_atlas()

    # Scale up atlas for visibility
    atlas_scaled = atlas.resize((ATLAS_SIZE * 2, ATLAS_SIZE * 2), Image.NEAREST)

    # Create final image with room for labels
    preview_width = 1200
    preview_height = 1400
    preview = Image.new('RGB', (preview_width, preview_height), (26, 26, 26))
    draw = ImageDraw.Draw(preview)

    # Try to use a monospace font, fallback to default
    try:
        font_title = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono-Bold.ttf", 24)
        font_section = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono-Bold.ttf", 18)
        font_label = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf", 14)
    except:
        font_title = ImageFont.load_default()
        font_section = ImageFont.load_default()
        font_label = ImageFont.load_default()

    # Title
    title = "STARDEW VALLEY CLONE - Sprite Atlas"
    draw.text((preview_width // 2 - 250, 20), title, fill=(76, 175, 80), font=font_title)

    # Atlas
    atlas_x = (preview_width - ATLAS_SIZE * 2) // 2
    preview.paste(atlas_scaled, (atlas_x, 70))
    draw.text((atlas_x, 70 + ATLAS_SIZE * 2 + 10), "Full Sprite Atlas (16x16 px sprites, 2x scale)",
              fill=(136, 136, 136), font=font_label)

    # Detailed sprite sections
    y_offset = 70 + ATLAS_SIZE * 2 + 60

    # Player section
    draw.text((50, y_offset), "PLAYER CHARACTER - Farmer", fill=(76, 175, 80), font=font_section)
    y_offset += 35
    draw.text((50, y_offset), "Royal blue shirt, brown hair, animated", fill=(170, 170, 170), font=font_label)
    y_offset += 30

    for frame in range(4):
        sprite = atlas.crop((frame * SPRITE_SIZE, 0, (frame + 1) * SPRITE_SIZE, SPRITE_SIZE))
        sprite_scaled = sprite.resize((SPRITE_SIZE * SCALE, SPRITE_SIZE * SCALE), Image.NEAREST)
        preview.paste(sprite_scaled, (70 + frame * (SPRITE_SIZE * SCALE + 10), y_offset))
        draw.text((70 + frame * (SPRITE_SIZE * SCALE + 10), y_offset + SPRITE_SIZE * SCALE + 5),
                 f"Frame {frame + 1}", fill=(136, 136, 136), font=font_label)

    y_offset += 90

    # Crops section
    draw.text((50, y_offset), "CROPS - 7 Types, 5 Growth Stages", fill=(76, 175, 80), font=font_section)
    y_offset += 35

    crop_names = ['Parsnip', 'Cauliflower', 'Potato', 'Tomato', 'Corn', 'Pumpkin', 'Wheat']
    stages = ['Seed', 'Sprout', 'Growing', 'Mature', 'Harvest']

    for crop_idx in range(7):
        draw.text((70, y_offset), crop_names[crop_idx], fill=(255, 255, 255), font=font_label)

        for stage in range(5):
            sprite = atlas.crop((stage * SPRITE_SIZE, (4 + crop_idx) * SPRITE_SIZE,
                               (stage + 1) * SPRITE_SIZE, (5 + crop_idx) * SPRITE_SIZE))
            sprite_scaled = sprite.resize((SPRITE_SIZE * SCALE, SPRITE_SIZE * SCALE), Image.NEAREST)
            preview.paste(sprite_scaled, (200 + stage * (SPRITE_SIZE * SCALE + 5), y_offset - 5))

        y_offset += SPRITE_SIZE * SCALE + 15

    y_offset += 30

    # NPCs section
    draw.text((50, y_offset), "NPCs - Unique Characters", fill=(76, 175, 80), font=font_section)
    y_offset += 35

    npc_names = ['Pierre (Merchant)', 'Emily (Villager)', 'Shane (Villager)']

    for npc_idx in range(3):
        sprite = atlas.crop((0, (11 + npc_idx) * SPRITE_SIZE,
                           SPRITE_SIZE, (12 + npc_idx) * SPRITE_SIZE))
        sprite_scaled = sprite.resize((SPRITE_SIZE * SCALE, SPRITE_SIZE * SCALE), Image.NEAREST)
        preview.paste(sprite_scaled, (70, y_offset))
        draw.text((120, y_offset + 10), npc_names[npc_idx], fill=(255, 255, 255), font=font_label)
        y_offset += SPRITE_SIZE * SCALE + 20

    return preview

# Generate the preview
print("Generating sprite preview...")
preview = create_labeled_preview()
output_path = os.path.join(os.path.dirname(__file__), 'sprite-preview-screenshot.png')
preview.save(output_path)
print(f"Sprite preview saved to: {output_path}")
print(f"Image size: {preview.width}x{preview.height}")
