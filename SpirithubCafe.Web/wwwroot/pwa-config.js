/**
 * PWA Configuration File
 * Customize your Progressive Web App settings here
 */

const PWA_CONFIG = {
    // Application Information
    app: {
        name: 'Spirithub Cafe',
        shortName: 'Spirithub',
        description: 'Premium Coffee Roastery & Online Shop',
        version: '1.0.0',
        themeColor: '#715A5A',
        backgroundColor: '#ffffff'
    },

    // Service Worker Settings
    serviceWorker: {
        enabled: true,
        scope: '/',
        updateInterval: 3600000, // 1 hour in milliseconds
        skipWaiting: true,
        clientsClaim: true
    },

    // Caching Strategy
    cache: {
        version: 'v1.0.0',
        strategies: {
            static: 'cache-first',      // CSS, JS, Fonts
            images: 'cache-first',      // Images
            api: 'network-first',       // API calls
            pages: 'network-first'      // HTML pages
        },
        duration: {
            static: 2592000000,         // 30 days
            images: 1209600000,         // 14 days
            api: 300000,                // 5 minutes
            pages: 604800000            // 7 days
        },
        maxEntries: {
            static: 100,
            images: 200,
            api: 50,
            pages: 50
        }
    },

    // Install Prompt Settings
    install: {
        showPrompt: false,              // Set to true to show install button
        deferPrompt: true,              // Defer the install prompt
        autoHideDelay: 10000           // Auto-hide install button after 10s
    },

    // Offline Settings
    offline: {
        enabled: true,
        pagePath: '/offline.html',
        message: 'You are currently offline'
    },

    // Update Notification
    update: {
        enabled: true,
        autoHide: true,
        autoHideDelay: 10000,
        message: 'New version available!'
    },

    // Push Notifications (Future feature)
    notifications: {
        enabled: false,
        vapidPublicKey: '',
        options: {
            icon: '/images/icon-192x192.png',
            badge: '/images/icon-72x72.png',
            vibrate: [200, 100, 200]
        }
    },

    // Background Sync (Future feature)
    backgroundSync: {
        enabled: false,
        tags: ['sync-orders', 'sync-favorites']
    },

    // Analytics
    analytics: {
        enabled: true,
        trackInstall: true,
        trackOffline: true,
        trackUpdate: true
    },

    // Debug Mode
    debug: {
        enabled: false,
        verbose: false,
        logCacheHits: false
    }
};

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = PWA_CONFIG;
}

// Make available globally
if (typeof window !== 'undefined') {
    window.PWA_CONFIG = PWA_CONFIG;
}
