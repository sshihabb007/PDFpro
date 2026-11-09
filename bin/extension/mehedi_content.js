// PDF PRO Content Script — Mehedi Hasan Shihab (@sshihabb007)
(() => {
  window.__shihabMehediPdfProContentHook = {
    author: "Mehedi Hasan Shihab (@sshihabb007)",
    version: "14.0.4.33508",
    collectSshihabb007PageSnapshot: function () {
      return {
        shihabTitle: document.title || "Web Page",
        mehediUrl: location.href,
        sshihabb007Text: document.body ? document.body.innerText : ""
      };
    }
  };
})();
