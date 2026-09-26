import urllib.request
import json
import base64
import os
import time

OUT_DIR = "/home/subodh/Projects/Unity Projects/DexHigh Task/Assets/_Project/Art/UI/VictoryScreen"
os.makedirs(OUT_DIR, exist_ok=True)

def exec_figma(code):
    req = urllib.request.Request(
        "http://localhost:8766/exec",
        data=json.dumps({"code": code}).encode("utf-8"),
        headers={"Content-Type": "application/json"}
    )
    with urllib.request.urlopen(req, timeout=45) as resp:
        return json.loads(resp.read().decode("utf-8"))

setup_slices_code = """
// 1. Remove previous export board if it exists
for (const child of figma.currentPage.children) {
    if (child.name === "Victory_Components_Export") {
        child.remove();
    }
}

const solid = (r, g, b, a = 1) => [{ type: 'SOLID', color: { r: r / 255, g: g / 255, b: b / 255 }, opacity: a }];

const exportBoard = figma.createFrame();
exportBoard.name = "Victory_Components_Export";
exportBoard.x = 7400;
exportBoard.y = 1200;
exportBoard.resize(1800, 900);
exportBoard.fills = solid(12, 14, 18);
figma.currentPage.appendChild(exportBoard);

// 1. Victory_Modal_Chassis (640 x 680)
const modal = figma.createFrame();
modal.name = "Victory_Modal_Chassis";
modal.x = 40;
modal.y = 40;
modal.resize(640, 680);
modal.fills = solid(16, 19, 26, 0.96);
modal.cornerRadius = 14;
modal.strokes = solid(160, 125, 45, 0.7);
modal.strokeWeight = 1.5;
exportBoard.appendChild(modal);

const topTrim = figma.createFrame();
topTrim.x = 0;
topTrim.y = 0;
topTrim.resize(640, 3);
topTrim.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[1, 0, 0], [0, 1, 0]],
    gradientStops: [
        { position: 0, color: { r: 0.3, g: 0.25, b: 0.1, a: 0 } },
        { position: 0.5, color: { r: 1, g: 0.8, b: 0.25, a: 1 } },
        { position: 1, color: { r: 0.3, g: 0.25, b: 0.1, a: 0 } }
    ]
}];
modal.appendChild(topTrim);

// 2. Victory_Crest_Horns (60 x 26)
const crest = figma.createVector();
crest.name = "Victory_Crest_Horns";
crest.x = 720;
crest.y = 40;
crest.vectorPaths = [{
    windingRule: 'NONZERO',
    data: 'M 0 23 L 30 0 L 60 23 L 50 26 L 30 12 L 10 26 Z'
}];
crest.fills = solid(210, 165, 55);
exportBoard.appendChild(crest);

// 3. Victory_Portrait_Chassis (110 x 110)
const chassis = figma.createFrame();
chassis.name = "Victory_Portrait_Chassis";
chassis.x = 720;
chassis.y = 100;
chassis.resize(110, 110);
chassis.fills = solid(24, 28, 38);
chassis.cornerRadius = 24;
chassis.strokes = solid(210, 165, 55);
chassis.strokeWeight = 2.5;
exportBoard.appendChild(chassis);

// 4. Victory_Stat_Strip_Chassis (520 x 95)
const statStrip = figma.createFrame();
statStrip.name = "Victory_Stat_Strip_Chassis";
statStrip.x = 720;
statStrip.y = 250;
statStrip.resize(520, 95);
statStrip.fills = solid(20, 24, 32);
statStrip.cornerRadius = 8;
statStrip.strokes = solid(45, 52, 68);
statStrip.strokeWeight = 1;
exportBoard.appendChild(statStrip);

// Add the two vertical dividers
const vDiv1 = figma.createLine();
vDiv1.rotation = 90;
vDiv1.x = 173;
vDiv1.y = 15;
vDiv1.resize(65, 0);
vDiv1.strokes = solid(50, 58, 75);
vDiv1.strokeWeight = 1;
statStrip.appendChild(vDiv1);

const vDiv2 = figma.createLine();
vDiv2.rotation = 90;
vDiv2.x = 346;
vDiv2.y = 15;
vDiv2.resize(65, 0);
vDiv2.strokes = solid(50, 58, 75);
vDiv2.strokeWeight = 1;
statStrip.appendChild(vDiv2);

// 5. Victory_Btn_Rematch (280 x 56)
const btnRematch = figma.createFrame();
btnRematch.name = "Victory_Btn_Rematch";
btnRematch.x = 720;
btnRematch.y = 380;
btnRematch.resize(280, 56);
btnRematch.cornerRadius = 8;
btnRematch.fills = [{
    type: 'GRADIENT_LINEAR',
    gradientTransform: [[0, 1, 0], [-1, 0, 1]],
    gradientStops: [
        { position: 0, color: { r: 0.95, g: 0.75, b: 0.25, a: 1 } },
        { position: 0.5, color: { r: 0.85, g: 0.55, b: 0.1, a: 1 } },
        { position: 1, color: { r: 0.65, g: 0.35, b: 0.05, a: 1 } }
    ]
}];
btnRematch.strokes = solid(255, 225, 140);
btnRematch.strokeWeight = 1.5;
exportBoard.appendChild(btnRematch);

// 6. Victory_Btn_Leave (200 x 38)
const btnLeave = figma.createFrame();
btnLeave.name = "Victory_Btn_Leave";
btnLeave.x = 720;
btnLeave.y = 460;
btnLeave.resize(200, 38);
btnLeave.cornerRadius = 6;
btnLeave.fills = solid(22, 26, 35);
btnLeave.strokes = solid(60, 70, 88);
btnLeave.strokeWeight = 1;
exportBoard.appendChild(btnLeave);

return {
    boardId: exportBoard.id,
    slices: {
        "Victory_Modal_Chassis": modal.id,
        "Victory_Crest_Horns": crest.id,
        "Victory_Portrait_Chassis": chassis.id,
        "Victory_Stat_Strip_Chassis": statStrip.id,
        "Victory_Btn_Rematch": btnRematch.id,
        "Victory_Btn_Leave": btnLeave.id
    }
};
"""

print("Creating export slice board in Figma...")
setup_res = exec_figma(setup_slices_code)
print(f"Setup result: {setup_res}")

slices = setup_res.get("result", {}).get("slices", {})
print(f"Exporting {len(slices)} components to {OUT_DIR}...")

for name, node_id in slices.items():
    file_path = os.path.join(OUT_DIR, f"{name}.png")
    export_code = f"""
    const node = await figma.getNodeByIdAsync('{node_id}');
    if (!node) return {{ error: 'Node not found' }};
    const bytes = await node.exportAsync({{ format: 'PNG', constraint: {{ type: 'SCALE', value: 2 }} }});
    let binary = '';
    const len = bytes.byteLength;
    for (let i = 0; i < len; i++) {{
        binary += String.fromCharCode(bytes[i]);
    }}
    return {{ success: true, base64: btoa(binary) }};
    """
    for attempt in range(3):
        try:
            res = exec_figma(export_code)
            if res.get("success") and res.get("result", {}).get("success"):
                b64 = res["result"]["base64"]
                data = base64.b64decode(b64)
                with open(file_path, "wb") as f:
                    f.write(data)
                print(f"  ✓ Exported {name}.png ({len(data)} bytes)")
                break
        except Exception as e:
            print(f"  Attempt {attempt+1} failed for {name}: {e}")
            time.sleep(1)

print("Export complete.")
