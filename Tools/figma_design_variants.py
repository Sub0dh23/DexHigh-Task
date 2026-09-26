import urllib.request
import json

def exec_figma(code: str):
    url = "http://localhost:8766/exec"
    payload = json.dumps({"code": code}).encode("utf-8")
    req = urllib.request.Request(url, data=payload, headers={"Content-Type": "application/json"})
    with urllib.request.urlopen(req) as resp:
        return json.loads(resp.read().decode("utf-8"))

test_code = """
try {
  await figma.loadFontAsync({ family: "Inter", style: "Regular" });
  await figma.loadFontAsync({ family: "Inter", style: "Bold" });
  return { status: "Inter loaded ok" };
} catch(e) {
  return { error: e.message };
}
"""

res = exec_figma(test_code)
print(res)
