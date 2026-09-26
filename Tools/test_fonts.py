import urllib.request
import json

def exec_figma(code: str):
    url = "http://localhost:8766/exec"
    payload = json.dumps({"code": code}).encode("utf-8")
    req = urllib.request.Request(url, data=payload, headers={"Content-Type": "application/json"})
    with urllib.request.urlopen(req) as resp:
        return json.loads(resp.read().decode("utf-8"))

test_fonts = """
const candidates = ["Cinzel", "Marcellus", "Trajan", "Cinzel Decorative", "Times New Roman", "Georgia", "Inter"];
const available = [];
for (const f of candidates) {
  try {
    await figma.loadFontAsync({ family: f, style: "Regular" });
    available.push(f);
  } catch(e) {}
}
return { available };
"""

res = exec_figma(test_fonts)
print(res)
