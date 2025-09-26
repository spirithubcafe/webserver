// SpirithubCofe JavaScript Utilities
window.spirithubcofe = window.spirithubcofe || {};

// Add error handling for Blazor
window.addEventListener('DOMContentLoaded', function() {
    // Blazor error handler
    Blazor.defaultReconnectionHandler._reconnectCallback = function(d) {
        console.log('Blazor reconnecting...');
        return Blazor.defaultReconnectionHandler._currentReconnectionProcess = 
            Blazor.defaultReconnectionHandler._currentReconnectionProcess || 
            Blazor.defaultReconnectionHandler._reconnectCallback.call(this, d);
    };
});

// Utility functions for better DOM handling
window.spirithubcofe.domUtils = {
    safeRemove: function(element) {
        try {
            if (element && element.parentNode) {
                element.parentNode.removeChild(element);
                return true;
            }
        } catch (e) {
            console.warn('Safe remove failed:', e);
        }
        return false;
    },

    safeQuery: function(selector) {
        try {
            return document.querySelector(selector);
        } catch (e) {
            console.warn('Safe query failed:', e);
            return null;
        }
    },

    safeQueryAll: function(selector) {
        try {
            return document.querySelectorAll(selector);
        } catch (e) {
            console.warn('Safe query all failed:', e);
            return [];
        }
    }
};

// Handle global errors
window.addEventListener('error', function(e) {
    if (e.message && e.message.includes('removeChild')) {
        console.warn('DOM removeChild error caught and handled:', e.message);
        return true; // Prevent default error handling
    }
});

window.addEventListener('unhandledrejection', function(e) {
    if (e.reason && e.reason.message && e.reason.message.includes('removeChild')) {
        console.warn('Unhandled promise rejection for removeChild caught and handled:', e.reason.message);
        e.preventDefault(); // Prevent default handling
    }
});