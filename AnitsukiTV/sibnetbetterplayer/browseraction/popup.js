!function(){"use strict";$(function(){var e=$("#switch"),t=$("#version"),s=!1;$("[data-i18n]").each(function(){var e=$(this),t=e.text();e.html(chrome.i18n.getMessage(t))}),chrome.storage.sync.get(function(t){t.enabled&&(e.addClass("active"),s=!0)}),e.click(function(){s?(s=!1,e.removeClass("active"),chrome.storage.sync.set({enabled:!1})):(s=!0,e.addClass("active"),chrome.storage.sync.set({enabled:!0}))}),t.text("v"+chrome.runtime.getManifest().version)})}();