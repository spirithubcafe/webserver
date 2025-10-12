// Offline Detection and Handling System
window.offlineHandler = {
    isOnline: navigator.onLine,
    offlinePageShown: false,
    originalContent: null,
    retryInterval: null,
    
    initialize: function() {
        this.setupEventListeners();
        this.interceptNetworkRequests();
        this.checkInitialConnection();
        this.setupBlazorErrorHandling();
    },
    
    setupEventListeners: function() {
        const self = this;
        
        // Browser online/offline events
        window.addEventListener('online', () => {
            console.log('Browser detected online');
            self.isOnline = true;
            self.handleOnline();
        });
        
        window.addEventListener('offline', () => {
            console.log('Browser detected offline');
            self.isOnline = false;
            self.handleOffline();
        });
        
        // Handle page visibility changes
        document.addEventListener('visibilitychange', () => {
            if (!document.hidden && self.offlinePageShown) {
                self.checkConnection();
            }
        });
    },
    
    setupBlazorErrorHandling: function() {
        const self = this;
        
        // Override console errors to catch Blazor connection errors
        const originalError = console.error;
        console.error = function(...args) {
            const message = args.join(' ');
            
            // Check for Blazor connection errors
            if (message.includes('ERR_CONNECTION_REFUSED') || 
                message.includes('Failed to fetch') ||
                message.includes('_blazor') ||
                message.includes('blazor.server.js')) {
                console.log('Blazor connection error detected, switching to offline mode');
                self.handleOffline();
                return; // Don't show the error
            }
            
            originalError.apply(console, args);
        };
        
        // Handle unhandled promise rejections (like fetch errors)
        window.addEventListener('unhandledrejection', (event) => {
            const error = event.reason;
            if (error && (
                error.message?.includes('Failed to fetch') ||
                error.message?.includes('ERR_CONNECTION_REFUSED') ||
                error.toString().includes('TypeError: Failed to fetch')
            )) {
                console.log('Network error detected, switching to offline mode');
                event.preventDefault(); // Prevent the error from showing
                self.handleOffline();
            }
        });
    },
    
    interceptNetworkRequests: function() {
        const self = this;
        
        // Override fetch to detect connection issues
        const originalFetch = window.fetch;
        window.fetch = function(...args) {
            return originalFetch.apply(this, args)
                .catch(error => {
                    if (error.name === 'TypeError' && error.message === 'Failed to fetch') {
                        console.log('Fetch failed, switching to offline mode');
                        self.handleOffline();
                    }
                    throw error;
                });
        };
    },
    
    checkInitialConnection: function() {
        // Check if we can reach the server
        this.checkConnection();
    },
    
    checkConnection: function() {
        const self = this;
        
        fetch('/_health', { 
            method: 'HEAD',
            cache: 'no-cache',
            timeout: 5000
        })
        .then(response => {
            if (response.ok) {
                self.handleOnline();
            } else {
                self.handleOffline();
            }
        })
        .catch(() => {
            self.handleOffline();
        });
    },
    
    handleOffline: function() {
        if (!this.offlinePageShown) {
            console.log('Switching to offline mode');
            this.showOfflinePage();
            this.startRetryLoop();
        }
    },
    
    handleOnline: function() {
        if (this.offlinePageShown) {
            console.log('Connection restored, returning to online mode');
            this.hideOfflinePage();
            this.stopRetryLoop();
            // If we're on the offline page, go back to the main site
            if (window.location.pathname === '/offline.html') {
                window.location.href = '/';
            } else {
                // Reload the page to restore Blazor connection
                window.location.reload();
            }
        }
    },
    
    showOfflinePage: function() {
        this.offlinePageShown = true;
        console.log('Redirecting to offline page');
        
        // Redirect to the existing offline page
        window.location.href = '/offline.html';
    },
    
    hideOfflinePage: function() {
        this.offlinePageShown = false;
        // No need to restore content since we redirected to offline.html
    },
    
    startRetryLoop: function() {
        if (this.retryInterval) return;
        
        const self = this;
        this.retryInterval = setInterval(() => {
            if (self.offlinePageShown) {
                self.checkConnection();
            }
        }, 5000); // Check every 5 seconds
    },
    
    stopRetryLoop: function() {
        if (this.retryInterval) {
            clearInterval(this.retryInterval);
            this.retryInterval = null;
        }
    }
};

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.offlineHandler.initialize();
    });
} else {
    window.offlineHandler.initialize();
}