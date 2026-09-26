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

setup_slices_code = """
async function setupSlices() {
  await figma.loadFontAsync({ family: "Cinzel", style: "Bold" });
  await figma.loadFontAsync({ family: "Inter", style: "Bold" });

  // Dedicated Export Canvas
  const exportBoard = figma.createFrame();
  exportBoard.name = "UI_Components_Export_Dota2";
  exportBoard.resize(1400, 800);
  exportBoard.x = 5800;
  exportBoard.y = 0;
  exportBoard.fills = [{ type: "SOLID", color: { r: 0.05, g: 0.06, b: 0.08 } }];

  const slices = {};

  // 1. Player Status Frame Chassis (Empty of dynamic text & blocks, ready for Unity UI)
  const playerFrame = figma.createFrame();
  playerFrame.name = "Frame_DragonBastion_Player";
  playerFrame.resize(502, 190);
  playerFrame.x = 40;
  playerFrame.y = 40;
  playerFrame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
  playerFrame.strokes = [{ type: "SOLID", color: { r: 0.55, g: 0.44, b: 0.22 } }];
  playerFrame.strokeWeight = 2;
  playerFrame.cornerRadius = 6;
  exportBoard.appendChild(playerFrame);

  // Filigree corners on player frame
  const filigreePath = [
    { x: 4, y: 4, path: "M 0 16 L 0 0 L 16 0 L 8 4 L 4 8 L 4 16 Z" },
    { x: 482, y: 4, path: "M 16 16 L 16 0 L 0 0 L 8 4 L 12 8 L 12 16 Z" },
    { x: 4, y: 170, path: "M 0 0 L 0 16 L 16 16 L 8 12 L 4 8 L 4 0 Z" },
    { x: 482, y: 170, path: "M 16 0 L 16 16 L 0 16 L 8 12 L 12 8 L 12 0 Z" }
  ];
  filigreePath.forEach(c => {
    const v = figma.createVector();
    v.vectorPaths = [{ windingRule: "NONZERO", data: c.path }];
    v.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.70, b: 0.35 } }];
    v.x = c.x;
    v.y = c.y;
    playerFrame.appendChild(v);
  });
  slices["Frame_DragonBastion_Player"] = playerFrame.id;

  // 2. AI Status Frame Chassis (Mirrored Frost Blue)
  const aiFrame = figma.createFrame();
  aiFrame.name = "Frame_DragonBastion_AI";
  aiFrame.resize(502, 190);
  aiFrame.x = 580;
  aiFrame.y = 40;
  aiFrame.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
  aiFrame.strokes = [{ type: "SOLID", color: { r: 0.28, g: 0.60, b: 0.90 } }];
  aiFrame.strokeWeight = 2;
  aiFrame.cornerRadius = 6;
  exportBoard.appendChild(aiFrame);

  filigreePath.forEach(c => {
    const v = figma.createVector();
    v.vectorPaths = [{ windingRule: "NONZERO", data: c.path }];
    v.fills = [{ type: "SOLID", color: { r: 0.45, g: 0.75, b: 0.98 } }];
    v.x = c.x;
    v.y = c.y;
    aiFrame.appendChild(v);
  });
  slices["Frame_DragonBastion_AI"] = aiFrame.id;

  // 3. Gothic Notched Portrait Chassis - Player Gold (112 x 124)
  const portPlayer = figma.createFrame();
  portPlayer.name = "Chassis_Portrait_Player";
  portPlayer.resize(112, 126);
  portPlayer.x = 40;
  portPlayer.y = 260;
  portPlayer.fills = []; // Transparent background for clean alpha cutout
  exportBoard.appendChild(portPlayer);

  const vChassisPath = "M 18 0 L 86 0 L 104 18 L 104 78 L 86 104 L 52 114 L 18 104 L 0 78 L 0 18 Z";
  const pOuter = figma.createVector();
  pOuter.vectorPaths = [{ windingRule: "NONZERO", data: vChassisPath }];
  pOuter.fills = [{ type: "SOLID", color: { r: 0.14, g: 0.17, b: 0.22 } }];
  pOuter.strokes = [{ type: "SOLID", color: { r: 0.88, g: 0.72, b: 0.32 } }];
  pOuter.strokeWeight = 2.5;
  pOuter.x = 4;
  pOuter.y = 10;
  portPlayer.appendChild(pOuter);

  const pInner = figma.createVector();
  pInner.vectorPaths = [{ windingRule: "NONZERO", data: "M 15 0 L 71 0 L 86 15 L 86 65 L 71 86 L 43 94 L 15 86 L 0 65 L 0 15 Z" }];
  pInner.fills = [{ type: "SOLID", color: { r: 0.20, g: 0.05, b: 0.05 } }];
  pInner.strokes = [{ type: "SOLID", color: { r: 0.45, g: 0.15, b: 0.15 } }];
  pInner.strokeWeight = 1.5;
  pInner.x = 13;
  pInner.y = 19;
  portPlayer.appendChild(pInner);

  const pHorns = figma.createVector();
  pHorns.vectorPaths = [{ windingRule: "NONZERO", data: "M 0 12 Q 26 -6 52 12 Q 78 -6 104 12 L 94 18 Q 52 6 10 18 Z" }];
  pHorns.fills = [{ type: "SOLID", color: { r: 0.95, g: 0.80, b: 0.38 } }];
  pHorns.x = 4;
  pHorns.y = 0;
  portPlayer.appendChild(pHorns);
  slices["Chassis_Portrait_Player"] = portPlayer.id;

  // 4. Gothic Notched Portrait Chassis - AI Frost Blue
  const portAI = figma.createFrame();
  portAI.name = "Chassis_Portrait_AI";
  portAI.resize(112, 126);
  portAI.x = 180;
  portAI.y = 260;
  portAI.fills = [];
  exportBoard.appendChild(portAI);

  const aiOuter = figma.createVector();
  aiOuter.vectorPaths = [{ windingRule: "NONZERO", data: vChassisPath }];
  aiOuter.fills = [{ type: "SOLID", color: { r: 0.06, g: 0.14, b: 0.22 } }];
  aiOuter.strokes = [{ type: "SOLID", color: { r: 0.45, g: 0.75, b: 0.98 } }];
  aiOuter.strokeWeight = 2.5;
  aiOuter.x = 4;
  aiOuter.y = 10;
  portAI.appendChild(aiOuter);

  const aiInner = figma.createVector();
  aiInner.vectorPaths = [{ windingRule: "NONZERO", data: "M 15 0 L 71 0 L 86 15 L 86 65 L 71 86 L 43 94 L 15 86 L 0 65 L 0 15 Z" }];
  aiInner.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.10, b: 0.18 } }];
  aiInner.strokes = [{ type: "SOLID", color: { r: 0.20, g: 0.45, b: 0.70 } }];
  aiInner.strokeWeight = 1.5;
  aiInner.x = 13;
  aiInner.y = 19;
  portAI.appendChild(aiInner);

  const aiHorns = figma.createVector();
  aiHorns.vectorPaths = [{ windingRule: "NONZERO", data: "M 0 12 Q 26 -6 52 12 Q 78 -6 104 12 L 94 18 Q 52 6 10 18 Z" }];
  aiHorns.fills = [{ type: "SOLID", color: { r: 0.60, g: 0.85, b: 1.0 } }];
  aiHorns.x = 4;
  aiHorns.y = 0;
  portAI.appendChild(aiHorns);
  slices["Chassis_Portrait_AI"] = portAI.id;

  // 5. Level Shield Badge
  const badge = figma.createFrame();
  badge.name = "Badge_LevelShield";
  badge.resize(36, 22);
  badge.x = 320;
  badge.y = 260;
  badge.fills = [{ type: "SOLID", color: { r: 0.10, g: 0.12, b: 0.16 } }];
  badge.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.80, b: 0.35 } }];
  badge.strokeWeight = 2;
  badge.cornerRadius = 4;
  exportBoard.appendChild(badge);
  slices["Badge_LevelShield"] = badge.id;

  // 6. Health Bar Cradle Trough
  const hpCradle = figma.createFrame();
  hpCradle.name = "Health_Channel_Cradle";
  hpCradle.resize(344, 28);
  hpCradle.x = 40;
  hpCradle.y = 420;
  hpCradle.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.05, b: 0.07 } }];
  hpCradle.strokes = [{ type: "SOLID", color: { r: 0.32, g: 0.38, b: 0.48 } }];
  hpCradle.strokeWeight = 1.5;
  hpCradle.cornerRadius = 4;
  exportBoard.appendChild(hpCradle);
  slices["Health_Channel_Cradle"] = hpCradle.id;

  // 7. Health Armor Blocks (Emerald Full, Crimson Enemy, Amber Decay, Empty Grate)
  const cellEmerald = figma.createFrame();
  cellEmerald.name = "Health_Block_Emerald";
  cellEmerald.resize(38.5, 20);
  cellEmerald.x = 400;
  cellEmerald.y = 420;
  cellEmerald.fills = [{ type: "SOLID", color: { r: 0.16, g: 0.74, b: 0.40 } }];
  cellEmerald.strokes = [{ type: "SOLID", color: { r: 0.28, g: 0.88, b: 0.50 } }];
  cellEmerald.strokeWeight = 1;
  cellEmerald.cornerRadius = 2;
  exportBoard.appendChild(cellEmerald);
  slices["Health_Block_Emerald"] = cellEmerald.id;

  const cellCrimson = figma.createFrame();
  cellCrimson.name = "Health_Block_Crimson";
  cellCrimson.resize(38.5, 20);
  cellCrimson.x = 460;
  cellCrimson.y = 420;
  cellCrimson.fills = [{ type: "SOLID", color: { r: 0.85, g: 0.24, b: 0.24 } }];
  cellCrimson.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.45, b: 0.45 } }];
  cellCrimson.strokeWeight = 1;
  cellCrimson.cornerRadius = 2;
  exportBoard.appendChild(cellCrimson);
  slices["Health_Block_Crimson"] = cellCrimson.id;

  const cellAmber = figma.createFrame();
  cellAmber.name = "Health_Block_GhostAmber";
  cellAmber.resize(38.5, 20);
  cellAmber.x = 520;
  cellAmber.y = 420;
  cellAmber.fills = [{ type: "SOLID", color: { r: 0.88, g: 0.58, b: 0.15 } }];
  cellAmber.strokes = [{ type: "SOLID", color: { r: 0.98, g: 0.72, b: 0.25 } }];
  cellAmber.strokeWeight = 1;
  cellAmber.cornerRadius = 2;
  exportBoard.appendChild(cellAmber);
  slices["Health_Block_GhostAmber"] = cellAmber.id;

  const cellEmpty = figma.createFrame();
  cellEmpty.name = "Health_Block_Depleted";
  cellEmpty.resize(38.5, 20);
  cellEmpty.x = 580;
  cellEmpty.y = 420;
  cellEmpty.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.10, b: 0.13 } }];
  cellEmpty.strokes = [{ type: "SOLID", color: { r: 0.14, g: 0.17, b: 0.22 } }];
  cellEmpty.strokeWeight = 1;
  cellEmpty.cornerRadius = 2;
  exportBoard.appendChild(cellEmpty);
  slices["Health_Block_Depleted"] = cellEmpty.id;

  // 8. Notched Octagonal Talisman Sockets (Ready Green, Cooldown Amber, Ready Frost)
  const talismanPath = "M 8 0 L 102 0 L 110 8 L 110 34 L 102 42 L 8 42 L 0 34 L 0 8 Z";

  const talGreen = figma.createFrame();
  talGreen.name = "Talisman_Socket_Ready_Green";
  talGreen.resize(110, 42);
  talGreen.x = 40;
  talGreen.y = 500;
  talGreen.fills = [];
  exportBoard.appendChild(talGreen);

  const tgVec = figma.createVector();
  tgVec.vectorPaths = [{ windingRule: "NONZERO", data: talismanPath }];
  tgVec.fills = [{ type: "SOLID", color: { r: 0.08, g: 0.12, b: 0.10 } }];
  tgVec.strokes = [{ type: "SOLID", color: { r: 0.25, g: 0.88, b: 0.48 } }];
  tgVec.strokeWeight = 2;
  talGreen.appendChild(tgVec);
  slices["Talisman_Socket_Ready_Green"] = talGreen.id;

  const talAmber = figma.createFrame();
  talAmber.name = "Talisman_Socket_Cooldown_Amber";
  talAmber.resize(110, 42);
  talAmber.x = 180;
  talAmber.y = 500;
  talAmber.fills = [];
  exportBoard.appendChild(talAmber);

  const taVec = figma.createVector();
  taVec.vectorPaths = [{ windingRule: "NONZERO", data: talismanPath }];
  taVec.fills = [{ type: "SOLID", color: { r: 0.12, g: 0.09, b: 0.07 } }];
  taVec.strokes = [{ type: "SOLID", color: { r: 0.96, g: 0.65, b: 0.22 } }];
  taVec.strokeWeight = 2;
  talAmber.appendChild(taVec);
  slices["Talisman_Socket_Cooldown_Amber"] = talAmber.id;

  const talFrost = figma.createFrame();
  talFrost.name = "Talisman_Socket_Ready_Frost";
  talFrost.resize(110, 42);
  talFrost.x = 320;
  talFrost.y = 500;
  talFrost.fills = [];
  exportBoard.appendChild(talFrost);

  const tfVec = figma.createVector();
  tfVec.vectorPaths = [{ windingRule: "NONZERO", data: talismanPath }];
  tfVec.fills = [{ type: "SOLID", color: { r: 0.06, g: 0.10, b: 0.16 } }];
  tfVec.strokes = [{ type: "SOLID", color: { r: 0.40, g: 0.80, b: 1.0 } }];
  tfVec.strokeWeight = 2;
  talFrost.appendChild(tfVec);
  slices["Talisman_Socket_Ready_Frost"] = talFrost.id;

  // 9. Diamond Hotkey Jewel
  const dJewel = figma.createFrame();
  dJewel.name = "Jewel_Hotkey_Diamond";
  dJewel.resize(20, 20);
  dJewel.x = 460;
  dJewel.y = 500;
  dJewel.fills = [];
  exportBoard.appendChild(dJewel);

  const djVec = figma.createVector();
  djVec.vectorPaths = [{ windingRule: "NONZERO", data: "M 10 0 L 20 10 L 10 20 L 0 10 Z" }];
  djVec.fills = [{ type: "SOLID", color: { r: 0.14, g: 0.18, b: 0.24 } }];
  djVec.strokes = [{ type: "SOLID", color: { r: 0.95, g: 0.80, b: 0.35 } }];
  djVec.strokeWeight = 1.5;
  dJewel.appendChild(djVec);
  slices["Jewel_Hotkey_Diamond"] = dJewel.id;

  // 10. Dragon Portrait Icons (Inferno & Frost)
  const portInferno = figma.createFrame();
  portInferno.name = "Icon_Dragon_Player";
  portInferno.resize(80, 80);
  portInferno.x = 520;
  portInferno.y = 500;
  portInferno.fills = [{ type: "SOLID", color: { r: 0.18, g: 0.05, b: 0.05 } }];
  portInferno.cornerRadius = 8;
  exportBoard.appendChild(portInferno);

  const pDrgTxt = figma.createText();
  pDrgTxt.fontName = { family: "Cinzel", style: "Bold" };
  pDrgTxt.characters = "🐉";
  pDrgTxt.fontSize = 44;
  pDrgTxt.x = 18;
  pDrgTxt.y = 18;
  portInferno.appendChild(pDrgTxt);
  slices["Icon_Dragon_Player"] = portInferno.id;

  const portFrost = figma.createFrame();
  portFrost.name = "Icon_Dragon_AI";
  portFrost.resize(80, 80);
  portFrost.x = 620;
  portFrost.y = 500;
  portFrost.fills = [{ type: "SOLID", color: { r: 0.04, g: 0.12, b: 0.22 } }];
  portFrost.cornerRadius = 8;
  exportBoard.appendChild(portFrost);

  const pFstTxt = figma.createText();
  pFstTxt.fontName = { family: "Cinzel", style: "Bold" };
  pFstTxt.characters = "❄️";
  pFstTxt.fontSize = 44;
  pFstTxt.x = 18;
  pFstTxt.y = 18;
  portFrost.appendChild(pFstTxt);
  slices["Icon_Dragon_AI"] = portFrost.id;

  // 11. Ability Icons (Fire Breath, Tail Whip, Sky Dive)
  const abilitiesArt = [
    { name: "Icon_Ability_FireBreath", icon: "🔥", bg: { r: 0.25, g: 0.08, b: 0.04 } },
    { name: "Icon_Ability_TailWhip", icon: "💫", bg: { r: 0.20, g: 0.14, b: 0.05 } },
    { name: "Icon_Ability_FlyDive", icon: "⚡", bg: { r: 0.08, g: 0.16, b: 0.25 } }
  ];

  for (let a = 0; a < 3; a++) {
    const abData = abilitiesArt[a];
    const abF = figma.createFrame();
    abF.name = abData.name;
    abF.resize(64, 64);
    abF.x = 740 + a * 80;
    abF.y = 500;
    abF.fills = [{ type: "SOLID", color: abData.bg }];
    abF.strokes = [{ type: "SOLID", color: { r: 0.85, g: 0.70, b: 0.35 } }];
    abF.strokeWeight = 2;
    abF.cornerRadius = 8;
    exportBoard.appendChild(abF);

    const txt = figma.createText();
    txt.fontName = { family: "Cinzel", style: "Bold" };
    txt.characters = abData.icon;
    txt.fontSize = 32;
    txt.x = 16;
    txt.y = 14;
    abF.appendChild(txt);
    slices[abData.name] = abF.id;
  }

  return { success: true, boardId: exportBoard.id, slices };
}

return await setupSlices();
"""

