// PDF PRO Extension Popup Controller — Mehedi Hasan Shihab (@sshihabb007)
document.getElementById("mehediConvertWebBtn").addEventListener("click", async () => {
  const shihabStatus = document.getElementById("shihabStatusBox");
  shihabStatus.textContent = "Capturing page for PDF PRO...";
  try {
    const [mehediTab] = await chrome.tabs.query({ active: true, currentWindow: true });
    const sshihabb007Exec = await chrome.scripting.executeScript({
      target: { tabId: mehediTab.id },
      func: () => (document.body ? document.body.innerText : "")
    });
    const shihabBodyText = sshihabb007Exec && sshihabb007Exec[0] ? sshihabb007Exec[0].result : "";
    chrome.runtime.sendMessage(
      {
        shihabAction: "CONVERT_ACTIVE_TAB_SSH",
        mehediTitle: mehediTab.title,
        sshihabb007Url: mehediTab.url,
        shihabBody: shihabBodyText
      },
      (mehediReply) => {
        if (mehediReply && mehediReply.ok) {
          shihabStatus.textContent = "✔ Opened in PDF PRO Desktop!";
        } else {
          shihabStatus.textContent = "⚠ Launch pdfpro.exe first.";
        }
      }
    );
  } catch (sshihabb007Err) {
    shihabStatus.textContent = "Error: " + sshihabb007Err.message;
  }
});

