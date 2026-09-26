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

fusion_script = """
async function createFusionVariants() {
  await figma.loadFontAsync({ family: "Cinzel", style: "Regular" });
  await figma.loadFontAsync({ family: "Cinzel", style: "Bold" });
  await figma.loadFontAsync({ family: "Inter", style: "Regular" });
  await figma.loadFontAsync({ family: "Inter", style: "Bold" });

  const page = figma.currentPage;

  // Master Board
  const board = figma.createFrame();
  board.name = "Dota 2 Dragon Status Frame - 3 Refined Fusion Concepts";
  board.resize(1840, 1260);
  board.x = 1860;
  board.y = 0;
  board.fills = [{ type: "SOLID", color: { r: 0.047, g: 0.059, b: 0.078 } }]; // #0C0F14 deep Dota background
  board.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  board.strokeWeight = 2;
  board.cornerRadius = 16;
  board.clipsContent = true;

  // Header Title
  const title = figma.createText();
  title.fontName = { family: "Cinzel", style: "Bold" };
  title.characters = "DRAGON STATUS FRAME : FUSION ITERATIONS";
  title.fontSize = 32;
  title.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.84, b: 0.52 } }]; // Gold
  title.x = 60;
  title.y = 50;
  board.appendChild(title);

  const subtitle = figma.createText();
  subtitle.fontName = { family: "Inter", style: "Regular" };
  subtitle.characters = "Combining Variant-A's Forged Iron / Block-Spaced HP aesthetic with Variant-B's Tactical Ability Sockets";
  subtitle.fontSize = 15;
  subtitle.fills = [{ type: "SOLID", color: { r: 0.55, g: 0.62, b: 0.72 } }];
  subtitle.x = 60;
  subtitle.y = 96;
  board.appendChild(subtitle);

  // Helper for corner rivets
  function addRivets(parent, w, h, color = { r: 0.75, g: 0.65, b: 0.35 }) {
    [[6, 6], [w - 12, 6], [6, h - 12], [w - 12, h - 12]].forEach(([rx, ry]) => {
      const r = figma.createEllipse();
      r.resize(6, 6);
      r.x = rx;
      r.y = ry;
      r.fills = [{ type: "SOLID", color }];
      parent.appendChild(r);
    });
  }

  // =========================================================================
  // FUSION VARIANT 1: "The Ironclad Citadel" (Tactile Armor Cell Blocks & Runic Sockets)
  // =========================================================================
  const v1Group = figma.createFrame();
  v1Group.name = "Fusion 1: Ironclad Citadel (Tactile Armor Cells & Runic Sockets)";
  v1Group.resize(540, 1020);
  v1Group.x = 60;
  v1Group.y = 150;
  v1Group.fills = [{ type: "SOLID", color: { r: 0.07, g: 0.09, b: 0.12 } }];
  v1Group.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v1Group.strokeWeight = 1;
  v1Group.cornerRadius = 12;
  board.appendChild(v1Group);

  const v1Title = figma.createText();
  v1Title.fontName = { family: "Cinzel", style: "Bold" };
  v1Title.characters = "FUSION 1 : IRONCLAD CITADEL";
  v1Title.fontSize = 18;
  v1Title.fills = [{ type: "SOLID", color: { r: 0.92, g: 0.80, b: 0.44 } }];
  v1Title.x = 24;
  v1Title.y = 24;
  v1Group.appendChild(v1Title);

  const v1Desc = figma.createText();
  v1Desc.fontName = { family: "Inter", style: "Regular" };
  v1Desc.characters = "Tactile modular armor plates: health is split into 8 discrete armor cells with 3px beveled gaps. Three runic ability sockets directly underneath with live countdowns.";
  v1Desc.fontSize = 12;
  v1Desc.fills = [{ type: "SOLID", color: { r: 0.6, g: 0.65, b: 0.75 } }];
  v1Desc.resize(492, 40);
  v1Desc.textAutoResize = "NONE";
  v1Desc.x = 24;
  v1Desc.y = 54;
  v1Group.appendChild(v1Desc);

  // Status Frame Component (Player Inferno)
  const v1Frame = figma.createFrame();
  v1Frame.name = "StatusFrame_Fusion1_Player";
  v1Frame.resize(492, 172);
  v1Frame.x = 24;
  v1Frame.y = 110;
  v1Frame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }]; // Heavy slate
  v1Frame.strokes = [{ type: "SOLID", color: { r: 0.52, g: 0.42, b: 0.22 } }]; // Forged brass
  v1Frame.strokeWeight = 2;
  v1Frame.cornerRadius = 6;
  v1Group.appendChild(v1Frame);
  addRivets(v1Frame, 492, 172);

  // Portrait Box
  const v1Portrait = figma.createFrame();
  v1Portrait.resize(92, 92);
  v1Portrait.x = 18;
  v1Portrait.y = 18;
  v1Portrait.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.05, b: 0.05 } }];
  v1Portrait.strokes = [{ type: "SOLID", color: { r: 0.82, g: 0.68, b: 0.32 } }];
  v1Portrait.strokeWeight = 2.5;
  v1Portrait.cornerRadius = 4;
  v1Frame.appendChild(v1Portrait);

  const v1Emblem = figma.createText();
  v1Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v1Emblem.characters = "🐉";
  v1Emblem.fontSize = 44;
  v1Emblem.x = 24;
  v1Emblem.y = 20;
  v1Portrait.appendChild(v1Emblem);

  // Level Badge Shield
  const v1Badge = figma.createFrame();
  v1Badge.resize(34, 22);
  v1Badge.x = 47;
  v1Badge.y = 98;
  v1Badge.fills = [{ type: "SOLID", color: { r: 0.12, g: 0.14, b: 0.18 } }];
  v1Badge.strokes = [{ type: "SOLID", color: { r: 0.90, g: 0.75, b: 0.35 } }];
  v1Badge.strokeWeight = 1.5;
  v1Badge.cornerRadius = 3;
  v1Frame.appendChild(v1Badge);

  const v1LvlTxt = figma.createText();
  v1LvlTxt.fontName = { family: "Inter", style: "Bold" };
  v1LvlTxt.characters = "1";
  v1LvlTxt.fontSize = 11;
  v1LvlTxt.fills = [{ type: "SOLID", color: { r: 0.98, g: 0.88, b: 0.45 } }];
  v1LvlTxt.x = 13;
  v1LvlTxt.y = 4;
  v1Badge.appendChild(v1LvlTxt);

  // Header Title
  const v1Name = figma.createText();
  v1Name.fontName = { family: "Cinzel", style: "Bold" };
  v1Name.characters = "INFERNO WYRM";
  v1Name.fontSize = 17;
  v1Name.fills = [{ type: "SOLID", color: { r: 0.96, g: 0.92, b: 0.82 } }];
  v1Name.x = 126;
  v1Name.y = 18;
  v1Frame.appendChild(v1Name);

  const v1Role = figma.createText();
  v1Role.fontName = { family: "Inter", style: "Bold" };
  v1Role.characters = "PLAYER • PYROCLASTIC APEX";
  v1Role.fontSize = 10;
  v1Role.fills = [{ type: "SOLID", color: { r: 0.88, g: 0.48, b: 0.22 } }];
  v1Role.x = 128;
  v1Role.y = 40;
  v1Frame.appendChild(v1Role);

  // Numerical Health Readout
  const v1HpNum = figma.createText();
  v1HpNum.fontName = { family: "Inter", style: "Bold" };
  v1HpNum.characters = "150 / 200 HP";
  v1HpNum.fontSize = 12;
  v1HpNum.fills = [{ type: "SOLID", color: { r: 0.92, g: 0.95, b: 1.0 } }];
  v1HpNum.x = 380;
  v1HpNum.y = 40;
  v1Frame.appendChild(v1HpNum);

  // Health Bar: 8 Discrete Armor Cell Blocks (each 25 HP)
  const v1HpChannel = figma.createFrame();
  v1HpChannel.name = "HealthChannel_8Blocks";
  v1HpChannel.resize(348, 26);
  v1HpChannel.x = 126;
  v1HpChannel.y = 60;
  v1HpChannel.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.06, b: 0.08 } }];
  v1HpChannel.strokes = [{ type: "SOLID", color: { r: 0.28, g: 0.32, b: 0.40 } }];
  v1HpChannel.strokeWeight = 1.5;
  v1HpChannel.cornerRadius = 4;
  v1Frame.appendChild(v1HpChannel);

  // 8 Blocks (6 full green, 1 ease amber, 1 empty dark)
  const blockW = 39.5;
  for (let b = 0; b < 8; b++) {
    const block = figma.createFrame();
    block.resize(blockW, 20);
    block.x = 4 + b * (blockW + 3.5);
    block.y = 3;
    block.cornerRadius = 2;

    if (b < 5) {
      // Full Health (Emerald Green with top specular sheen)
      block.fills = [{ type: "SOLID", color: { r: 0.16, g: 0.72, b: 0.38 } }];
      block.strokes = [{ type: "SOLID", color: { r: 0.25, g: 0.85, b: 0.48 } }];
      block.strokeWeight = 1;
    } else if (b === 5) {
      // Partial block (50% fill)
      block.fills = [{ type: "SOLID", color: { r: 0.16, g: 0.72, b: 0.38 } }];
      block.strokes = [{ type: "SOLID", color: { r: 0.25, g: 0.85, b: 0.48 } }];
      block.strokeWeight = 1;
    } else if (b === 6) {
      // Decay Ease Ghost Block (Damage taken)
      block.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.55, b: 0.15 } }];
      block.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.68, b: 0.25 } }];
      block.strokeWeight = 1;
    } else {
      // Empty depleted block
      block.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
      block.strokes = [{ type: "SOLID", color: { r: 0.14, g: 0.17, b: 0.22 } }];
      block.strokeWeight = 1;
    }
    v1HpChannel.appendChild(block);
  }

  // 3 Integrated Ability Sockets (Q, W, E)
  const abilities = [
    { key: "Q", name: "FIRE BREATH", state: "READY", color: { r: 0.2, g: 0.85, b: 0.45 }, bg: { r: 0.08, g: 0.14, b: 0.10 }, icon: "🔥" },
    { key: "W", name: "TAIL WHIP", state: "READY", color: { r: 0.2, g: 0.85, b: 0.45 }, bg: { r: 0.08, g: 0.14, b: 0.10 }, icon: "💫" },
    { key: "E", name: "SKY DIVE", state: "4.2s", color: { r: 0.95, g: 0.65, b: 0.20 }, bg: { r: 0.15, g: 0.10, b: 0.06 }, icon: "⚡" }
  ];

  for (let i = 0; i < 3; i++) {
    const ab = abilities[i];
    const sock = figma.createFrame();
    sock.name = "AbilitySocket_" + ab.key;
    sock.resize(112, 34);
    sock.x = 126 + i * 118;
    sock.y = 96;
    sock.fills = [{ type: "SOLID", color: ab.bg }];
    sock.strokes = [{ type: "SOLID", color: ab.color }];
    sock.strokeWeight = 1.5;
    sock.cornerRadius = 4;
    v1Frame.appendChild(sock);

    // Key badge pill
    const kBadge = figma.createFrame();
    kBadge.resize(18, 18);
    kBadge.x = 6;
    kBadge.y = 8;
    kBadge.fills = [{ type: "SOLID", color: { r: 0.12, g: 0.15, b: 0.20 } }];
    kBadge.strokes = [{ type: "SOLID", color: ab.color }];
    kBadge.strokeWeight = 1;
    kBadge.cornerRadius = 2;
    sock.appendChild(kBadge);

    const kTxt = figma.createText();
    kTxt.fontName = { family: "Inter", style: "Bold" };
    kTxt.characters = ab.key;
    kTxt.fontSize = 10;
    kTxt.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
    kTxt.x = 4;
    kTxt.y = 2;
    kBadge.appendChild(kTxt);

    // Ability name & status
    const abName = figma.createText();
    abName.fontName = { family: "Inter", style: "Bold" };
    abName.characters = ab.name;
    abName.fontSize = 8;
    abName.fills = [{ type: "SOLID", color: { r: 0.75, g: 0.80, b: 0.88 } }];
    abName.x = 30;
    abName.y = 5;
    sock.appendChild(abName);

    const abStatus = figma.createText();
    abStatus.fontName = { family: "Inter", style: "Bold" };
    abStatus.characters = ab.state;
    abStatus.fontSize = 11;
    abStatus.fills = [{ type: "SOLID", color: ab.color }];
    abStatus.x = 30;
    abStatus.y = 16;
    sock.appendChild(abStatus);
  }

  // Secondary Preview in V1: Mirrored Enemy AI Frame
  const v1AiLabel = figma.createText();
  v1AiLabel.fontName = { family: "Inter", style: "Bold" };
  v1AiLabel.characters = "MIRRORED ENEMY AI FRAME (FROST WYRM):";
  v1AiLabel.fontSize = 11;
  v1AiLabel.fills = [{ type: "SOLID", color: { r: 0.45, g: 0.72, b: 0.95 } }];
  v1AiLabel.x = 24;
  v1AiLabel.y = 310;
  v1Group.appendChild(v1AiLabel);

  const v1AiFrame = figma.createFrame();
  v1AiFrame.name = "StatusFrame_Fusion1_AI";
  v1AiFrame.resize(492, 172);
  v1AiFrame.x = 24;
  v1AiFrame.y = 336;
  v1AiFrame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
  v1AiFrame.strokes = [{ type: "SOLID", color: { r: 0.28, g: 0.58, b: 0.88 } }]; // Frost Runic
  v1AiFrame.strokeWeight = 2;
  v1AiFrame.cornerRadius = 6;
  v1Group.appendChild(v1AiFrame);
  addRivets(v1AiFrame, 492, 172, { r: 0.45, g: 0.75, b: 0.95 });

  const v1AiName = figma.createText();
  v1AiName.fontName = { family: "Cinzel", style: "Bold" };
  v1AiName.characters = "FROST WYRM";
  v1AiName.fontSize = 17;
  v1AiName.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.94, b: 1.0 } }];
  v1AiName.x = 18;
  v1AiName.y = 18;
  v1AiFrame.appendChild(v1AiName);

  const v1AiRole = figma.createText();
  v1AiRole.fontName = { family: "Inter", style: "Bold" };
  v1AiRole.characters = "ENEMY AI • GLACIAL CONVERGENCE";
  v1AiRole.fontSize = 10;
  v1AiRole.fills = [{ type: "SOLID", color: { r: 0.45, g: 0.78, b: 0.98 } }];
  v1AiRole.x = 18;
  v1AiRole.y = 40;
  v1AiFrame.appendChild(v1AiRole);

  const v1AiHpNum = figma.createText();
  v1AiHpNum.fontName = { family: "Inter", style: "Bold" };
  v1AiHpNum.characters = "175 / 200 HP";
  v1AiHpNum.fontSize = 12;
  v1AiHpNum.fills = [{ type: "SOLID", color: { r: 0.92, g: 0.95, b: 1.0 } }];
  v1AiHpNum.x = 280;
  v1AiHpNum.y = 40;
  v1AiFrame.appendChild(v1AiHpNum);

  // Mirrored AI Portrait on right
  const v1AiPort = figma.createFrame();
  v1AiPort.resize(92, 92);
  v1AiPort.x = 382;
  v1AiPort.y = 18;
  v1AiPort.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.12, b: 0.22 } }];
  v1AiPort.strokes = [{ type: "SOLID", color: { r: 0.45, g: 0.75, b: 0.98 } }];
  v1AiPort.strokeWeight = 2.5;
  v1AiPort.cornerRadius = 4;
  v1AiFrame.appendChild(v1AiPort);

  const v1AiEmblem = figma.createText();
  v1AiEmblem.fontName = { family: "Cinzel", style: "Bold" };
  v1AiEmblem.characters = "❄️";
  v1AiEmblem.fontSize = 44;
  v1AiEmblem.x = 24;
  v1AiEmblem.y = 20;
  v1AiPort.appendChild(v1AiEmblem);

  // AI 8-Block Health Bar (Crimson/Frost red)
  const v1AiHpChannel = figma.createFrame();
  v1AiHpChannel.resize(348, 26);
  v1AiHpChannel.x = 18;
  v1AiHpChannel.y = 60;
  v1AiHpChannel.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.06, b: 0.08 } }];
  v1AiHpChannel.strokes = [{ type: "SOLID", color: { r: 0.25, g: 0.40, b: 0.58 } }];
  v1AiHpChannel.strokeWeight = 1.5;
  v1AiHpChannel.cornerRadius = 4;
  v1AiFrame.appendChild(v1AiHpChannel);

  for (let b = 0; b < 8; b++) {
    const block = figma.createFrame();
    block.resize(blockW, 20);
    block.x = 4 + b * (blockW + 3.5);
    block.y = 3;
    block.cornerRadius = 2;
    if (b < 7) {
      block.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.24, b: 0.24 } }]; // Enemy Red
      block.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.45, b: 0.45 } }];
      block.strokeWeight = 1;
    } else {
      block.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
      block.strokes = [{ type: "SOLID", color: { r: 0.14, g: 0.17, b: 0.22 } }];
      block.strokeWeight = 1;
    }
    v1AiHpChannel.appendChild(block);
  }

  // AI Ability Nodes
  const aiAbilities = [
    { key: "Q", name: "FROST BREATH", state: "READY", color: { r: 0.4, g: 0.8, b: 1.0 }, bg: { r: 0.06, g: 0.12, b: 0.18 } },
    { key: "W", name: "TAIL SWEEP", state: "1.8s", color: { r: 0.95, g: 0.65, b: 0.20 }, bg: { r: 0.15, g: 0.10, b: 0.06 } },
    { key: "E", name: "BLIZZARD SLAM", state: "READY", color: { r: 0.4, g: 0.8, b: 1.0 }, bg: { r: 0.06, g: 0.12, b: 0.18 } }
  ];

  for (let i = 0; i < 3; i++) {
    const ab = aiAbilities[i];
    const sock = figma.createFrame();
    sock.resize(112, 34);
    sock.x = 18 + i * 118;
    sock.y = 96;
    sock.fills = [{ type: "SOLID", color: ab.bg }];
    sock.strokes = [{ type: "SOLID", color: ab.color }];
    sock.strokeWeight = 1.5;
    sock.cornerRadius = 4;
    v1AiFrame.appendChild(sock);

    const abName = figma.createText();
    abName.fontName = { family: "Inter", style: "Bold" };
    abName.characters = ab.name;
    abName.fontSize = 8;
    abName.fills = [{ type: "SOLID", color: { r: 0.75, g: 0.85, b: 0.95 } }];
    abName.x = 10;
    abName.y = 5;
    sock.appendChild(abName);

    const abStatus = figma.createText();
    abStatus.fontName = { family: "Inter", style: "Bold" };
    abStatus.characters = ab.state;
    abStatus.fontSize = 11;
    abStatus.fills = [{ type: "SOLID", color: ab.color }];
    abStatus.x = 10;
    abStatus.y = 16;
    sock.appendChild(abStatus);
  }

  // =========================================================================
  // FUSION VARIANT 2: "The Radiant Bulwark" (Dota 2 Tick Grids & Flush Tactical Tabs)
  // =========================================================================
  const v2Group = figma.createFrame();
  v2Group.name = "Fusion 2: Radiant Bulwark (Dota 2 Tick Grids & Flush Tactical Tabs)";
  v2Group.resize(540, 1020);
  v2Group.x = 650;
  v2Group.y = 150;
  v2Group.fills = [{ type: "SOLID", color: { r: 0.07, g: 0.09, b: 0.12 } }];
  v2Group.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v2Group.strokeWeight = 1;
  v2Group.cornerRadius = 12;
  board.appendChild(v2Group);

  const v2Title = figma.createText();
  v2Title.fontName = { family: "Cinzel", style: "Bold" };
  v2Title.characters = "FUSION 2 : RADIANT BULWARK";
  v2Title.fontSize = 18;
  v2Title.fills = [{ type: "SOLID", color: { r: 0.92, g: 0.80, b: 0.44 } }];
  v2Title.x = 24;
  v2Title.y = 24;
  v2Group.appendChild(v2Title);

  const v2Desc = figma.createText();
  v2Desc.fontName = { family: "Inter", style: "Regular" };
  v2Desc.characters = "Dota 2 notched tick-grid HP bar: continuous emerald gradient divided into tactile 25-HP segments with thick 100-HP notches, plus flush interlocking ability status tabs with micro progress bars.";
  v2Desc.fontSize = 12;
  v2Desc.fills = [{ type: "SOLID", color: { r: 0.6, g: 0.65, b: 0.75 } }];
  v2Desc.resize(492, 40);
  v2Desc.textAutoResize = "NONE";
  v2Desc.x = 24;
  v2Desc.y = 54;
  v2Group.appendChild(v2Desc);

  const v2Frame = figma.createFrame();
  v2Frame.name = "StatusFrame_Fusion2_Player";
  v2Frame.resize(492, 172);
  v2Frame.x = 24;
  v2Frame.y = 110;
  v2Frame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
  v2Frame.strokes = [{ type: "SOLID", color: { r: 0.65, g: 0.52, b: 0.25 } }]; // Burnished Gold
  v2Frame.strokeWeight = 2;
  v2Frame.cornerRadius = 6;
  v2Group.appendChild(v2Frame);
  addRivets(v2Frame, 492, 172);

  // Octagonal Portrait
  const v2Portrait = figma.createFrame();
  v2Portrait.resize(92, 92);
  v2Portrait.x = 18;
  v2Portrait.y = 18;
  v2Portrait.fills = [{ type: "SOLID", color: { r: 0.16, g: 0.06, b: 0.04 } }];
  v2Portrait.strokes = [{ type: "SOLID", color: { r: 0.90, g: 0.75, b: 0.35 } }];
  v2Portrait.strokeWeight = 2.5;
  v2Portrait.cornerRadius = 14;
  v2Frame.appendChild(v2Portrait);

  const v2Emblem = figma.createText();
  v2Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v2Emblem.characters = "🔥";
  v2Emblem.fontSize = 44;
  v2Emblem.x = 24;
  v2Emblem.y = 20;
  v2Portrait.appendChild(v2Emblem);

  const v2Name = figma.createText();
  v2Name.fontName = { family: "Cinzel", style: "Bold" };
  v2Name.characters = "INFERNAL SOVEREIGN";
  v2Name.fontSize = 17;
  v2Name.fills = [{ type: "SOLID", color: { r: 0.98, g: 0.94, b: 0.85 } }];
  v2Name.x = 126;
  v2Name.y = 18;
  v2Frame.appendChild(v2Name);

  const v2Role = figma.createText();
  v2Role.fontName = { family: "Inter", style: "Bold" };
  v2Role.characters = "RANK: RADIANT ANCIENT • 100% COMBAT READY";
  v2Role.fontSize = 10;
  v2Role.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.70, b: 0.35 } }];
  v2Role.x = 128;
  v2Role.y = 40;
  v2Frame.appendChild(v2Role);

  const v2HpNum = figma.createText();
  v2HpNum.fontName = { family: "Inter", style: "Bold" };
  v2HpNum.characters = "160 / 200 HP";
  v2HpNum.fontSize = 12;
  v2HpNum.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
  v2HpNum.x = 380;
  v2HpNum.y = 40;
  v2Frame.appendChild(v2HpNum);

  // Dota 2 Block-Notched Channel
  const v2HpChannel = figma.createFrame();
  v2HpChannel.resize(348, 26);
  v2HpChannel.x = 126;
  v2HpChannel.y = 60;
  v2HpChannel.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.06, b: 0.08 } }];
  v2HpChannel.strokes = [{ type: "SOLID", color: { r: 0.40, g: 0.45, b: 0.55 } }];
  v2HpChannel.strokeWeight = 1.5;
  v2HpChannel.cornerRadius = 3;
  v2HpChannel.clipsContent = true;
  v2Frame.appendChild(v2HpChannel);

  // Ease Ghost bar
  const v2HpEase = figma.createRectangle();
  v2HpEase.resize(300, 26);
  v2HpEase.fills = [{ type: "SOLID", color: { r: 0.88, g: 0.58, b: 0.18 } }];
  v2HpChannel.appendChild(v2HpEase);

  // Fill
  const v2HpFill = figma.createRectangle();
  v2HpFill.resize(250, 26);
  v2HpFill.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.75, b: 0.40 } }];
  v2HpChannel.appendChild(v2HpFill);

  // Segment Block Dividers (3px dark grooves every 43.5px)
  for (let s = 1; s < 8; s++) {
    const notch = figma.createRectangle();
    const isMajor = s === 4; // Major 100 HP divider
    notch.resize(isMajor ? 3.5 : 2, 26);
    notch.x = s * 43.5;
    notch.y = 0;
    notch.fills = [{ type: "SOLID", color: isMajor ? { r: 0.10, g: 0.12, b: 0.16 } : { r: 0.04, g: 0.05, b: 0.07 } }];
    v2HpChannel.appendChild(notch);
  }

  // Flush Tactical Tabs with Micro Progress Bar Underneath
  const v2Tabs = [
    { key: "Q", name: "FIRE CONE", ready: true, status: "READY", progress: 1.0 },
    { key: "W", name: "TAIL SWEEP", ready: true, status: "READY", progress: 1.0 },
    { key: "E", name: "SKY DIVE", ready: false, status: "CD: 5.8s", progress: 0.35 }
  ];

  for (let i = 0; i < 3; i++) {
    const tab = v2Tabs[i];
    const tFrame = figma.createFrame();
    tFrame.resize(112, 36);
    tFrame.x = 126 + i * 118;
    tFrame.y = 96;
    tFrame.fills = [{ type: "SOLID", color: { r: 0.09, g: 0.11, b: 0.15 } }];
    tFrame.strokes = [{ type: "SOLID", color: tab.ready ? { r: 0.22, g: 0.75, b: 0.40 } : { r: 0.55, g: 0.40, b: 0.20 } }];
    tFrame.strokeWeight = 1.5;
    tFrame.cornerRadius = 3;
    tFrame.clipsContent = true;
    v2Frame.appendChild(tFrame);

    const tKey = figma.createText();
    tKey.fontName = { family: "Inter", style: "Bold" };
    tKey.characters = `[${tab.key}] ${tab.name}`;
    tKey.fontSize = 8;
    tKey.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.90, b: 0.98 } }];
    tKey.x = 8;
    tKey.y = 5;
    tFrame.appendChild(tKey);

    const tStat = figma.createText();
    tStat.fontName = { family: "Inter", style: "Bold" };
    tStat.characters = tab.status;
    tStat.fontSize = 11;
    tStat.fills = [{ type: "SOLID", color: tab.ready ? { r: 0.25, g: 0.88, b: 0.45 } : { r: 0.98, g: 0.72, b: 0.25 } }];
    tStat.x = 8;
    tStat.y = 16;
    tFrame.appendChild(tStat);

    // Micro recovery progress line at bottom of tab
    const pBg = figma.createRectangle();
    pBg.resize(112, 3);
    pBg.x = 0;
    pBg.y = 33;
    pBg.fills = [{ type: "SOLID", color: { r: 0.15, g: 0.18, b: 0.24 } }];
    tFrame.appendChild(pBg);

    const pFill = figma.createRectangle();
    pFill.resize(112 * tab.progress, 3);
    pFill.x = 0;
    pFill.y = 33;
    pFill.fills = [{ type: "SOLID", color: tab.ready ? { r: 0.25, g: 0.88, b: 0.45 } : { r: 0.98, g: 0.72, b: 0.25 } }];
    tFrame.appendChild(pFill);
  }

  // =========================================================================
  // FUSION VARIANT 3: "The Ancient Warplate" (10 Power-Cell Segment Blocks & Dial Sockets)
  // =========================================================================
  const v3Group = figma.createFrame();
  v3Group.name = "Fusion 3: Ancient Warplate (10 Power-Cells & Dial Sockets)";
  v3Group.resize(540, 1020);
  v3Group.x = 1240;
  v3Group.y = 150;
  v3Group.fills = [{ type: "SOLID", color: { r: 0.07, g: 0.09, b: 0.12 } }];
  v3Group.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v3Group.strokeWeight = 1;
  v3Group.cornerRadius = 12;
  board.appendChild(v3Group);

  const v3Title = figma.createText();
  v3Title.fontName = { family: "Cinzel", style: "Bold" };
  v3Title.characters = "FUSION 3 : ANCIENT WARPLATE";
  v3Title.fontSize = 18;
  v3Title.fills = [{ type: "SOLID", color: { r: 0.92, g: 0.80, b: 0.44 } }];
  v3Title.x = 24;
  v3Title.y = 24;
  v3Group.appendChild(v3Title);

  const v3Desc = figma.createText();
  v3Desc.fontName = { family: "Inter", style: "Regular" };
  v3Desc.characters = "Mechanical heavy-chassis feel: 10 glowing dragon-scale power cells with recessed stone grates. Ability nodes feature circular radial countdown badges and high-visibility status pips.";
  v3Desc.fontSize = 12;
  v3Desc.fills = [{ type: "SOLID", color: { r: 0.6, g: 0.65, b: 0.75 } }];
  v3Desc.resize(492, 40);
  v3Desc.textAutoResize = "NONE";
  v3Desc.x = 24;
  v3Desc.y = 54;
  v3Group.appendChild(v3Desc);

  const v3Frame = figma.createFrame();
  v3Frame.name = "StatusFrame_Fusion3_Player";
  v3Frame.resize(492, 172);
  v3Frame.x = 24;
  v3Frame.y = 110;
  v3Frame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
  v3Frame.strokes = [{ type: "SOLID", color: { r: 0.75, g: 0.60, b: 0.28 } }]; // Antiqued Gold
  v3Frame.strokeWeight = 2;
  v3Frame.cornerRadius = 6;
  v3Group.appendChild(v3Frame);
  addRivets(v3Frame, 492, 172);

  // Round Medallion Portrait
  const v3Portrait = figma.createFrame();
  v3Portrait.resize(92, 92);
  v3Portrait.x = 18;
  v3Portrait.y = 18;
  v3Portrait.fills = [{ type: "SOLID", color: { r: 0.14, g: 0.04, b: 0.04 } }];
  v3Portrait.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.82, b: 0.38 } }];
  v3Portrait.strokeWeight = 3;
  v3Portrait.cornerRadius = 46;
  v3Frame.appendChild(v3Portrait);

  const v3Emblem = figma.createText();
  v3Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v3Emblem.characters = "👑";
  v3Emblem.fontSize = 42;
  v3Emblem.x = 25;
  v3Emblem.y = 22;
  v3Portrait.appendChild(v3Emblem);

  const v3Name = figma.createText();
  v3Name.fontName = { family: "Cinzel", style: "Bold" };
  v3Name.characters = "PYROCLAST • THE DREAD";
  v3Name.fontSize = 17;
  v3Name.fills = [{ type: "SOLID", color: { r: 0.98, g: 0.94, b: 0.85 } }];
  v3Name.x = 126;
  v3Name.y = 18;
  v3Frame.appendChild(v3Name);

  const v3Role = figma.createText();
  v3Role.fontName = { family: "Inter", style: "Bold" };
  v3Role.characters = "CHAMPIONSHIP TIER • 80% HP INTEGRITY";
  v3Role.fontSize = 10;
  v3Role.fills = [{ type: "SOLID", color: { r: 0.92, g: 0.75, b: 0.35 } }];
  v3Role.x = 128;
  v3Role.y = 40;
  v3Frame.appendChild(v3Role);

  const v3HpNum = figma.createText();
  v3HpNum.fontName = { family: "Inter", style: "Bold" };
  v3HpNum.characters = "160 / 200 HP";
  v3HpNum.fontSize = 12;
  v3HpNum.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
  v3HpNum.x = 380;
  v3HpNum.y = 40;
  v3Frame.appendChild(v3HpNum);

  // 10 Glowing Dragon-Scale Power Cells (20 HP each)
  const v3HpChannel = figma.createFrame();
  v3HpChannel.resize(348, 26);
  v3HpChannel.x = 126;
  v3HpChannel.y = 60;
  v3HpChannel.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.06, b: 0.08 } }];
  v3HpChannel.strokes = [{ type: "SOLID", color: { r: 0.35, g: 0.40, b: 0.48 } }];
  v3HpChannel.strokeWeight = 1.5;
  v3HpChannel.cornerRadius = 4;
  v3Frame.appendChild(v3HpChannel);

  const cellW = 30.5;
  for (let c = 0; c < 10; c++) {
    const cell = figma.createFrame();
    cell.resize(cellW, 20);
    cell.x = 3.5 + c * (cellW + 3.8);
    cell.y = 3;
    cell.cornerRadius = 2;
    if (c < 8) {
      cell.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.78, b: 0.42 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.30, g: 0.90, b: 0.52 } }];
      cell.strokeWeight = 1;
    } else if (c === 8) {
      // Ease flash
      cell.fills = [{ type: "SOLID", color: { r: 0.88, g: 0.55, b: 0.15 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.98, g: 0.70, b: 0.25 } }];
      cell.strokeWeight = 1;
    } else {
      // Empty grate
      cell.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.14 } }];
      cell.strokes = [{ type: "SOLID", color: { r: 0.15, g: 0.18, b: 0.24 } }];
      cell.strokeWeight = 1;
    }
    v3HpChannel.appendChild(cell);
  }

  // Dial Sockets with Circular Mini-Gauge
  const v3Nodes = [
    { key: "Q", name: "BREATH", state: "READY", ready: true },
    { key: "W", name: "WHIP", state: "READY", ready: true },
    { key: "E", name: "SLAM", state: "3.5s", ready: false }
  ];

  for (let i = 0; i < 3; i++) {
    const n = v3Nodes[i];
    const nFrame = figma.createFrame();
    nFrame.resize(112, 34);
    nFrame.x = 126 + i * 118;
    nFrame.y = 96;
    nFrame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.14 } }];
    nFrame.strokes = [{ type: "SOLID", color: n.ready ? { r: 0.25, g: 0.85, b: 0.45 } : { r: 0.85, g: 0.55, b: 0.20 } }];
    nFrame.strokeWeight = 1.5;
    nFrame.cornerRadius = 17; // Pill shape
    v3Frame.appendChild(nFrame);

    // Circular dial indicator
    const dial = figma.createEllipse();
    dial.resize(22, 22);
    dial.x = 6;
    dial.y = 6;
    dial.fills = [{ type: "SOLID", color: n.ready ? { r: 0.15, g: 0.65, b: 0.35 } : { r: 0.55, g: 0.30, b: 0.10 } }];
    dial.strokes = [{ type: "SOLID", color: n.ready ? { r: 0.30, g: 0.90, b: 0.50 } : { r: 0.90, g: 0.60, b: 0.20 } }];
    dial.strokeWeight = 1.5;
    nFrame.appendChild(dial);

    const dTxt = figma.createText();
    dTxt.fontName = { family: "Inter", style: "Bold" };
    dTxt.characters = n.key;
    dTxt.fontSize = 11;
    dTxt.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
    dTxt.x = 12;
    dTxt.y = 9;
    nFrame.appendChild(dTxt);

    const nTxt = figma.createText();
    nTxt.fontName = { family: "Inter", style: "Bold" };
    nTxt.characters = n.state;
    nTxt.fontSize = 11;
    nTxt.fills = [{ type: "SOLID", color: n.ready ? { r: 0.30, g: 0.90, b: 0.50 } : { r: 0.95, g: 0.70, b: 0.25 } }];
    nTxt.x = 36;
    nTxt.y = 10;
    nFrame.appendChild(nTxt);
  }

  // Select board and zoom into view
  figma.currentPage.selection = [board];
  figma.viewport.scrollAndZoomIntoView([board]);

  return {
    success: true,
    boardId: board.id,
    boardName: board.name
  };
}

return await createFusionVariants();
"""

res = exec_figma(fusion_script)
print(res)

if res.get("success") and "result" in res:
    board_id = res["result"].get("boardId")
    export_code = f"""
    async function exportNewBoard() {{
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
    return await exportNewBoard();
    """
    exp_res = exec_figma(export_code)
    if "result" in exp_res and "base64" in exp_res["result"]:
        img_bytes = base64.b64decode(exp_res["result"]["base64"])
        out_path = "/home/subodh/.gemini/antigravity-cli/brain/c5488275-621a-4183-8c8b-c55127a653d3/dota2_fusion_variants.png"
        with open(out_path, "wb") as f:
            f.write(img_bytes)
        print(f"Exported fusion preview to {out_path} ({len(img_bytes)} bytes)")