res = exec_figma(setup_slices_code)
print(res)

if res.get("success") and "result" in res:
    slices = res["result"].get("slices", {})
    out_dir = "/home/subodh/Projects/Unity Projects/DexHigh Task/Assets/_Project/Art/UI/StatusFrame"
    os.makedirs(out_dir, exist_ok=True)
    
    print(f"Exporting {len(slices)} components to {out_dir}...")
    for name, node_id in slices.items():
        export_code = f"""
        async function exp() {{
          const node = await figma.getNodeByIdAsync("{node_id}");
          if (!node) return {{ error: "Node not found" }};
          const bytes = await node.exportAsync({{
            format: "PNG",
            constraint: {{ type: "SCALE", value: 2 }}
          }});
          let binary = "";
          for (let i = 0; i < bytes.byteLength; i++) {{
            binary += String.fromCharCode(bytes[i]);
          }}
          return {{ base64: btoa(binary) }};
        }}
        return await exp();
        """
        exp_res = exec_figma(export_code)
        if "result" in exp_res and "base64" in exp_res["result"]:
            img_bytes = base64.b64decode(exp_res["result"]["base64"])
            file_path = os.path.join(out_dir, f"{name}.png")
            with open(file_path, "wb") as f:
                f.write(img_bytes)
            print(f"  ✓ Exported {name}.png ({len(img_bytes)} bytes)")
        else:
            print(f"  ✗ Failed to export {name}:", exp_res)
    print("Export complete!")
