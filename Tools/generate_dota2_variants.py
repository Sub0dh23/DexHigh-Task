import urllib.request
import json

def exec_figma(code: str):
    url = "http://localhost:8766/exec"
    payload = json.dumps({"code": code}).encode("utf-8")
    req = urllib.request.Request(url, data=payload, headers={"Content-Type": "application/json"})
    with urllib.request.urlopen(req) as resp:
        return json.loads(resp.read().decode("utf-8"))

figma_script = """
async function createDota2StatusFrames() {
  await figma.loadFontAsync({ family: "Cinzel", style: "Regular" });
  await figma.loadFontAsync({ family: "Cinzel", style: "Bold" });
  await figma.loadFontAsync({ family: "Inter", style: "Regular" });
  await figma.loadFontAsync({ family: "Inter", style: "Bold" });

  const page = figma.currentPage;

  // 1. Master Canvas Frame
  const board = figma.createFrame();
  board.name = "Dota 2 Dragon Status Frames - 3 Concept Variants";
  board.resize(1760, 1180);
  board.x = 0;
  board.y = 0;
  board.fills = [{ type: "SOLID", color: { r: 0.047, g: 0.059, b: 0.078 } }]; // #0C0F14 deep Dota background
  board.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  board.strokeWeight = 2;
  board.cornerRadius = 16;
  board.clipsContent = true;

  // Board Header
  const title = figma.createText();
  title.fontName = { family: "Cinzel", style: "Bold" };
  title.characters = "DRAGON STATUS FRAME CONCEPTS";
  title.fontSize = 32;
  title.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.84, b: 0.52 } }]; // Warm Gold
  title.x = 60;
  title.y = 50;
  board.appendChild(title);

  const subtitle = figma.createText();
  subtitle.fontName = { family: "Inter", style: "Regular" };
  subtitle.characters = "Dota 2 / Auto-Battler Aesthetic Exploration • 3 Distinct Architectural Approaches";
  subtitle.fontSize = 15;
  subtitle.fills = [{ type: "SOLID", color: { r: 0.55, g: 0.62, b: 0.72 } }];
  subtitle.x = 60;
  subtitle.y = 96;
  board.appendChild(subtitle);

  // Helper function to create metallic border frame
  function createMetalPanel(w, h, bgColor, strokeColor, radius = 6) {
    const frame = figma.createFrame();
    frame.resize(w, h);
    frame.fills = [{ type: "SOLID", color: bgColor }];
    frame.strokes = [{ type: "SOLID", color: strokeColor }];
    frame.strokeWeight = 1.5;
    frame.cornerRadius = radius;
    return frame;
  }

  // =========================================================================
  // VARIANT 1: The Radiant Bastion (Classic Stone & Forged Brass)
  // =========================================================================
  const v1Group = figma.createFrame();
  v1Group.name = "Variant 1: Radiant Bastion (Classic Stone & Forged Brass)";
  v1Group.resize(520, 940);
  v1Group.x = 60;
  v1Group.y = 150;
  v1Group.fills = [{ type: "SOLID", color: { r: 0.07, g: 0.09, b: 0.12 } }];
  v1Group.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v1Group.strokeWeight = 1;
  v1Group.cornerRadius = 12;
  board.appendChild(v1Group);

  // Variant 1 Label
  const v1Label = figma.createText();
  v1Label.fontName = { family: "Cinzel", style: "Bold" };
  v1Label.characters = "VARIANT A : RADIANT BASTION";
  v1Label.fontSize = 18;
  v1Label.fills = [{ type: "SOLID", color: { r: 0.91, g: 0.78, b: 0.42 } }];
  v1Label.x = 24;
  v1Label.y = 24;
  v1Group.appendChild(v1Label);

  const v1Desc = figma.createText();
  v1Desc.fontName = { family: "Inter", style: "Regular" };
  v1Desc.characters = "Classic Dota 2 HUD heritage: chiseled dark slate, forged iron brackets, tarnished brass filigree corners, and segmented HP ticks.";
  v1Desc.fontSize = 12;
  v1Desc.fills = [{ type: "SOLID", color: { r: 0.6, g: 0.65, b: 0.75 } }];
  v1Desc.resize(472, 40);
  v1Desc.textAutoResize = "NONE";
  v1Desc.x = 24;
  v1Desc.y = 54;
  v1Group.appendChild(v1Desc);

  // V1 Frame Component
  const v1Frame = figma.createFrame();
  v1Frame.name = "StatusFrame_VariantA";
  v1Frame.resize(472, 140);
  v1Frame.x = 24;
  v1Frame.y = 110;
  v1Frame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }]; // Heavy slate
  v1Frame.strokes = [{ type: "SOLID", color: { r: 0.45, g: 0.38, b: 0.22 } }]; // Tarnished Brass
  v1Frame.strokeWeight = 2;
  v1Frame.cornerRadius = 4;
  v1Group.appendChild(v1Frame);

  // Outer corner rivets
  const rivetColors = { r: 0.75, g: 0.65, b: 0.35 };
  [[6,6], [460,6], [6,128], [460,128]].forEach(([rx, ry]) => {
    const rivet = figma.createEllipse();
    rivet.resize(6, 6);
    rivet.x = rx;
    rivet.y = ry;
    rivet.fills = [{ type: "SOLID", color: rivetColors }];
    v1Frame.appendChild(rivet);
  });

  // Portrait Box (Square with inner shadow)
  const v1Portrait = figma.createFrame();
  v1Portrait.name = "Portrait_Container";
  v1Portrait.resize(80, 80);
  v1Portrait.x = 18;
  v1Portrait.y = 18;
  v1Portrait.fills = [{ type: "SOLID", color: { r: 0.16, g: 0.05, b: 0.05 } }]; // Dark crimson background
  v1Portrait.strokes = [{ type: "SOLID", color: { r: 0.75, g: 0.62, b: 0.32 } }];
  v1Portrait.strokeWeight = 2;
  v1Portrait.cornerRadius = 4;
  v1Frame.appendChild(v1Portrait);

  // Dragon silhouette / emblem placeholder
  const v1Emblem = figma.createText();
  v1Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v1Emblem.characters = "🐉";
  v1Emblem.fontSize = 38;
  v1Emblem.x = 20;
  v1Emblem.y = 16;
  v1Portrait.appendChild(v1Emblem);

  // Level Badge Shield
  const v1Badge = figma.createFrame();
  v1Badge.name = "Level_Badge";
  v1Badge.resize(30, 20);
  v1Badge.x = 43;
  v1Badge.y = 86;
  v1Badge.fills = [{ type: "SOLID", color: { r: 0.12, g: 0.14, b: 0.18 } }];
  v1Badge.strokes = [{ type: "SOLID", color: { r: 0.85, g: 0.72, b: 0.35 } }];
  v1Badge.strokeWeight = 1.5;
  v1Badge.cornerRadius = 3;
  v1Frame.appendChild(v1Badge);

  const v1LvlText = figma.createText();
  v1LvlText.fontName = { family: "Inter", style: "Bold" };
  v1LvlText.characters = "1";
  v1LvlText.fontSize = 11;
  v1LvlText.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.85, b: 0.45 } }];
  v1LvlText.x = 11;
  v1LvlText.y = 3;
  v1Badge.appendChild(v1LvlText);

  // Header Title & Subtitle
  const v1Name = figma.createText();
  v1Name.fontName = { family: "Cinzel", style: "Bold" };
  v1Name.characters = "INFERNO WYRM";
  v1Name.fontSize = 17;
  v1Name.fills = [{ type: "SOLID", color: { r: 0.96, g: 0.92, b: 0.82 } }];
  v1Name.x = 112;
  v1Name.y = 20;
  v1Frame.appendChild(v1Name);

  const v1Role = figma.createText();
  v1Role.fontName = { family: "Inter", style: "Bold" };
  v1Role.characters = "PLAYER • PYRO-ASPECT";
  v1Role.fontSize = 10;
  v1Role.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.45, b: 0.22 } }]; // Fire accent
  v1Role.x = 114;
  v1Role.y = 42;
  v1Frame.appendChild(v1Role);

  // Health Bar Channel (Stone Trough)
  const v1HpChannel = figma.createFrame();
  v1HpChannel.name = "HealthBar_Channel";
  v1HpChannel.resize(336, 26);
  v1HpChannel.x = 112;
  v1HpChannel.y = 62;
  v1HpChannel.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.06, b: 0.08 } }];
  v1HpChannel.strokes = [{ type: "SOLID", color: { r: 0.25, g: 0.30, b: 0.38 } }];
  v1HpChannel.strokeWeight = 1.5;
  v1HpChannel.cornerRadius = 3;
  v1HpChannel.clipsContent = true;
  v1Frame.appendChild(v1HpChannel);

  // Ease / Damage ghost bar
  const v1HpEase = figma.createRectangle();
  v1HpEase.name = "Health_EaseGhost";
  v1HpEase.resize(280, 26);
  v1HpEase.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.55, b: 0.15 } }]; // Amber decay
  v1HpChannel.appendChild(v1HpEase);

  // Main Health Fill (Emerald Green gradient representation)
  const v1HpFill = figma.createRectangle();
  v1HpFill.name = "Health_Fill";
  v1HpFill.resize(235, 26);
  v1HpFill.fills = [{ type: "SOLID", color: { r: 0.16, g: 0.68, b: 0.38 } }]; // Dota Green
  v1HpChannel.appendChild(v1HpFill);

  // Segment Notches (every 250 HP tick marks like Dota 2)
  for (let step = 35; step < 336; step += 35) {
    const notch = figma.createRectangle();
    notch.name = "Notch";
    notch.resize(step % 140 === 0 ? 2 : 1, 26);
    notch.x = step;
    notch.y = 0;
    notch.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.05, b: 0.06 } }];
    notch.opacity = step % 140 === 0 ? 0.9 : 0.5;
    v1HpChannel.appendChild(notch);
  }

  // Health numbers overlay
  const v1HpText = figma.createText();
  v1HpText.fontName = { family: "Inter", style: "Bold" };
  v1HpText.characters = "140 / 200";
  v1HpText.fontSize = 12;
  v1HpText.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
  v1HpText.x = 135;
  v1HpText.y = 5;
  v1HpChannel.appendChild(v1HpText);

  // Secondary Energy / Stamina Bar
  const v1EnergyChannel = figma.createFrame();
  v1EnergyChannel.name = "EnergyBar_Channel";
  v1EnergyChannel.resize(336, 12);
  v1EnergyChannel.x = 112;
  v1EnergyChannel.y = 96;
  v1EnergyChannel.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.06, b: 0.08 } }];
  v1EnergyChannel.strokes = [{ type: "SOLID", color: { r: 0.20, g: 0.25, b: 0.32 } }];
  v1EnergyChannel.strokeWeight = 1;
  v1EnergyChannel.cornerRadius = 2;
  v1EnergyChannel.clipsContent = true;
  v1Frame.appendChild(v1EnergyChannel);

  const v1EnergyFill = figma.createRectangle();
  v1EnergyFill.name = "Energy_Fill";
  v1EnergyFill.resize(280, 12);
  v1EnergyFill.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.48, b: 0.85 } }]; // Dota Mana Azure
  v1EnergyChannel.appendChild(v1EnergyFill);

  // Interactive State Sample: Injured / Ghost Bar State Preview
  const v1InjuredLabel = figma.createText();
  v1InjuredLabel.fontName = { family: "Inter", style: "Bold" };
  v1InjuredLabel.characters = "PREVIEW: AI ENEMY MIRRORED VARIANT (FROST WYRM)";
  v1InjuredLabel.fontSize = 11;
  v1InjuredLabel.fills = [{ type: "SOLID", color: { r: 0.45, g: 0.72, b: 0.95 } }];
  v1InjuredLabel.x = 24;
  v1InjuredLabel.y = 280;
  v1Group.appendChild(v1InjuredLabel);

  // Mirrored AI Frame
  const v1AiFrame = figma.createFrame();
  v1AiFrame.name = "StatusFrame_VariantA_AI";
  v1AiFrame.resize(472, 140);
  v1AiFrame.x = 24;
  v1AiFrame.y = 310;
  v1AiFrame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
  v1AiFrame.strokes = [{ type: "SOLID", color: { r: 0.25, g: 0.55, b: 0.85 } }]; // Frost Runic Blue
  v1AiFrame.strokeWeight = 2;
  v1AiFrame.cornerRadius = 4;
  v1Group.appendChild(v1AiFrame);

  // AI Name & Bars
  const v1AiName = figma.createText();
  v1AiName.fontName = { family: "Cinzel", style: "Bold" };
  v1AiName.characters = "FROST WYRM";
  v1AiName.fontSize = 17;
  v1AiName.fills = [{ type: "SOLID", color: { r: 0.82, g: 0.92, b: 1.0 } }];
  v1AiName.x = 24;
  v1AiName.y = 20;
  v1AiFrame.appendChild(v1AiName);

  const v1AiRole = figma.createText();
  v1AiRole.fontName = { family: "Inter", style: "Bold" };
  v1AiRole.characters = "ENEMY AI • GLACIAL-ASPECT";
  v1AiRole.fontSize = 10;
  v1AiRole.fills = [{ type: "SOLID", color: { r: 0.40, g: 0.75, b: 0.95 } }];
  v1AiRole.x = 24;
  v1AiRole.y = 42;
  v1AiFrame.appendChild(v1AiRole);

  const v1AiHpChannel = figma.createFrame();
  v1AiHpChannel.resize(336, 26);
  v1AiHpChannel.x = 24;
  v1AiHpChannel.y = 62;
  v1AiHpChannel.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.06, b: 0.08 } }];
  v1AiHpChannel.strokes = [{ type: "SOLID", color: { r: 0.25, g: 0.45, b: 0.65 } }];
  v1AiHpChannel.strokeWeight = 1.5;
  v1AiHpChannel.cornerRadius = 3;
  v1AiHpChannel.clipsContent = true;
  v1AiFrame.appendChild(v1AiHpChannel);

  const v1AiHpFill = figma.createRectangle();
  v1AiHpFill.resize(300, 26);
  v1AiHpFill.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.22, b: 0.22 } }]; // Enemy Red/Crimson
  v1AiHpChannel.appendChild(v1AiHpFill);

  const v1AiHpText = figma.createText();
  v1AiHpText.fontName = { family: "Inter", style: "Bold" };
  v1AiHpText.characters = "180 / 200";
  v1AiHpText.fontSize = 12;
  v1AiHpText.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
  v1AiHpText.x = 135;
  v1AiHpText.y = 5;
  v1AiHpChannel.appendChild(v1AiHpText);

  // AI Portrait on right
  const v1AiPortrait = figma.createFrame();
  v1AiPortrait.resize(80, 80);
  v1AiPortrait.x = 374;
  v1AiPortrait.y = 18;
  v1AiPortrait.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.12, b: 0.20 } }];
  v1AiPortrait.strokes = [{ type: "SOLID", color: { r: 0.35, g: 0.65, b: 0.95 } }];
  v1AiPortrait.strokeWeight = 2;
  v1AiPortrait.cornerRadius = 4;
  v1AiFrame.appendChild(v1AiPortrait);

  const v1AiEmblem = figma.createText();
  v1AiEmblem.fontName = { family: "Cinzel", style: "Bold" };
  v1AiEmblem.characters = "❄️";
  v1AiEmblem.fontSize = 38;
  v1AiEmblem.x = 20;
  v1AiEmblem.y = 16;
  v1AiPortrait.appendChild(v1AiEmblem);

  // =========================================================================
  // VARIANT 2: The Ancient Wyrmstone (Carved Obsidian & Dragon Fangs)
  // =========================================================================
  const v2Group = figma.createFrame();
  v2Group.name = "Variant 2: Ancient Wyrmstone (Carved Obsidian & Dragon Fangs)";
  v2Group.resize(520, 940);
  v2Group.x = 620;
  v2Group.y = 150;
  v2Group.fills = [{ type: "SOLID", color: { r: 0.07, g: 0.09, b: 0.12 } }];
  v2Group.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v2Group.strokeWeight = 1;
  v2Group.cornerRadius = 12;
  board.appendChild(v2Group);

  const v2Label = figma.createText();
  v2Label.fontName = { family: "Cinzel", style: "Bold" };
  v2Label.characters = "VARIANT B : ANCIENT WYRMSTONE";
  v2Label.fontSize = 18;
  v2Label.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.45, b: 0.25 } }]; // Flame Red/Orange
  v2Label.x = 24;
  v2Label.y = 24;
  v2Group.appendChild(v2Label);

  const v2Desc = figma.createText();
  v2Desc.fontName = { family: "Inter", style: "Regular" };
  v2Desc.characters = "Aggressive draconic aesthetic: angular faceted plates, glowing runic channels, fiery bevels, and diamond-cut portrait frame.";
  v2Desc.fontSize = 12;
  v2Desc.fills = [{ type: "SOLID", color: { r: 0.6, g: 0.65, b: 0.75 } }];
  v2Desc.resize(472, 40);
  v2Desc.textAutoResize = "NONE";
  v2Desc.x = 24;
  v2Desc.y = 54;
  v2Group.appendChild(v2Desc);

  // V2 Frame Component
  const v2Frame = figma.createFrame();
  v2Frame.name = "StatusFrame_VariantB";
  v2Frame.resize(472, 140);
  v2Frame.x = 24;
  v2Frame.y = 110;
  v2Frame.fills = [{ type: "SOLID", color: { r: 0.06, g: 0.07, b: 0.10 } }]; // Dark obsidian
  v2Frame.strokes = [{ type: "SOLID", color: { r: 0.85, g: 0.35, b: 0.15 } }]; // Ember edge
  v2Frame.strokeWeight = 2;
  v2Frame.cornerRadius = 8;
  v2Group.appendChild(v2Frame);

  // Hexagonal Portrait
  const v2Portrait = figma.createFrame();
  v2Portrait.resize(84, 84);
  v2Portrait.x = 18;
  v2Portrait.y = 18;
  v2Portrait.fills = [{ type: "SOLID", color: { r: 0.15, g: 0.04, b: 0.04 } }];
  v2Portrait.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.50, b: 0.15 } }];
  v2Portrait.strokeWeight = 2.5;
  v2Portrait.cornerRadius = 16; // Rounded diamond look
  v2Frame.appendChild(v2Portrait);

  const v2Emblem = figma.createText();
  v2Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v2Emblem.characters = "🔥";
  v2Emblem.fontSize = 40;
  v2Emblem.x = 22;
  v2Emblem.y = 16;
  v2Portrait.appendChild(v2Emblem);

  const v2Name = figma.createText();
  v2Name.fontName = { family: "Cinzel", style: "Bold" };
  v2Name.characters = "IGNIS • THE HELLKITE";
  v2Name.fontSize = 17;
  v2Name.fills = [{ type: "SOLID", color: { r: 1.0, g: 0.85, b: 0.65 } }];
  v2Name.x = 116;
  v2Name.y = 20;
  v2Frame.appendChild(v2Name);

  const v2RuneBadge = figma.createText();
  v2RuneBadge.fontName = { family: "Cinzel", style: "Regular" };
  v2RuneBadge.characters = "᚛ PRIMORDIAL DRAKE ᚜";
  v2RuneBadge.fontSize = 10;
  v2RuneBadge.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.40, b: 0.15 } }];
  v2RuneBadge.x = 118;
  v2RuneBadge.y = 42;
  v2Frame.appendChild(v2RuneBadge);

  // Angular Slanted HP Bar
  const v2HpChannel = figma.createFrame();
  v2HpChannel.resize(332, 28);
  v2HpChannel.x = 116;
  v2HpChannel.y = 62;
  v2HpChannel.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.05, b: 0.07 } }];
  v2HpChannel.strokes = [{ type: "SOLID", color: { r: 0.40, g: 0.20, b: 0.15 } }];
  v2HpChannel.strokeWeight = 1.5;
  v2HpChannel.cornerRadius = 6;
  v2HpChannel.clipsContent = true;
  v2Frame.appendChild(v2HpChannel);

  const v2HpFill = figma.createRectangle();
  v2HpFill.resize(250, 28);
  v2HpFill.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.32, b: 0.12 } }]; // Molten Lava
  v2HpChannel.appendChild(v2HpFill);

  const v2HpText = figma.createText();
  v2HpText.fontName = { family: "Inter", style: "Bold" };
  v2HpText.characters = "160 / 200 HP";
  v2HpText.fontSize = 12;
  v2HpText.fills = [{ type: "SOLID", color: { r: 1.0, g: 0.95, b: 0.90 } }];
  v2HpText.x = 125;
  v2HpText.y = 6;
  v2HpChannel.appendChild(v2HpText);

  // Micro Ability Ready Nodes below HP bar
  const nodeLabels = ["Q: READY", "W: READY", "E: 4.2s"];
  const nodeColors = [{ r: 0.2, g: 0.8, b: 0.4 }, { r: 0.2, g: 0.8, b: 0.4 }, { r: 0.8, g: 0.4, b: 0.1 }];
  for (let i = 0; i < 3; i++) {
    const node = figma.createFrame();
    node.resize(102, 18);
    node.x = 116 + i * 115;
    node.y = 98;
    node.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.14 } }];
    node.strokes = [{ type: "SOLID", color: nodeColors[i] }];
    node.strokeWeight = 1;
    node.cornerRadius = 3;
    v2Frame.appendChild(node);

    const nTxt = figma.createText();
    nTxt.fontName = { family: "Inter", style: "Bold" };
    nTxt.characters = nodeLabels[i];
    nTxt.fontSize = 9;
    nTxt.fills = [{ type: "SOLID", color: nodeColors[i] }];
    nTxt.x = 18;
    nTxt.y = 3;
    node.appendChild(nTxt);
  }

  // =========================================================================
  // VARIANT 3: The Aegis Crest (Imperial Tournament / Underlords Style)
  // =========================================================================
  const v3Group = figma.createFrame();
  v3Group.name = "Variant 3: Aegis Crest (Imperial Tournament / Underlords Style)";
  v3Group.resize(520, 940);
  v3Group.x = 1180;
  v3Group.y = 150;
  v3Group.fills = [{ type: "SOLID", color: { r: 0.07, g: 0.09, b: 0.12 } }];
  v3Group.strokes = [{ type: "SOLID", color: { r: 0.18, g: 0.22, b: 0.28 } }];
  v3Group.strokeWeight = 1;
  v3Group.cornerRadius = 12;
  board.appendChild(v3Group);

  const v3Label = figma.createText();
  v3Label.fontName = { family: "Cinzel", style: "Bold" };
  v3Label.characters = "VARIANT C : AEGIS CREST";
  v3Label.fontSize = 18;
  v3Label.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.85, b: 0.35 } }]; // Gold Imperial
  v3Label.x = 24;
  v3Label.y = 24;
  v3Group.appendChild(v3Label);

  const v3Desc = figma.createText();
  v3Desc.fontName = { family: "Inter", style: "Regular" };
  v3Desc.characters = "Dota Underlords / TI Championship tournament frame: circular golden medallion, 3-star rating crest, polished wing filigree, and compact streamlined profile.";
  v3Desc.fontSize = 12;
  v3Desc.fills = [{ type: "SOLID", color: { r: 0.6, g: 0.65, b: 0.75 } }];
  v3Desc.resize(472, 40);
  v3Desc.textAutoResize = "NONE";
  v3Desc.x = 24;
  v3Desc.y = 54;
  v3Group.appendChild(v3Desc);

  // V3 Frame Component
  const v3Frame = figma.createFrame();
  v3Frame.name = "StatusFrame_VariantC";
  v3Frame.resize(472, 140);
  v3Frame.x = 24;
  v3Frame.y = 110;
  v3Frame.fills = [{ type: "SOLID", color: { r: 0.09, g: 0.11, b: 0.15 } }];
  v3Frame.strokes = [{ type: "SOLID", color: { r: 0.85, g: 0.72, b: 0.32 } }]; // Polished Tournament Gold
  v3Frame.strokeWeight = 2;
  v3Frame.cornerRadius = 12;
  v3Group.appendChild(v3Frame);

  // Circular Medallion Portrait
  const v3Portrait = figma.createFrame();
  v3Portrait.resize(84, 84);
  v3Portrait.x = 18;
  v3Portrait.y = 18;
  v3Portrait.fills = [{ type: "SOLID", color: { r: 0.12, g: 0.06, b: 0.06 } }];
  v3Portrait.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.82, b: 0.35 } }];
  v3Portrait.strokeWeight = 3;
  v3Portrait.cornerRadius = 42; // Perfect Circle
  v3Frame.appendChild(v3Portrait);

  const v3Emblem = figma.createText();
  v3Emblem.fontName = { family: "Cinzel", style: "Bold" };
  v3Emblem.characters = "👑";
  v3Emblem.fontSize = 38;
  v3Emblem.x = 22;
  v3Emblem.y = 18;
  v3Portrait.appendChild(v3Emblem);

  // 3-Star Rating
  const v3Stars = figma.createText();
  v3Stars.fontName = { family: "Inter", style: "Bold" };
  v3Stars.characters = "★★★";
  v3Stars.fontSize = 12;
  v3Stars.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.85, b: 0.35 } }];
  v3Stars.x = 42;
  v3Stars.y = 96;
  v3Frame.appendChild(v3Stars);

  const v3Name = figma.createText();
  v3Name.fontName = { family: "Cinzel", style: "Bold" };
  v3Name.characters = "ARCH-DRAKE OF RADIANCE";
  v3Name.fontSize = 16;
  v3Name.fills = [{ type: "SOLID", color: { r: 0.98, g: 0.92, b: 0.78 } }];
  v3Name.x = 118;
  v3Name.y = 20;
  v3Frame.appendChild(v3Name);

  const v3TeamRibbon = figma.createText();
  v3TeamRibbon.fontName = { family: "Inter", style: "Bold" };
  v3TeamRibbon.characters = "TI CHAMPIONSHIP • TIER 3";
  v3TeamRibbon.fontSize = 10;
  v3TeamRibbon.fills = [{ type: "SOLID", color: { r: 0.90, g: 0.75, b: 0.35 } }];
  v3TeamRibbon.x = 118;
  v3TeamRibbon.y = 42;
  v3Frame.appendChild(v3TeamRibbon);

  const v3HpChannel = figma.createFrame();
  v3HpChannel.resize(330, 26);
  v3HpChannel.x = 118;
  v3HpChannel.y = 62;
  v3HpChannel.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.07, b: 0.09 } }];
  v3HpChannel.strokes = [{ type: "SOLID", color: { r: 0.55, g: 0.48, b: 0.25 } }];
  v3HpChannel.strokeWeight = 1.5;
  v3HpChannel.cornerRadius = 6;
  v3HpChannel.clipsContent = true;
  v3Frame.appendChild(v3HpChannel);

  const v3HpFill = figma.createRectangle();
  v3HpFill.resize(265, 26);
  v3HpFill.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.75, b: 0.42 } }]; // Crisp Emerald
  v3HpChannel.appendChild(v3HpFill);

  const v3HpText = figma.createText();
  v3HpText.fontName = { family: "Inter", style: "Bold" };
  v3HpText.characters = "165 / 200 HP (82%)";
  v3HpText.fontSize = 12;
  v3HpText.fills = [{ type: "SOLID", color: { r: 1.0, g: 1.0, b: 1.0 } }];
  v3HpText.x = 110;
  v3HpText.y = 5;
  v3HpChannel.appendChild(v3HpText);

  // Status badges: ENRAGED / SHIELDED
  const v3StatusPill = figma.createFrame();
  v3StatusPill.resize(110, 18);
  v3StatusPill.x = 118;
  v3StatusPill.y = 96;
  v3StatusPill.fills = [{ type: "SOLID", color: { r: 0.5, g: 0.15, b: 0.15 } }];
  v3StatusPill.cornerRadius = 9;
  v3Frame.appendChild(v3StatusPill);

  const v3StatusText = figma.createText();
  v3StatusText.fontName = { family: "Inter", style: "Bold" };
  v3StatusText.characters = "⚔️ ENRAGED +25%";
  v3StatusText.fontSize = 9;
  v3StatusText.fills = [{ type: "SOLID", color: { r: 1.0, g: 0.8, b: 0.8 } }];
  v3StatusText.x = 12;
  v3StatusText.y = 3;
  v3StatusPill.appendChild(v3StatusText);

  // Select board and zoom to fit
  figma.currentPage.selection = [board];
  figma.viewport.scrollAndZoomIntoView([board]);

  return {
    success: true,
    boardId: board.id,
    boardName: board.name,
    variantIds: [v1Frame.id, v2Frame.id, v3Frame.id]
  };
}

return await createDota2StatusFrames();
"""

res = exec_figma(figma_script)
print(res)
