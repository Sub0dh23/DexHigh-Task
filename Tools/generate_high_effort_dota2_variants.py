import urllib.request
import json
import base64
import os

def exec_figma(code: str):
    url = "http://localhost:8766/exec"
    payload = json.dumps({"code": code}).encode("utf-8")
    req = urllib.request.Request(url, data=payload, headers={"Content-Type": "application/json"})
    with urllib.request.urlopen(req) as resp:
        return json.loads(resp.read().decode("utf-8"))

script = """
async function createHighEffortDota2Variants() {
  await figma.loadFontAsync({ family: "Cinzel", style: "Regular" });
  await figma.loadFontAsync({ family: "Cinzel", style: "Bold" });
  await figma.loadFontAsync({ family: "Inter", style: "Regular" });
  await figma.loadFontAsync({ family: "Inter", style: "Bold" });

  const board = figma.createFrame();
  board.name = "Dota 2 Dragon Status Frame - High-Craft Sculpted Chassis";
  board.resize(1880, 1260);
  board.x = 3800;
  board.y = 0;
  board.fills = [{ type: "SOLID", color: { r: 0.043, g: 0.051, b: 0.067 } }]; // #0B0D11 Deep Dota 2 dark
  board.strokes = [{ type: "SOLID", color: { r: 0.22, g: 0.26, b: 0.32 } }];
  board.strokeWeight = 2;
  board.cornerRadius = 16;
  board.clipsContent = true;

  // Board Header
  const title = figma.createText();
  title.fontName = { family: "Cinzel", style: "Bold" };
  title.characters = "DRAGON STATUS FRAMES : BESPOKE SCULPTED CHASSIS";
  title.fontSize = 32;
  title.fills = [{ type: "SOLID", color: { r: 0.96, g: 0.86, b: 0.54 } }];
  title.x = 60;
  title.y = 50;
  board.appendChild(title);

  const subtitle = figma.createText();
  subtitle.fontName = { family: "Inter", style: "Regular" };
  subtitle.characters = "High-Effort Dota 2 Heraldry • Custom Vector Chassis • Runic Talismans • Block-Spaced HP Cells";
  subtitle.fontSize = 15;
  subtitle.fills = [{ type: "SOLID", color: { r: 0.58, g: 0.65, b: 0.76 } }];
  subtitle.x = 60;
  subtitle.y = 96;
  board.appendChild(subtitle);

  // Helper for ornate corner brackets
  function addFiligreeCorners(parent, w, h, goldColor = { r: 0.85, g: 0.70, b: 0.35 }) {
    const size = 16;
    const corners = [
      { x: 4, y: 4, path: "M 0 16 L 0 0 L 16 0 L 8 4 L 4 8 L 4 16 Z" },
      { x: w - 20, y: 4, path: "M 16 16 L 16 0 L 0 0 L 8 4 L 12 8 L 12 16 Z" },
      { x: 4, y: h - 20, path: "M 0 0 L 0 16 L 16 16 L 8 12 L 4 8 L 4 0 Z" },
      { x: w - 20, y: h - 20, path: "M 16 0 L 16 16 L 0 16 L 8 12 L 12 8 L 12 0 Z" }
    ];
    corners.forEach(c => {
      const v = figma.createVector();
      v.vectorPaths = [{ windingRule: "NONZERO", data: c.path }];
      v.fills = [{ type: "SOLID", color: goldColor }];
      v.x = c.x;
      v.y = c.y;
      parent.appendChild(v);
    });
  }

  // =========================================================================
  // VARIANT 1: "The Ironclad Dragonbastion" (Gothic Notched Shield & Runic Talismans)
  // =========================================================================
  const v1Col = figma.createFrame();
  v1Col.name = "Variant 1: Ironclad Dragonbastion";
  v1Col.resize(550, 1020);
  v1Col.x = 60;
  v1Col.y = 150;
  v1Col.fills = [{ type: "SOLID", color: { r: 0.065, g: 0.08, b: 0.105 } }];
  v1Col.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v1Col.strokeWeight = 1;
  v1Col.cornerRadius = 12;
  board.appendChild(v1Col);

  const v1Title = figma.createText();
  v1Title.fontName = { family: "Cinzel", style: "Bold" };
  v1Title.characters = "1. IRONCLAD DRAGONBASTION";
  v1Title.fontSize = 18;
  v1Title.fills = [{ type: "SOLID", color: { r: 0.94, g: 0.82, b: 0.46 } }];
  v1Title.x = 24;
  v1Title.y = 24;
  v1Col.appendChild(v1Title);

  const v1Desc = figma.createText();
  v1Desc.fontName = { family: "Inter", style: "Regular" };
  v1Desc.characters = "Gothic notched-shield portrait chassis with dragon horn crest. 8 discrete stone armor cells with amber decay ghost. Sculpted notched octagonal ability talismans.";
  v1Desc.fontSize = 12;
  v1Desc.fills = [{ type: "SOLID", color: { r: 0.62, g: 0.68, b: 0.78 } }];
  v1Desc.resize(502, 38);
  v1Desc.textAutoResize = "NONE";
  v1Desc.x = 24;
  v1Desc.y = 54;
  v1Col.appendChild(v1Desc);

  // Main Status Frame
  const v1Frame = figma.createFrame();
  v1Frame.name = "DragonStatusFrame_Ironclad";
  v1Frame.resize(502, 190);
  v1Frame.x = 24;
  v1Frame.y = 106;
  v1Frame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }]; // Heavy slate
  v1Frame.strokes = [{ type: "SOLID", color: { r: 0.55, g: 0.44, b: 0.22 } }]; // Forged brass
  v1Frame.strokeWeight = 2;
  v1Frame.cornerRadius = 6;
  v1Col.appendChild(v1Frame);
  addFiligreeCorners(v1Frame, 502, 190);

  // 1A. Sculpted Portrait Chassis (Notched Gothic Shield)
  // Outer chassis path: 104 x 108 px with notched 45-deg shoulders and shield point
  const v1ChassisPath = "M 18 0 L 86 0 L 104 18 L 104 78 L 86 104 L 52 114 L 18 104 L 0 78 L 0 18 Z";
  
  const v1ChassisOuter = figma.createVector();
  v1ChassisOuter.name = "Portrait_Chassis_Outer";
  v1ChassisOuter.vectorPaths = [{ windingRule: "NONZERO", data: v1ChassisPath }];
  v1ChassisOuter.fills = [{ type: "SOLID", color: { r: 0.14, g: 0.17, b: 0.22 } }];
  v1ChassisOuter.strokes = [{ type: "SOLID", color: { r: 0.88, g: 0.72, b: 0.32 } }]; // Polished Gold Rim
  v1ChassisOuter.strokeWeight = 2.5;
  v1ChassisOuter.x = 18;
  v1ChassisOuter.y = 20;
  v1Frame.appendChild(v1ChassisOuter);

  // Inner portrait well (dark crimson velvet)
  const v1ChassisInner = figma.createVector();
  v1ChassisInner.name = "Portrait_Chassis_Inner";
  v1ChassisInner.vectorPaths = [{ windingRule: "NONZERO", data: "M 15 0 L 71 0 L 86 15 L 86 65 L 71 86 L 43 94 L 15 86 L 0 65 L 0 15 Z" }];
  v1ChassisInner.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.05, b: 0.05 } }];
  v1ChassisInner.strokes = [{ type: "SOLID", color: { r: 0.45, g: 0.15, b: 0.15 } }];
  v1ChassisInner.strokeWeight = 1.5;
  v1ChassisInner.x = 27;
  v1ChassisInner.y = 29;
  v1Frame.appendChild(v1ChassisInner);

  // Dragon Horn Top Crest
  const v1TopCrest = figma.createVector();
  v1TopCrest.name = "Top_Dragon_Horns";
  v1TopCrest.vectorPaths = [{ windingRule: "NONZERO", data: "M 0 12 Q 26 -6 52 12 Q 78 -6 104 12 L 94 18 Q 52 6 10 18 Z" }];
  v1TopCrest.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.80, b: 0.38 } }];
  v1TopCrest.x = 18;
  v1TopCrest.y = 10;
  v1Frame.appendChild(v1TopCrest);

  // Dragon Emblem Icon
  const v1Emblem = figma.createText();
  v1Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v1Emblem.characters = "🐉";
  v1Emblem.fontSize = 42;
  v1Emblem.x = 44;
  v1Emblem.y = 48;
  v1Frame.appendChild(v1Emblem);

  // Bottom Level Shield Badge
  const v1LvlBadge = figma.createFrame();
  v1LvlBadge.resize(36, 22);
  v1LvlBadge.x = 52;
  v1LvlBadge.y = 128;
  v1LvlBadge.fills = [{ type: "SOLID", color: { r: 0.10, g: 0.12, b: 0.16 } }];
  v1LvlBadge.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.80, b: 0.35 } }];
  v1LvlBadge.strokeWeight = 2;
  v1LvlBadge.cornerRadius = 4;
  v1Frame.appendChild(v1LvlBadge);

  const v1LvlTxt = figma.createText();
  v1LvlTxt.fontName = { family: "Inter", style: "Bold" };
  v1LvlTxt.characters = "1";
  v1LvlTxt.fontSize = 11;
  v1LvlTxt.fills = [{ type: "SOLID", color: { r: 1.0, g: 0.90, b: 0.50 } }];
  v1LvlTxt.x = 14;
  v1LvlTxt.y = 4;
  v1LvlBadge.appendChild(v1LvlTxt);

  // Header Title & Stats
  const v1Name = figma.createText();
  v1Name.fontName = { family: "Cinzel", style: "Bold" };
  v1Name.characters = "INFERNO WYRM";
  v1Name.fontSize = 18;
  v1Name.fills = [{ type: "SOLID", color: { r: 0.98, g: 0.94, b: 0.86 } }];
  v1Name.x = 142;
  v1Name.y = 20;
  v1Frame.appendChild(v1Name);

  const v1Sub = figma.createText();
  v1Sub.fontName = { family: "Inter", style: "Bold" };
  v1Sub.characters = "ANCIENT PYROCLAST • PLAYER";
  v1Sub.fontSize = 9;
  v1Sub.fills = [{ type: "SOLID", color: { r: 0.88, g: 0.50, b: 0.22 } }];
  v1Sub.x = 144;
  v1Sub.y = 44;
  v1Frame.appendChild(v1Sub);

  const v1HpNum = figma.createText();
  v1HpNum.fontName = { family: "Inter", style: "Bold" };
  v1HpNum.characters = "150 / 200 HP";
  v1HpNum.fontSize = 12;
  v1HpNum.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.97, b: 1.0 } }];
  v1HpNum.x = 405;
  v1HpNum.y = 44;
  v1Frame.appendChild(v1HpNum);

  // Health Bar: 8 Segmented Armor Cell Blocks
  const v1HpCradle = figma.createFrame();
  v1HpCradle.resize(344, 28);
  v1HpCradle.x = 142;
  v1HpCradle.y = 64;
  v1HpCradle.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.05, b: 0.07 } }];
  v1HpCradle.strokes = [{ type: "SOLID", color: { r: 0.32, g: 0.38, b: 0.48 } }];
  v1HpCradle.strokeWeight = 1.5;
  v1HpCradle.cornerRadius = 4;
  v1Frame.appendChild(v1HpCradle);

  const v1CellW = 38.5;
  for (let b = 0; b < 8; b++) {
    const cell = figma.createFrame();
    cell.resize(v1CellW, 20);
    cell.x = 4 + b * (v1CellW + 4);
    cell.y = 4;
    cell.cornerRadius = 2;
    if (b < 6) {
      cell.fills = [{ type: "SOLID", color: { r: 0.16, g: 0.74, b: 0.40 } }]; // Emerald
      cell.strokes = [{ type: "SOLID", color: { r: 0.28, g: 0.88, b: 0.50 } }];
      cell.strokeWeight = 1;
    } else if (b === 6) {
      cell.fills = [{ type: "SOLID", color: { r: 0.88, g: 0.58, b: 0.15 } }]; // Amber decay ghost
      cell.strokes = [{ type: "SOLID", color: { r: 0.98, g: 0.72, b: 0.25 } }];
      cell.strokeWeight = 1;
    } else {
      cell.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }]; // Empty grate
      cell.strokes = [{ type: "SOLID", color: { r: 0.14, g: 0.17, b: 0.22 } }];
      cell.strokeWeight = 1;
    }
    v1HpCradle.appendChild(cell);
  }

  // 1B. Sculpted Ability State Sockets (Notched Octagonal Talismans)
  const v1Abilities = [
    { key: "Q", name: "FIRE BREATH", state: "READY", color: { r: 0.25, g: 0.88, b: 0.48 }, ready: true, icon: "🔥" },
    { key: "W", name: "TAIL WHIP", state: "READY", color: { r: 0.25, g: 0.88, b: 0.48 }, ready: true, icon: "💫" },
    { key: "E", name: "SKY DIVE", state: "4.2s", color: { r: 0.96, g: 0.65, b: 0.22 }, ready: false, icon: "⚡" }
  ];

  // Octagonal Talisman SVG path: 110 x 42
  const talismanPath = "M 8 0 L 102 0 L 110 8 L 110 34 L 102 42 L 8 42 L 0 34 L 0 8 Z";

  for (let i = 0; i < 3; i++) {
    const ab = v1Abilities[i];
    const sockX = 142 + i * 117;
    const sockY = 104;

    // Outer Talisman Frame
    const sock = figma.createVector();
    sock.name = "Talisman_" + ab.key;
    sock.vectorPaths = [{ windingRule: "NONZERO", data: talismanPath }];
    sock.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.14 } }];
    sock.strokes = [{ type: "SOLID", color: ab.color }];
    sock.strokeWeight = 1.8;
    sock.x = sockX;
    sock.y = sockY;
    v1Frame.appendChild(sock);

    // Inner Diamond Hotkey Jewel
    const jewel = figma.createVector();
    jewel.vectorPaths = [{ windingRule: "NONZERO", data: "M 10 0 L 20 10 L 10 20 L 0 10 Z" }];
    jewel.fills = [{ type: "SOLID", color: { r: 0.14, g: 0.18, b: 0.24 } }];
    jewel.strokes = [{ type: "SOLID", color: ab.color }];
    jewel.strokeWeight = 1.2;
    jewel.x = sockX + 8;
    jewel.y = sockY + 11;
    v1Frame.appendChild(jewel);

    const kTxt = figma.createText();
    kTxt.fontName = { family: "Inter", style: "Bold" };
    kTxt.characters = ab.key;
    kTxt.fontSize = 11;
    kTxt.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
    kTxt.x = sockX + 14;
    kTxt.y = sockY + 14;
    v1Frame.appendChild(kTxt);

    const nTxt = figma.createText();
    nTxt.fontName = { family: "Inter", style: "Bold" };
    nTxt.characters = ab.name;
    nTxt.fontSize = 8;
    nTxt.fills = [{ type: "SOLID", color: { r: 0.75, g: 0.82, b: 0.90 } }];
    nTxt.x = sockX + 34;
    nTxt.y = sockY + 9;
    v1Frame.appendChild(nTxt);

    const sTxt = figma.createText();
    sTxt.fontName = { family: "Inter", style: "Bold" };
    sTxt.characters = ab.state;
    sTxt.fontSize = 12;
    sTxt.fills = [{ type: "SOLID", color: ab.color }];
    sTxt.x = sockX + 34;
    sTxt.y = sockY + 21;
    v1Frame.appendChild(sTxt);
  }

  // Mirrored AI Frame in V1
  const v1AiLabel = figma.createText();
  v1AiLabel.fontName = { family: "Inter", style: "Bold" };
  v1AiLabel.characters = "MIRRORED ENEMY AI FRAME (FROST WYRM):";
  v1AiLabel.fontSize = 11;
  v1AiLabel.fills = [{ type: "SOLID", color: { r: 0.45, g: 0.75, b: 0.98 } }];
  v1AiLabel.x = 24;
  v1AiLabel.y = 330;
  v1Col.appendChild(v1AiLabel);

  const v1AiFrame = figma.createFrame();
  v1AiFrame.name = "DragonStatusFrame_Ironclad_AI";
  v1AiFrame.resize(502, 190);
  v1AiFrame.x = 24;
  v1AiFrame.y = 356;
  v1AiFrame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
  v1AiFrame.strokes = [{ type: "SOLID", color: { r: 0.28, g: 0.60, b: 0.90 } }]; // Frost Runic
  v1AiFrame.strokeWeight = 2;
  v1AiFrame.cornerRadius = 6;
  v1Col.appendChild(v1AiFrame);
  addFiligreeCorners(v1AiFrame, 502, 190, { r: 0.45, g: 0.75, b: 0.98 });

  // AI Portrait on right
  const v1AiChassisOuter = figma.createVector();
  v1AiChassisOuter.vectorPaths = [{ windingRule: "NONZERO", data: v1ChassisPath }];
  v1AiChassisOuter.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.12, b: 0.22 } }];
  v1AiChassisOuter.strokes = [{ type: "SOLID", color: { r: 0.45, g: 0.75, b: 0.98 } }];
  v1AiChassisOuter.strokeWeight = 2.5;
  v1AiChassisOuter.x = 380;
  v1AiChassisOuter.y = 20;
  v1AiFrame.appendChild(v1AiChassisOuter);

  const v1AiEmblem = figma.createText();
  v1AiEmblem.fontName = { family: "Cinzel", style: "Bold" };
  v1AiEmblem.characters = "❄️";
  v1AiEmblem.fontSize = 42;
  v1AiEmblem.x = 408;
  v1AiEmblem.y = 50;
  v1AiFrame.appendChild(v1AiEmblem);

  const v1AiName = figma.createText();
  v1AiName.fontName = { family: "Cinzel", style: "Bold" };
  v1AiName.characters = "FROST WYRM";
  v1AiName.fontSize = 18;
  v1AiName.fills = [{ type: "SOLID", color: { r: 0.88, g: 0.95, b: 1.0 } }];
  v1AiName.x = 18;
  v1AiName.y = 20;
  v1AiFrame.appendChild(v1AiName);

  const v1AiSub = figma.createText();
  v1AiSub.fontName = { family: "Inter", style: "Bold" };
  v1AiSub.characters = "GLACIAL REAVER • ENEMY AI";
  v1AiSub.fontSize = 9;
  v1AiSub.fills = [{ type: "SOLID", color: { r: 0.45, g: 0.75, b: 0.95 } }];
  v1AiSub.x = 20;
  v1AiSub.y = 44;
  v1AiFrame.appendChild(v1AiSub);

  const v1AiHpNum = figma.createText();
  v1AiHpNum.fontName = { family: "Inter", style: "Bold" };
  v1AiHpNum.characters = "175 / 200 HP";
  v1AiHpNum.fontSize = 12;
  v1AiHpNum.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.97, b: 1.0 } }];
  v1AiHpNum.x = 280;
  v1AiHpNum.y = 44;
  v1AiFrame.appendChild(v1AiHpNum);

  // AI 8-Block Bar (Crimson)
  const v1AiHpCradle = figma.createFrame();
  v1AiHpCradle.resize(344, 28);
  v1AiHpCradle.x = 18;
  v1AiHpCradle.y = 64;
  v1AiHpCradle.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.05, b: 0.07 } }];
  v1AiHpCradle.strokes = [{ type: "SOLID", color: { r: 0.25, g: 0.42, b: 0.60 } }];
  v1AiHpCradle.strokeWeight = 1.5;
  v1AiHpCradle.cornerRadius = 4;
  v1AiFrame.appendChild(v1AiHpCradle);

  for (let b = 0; b < 8; b++) {
    const cell = figma.createFrame();
    cell.resize(v1CellW, 20);
    cell.x = 4 + b * (v1CellW + 4);
    cell.y = 4;
    cell.cornerRadius = 2;
    if (b < 7) {
      cell.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.24, b: 0.24 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.45, b: 0.45 } }];
      cell.strokeWeight = 1;
    } else {
      cell.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.14, g: 0.17, b: 0.22 } }];
      cell.strokeWeight = 1;
    }
    v1AiHpCradle.appendChild(cell);
  }

  // AI Ability Talismans
  const v1AiAbilities = [
    { key: "Q", name: "FROST BREATH", state: "READY", color: { r: 0.4, g: 0.8, b: 1.0 }, ready: true },
    { key: "W", name: "TAIL SWEEP", state: "1.8s", color: { r: 0.95, g: 0.65, b: 0.22 }, ready: false },
    { key: "E", name: "BLIZZARD SLAM", state: "READY", color: { r: 0.4, g: 0.8, b: 1.0 }, ready: true }
  ];

  for (let i = 0; i < 3; i++) {
    const ab = v1AiAbilities[i];
    const sockX = 18 + i * 117;
    const sockY = 104;

    const sock = figma.createVector();
    sock.vectorPaths = [{ windingRule: "NONZERO", data: talismanPath }];
    sock.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.14 } }];
    sock.strokes = [{ type: "SOLID", color: ab.color }];
    sock.strokeWeight = 1.8;
    sock.x = sockX;
    sock.y = sockY;
    v1AiFrame.appendChild(sock);

    const nTxt = figma.createText();
    nTxt.fontName = { family: "Inter", style: "Bold" };
    nTxt.characters = ab.name;
    nTxt.fontSize = 8;
    nTxt.fills = [{ type: "SOLID", color: { r: 0.75, g: 0.85, b: 0.95 } }];
    nTxt.x = sockX + 14;
    nTxt.y = sockY + 9;
    v1AiFrame.appendChild(nTxt);

    const sTxt = figma.createText();
    sTxt.fontName = { family: "Inter", style: "Bold" };
    sTxt.characters = ab.state;
    sTxt.fontSize = 12;
    sTxt.fills = [{ type: "SOLID", color: ab.color }];
    sTxt.x = sockX + 14;
    sTxt.y = sockY + 21;
    v1AiFrame.appendChild(sTxt);
  }

  // =========================================================================
  // VARIANT 2: "The Ancient Wyrmguard" (Chiseled Dragon Scale & Interlocking Chevron Tabs)
  // =========================================================================
  const v2Col = figma.createFrame();
  v2Col.name = "Variant 2: Ancient Wyrmguard";
  v2Col.resize(550, 1020);
  v2Col.x = 660;
  v2Col.y = 150;
  v2Col.fills = [{ type: "SOLID", color: { r: 0.065, g: 0.08, b: 0.105 } }];
  v2Col.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v2Col.strokeWeight = 1;
  v2Col.cornerRadius = 12;
  board.appendChild(v2Col);

  const v2Title = figma.createText();
  v2Title.fontName = { family: "Cinzel", style: "Bold" };
  v2Title.characters = "2. ANCIENT WYRMGUARD";
  v2Title.fontSize = 18;
  v2Title.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.55, b: 0.25 } }]; // Amber / Flame
  v2Title.x = 24;
  v2Title.y = 24;
  v2Col.appendChild(v2Title);

  const v2Desc = figma.createText();
  v2Desc.fontName = { family: "Inter", style: "Regular" };
  v2Desc.characters = "Faceted dragon scale hex-chassis with flanking wing-blade spurs. 10 chiseled scale armor cells with a central Major Spire divider. Interlocking chevron ability tabs.";
  v2Desc.fontSize = 12;
  v2Desc.fills = [{ type: "SOLID", color: { r: 0.62, g: 0.68, b: 0.78 } }];
  v2Desc.resize(502, 38);
  v2Desc.textAutoResize = "NONE";
  v2Desc.x = 24;
  v2Desc.y = 54;
  v2Col.appendChild(v2Desc);

  const v2Frame = figma.createFrame();
  v2Frame.name = "DragonStatusFrame_Wyrmguard";
  v2Frame.resize(502, 190);
  v2Frame.x = 24;
  v2Frame.y = 106;
  v2Frame.fills = [{ type: "SOLID", color: { r: 0.07, g: 0.08, b: 0.11 } }];
  v2Frame.strokes = [{ type: "SOLID", color: { r: 0.75, g: 0.40, b: 0.20 } }]; // Molten bronze
  v2Frame.strokeWeight = 2;
  v2Frame.cornerRadius = 8;
  v2Col.appendChild(v2Frame);
  addFiligreeCorners(v2Frame, 502, 190, { r: 0.90, g: 0.50, b: 0.25 });

  // 2A. Faceted Dragon Scale Hex-Chassis (Vector Path)
  // 108 x 112 with aggressive flared wing spurs
  const hexScalePath = "M 54 0 L 102 18 L 108 56 L 96 100 L 54 116 L 12 100 L 0 56 L 6 18 Z";

  const v2ChassisOuter = figma.createVector();
  v2ChassisOuter.vectorPaths = [{ windingRule: "NONZERO", data: hexScalePath }];
  v2ChassisOuter.fills = [{ type: "SOLID", color: { r: 0.12, g: 0.06, b: 0.05 } }];
  v2ChassisOuter.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.55, b: 0.20 } }];
  v2ChassisOuter.strokeWeight = 2.5;
  v2ChassisOuter.x = 16;
  v2ChassisOuter.y = 20;
  v2Frame.appendChild(v2ChassisOuter);

  // Outer Dragon Wing Spurs flanking the portrait
  const v2WingSpurs = figma.createVector();
  v2WingSpurs.vectorPaths = [{ windingRule: "NONZERO", data: "M 0 30 L 16 0 L 10 40 L 22 25 L 14 55 Z" }];
  v2WingSpurs.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.45, b: 0.18 } }];
  v2WingSpurs.x = 6;
  v2WingSpurs.y = 35;
  v2Frame.appendChild(v2WingSpurs);

  const v2Emblem = figma.createText();
  v2Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v2Emblem.characters = "🔥";
  v2Emblem.fontSize = 44;
  v2Emblem.x = 48;
  v2Emblem.y = 52;
  v2Frame.appendChild(v2Emblem);

  const v2Name = figma.createText();
  v2Name.fontName = { family: "Cinzel", style: "Bold" };
  v2Name.characters = "PYROCLAST • THE DREAD";
  v2Name.fontSize = 18;
  v2Name.fills = [{ type: "SOLID", color: { r: 1.0, g: 0.92, b: 0.80 } }];
  v2Name.x = 142;
  v2Name.y = 20;
  v2Frame.appendChild(v2Name);

  const v2Sub = figma.createText();
  v2Sub.fontName = { family: "Inter", style: "Bold" };
  v2Sub.characters = "ELITE ANCIENT • FIRE EMBEDDED";
  v2Sub.fontSize = 9;
  v2Sub.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.50, b: 0.20 } }];
  v2Sub.x = 144;
  v2Sub.y = 44;
  v2Frame.appendChild(v2Sub);

  const v2HpNum = figma.createText();
  v2HpNum.fontName = { family: "Inter", style: "Bold" };
  v2HpNum.characters = "160 / 200 HP";
  v2HpNum.fontSize = 12;
  v2HpNum.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.97, b: 1.0 } }];
  v2HpNum.x = 405;
  v2HpNum.y = 44;
  v2Frame.appendChild(v2HpNum);

  // Health Bar: 10 Chiseled Scale Armor Cells (20 HP each) with Spire Divider
  const v2HpCradle = figma.createFrame();
  v2HpCradle.resize(344, 28);
  v2HpCradle.x = 142;
  v2HpCradle.y = 64;
  v2HpCradle.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.05, b: 0.07 } }];
  v2HpCradle.strokes = [{ type: "SOLID", color: { r: 0.45, g: 0.25, b: 0.18 } }];
  v2HpCradle.strokeWeight = 1.5;
  v2HpCradle.cornerRadius = 4;
  v2Frame.appendChild(v2HpCradle);

  const v2CellW = 29.5;
  for (let c = 0; c < 10; c++) {
    const cell = figma.createFrame();
    cell.resize(v2CellW, 20);
    // Extra gap at center (after cell 4) for Major Spire Divider
    const centerOffset = c >= 5 ? 6 : 0;
    cell.x = 3.5 + c * (v2CellW + 3.5) + centerOffset;
    cell.y = 4;
    cell.cornerRadius = 2;

    if (c < 8) {
      cell.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.78, b: 0.42 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.32, g: 0.90, b: 0.52 } }];
      cell.strokeWeight = 1;
    } else if (c === 8) {
      cell.fills = [{ type: "SOLID", color: { r: 0.88, g: 0.55, b: 0.15 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.98, g: 0.70, b: 0.25 } }];
      cell.strokeWeight = 1;
    } else {
      cell.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.14 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.15, g: 0.18, b: 0.24 } }];
      cell.strokeWeight = 1;
    }
    v2HpCradle.appendChild(cell);
  }

  // Major Center Spire Divider in V2
  const spireDivider = figma.createRectangle();
  spireDivider.resize(3, 24);
  spireDivider.x = 3.5 + 5 * (v2CellW + 3.5) - 3.5;
  spireDivider.y = 2;
  spireDivider.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.75, b: 0.35 } }]; // Golden spire
  v2HpCradle.appendChild(spireDivider);

  // 2B. Sculpted Chevron Ability Cartridges
  // Chevron slanted path: 110 x 42 with 10-degree angled edge
  const chevronPath = "M 10 0 L 110 0 L 100 42 L 0 42 Z";

  const v2Abilities = [
    { key: "Q", name: "FIRE BREATH", state: "READY", color: { r: 0.25, g: 0.88, b: 0.48 }, prog: 1.0 },
    { key: "W", name: "TAIL WHIP", state: "READY", color: { r: 0.25, g: 0.88, b: 0.48 }, prog: 1.0 },
    { key: "E", name: "SKY DIVE", state: "CD: 5.4s", color: { r: 0.95, g: 0.65, b: 0.22 }, prog: 0.38 }
  ];

  for (let i = 0; i < 3; i++) {
    const ab = v2Abilities[i];
    const sockX = 142 + i * 117;
    const sockY = 104;

    const sock = figma.createVector();
    sock.name = "Chevron_" + ab.key;
    sock.vectorPaths = [{ windingRule: "NONZERO", data: chevronPath }];
    sock.fills = [{ type: "SOLID", color: { r: 0.09, g: 0.11, b: 0.15 } }];
    sock.strokes = [{ type: "SOLID", color: ab.color }];
    sock.strokeWeight = 1.8;
    sock.x = sockX;
    sock.y = sockY;
    v2Frame.appendChild(sock);

    // Laser recharge progress line along bottom slanted edge
    const rLine = figma.createRectangle();
    rLine.resize(95 * ab.prog, 3);
    rLine.x = sockX + 6;
    rLine.y = sockY + 37;
    rLine.fills = [{ type: "SOLID", color: ab.color }];
    v2Frame.appendChild(rLine);

    const kTxt = figma.createText();
    kTxt.fontName = { family: "Inter", style: "Bold" };
    kTxt.characters = `[${ab.key}]`;
    kTxt.fontSize = 11;
    kTxt.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
    kTxt.x = sockX + 16;
    kTxt.y = sockY + 8;
    v2Frame.appendChild(kTxt);

    const nTxt = figma.createText();
    nTxt.fontName = { family: "Inter", style: "Bold" };
    nTxt.characters = ab.name;
    nTxt.fontSize = 8;
    nTxt.fills = [{ type: "SOLID", color: { r: 0.78, g: 0.84, b: 0.92 } }];
    nTxt.x = sockX + 40;
    nTxt.y = sockY + 8;
    v2Frame.appendChild(nTxt);

    const sTxt = figma.createText();
    sTxt.fontName = { family: "Inter", style: "Bold" };
    sTxt.characters = ab.state;
    sTxt.fontSize = 12;
    sTxt.fills = [{ type: "SOLID", color: ab.color }];
    sTxt.x = sockX + 24;
    sTxt.y = sockY + 22;
    v2Frame.appendChild(sTxt);
  }

  // =========================================================================
  // VARIANT 3: "The Aegis Imperator" (Imperial Crown Cartouche & Shield Seals)
  // =========================================================================
  const v3Col = figma.createFrame();
  v3Col.name = "Variant 3: Aegis Imperator";
  v3Col.resize(550, 1020);
  v3Col.x = 1260;
  v3Col.y = 150;
  v3Col.fills = [{ type: "SOLID", color: { r: 0.065, g: 0.08, b: 0.105 } }];
  v3Col.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v3Col.strokeWeight = 1;
  v3Col.cornerRadius = 12;
  board.appendChild(v3Col);

  const v3Title = figma.createText();
  v3Title.fontName = { family: "Cinzel", style: "Bold" };
  v3Title.characters = "3. AEGIS IMPERATOR";
  v3Title.fontSize = 18;
  v3Title.fills = [{ type: "SOLID", color: { r: 0.98, g: 0.88, b: 0.40 } }]; // Tournament Gold
  v3Title.x = 24;
  v3Title.y = 24;
  v3Col.appendChild(v3Title);

  const v3Desc = figma.createText();
  v3Desc.fontName = { family: "Inter", style: "Regular" };
  v3Desc.characters = "TI Championship tournament cartouche with triple-pointed dragon crown and 3-Star heraldry. 8 dual-pip jewel health ingots. Sculpted heraldic shield seals for abilities.";
  v3Desc.fontSize = 12;
  v3Desc.fills = [{ type: "SOLID", color: { r: 0.62, g: 0.68, b: 0.78 } }];
  v3Desc.resize(502, 38);
  v3Desc.textAutoResize = "NONE";
  v3Desc.x = 24;
  v3Desc.y = 54;
  v3Col.appendChild(v3Desc);

  const v3Frame = figma.createFrame();
  v3Frame.name = "DragonStatusFrame_Imperator";
  v3Frame.resize(502, 190);
  v3Frame.x = 24;
  v3Frame.y = 106;
  v3Frame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.14 } }];
  v3Frame.strokes = [{ type: "SOLID", color: { r: 0.85, g: 0.72, b: 0.32 } }]; // Gold filigree
  v3Frame.strokeWeight = 2;
  v3Frame.cornerRadius = 8;
  v3Col.appendChild(v3Frame);
  addFiligreeCorners(v3Frame, 502, 190, { r: 0.98, g: 0.88, b: 0.42 });

  // 3A. Sculpted Imperial Crown Cartouche (Vector Path)
  // Oval Cartouche with Crown: 104 x 116
  const cartouchePath = "M 52 0 L 64 12 L 78 4 L 84 18 L 102 24 C 106 50 102 86 86 104 L 52 114 L 18 104 C 2 86 -2 50 2 24 L 20 18 L 26 4 L 40 12 Z";

  const v3ChassisOuter = figma.createVector();
  v3ChassisOuter.vectorPaths = [{ windingRule: "NONZERO", data: cartouchePath }];
  v3ChassisOuter.fills = [{ type: "SOLID", color: { r: 0.14, g: 0.08, b: 0.05 } }];
  v3ChassisOuter.strokes = [{ type: "SOLID", color: { r: 0.98, g: 0.85, b: 0.38 } }];
  v3ChassisOuter.strokeWeight = 2.5;
  v3ChassisOuter.x = 18;
  v3ChassisOuter.y = 16;
  v3Frame.appendChild(v3ChassisOuter);

  const v3Emblem = figma.createText();
  v3Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v3Emblem.characters = "👑";
  v3Emblem.fontSize = 42;
  v3Emblem.x = 48;
  v3Emblem.y = 48;
  v3Frame.appendChild(v3Emblem);

  // 3-Star Rating Ribbon
  const v3Stars = figma.createText();
  v3Stars.fontName = { family: "Inter", style: "Bold" };
  v3Stars.characters = "★★★";
  v3Stars.fontSize = 12;
  v3Stars.fills = [{ type: "SOLID", color: { r: 0.98, g: 0.88, b: 0.40 } }];
  v3Stars.x = 52;
  v3Stars.y = 126;
  v3Frame.appendChild(v3Stars);

  const v3Name = figma.createText();
  v3Name.fontName = { family: "Cinzel", style: "Bold" };
  v3Name.characters = "AEGIS WYRM OF RADIANCE";
  v3Name.fontSize = 17;
  v3Name.fills = [{ type: "SOLID", color: { r: 0.98, g: 0.94, b: 0.85 } }];
  v3Name.x = 142;
  v3Name.y = 20;
  v3Frame.appendChild(v3Name);

  const v3Sub = figma.createText();
  v3Sub.fontName = { family: "Inter", style: "Bold" };
  v3Sub.characters = "TI GRAND FINALS • TIER 3 CHAMPION";
  v3Sub.fontSize = 9;
  v3Sub.fills = [{ type: "SOLID", color: { r: 0.92, g: 0.78, b: 0.35 } }];
  v3Sub.x = 144;
  v3Sub.y = 44;
  v3Frame.appendChild(v3Sub);

  const v3HpNum = figma.createText();
  v3HpNum.fontName = { family: "Inter", style: "Bold" };
  v3HpNum.characters = "160 / 200 HP";
  v3HpNum.fontSize = 12;
  v3HpNum.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
  v3HpNum.x = 405;
  v3HpNum.y = 44;
  v3Frame.appendChild(v3HpNum);

  // Health Bar: 8 Jewel Ingot Blocks (Each 25 HP) with Golden Dividers
  const v3HpCradle = figma.createFrame();
  v3HpCradle.resize(344, 28);
  v3HpCradle.x = 142;
  v3HpCradle.y = 64;
  v3HpCradle.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.05, b: 0.07 } }];
  v3HpCradle.strokes = [{ type: "SOLID", color: { r: 0.65, g: 0.52, b: 0.25 } }]; // Gold cradle
  v3HpCradle.strokeWeight = 1.5;
  v3HpCradle.cornerRadius = 4;
  v3Frame.appendChild(v3HpCradle);

  for (let b = 0; b < 8; b++) {
    const cell = figma.createFrame();
    cell.resize(v1CellW, 20);
    cell.x = 4 + b * (v1CellW + 4);
    cell.y = 4;
    cell.cornerRadius = 3;
    if (b < 6) {
      cell.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.76, b: 0.44 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.85, g: 0.95, b: 0.65 } }];
      cell.strokeWeight = 1;
    } else if (b === 6) {
      cell.fills = [{ type: "SOLID", color: { r: 0.92, g: 0.62, b: 0.18 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 1.0, g: 0.80, b: 0.35 } }];
      cell.strokeWeight = 1;
    } else {
      cell.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.14 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
      cell.strokeWeight = 1;
    }
    v3HpCradle.appendChild(cell);
  }

  // 3B. Sculpted Heraldic Shield Seals for Abilities
  // Shield Seal SVG path: 110 x 42
  const shieldSealPath = "M 12 0 L 98 0 C 106 0 110 6 110 14 L 110 24 C 110 36 96 42 55 42 C 14 42 0 36 0 24 L 0 14 C 0 6 4 0 12 0 Z";

  const v3Abilities = [
    { key: "Q", name: "BREATH", state: "READY", color: { r: 0.25, g: 0.88, b: 0.48 }, ready: true },
    { key: "W", name: "WHIP", state: "READY", color: { r: 0.25, g: 0.88, b: 0.48 }, ready: true },
    { key: "E", name: "SLAM", state: "3.5s", color: { r: 0.95, g: 0.65, b: 0.22 }, ready: false }
  ];

  for (let i = 0; i < 3; i++) {
    const ab = v3Abilities[i];
    const sockX = 142 + i * 117;
    const sockY = 104;

    const sock = figma.createVector();
    sock.name = "ShieldSeal_" + ab.key;
    sock.vectorPaths = [{ windingRule: "NONZERO", data: shieldSealPath }];
    sock.fills = [{ type: "SOLID", color: { r: 0.09, g: 0.11, b: 0.15 } }];
    sock.strokes = [{ type: "SOLID", color: ab.color }];
    sock.strokeWeight = 1.8;
    sock.x = sockX;
    sock.y = sockY;
    v3Frame.appendChild(sock);

    // Circular Hotkey Seal
    const seal = figma.createEllipse();
    seal.resize(22, 22);
    seal.x = sockX + 8;
    seal.y = sockY + 10;
    seal.fills = [{ type: "SOLID", color: { r: 0.14, g: 0.18, b: 0.24 } }];
    seal.strokes = [{ type: "SOLID", color: ab.color }];
    seal.strokeWeight = 1.2;
    v3Frame.appendChild(seal);

    const kTxt = figma.createText();
    kTxt.fontName = { family: "Inter", style: "Bold" };
    kTxt.characters = ab.key;
    kTxt.fontSize = 11;
    kTxt.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
    kTxt.x = sockX + 14;
    kTxt.y = sockY + 13;
    v3Frame.appendChild(kTxt);

    const nTxt = figma.createText();
    nTxt.fontName = { family: "Inter", style: "Bold" };
    nTxt.characters = ab.name;
    nTxt.fontSize = 8;
    nTxt.fills = [{ type: "SOLID", color: { r: 0.75, g: 0.85, b: 0.95 } }];
    nTxt.x = sockX + 36;
    nTxt.y = sockY + 9;
    v3Frame.appendChild(nTxt);

    const sTxt = figma.createText();
    sTxt.fontName = { family: "Inter", style: "Bold" };
    sTxt.characters = ab.state;
    sTxt.fontSize = 12;
    sTxt.fills = [{ type: "SOLID", color: ab.color }];
    sTxt.x = sockX + 36;
    sTxt.y = sockY + 21;
    v3Frame.appendChild(sTxt);
  }

  // Zoom into new board
  figma.currentPage.selection = [board];
  figma.viewport.scrollAndZoomIntoView([board]);

  return {
    success: true,
    boardId: board.id,
    boardName: board.name
  };
}

return await createHighEffortDota2Variants();
"""

res = exec_figma(script)
print(res)

if res.get("success") and "result" in res:
    board_id = res["result"].get("boardId")
    export_code = f"""
    async function exportSculptedBoard() {{
      const node = await figma.getNodeByIdAsync("{board_id}");
      if (!node) return {{ error: "Node not found" }};
      const bytes = await node.exportAsync({{
        format: "PNG",
        constraint: {{ type: "SCALE", value: 1 }}
      }});
      let binary = "";
      for (let i = 0; i < bytes.byteLength; i++) {{
        binary += String.fromCharCode(bytes[i]);
      }}
      return {{ base64: btoa(binary) }};
    }}
    return await exportSculptedBoard();
    """
    exp_res = exec_figma(export_code)
    if "result" in exp_res and "base64" in exp_res["result"]:
        img_bytes = base64.b64decode(exp_res["result"]["base64"])
        out_path = "/home/subodh/.gemini/antigravity-cli/brain/c5488275-621a-4183-8c8b-c55127a653d3/dota2_sculpted_variants.png"
        with open(out_path, "wb") as f:
            f.write(img_bytes)
        print(f"Exported sculpted preview to {out_path} ({len(img_bytes)} bytes)")
