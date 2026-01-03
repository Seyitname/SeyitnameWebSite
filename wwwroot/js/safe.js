// Minimal safe JS — do NOT interfere with default event handling
// Only provides small helpers and safeguards for the app

(function() {
    'use strict';

    // No-op helper to safely attach listeners without overriding existing handlers
    window.safeOn = function(selector, event, handler) {
        const el = document.querySelector(selector);
        if (el) el.addEventListener(event, handler);
    };

    // Keep console errors visible during debug
    window.safeLog = function(...args) { if (window.console && console.log) console.log(...args); };

    // Ensure focus outlines are not removed by other styles
    document.addEventListener('DOMContentLoaded', function() {
        document.body.classList.remove('no-focus-outline');
    });
})();
