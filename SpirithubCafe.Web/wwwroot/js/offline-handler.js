// Offline Handler - DISABLED
// User requested no offline page display when server disconnects
// The page will just remain as-is like normal websites

console.log('Offline Handler: Disabled by user preference');

// Empty object to prevent errors
window.offlineHandler = {
    initialize: function() {},
    checkConnection: function() {}
};
