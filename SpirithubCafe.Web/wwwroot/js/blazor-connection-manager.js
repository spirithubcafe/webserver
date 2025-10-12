// Enhanced Blazor connection management
window.blazorConnectionManager = {
    retryAttempts: 0,
    maxRetries: 5,
    retryDelay: 3000,
    
    initialize: function() {
        if (typeof Blazor !== 'undefined') {
            this.setupBlazorCallbacks();
        } else {
            // Wait for Blazor to load
            document.addEventListener('DOMContentLoaded', () => {
                setTimeout(() => this.initialize(), 1000);
            });
        }
    },
    
    setupBlazorCallbacks: function() {
        const self = this;
        
        // Handle connection state changes
        Blazor.defaultReconnectionHandler = {
            onConnectionDown: function() {
                console.log('Blazor connection lost. Attempting to reconnect...');
                self.showConnectionStatus('Connection lost. Attempting to reconnect...', 'warning');
                return true; // Allow automatic reconnection
            },
            
            onConnectionUp: function() {
                console.log('Blazor connection restored.');
                self.showConnectionStatus('Connection restored.', 'success');
                self.retryAttempts = 0;
                setTimeout(() => self.hideConnectionStatus(), 3000);
            },
            
            onReconnectionFailed: function() {
                console.log('Blazor reconnection failed.');
                self.retryAttempts++;
                
                if (self.retryAttempts < self.maxRetries) {
                    self.showConnectionStatus(`Reconnection failed. Retrying (${self.retryAttempts}/${self.maxRetries})...`, 'error');
                    return true; // Continue trying
                } else {
                    self.showConnectionStatus('Connection lost. Please refresh the page.', 'error', true);
                    return false; // Stop trying
                }
            }
        };
        
        // Handle unhandled errors
        window.addEventListener('unhandledrejection', function(event) {
            console.error('Unhandled promise rejection:', event.reason);
            
            // Check if it's a Blazor-related error
            if (event.reason && event.reason.message && 
                (event.reason.message.includes('Invocation canceled') || 
                 event.reason.message.includes('Connection disconnected') ||
                 event.reason.message.includes('component records'))) {
                
                // Prevent the error from being logged to console
                event.preventDefault();
                
                // Try to restart the connection if possible
                setTimeout(() => {
                    if (Blazor && Blazor.start) {
                        console.log('Attempting to restart Blazor connection...');
                        this.attemptReconnection();
                    }
                }, 2000);
            }
        });
        
        // Monitor for component record errors
        const originalLog = console.error;
        console.error = function(...args) {
            const message = args.join(' ');
            if (message.includes('component records is not valid')) {
                console.log('Detected component records error - attempting recovery');
                self.handleComponentRecordsError();
                return; // Don't log the error
            }
            originalLog.apply(console, args);
        };
    },
    
    handleComponentRecordsError: function() {
        // Clear any stale state
        try {
            if (window.localStorage) {
                const blazorKeys = Object.keys(localStorage).filter(key => 
                    key.startsWith('_blazor') || key.startsWith('__blazor')
                );
                blazorKeys.forEach(key => localStorage.removeItem(key));
            }
        } catch (e) {
            console.log('Could not clear localStorage:', e);
        }
        
        // Force a page reload after a short delay
        setTimeout(() => {
            this.showConnectionStatus('Refreshing page to restore connection...', 'info');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        }, 1000);
    },
    
    attemptReconnection: function() {
        try {
            if (Blazor && typeof Blazor.reconnect === 'function') {
                Blazor.reconnect();
            } else {
                // Fallback to page reload
                setTimeout(() => window.location.reload(), 3000);
            }
        } catch (e) {
            console.error('Error attempting reconnection:', e);
            setTimeout(() => window.location.reload(), 3000);
        }
    },
    
    showConnectionStatus: function(message, type = 'info', persistent = false) {
        let statusElement = document.getElementById('blazor-connection-status');
        
        if (!statusElement) {
            statusElement = document.createElement('div');
            statusElement.id = 'blazor-connection-status';
            statusElement.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                padding: 12px 20px;
                border-radius: 8px;
                color: white;
                font-family: system-ui, -apple-system, sans-serif;
                font-size: 14px;
                font-weight: 500;
                z-index: 10000;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
                transition: all 0.3s ease;
                max-width: 300px;
                word-wrap: break-word;
            `;
            document.body.appendChild(statusElement);
        }
        
        // Set colors based on type
        const colors = {
            info: '#3b82f6',
            success: '#10b981',
            warning: '#f59e0b',
            error: '#ef4444'
        };
        
        statusElement.style.backgroundColor = colors[type] || colors.info;
        statusElement.textContent = message;
        statusElement.style.display = 'block';
        
        if (!persistent) {
            setTimeout(() => this.hideConnectionStatus(), 5000);
        }
    },
    
    hideConnectionStatus: function() {
        const statusElement = document.getElementById('blazor-connection-status');
        if (statusElement) {
            statusElement.style.display = 'none';
        }
    }
};

// Initialize when the script loads
document.addEventListener('DOMContentLoaded', function() {
    window.blazorConnectionManager.initialize();
});

// Also try immediate initialization in case DOM is already loaded
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        window.blazorConnectionManager.initialize();
    });
} else {
    window.blazorConnectionManager.initialize();
}