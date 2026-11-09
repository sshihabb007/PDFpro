// PDF PRO v14.0.4 MV3 Service Worker — Engineered by Mehedi Hasan Shihab (@sshihabb007)
const SHIHAB_PDFPRO_BRIDGE_URL = "http://127.0.0.1:45880";

chrome.runtime.onInstalled.addListener(() => {
  chrome.contextMenus.create({
    id: "shihab_convert_page_to_pdfpro",
    title: "Convert Page to PDF PRO (@sshihabb007)",
    contexts: ["page", "selection"]
  });
});

async function sendMehediPageToPdfProDesktop(shihabTitle, mehediUrl, sshihabb007TextBody) {
  const shihabPayload = [
    "PDF PRO WEB CAPTURE — Engineered by Mehedi Hasan Shihab (@sshihabb007)",
    "Source Title : " + (shihabTitle || "Web Page"),
    "Source URL   : " + (mehediUrl || ""),
    "Captured UTC : " + new Date().toISOString(),
    "--------------------------------------------------------------------------------",
    sshihabb007TextBody || ""
  ].join("\n");

  const mehediResp = await fetch(SHIHAB_PDFPRO_BRIDGE_URL + "/convert-web", {
    method: "POST",
    headers: { "Content-Type": "text/plain;charset=UTF-8" },
    body: shihabPayload
  });
  return await mehediResp.json();
}

chrome.contextMenus.onClicked.addListener(async (shihabInfo, mehediTab) => {
  if (shihabInfo.menuItemId === "shihab_convert_page_to_pdfpro" && mehediTab && mehediTab.id) {
    try {
      const sshihabb007Results = await chrome.scripting.executeScript({
        target: { tabId: mehediTab.id },
        func: () => document.body ? document.body.innerText : ""
      });
      const shihabExtracted = (sshihabb007Results && sshihabb007Results[0]) ? sshihabb007Results[0].result : "";
      await sendMehediPageToPdfProDesktop(mehediTab.title, mehediTab.url, shihabExtracted);
    } catch (mehediErr) {
      console.warn("Shihab PDF PRO Bridge error:", mehediErr);
    }
  }
});

chrome.runtime.onMessage.addListener((shihabMsg, mehediSender, sshihabb007SendResponse) => {
  if (shihabMsg && shihabMsg.shihabAction === "CONVERT_ACTIVE_TAB_SSH") {
    sendMehediPageToPdfProDesktop(shihabMsg.mehediTitle, shihabMsg.sshihabb007Url, shihabMsg.shihabBody)
      .then((mehediRes) => sshihabb007SendResponse({ ok: true, result: mehediRes }))
      .catch((shihabErr) => sshihabb007SendResponse({ ok: false, error: String(shihabErr) }));
    return true;
  }
  return false;
});

