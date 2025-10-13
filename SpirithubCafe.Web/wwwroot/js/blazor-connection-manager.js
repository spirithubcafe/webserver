// Blazor Connection Manager - Silent Mode
// Suppresses WebSocket errors and warnings - gracefully uses Long Polling fallback

(function() {
    'use strict';
    
    // Store original console methods
    const originalConsole = {
        warn: console.warn,
        error: console.error,
        info: console.info,
        log: console.log
    };
    
    // Keywords to filter
    const suppressKeywords = [
        'WebSocket',
        'websocket', 
        'transport',
        'Long Polling',
        'SignalR',
        'fallback',
        '_blazor',
        'connection could not be found',
        'sticky sessions',
        'proxy blocking'
    ];
    
    // Check if message should be suppressed
    function shouldSuppress(message) {
        if (typeof message !== 'string') {
            message = String(message);
        }
        return suppressKeywords.some(keyword => message.includes(keyword));
    }
    
    // Override console methods
    console.warn = function(...args) {
        const message = args.join(' ');
        if (!shouldSuppress(message)) {
            originalConsole.warn.apply(console, args);
        }
    };
    
    console.error = function(...args) {
        const message = args.join(' ');
        if (!shouldSuppress(message)) {
            originalConsole.error.apply(console, args);
        }
    };
    
    console.info = function(...args) {
        const message = args.join(' ');
        if (!shouldSuppress(message)) {
            originalConsole.info.apply(console, args);
        }
    };
    
    console.log = function(...args) {
        const message = args.join(' ');
        if (!shouldSuppress(message)) {
            originalConsole.log.apply(console, args);
        }
    };
    
    // Expose minimal API
    window.blazorConnectionManager = {
        initialize: function() {},
        restore: function() {
            // Restore original console methods if needed
            console.warn = originalConsole.warn;
            console.error = originalConsole.error;
            console.info = originalConsole.info;
            console.log = originalConsole.log;
        }
    };
    
})();
