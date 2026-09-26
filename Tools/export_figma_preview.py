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

export_script = """
async function exportBoard() {
  const node = await figma.getNodeByIdAsync("43:68");
  if (!node) return { error: "Node 43:68 not found" };
  const bytes = await node.exportAsync({
    format: "PNG",
    constraint: { type: "SCALE", value: 1 }
  });
  
  // Convert Uint8Array to base64 string
  let binary = "";
  const len = bytes.byteLength;
  for (let i = 0; i < len; i++) {
    binary += String.fromCharCode(bytes[i]);
  }
  return { base64: btoa(binary) };
}
return await exportBoard();
"""

res = exec_figma(export_script)
if "result" in res and "base64" in res["result"]:
    img_data = base64.b64decode(res["result"]["base64"])
    out_dir = "/home/subodh/.gemini/antigravity-cli/brain/c5488275-621a-4183-8c8b-c55127a653d3"
    out_path = os.path.join(out_dir, "dota2_dragon_status_variants.png")
    with open(out_path, "wb") as f:
        f.write(img_data)
    print(f"Exported preview to {out_path} ({len(img_data)} bytes)")
else:
    print("Export failed:", res)
