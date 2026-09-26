import urllib.request
import json
import base64
import os

ARTIFACT_PATH = "/home/subodh/.gemini/antigravity-cli/brain/c5488275-621a-4183-8c8b-c55127a653d3/dota2_victory_screens_simplistic.png"

def exec_figma(code):
    req = urllib.request.Request(
        "http://localhost:8766/exec",
        data=json.dumps({"code": code}).encode("utf-8"),
        headers={"Content-Type": "application/json"}
    )
    with urllib.request.urlopen(req, timeout=45) as resp:
        return json.loads(resp.read().decode("utf-8"))

js_code = """
// 1. Load fonts
await figma.loadFontAsync({ family: "Cinzel", style: "Bold" });
await figma.loadFontAsync({ family: "Cinzel", style: "Black" });
await figma.loadFontAsync({ family: "Cinzel", style: "Regular" });
await figma.loadFontAsync({ family: "Cinzel Decorative", style: "Bold" });
await figma.loadFontAsync({ family: "Cinzel Decorative", style: "Black" });
await figma.loadFontAsync({ family: "Inter", style: "Regular" });
await figma.loadFontAsync({ family: "Inter", style: "Medium" });
await figma.loadFontAsync({ family: "Inter", style: "Semi Bold" });
await figma.loadFontAsync({ family: "Inter", style: "Bold" });

// 2. Remove previous board if exists
for (const child of figma.currentPage.children) {
    if (child.name === "Dota 2 Victory Screen - 2 Simplistic Variants" || 
        child.name === "Dota 2 Victory Screen - 2 Monumental Variants") {
        child.remove();
    }
}

const solid = (r, g, b, a = 1) => [{ type: 'SOLID', color: { r: r / 255, g: g / 255, b: b / 255 }, opacity: a }];

// Master Board
const master = figma.createFrame();
master.name = "Dota 2 Victory Screen - 2 Simplistic Variants";
master.x = 7400;
master.y = 0;
master.resize(2440, 1160);
master.fills = solid(10, 12, 16);
figma.currentPage.appendChild(master);

// --- BOARD HEADER ---
const catTag = figma.createText();
catTag.fontName = { family: "Inter", style: "Bold" };
catTag.fontSize = 11;
catTag.letterSpacing = { value: 3, unit: 'PIXELS' };
catTag.characters = "UI / UX PRODUCTION — DOTA 2 VISUAL IDENTITY";
catTag.fills = solid(218, 165, 32);
catTag.x = 80;
catTag.y = 45;
master.appendChild(catTag);

const boardTitle = figma.createText();
boardTitle.fontName = { family: "Cinzel Decorative", style: "Bold" };
boardTitle.fontSize = 30;
boardTitle.characters = "SIMPLISTIC BATTLE VICTORY SCREENS (2 VARIANTS)";
boardTitle.fills = solid(245, 235, 220);
boardTitle.x = 80;
boardTitle.y = 65;
master.appendChild(boardTitle);

const boardSub = figma.createText();
boardSub.fontName = { family: "Inter", style: "Regular" };
boardSub.fontSize = 14;
boardSub.characters = "Minimalist, high-focus presentations: essential match feedback, zero clutter, bold Dota 2 typography, and sleek instant rematch actions.";
boardSub.fills = solid(145, 155, 170);
boardSub.x = 80;
boardSub.y = 110;
master.appendChild(boardSub);

// Grab existing vector dragon icon from export board
const pIconSrc = await figma.getNodeByIdAsync("43:471"); // Icon_Dragon_Player

// =========================================================================
// VARIANT 1: "THE SOVEREIGN EMBLEM" (Clean Minimalist Modal)
// =========================================================================
const previewA = figma.createFrame();
previewA.name = "Variant 1: The Sovereign Emblem (Clean Modal)";
previewA.x = 80;
previewA.y = 160;
previewA.resize(1100, 920);
previewA.fills = solid(13, 15, 20);
previewA.cornerRadius = 16;
previewA.strokes = solid(45, 52, 68);
previewA.strokeWeight = 1.5;
master.appendChild(previewA);

// Arena background vignette glow
const bgGlowA = figma.createEllipse();
bgGlowA.resize(600, 400);
bgGlowA.x = 250;
bgGlowA.y = 150;
bgGlowA.fills = [{
    type: 'GRADIENT_RADIAL',
    gradientTransform: [[0.5, 0, 0.5], [0, 0.5, 0.5]],
    gradientStops: [
        { position: 0, color: { r: 1, g: 0.55, b: 0.1, a: 0.18 } },
        { position: 1, color: { r: 0, g: 0, b: 0, a: 0 } }
    ]
}];
previewA.appendChild(bgGlowA);

// Tag
const tagA = figma.createFrame();
tagA.x = 40;
tagA.y = 30;
tagA.resize(260, 30);
tagA.fills = solid(22, 26, 36);
tagA.cornerRadius = 6;
tagA.strokes = solid(180, 135, 45);
tagA.strokeWeight = 1;
previewA.appendChild(tagA);

const tagAText = figma.createText();
tagAText.fontName = { family: "Inter", style: "Bold" };
tagAText.fontSize = 11;
tagAText.letterSpacing = { value: 1.5, unit: 'PIXELS' };
tagAText.characters = "VARIANT 1 : MINIMAL MODAL";
tagAText.fills = solid(240, 190, 60);
tagAText.x = 12;
tagAText.y = 8;
tagA.appendChild(tagAText);

// --- THE MODAL CARD (Clean 640 x 660) ---
const modalA = figma.createFrame();
modalA.name = "Modal_Sovereign_Emblem";
modalA.x = 230;
modalA.y = 130;
modalA.resize(640, 680);
modalA.fills = solid(16, 19, 26, 0.95);
modalA.cornerRadius = 14;
modalA.strokes = solid(160, 125, 45, 0.6);
modalA.strokeWeight = 1.5;
modalA.effects = [{
    type: 'DROP_SHADOW',
    color: { r: 0, g: 0, b: 0, a: 0.65 },
    offset: { x: 0, y: 16 },
    radius: 36,
    visible: true,
    blendMode: 'NORMAL'
}];
previewA.appendChild(modalA);

// Top subtle gold accent line
const topBarA = figma.createFrame();
topBarA.x = 0;
topBarA.y = 0;
topBarA.resize(640, 3);
topBarA.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[1, 0, 0], [0, 1, 0]],
    gradientStops: [
        { position: 0, color: { r: 0.3, g: 0.25, b: 0.1, a: 0 } },
        { position: 0.5, color: { r: 1, g: 0.8, b: 0.25, a: 1 } },
        { position: 1, color: { r: 0.3, g: 0.25, b: 0.1, a: 0 } }
    ]
}];
modalA.appendChild(topBarA);

// Top Crest Horn Icon (Minimalist SVG)
const crestA = figma.createVector();
crestA.name = "Minimal_Crest";
crestA.vectorPaths = [{
    windingRule: 'NONZERO',
    data: 'M 290 55 L 320 32 L 350 55 L 340 58 L 320 44 L 300 58 Z'
}];
crestA.fills = solid(210, 165, 55);
modalA.appendChild(crestA);

// Grand Clean Title: VICTORY
const titleA = figma.createText();
titleA.name = "Title_VICTORY";
titleA.fontName = { family: "Cinzel Decorative", style: "Bold" };
titleA.fontSize = 54;
titleA.letterSpacing = { value: 6, unit: 'PIXELS' };
titleA.characters = "VICTORY";
titleA.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[0, 1, 0], [-1, 0, 1]],
    gradientStops: [
        { position: 0, color: { r: 1, g: 0.95, b: 0.75, a: 1 } },
        { position: 0.5, color: { r: 0.95, g: 0.72, b: 0.2, a: 1 } },
        { position: 1, color: { r: 0.75, g: 0.42, b: 0.08, a: 1 } }
    ]
}];
titleA.x = 180;
titleA.y = 65;
titleA.effects = [{
    type: 'DROP_SHADOW',
    color: { r: 1, g: 0.5, b: 0, a: 0.4 },
    offset: { x: 0, y: 0 },
    radius: 14,
    visible: true,
    blendMode: 'NORMAL'
}];
modalA.appendChild(titleA);

// Clean Subtitle
const subA = figma.createText();
subA.name = "Subtitle";
subA.fontName = { family: "Cinzel", style: "Bold" };
subA.fontSize = 13;
subA.letterSpacing = { value: 3, unit: 'PIXELS' };
subA.characters = "INFERNO WYRM ASCENDANT";
subA.fills = solid(215, 185, 130);
subA.x = 210;
subA.y = 135;
modalA.appendChild(subA);

// Dragon Champion Circle/Octagon Frame
const champRing = figma.createFrame();
champRing.name = "Champion_Emblem";
champRing.x = 265;
champRing.y = 175;
champRing.resize(110, 110);
champRing.fills = solid(24, 28, 38);
champRing.cornerRadius = 24;
champRing.strokes = solid(210, 165, 55);
champRing.strokeWeight = 2.5;
modalA.appendChild(champRing);

if (pIconSrc) {
    const clone = pIconSrc.clone();
    clone.x = 12;
    clone.y = 12;
    clone.resize(86, 86);
    champRing.appendChild(clone);
}

// Opponent note
const oppNote = figma.createText();
oppNote.fontName = { family: "Inter", style: "Regular" };
oppNote.fontSize = 12;
oppNote.characters = "Frost Wyrm Vanquished in Arena Combat";
oppNote.fills = solid(145, 155, 170);
oppNote.x = 195;
oppNote.y = 305;
modalA.appendChild(oppNote);

// Divider line
const divLineA = figma.createLine();
divLineA.x = 60;
divLineA.y = 340;
divLineA.resize(520, 0);
divLineA.strokes = solid(50, 58, 75, 0.7);
divLineA.strokeWeight = 1;
modalA.appendChild(divLineA);

// --- CONCISE 3-COLUMN STAT STRIP ---
const statStrip = figma.createFrame();
statStrip.name = "Stat_Strip";
statStrip.x = 60;
statStrip.y = 365;
statStrip.resize(520, 95);
statStrip.fills = solid(20, 24, 32);
statStrip.cornerRadius = 8;
statStrip.strokes = solid(45, 52, 68);
statStrip.strokeWeight = 1;
modalA.appendChild(statStrip);

const statsA = [
    { label: "BATTLE TIME", val: "01:42", color: [245, 205, 80] },
    { label: "TOTAL DAMAGE", val: "1,450", color: [255, 95, 45] },
    { label: "SURVIVING HP", val: "75%", color: [60, 200, 110] }
];

statsA.forEach((st, idx) => {
    const col = figma.createFrame();
    col.x = idx * 173;
    col.y = 0;
    col.resize(173, 95);
    col.fills = solid(0, 0, 0, 0);
    statStrip.appendChild(col);

    if (idx > 0) {
        const vSep = figma.createLine();
        vSep.rotation = 90;
        vSep.x = 0;
        vSep.y = 15;
        vSep.resize(65, 0);
        vSep.strokes = solid(50, 58, 75);
        vSep.strokeWeight = 1;
        col.appendChild(vSep);
    }

    const lbl = figma.createText();
    lbl.fontName = { family: "Inter", style: "Bold" };
    lbl.fontSize = 10;
    lbl.letterSpacing = { value: 1.5, unit: 'PIXELS' };
    lbl.characters = st.label;
    lbl.fills = solid(135, 145, 160);
    lbl.x = 35;
    lbl.y = 22;
    col.appendChild(lbl);

    const v = figma.createText();
    v.fontName = { family: "Cinzel", style: "Bold" };
    v.fontSize = 24;
    v.characters = st.val;
    v.fills = solid(st.color[0], st.color[1], st.color[2]);
    v.x = 45;
    v.y = 46;
    col.appendChild(v);
});

// --- MINIMAL BUTTONS ---
// Primary: REMATCH
const btnRematchA = figma.createFrame();
btnRematchA.name = "Btn_Rematch";
btnRematchA.x = 180;
btnRematchA.y = 500;
btnRematchA.resize(280, 56);
btnRematchA.cornerRadius = 8;
btnRematchA.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[0, 1, 0], [-1, 0, 1]],
    gradientStops: [
        { position: 0, color: { r: 0.95, g: 0.75, b: 0.25, a: 1 } },
        { position: 0.5, color: { r: 0.85, g: 0.55, b: 0.1, a: 1 } },
        { position: 1, color: { r: 0.65, g: 0.35, b: 0.05, a: 1 } }
    ]
}];
btnRematchA.strokes = solid(255, 225, 140);
btnRematchA.strokeWeight = 1.5;
btnRematchA.effects = [{
    type: 'DROP_SHADOW',
    color: { r: 1, g: 0.45, b: 0, a: 0.4 },
    offset: { x: 0, y: 4 },
    radius: 14,
    visible: true,
    blendMode: 'NORMAL'
}];
modalA.appendChild(btnRematchA);

const btnTextA = figma.createText();
btnTextA.fontName = { family: "Cinzel", style: "Black" };
btnTextA.fontSize = 17;
btnTextA.letterSpacing = { value: 2.5, unit: 'PIXELS' };
btnTextA.characters = "REMATCH";
btnTextA.fills = solid(24, 12, 4);
btnTextA.x = 88;
btnTextA.y = 18;
btnRematchA.appendChild(btnTextA);

// Secondary: LEAVE ARENA
const btnLeaveA = figma.createFrame();
btnLeaveA.name = "Btn_Leave";
btnLeaveA.x = 220;
btnLeaveA.y = 575;
btnLeaveA.resize(200, 38);
btnLeaveA.cornerRadius = 6;
btnLeaveA.fills = solid(22, 26, 35);
btnLeaveA.strokes = solid(60, 70, 88);
btnLeaveA.strokeWeight = 1;
modalA.appendChild(btnLeaveA);

const btnLeaveTextA = figma.createText();
btnLeaveTextA.fontName = { family: "Cinzel", style: "Bold" };
btnLeaveTextA.fontSize = 12;
btnLeaveTextA.letterSpacing = { value: 2, unit: 'PIXELS' };
btnLeaveTextA.characters = "LEAVE ARENA";
btnLeaveTextA.fills = solid(150, 160, 175);
btnLeaveTextA.x = 42;
btnLeaveTextA.y = 11;
btnLeaveA.appendChild(btnLeaveTextA);

// =========================================================================
// VARIANT 2: "THE CINEMATIC BANNER" (Minimalist Horizontal Ribbon)
// =========================================================================
const previewB = figma.createFrame();
previewB.name = "Variant 2: The Cinematic Banner (Horizontal Ribbon)";
previewB.x = 1240;
previewB.y = 160;
previewB.resize(1120, 920);
previewB.fills = solid(13, 15, 20);
previewB.cornerRadius = 16;
previewB.strokes = solid(45, 52, 68);
previewB.strokeWeight = 1.5;
master.appendChild(previewB);

// Subtle radial aura
const bgGlowB = figma.createEllipse();
bgGlowB.resize(700, 350);
bgGlowB.x = 210;
bgGlowB.y = 280;
bgGlowB.fills = [{
    type: 'GRADIENT_RADIAL',
    gradientTransform: [[0.5, 0, 0.5], [0, 0.5, 0.5]],
    gradientStops: [
        { position: 0, color: { r: 1, g: 0.6, b: 0.15, a: 0.15 } },
        { position: 1, color: { r: 0, g: 0, b: 0, a: 0 } }
    ]
}];
previewB.appendChild(bgGlowB);

// Tag
const tagB = figma.createFrame();
tagB.x = 40;
tagB.y = 30;
tagB.resize(275, 30);
tagB.fills = solid(22, 26, 36);
tagB.cornerRadius = 6;
tagB.strokes = solid(80, 150, 220);
tagB.strokeWeight = 1;
previewB.appendChild(tagB);

const tagBText = figma.createText();
tagBText.fontName = { family: "Inter", style: "Bold" };
tagBText.fontSize = 11;
tagBText.letterSpacing = { value: 1.5, unit: 'PIXELS' };
tagBText.characters = "VARIANT 2 : CINEMATIC RIBBON";
tagBText.fills = solid(120, 190, 255);
tagBText.x = 12;
tagBText.y = 8;
tagB.appendChild(tagBText);

// Context Screen Mockup Header (Simulating In-Game View)
const mockNote = figma.createText();
mockNote.fontName = { family: "Inter", style: "Regular" };
mockNote.fontSize = 12;
mockNote.characters = "Cinematic letterbox overlay across active combat arena view";
mockNote.fills = solid(120, 130, 145);
mockNote.x = 380;
mockNote.y = 230;
previewB.appendChild(mockNote);

// --- THE HORIZONTAL CINEMATIC RIBBON (1040 x 300) ---
const ribbonB = figma.createFrame();
ribbonB.name = "Cinematic_Ribbon_Bar";
ribbonB.x = 40;
ribbonB.y = 270;
ribbonB.resize(1040, 320);
ribbonB.fills = solid(15, 18, 24, 0.95);
ribbonB.cornerRadius = 10;
ribbonB.strokes = solid(160, 125, 45, 0.7);
ribbonB.strokeWeight = 1.5;
ribbonB.effects = [{
    type: 'DROP_SHADOW',
    color: { r: 0, g: 0, b: 0, a: 0.7 },
    offset: { x: 0, y: 12 },
    radius: 30,
    visible: true,
    blendMode: 'NORMAL'
}];
previewB.appendChild(ribbonB);

// Top & Bottom Gold Accent Hairpins
const topTrim = figma.createFrame();
topTrim.x = 0;
topTrim.y = 0;
topTrim.resize(1040, 2);
topTrim.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[1, 0, 0], [0, 1, 0]],
    gradientStops: [
        { position: 0, color: { r: 0.3, g: 0.25, b: 0.1, a: 0 } },
        { position: 0.5, color: { r: 1, g: 0.8, b: 0.25, a: 1 } },
        { position: 1, color: { r: 0.3, g: 0.25, b: 0.1, a: 0 } }
    ]
}];
ribbonB.appendChild(topTrim);

const botTrim = figma.createFrame();
botTrim.x = 0;
botTrim.y = 318;
botTrim.resize(1040, 2);
botTrim.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[1, 0, 0], [0, 1, 0]],
    gradientStops: [
        { position: 0, color: { r: 0.3, g: 0.25, b: 0.1, a: 0 } },
        { position: 0.5, color: { r: 1, g: 0.8, b: 0.25, a: 1 } },
        { position: 1, color: { r: 0.3, g: 0.25, b: 0.1, a: 0 } }
    ]
}];
ribbonB.appendChild(botTrim);

// SECTION 1: LEFT - DRAGON EMBLEM & IDENTITY
const leftSec = figma.createFrame();
leftSec.x = 40;
leftSec.y = 40;
leftSec.resize(280, 240);
leftSec.fills = solid(0, 0, 0, 0);
ribbonB.appendChild(leftSec);

const leftChassis = figma.createFrame();
leftChassis.x = 0;
leftChassis.y = 20;
leftChassis.resize(96, 96);
leftChassis.fills = solid(24, 28, 38);
leftChassis.cornerRadius = 20;
leftChassis.strokes = solid(210, 165, 55);
leftChassis.strokeWeight = 2;
leftSec.appendChild(leftChassis);

if (pIconSrc) {
    const cloneB = pIconSrc.clone();
    cloneB.x = 10;
    cloneB.y = 10;
    cloneB.resize(76, 76);
    leftChassis.appendChild(cloneB);
}

const leftTag = figma.createFrame();
leftTag.x = 112;
leftTag.y = 30;
leftTag.resize(110, 22);
leftTag.fills = solid(180, 130, 30);
leftTag.cornerRadius = 4;
leftSec.appendChild(leftTag);

const leftTagTxt = figma.createText();
leftTagTxt.fontName = { family: "Inter", style: "Bold" };
leftTagTxt.fontSize = 10;
leftTagTxt.letterSpacing = { value: 1.5, unit: 'PIXELS' };
leftTagTxt.characters = "CHAMPION";
leftTagTxt.fills = solid(255, 245, 220);
leftTagTxt.x = 16;
leftTagTxt.y = 5;
leftTag.appendChild(leftTagTxt);

const leftName = figma.createText();
leftName.fontName = { family: "Cinzel", style: "Bold" };
leftName.fontSize = 17;
leftName.characters = "INFERNO WYRM";
leftName.fills = solid(255, 215, 110);
leftName.x = 112;
leftName.y = 60;
leftSec.appendChild(leftName);

const leftHp = figma.createText();
leftHp.fontName = { family: "Inter", style: "Regular" };
leftHp.fontSize = 12;
leftHp.characters = "150 / 200 HP (75%)";
leftHp.fills = solid(70, 205, 120);
leftHp.x = 112;
leftHp.y = 90;
leftSec.appendChild(leftHp);

// Vertical Divider 1
const vDiv1 = figma.createLine();
vDiv1.rotation = 90;
vDiv1.x = 330;
vDiv1.y = 40;
vDiv1.resize(240, 0);
vDiv1.strokes = solid(50, 58, 75);
vDiv1.strokeWeight = 1;
ribbonB.appendChild(vDiv1);

// SECTION 2: CENTER - MONUMENTAL VICTORY & TIME
const centerSec = figma.createFrame();
centerSec.x = 345;
centerSec.y = 40;
centerSec.resize(390, 240);
centerSec.fills = solid(0, 0, 0, 0);
ribbonB.appendChild(centerSec);

const vicBTitle = figma.createText();
vicBTitle.name = "Title_VICTORY";
vicBTitle.fontName = { family: "Cinzel Decorative", style: "Black" };
vicBTitle.fontSize = 50;
vicBTitle.letterSpacing = { value: 6, unit: 'PIXELS' };
vicBTitle.characters = "VICTORY";
vicBTitle.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[0, 1, 0], [-1, 0, 1]],
    gradientStops: [
        { position: 0, color: { r: 1, g: 0.95, b: 0.75, a: 1 } },
        { position: 0.5, color: { r: 0.95, g: 0.72, b: 0.2, a: 1 } },
        { position: 1, color: { r: 0.75, g: 0.42, b: 0.08, a: 1 } }
    ]
}];
vicBTitle.x = 55;
vicBTitle.y = 35;
vicBTitle.effects = [{
    type: 'DROP_SHADOW',
    color: { r: 1, g: 0.5, b: 0, a: 0.35 },
    offset: { x: 0, y: 0 },
    radius: 14,
    visible: true,
    blendMode: 'NORMAL'
}];
centerSec.appendChild(vicBTitle);

const centerSub = figma.createText();
centerSub.fontName = { family: "Cinzel", style: "Bold" };
centerSub.fontSize = 12;
centerSub.letterSpacing = { value: 3, unit: 'PIXELS' };
centerSub.characters = "FROST WYRM VANQUISHED";
centerSub.fills = solid(210, 180, 130);
centerSub.x = 90;
centerSub.y = 110;
centerSec.appendChild(centerSub);

const centerTime = figma.createText();
centerTime.fontName = { family: "Inter", style: "Regular" };
centerTime.fontSize = 12;
centerTime.characters = "Battle Duration: 01:42  •  Total Damage: 1,450";
centerTime.fills = solid(150, 160, 175);
centerTime.x = 75;
centerTime.y = 138;
centerSec.appendChild(centerTime);

// Vertical Divider 2
const vDiv2 = figma.createLine();
vDiv2.rotation = 90;
vDiv2.x = 750;
vDiv2.y = 40;
vDiv2.resize(240, 0);
vDiv2.strokes = solid(50, 58, 75);
vDiv2.strokeWeight = 1;
ribbonB.appendChild(vDiv2);

// SECTION 3: RIGHT - INSTANT ACTION BUTTONS
const rightSec = figma.createFrame();
rightSec.x = 780;
rightSec.y = 40;
rightSec.resize(220, 240);
rightSec.fills = solid(0, 0, 0, 0);
ribbonB.appendChild(rightSec);

// Rematch Button
const btnPlayAgain = figma.createFrame();
btnPlayAgain.name = "Btn_PlayAgain";
btnPlayAgain.x = 0;
btnPlayAgain.y = 50;
btnPlayAgain.resize(220, 52);
btnPlayAgain.cornerRadius = 8;
btnPlayAgain.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[0, 1, 0], [-1, 0, 1]],
    gradientStops: [
        { position: 0, color: { r: 0.95, g: 0.75, b: 0.25, a: 1 } },
        { position: 0.5, color: { r: 0.85, g: 0.55, b: 0.1, a: 1 } },
        { position: 1, color: { r: 0.65, g: 0.35, b: 0.05, a: 1 } }
    ]
}];
btnPlayAgain.strokes = solid(255, 225, 140);
btnPlayAgain.strokeWeight = 1.5;
btnPlayAgain.effects = [{
    type: 'DROP_SHADOW',
    color: { r: 1, g: 0.45, b: 0, a: 0.35 },
    offset: { x: 0, y: 3 },
    radius: 12,
    visible: true,
    blendMode: 'NORMAL'
}];
rightSec.appendChild(btnPlayAgain);

const btnPATxt = figma.createText();
btnPATxt.fontName = { family: "Cinzel", style: "Black" };
btnPATxt.fontSize = 15;
btnPATxt.letterSpacing = { value: 2, unit: 'PIXELS' };
btnPATxt.characters = "PLAY AGAIN";
btnPATxt.fills = solid(24, 12, 4);
btnPATxt.x = 48;
btnPATxt.y = 16;
btnPlayAgain.appendChild(btnPATxt);

// Exit Button
const btnExit = figma.createFrame();
btnExit.name = "Btn_Exit";
btnExit.x = 0;
btnExit.y = 120;
btnExit.resize(220, 42);
btnExit.cornerRadius = 6;
btnExit.fills = solid(22, 26, 35);
btnExit.strokes = solid(60, 70, 88);
btnExit.strokeWeight = 1;
rightSec.appendChild(btnExit);

const btnExitTxt = figma.createText();
btnExitTxt.fontName = { family: "Cinzel", style: "Bold" };
btnExitTxt.fontSize = 12;
btnExitTxt.letterSpacing = { value: 2, unit: 'PIXELS' };
btnExitTxt.characters = "EXIT TO MENU";
btnExitTxt.fills = solid(150, 160, 175);
btnExitTxt.x = 52;
btnExitTxt.y = 13;
btnExit.appendChild(btnExitTxt);

return {
    boardId: master.id,
    previewAId: previewA.id,
    previewBId: previewB.id
};
"""

