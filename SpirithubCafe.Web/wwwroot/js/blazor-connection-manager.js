// Blazor Connection Manager - DISABLED
// User requested no offline page display when server disconnects
// The connection will just fail silently like normal websites

console.log('Blazor Connection Manager: Offline UI disabled by user preference');

// Empty object to prevent errors if something tries to call it
window.blazorConnectionManager = {
    initialize: function() {},
    setupBlazorCallbacks: function() {},
    showConnectionStatus: function() {},
    hideConnectionStatus: function() {}
};