print("Executing Figma generation for 2 Simplistic Victory Screen variants...")
res = exec_figma(js_code)
print(f"Generation result: {res}")

if res.get("success") and res.get("result", {}).get("boardId"):
    board_id = res["result"]["boardId"]
    print(f"Exporting board {board_id} to artifact image...")
    
    export_code = f"""
    const node = await figma.getNodeByIdAsync('{board_id}');
    if (!node) return {{ error: 'Node not found' }};
    const bytes = await node.exportAsync({{ format: 'PNG', constraint: {{ type: 'SCALE', value: 1 }} }});
    let binary = '';
    const len = bytes.byteLength;
    for (let i = 0; i < len; i++) {{
        binary += String.fromCharCode(bytes[i]);
    }}
    return {{ success: true, base64: btoa(binary) }};
    """
    exp_res = exec_figma(export_code)
    if exp_res.get("success") and exp_res.get("result", {}).get("success"):
        b64 = exp_res["result"]["base64"]
        data = base64.b64decode(b64)
        with open(ARTIFACT_PATH, "wb") as f:
            f.write(data)
        print(f"Saved artifact preview to {ARTIFACT_PATH} ({len(data)} bytes)")
    else:
        print(f"Export error: {exp_res}")
else:
    print("Failed to generate victory screen board")
