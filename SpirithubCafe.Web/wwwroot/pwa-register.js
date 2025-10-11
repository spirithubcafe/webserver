/**
 * PWA Registration and Management
 * Handles Service Worker registration without install prompts
 */

(function() {
    'use strict';

    // Check if service workers are supported
    if (!('serviceWorker' in navigator)) {
        console.log('Service Workers not supported');
        return;
    }

    // Configuration
    const CONFIG = {
        serviceWorkerUrl: '/service-worker.js',
        scope: '/',
        updateInterval: 60 * 60 * 1000, // Check for updates every hour
        showInstallPrompt: false // Set to false to disable install popup
    };

    let deferredPrompt = null;
    let swRegistration = null;

    /**
     * Register Service Worker
     */
    async function registerServiceWorker() {
        try {
            const registration = await navigator.serviceWorker.register(
                CONFIG.serviceWorkerUrl,
                { scope: CONFIG.scope }
            );

            swRegistration = registration;

            console.log('✅ Service Worker registered:', registration.scope);

            // Check for updates periodically
            setInterval(() => {
                registration.update();
            }, CONFIG.updateInterval);

            // Handle service worker updates
            registration.addEventListener('updatefound', () => {
                const newWorker = registration.installing;
                
                newWorker.addEventListener('statechange', () => {
                    if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                        // New service worker available
                        console.log('🔄 New service worker available');
                        showUpdateNotification();
                    }
                });
            });

            // Listen for messages from service worker
            navigator.serviceWorker.addEventListener('message', handleServiceWorkerMessage);

            return registration;
        } catch (error) {
            console.error('❌ Service Worker registration failed:', error);
        }
    }

    /**
     * Handle Service Worker Messages
     */
    function handleServiceWorkerMessage(event) {
        if (event.data && event.data.type === 'CACHE_UPDATED') {
            console.log('📦 Cache updated:', event.data.url);
        }
    }

    /**
     * Show update notification (subtle, non-intrusive)
     */
    function showUpdateNotification() {
        // Create a subtle notification bar at the bottom
        const notification = document.createElement('div');
        notification.id = 'sw-update-notification';
        notification.innerHTML = `
            <style>
                #sw-update-notification {
                    position: fixed;
                    bottom: 20px;
                    left: 50%;
                    transform: translateX(-50%);
                    background: linear-gradient(135deg, #715A5A, #37353E);
                    color: white;
                    padding: 12px 24px;
                    border-radius: 8px;
                    box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                    z-index: 10000;
                    display: flex;
                    align-items: center;
                    gap: 16px;
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                    font-size: 14px;
                    animation: slideUp 0.3s ease-out;
                }
                @keyframes slideUp {
                    from { transform: translateX(-50%) translateY(100px); opacity: 0; }
                    to { transform: translateX(-50%) translateY(0); opacity: 1; }
                }
                #sw-update-notification button {
                    background: white;
                    color: #715A5A;
                    border: none;
                    padding: 8px 16px;
                    border-radius: 4px;
                    cursor: pointer;
                    font-weight: 600;
                    font-size: 13px;
                    transition: transform 0.2s;
                }
                #sw-update-notification button:hover {
                    transform: scale(1.05);
                }
                #sw-update-notification .close {
                    background: transparent;
                    color: white;
                    padding: 4px 8px;
                    font-size: 18px;
                }
            </style>
            <span>🎉 New version available!</span>
            <button onclick="window.swUpdateApp()">Update</button>
            <button class="close" onclick="this.parentElement.remove()">×</button>
        `;
        
        document.body.appendChild(notification);

        // Auto-hide after 10 seconds
        setTimeout(() => {
            notification.style.animation = 'slideUp 0.3s ease-out reverse';
            setTimeout(() => notification.remove(), 300);
        }, 10000);
    }

    /**
     * Update the app (reload with new service worker)
     */
    window.swUpdateApp = function() {
        if (swRegistration && swRegistration.waiting) {
            swRegistration.waiting.postMessage({ type: 'SKIP_WAITING' });
            window.location.reload();
        }
    };

    /**
     * Handle beforeinstallprompt event (PWA install prompt)
     * Capture but don't show automatically
     */
    window.addEventListener('beforeinstallprompt', (event) => {
        // Prevent the default install prompt
        event.preventDefault();
        
        // Store the event for later use if needed
        deferredPrompt = event;
        
        console.log('📱 PWA install prompt available (hidden by default)');
        
        // Optionally, you can show a custom install button
        if (CONFIG.showInstallPrompt) {
            showInstallButton();
        }
    });

    /**
     * Show custom install button (optional)
     */
    function showInstallButton() {
        const installBtn = document.createElement('button');
        installBtn.id = 'pwa-install-btn';
        installBtn.innerHTML = '📱 Install App';
        installBtn.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            background: linear-gradient(135deg, #715A5A, #37353E);
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 9999;
            transition: transform 0.2s;
        `;
        
        installBtn.addEventListener('click', async () => {
            if (!deferredPrompt) return;
            
            deferredPrompt.prompt();
            const { outcome } = await deferredPrompt.userChoice;
            
            console.log(`User ${outcome === 'accepted' ? 'accepted' : 'dismissed'} the install prompt`);
            
            deferredPrompt = null;
            installBtn.remove();
        });
        
        installBtn.addEventListener('mouseenter', () => {
            installBtn.style.transform = 'scale(1.05)';
        });
        
        installBtn.addEventListener('mouseleave', () => {
            installBtn.style.transform = 'scale(1)';
        });
        
        document.body.appendChild(installBtn);
    }

    /**
     * Track PWA install
     */
    window.addEventListener('appinstalled', () => {
        console.log('✅ PWA installed successfully');
        deferredPrompt = null;
        
        // Remove install button if exists
        const installBtn = document.getElementById('pwa-install-btn');
        if (installBtn) installBtn.remove();
        
        // Analytics tracking (optional)
        if (typeof gtag === 'function') {
            gtag('event', 'pwa_install', {
                event_category: 'engagement',
                event_label: 'PWA Installation'
            });
        }
    });

    /**
     * Detect if app is running in standalone mode
     */
    function isStandalone() {
        return window.matchMedia('(display-mode: standalone)').matches ||
               window.navigator.standalone === true;
    }

    /**
     * Check online/offline status
     */
    function updateOnlineStatus() {
        const status = navigator.onLine ? 'online' : 'offline';
        console.log(`📡 Connection status: ${status}`);
        
        document.body.classList.toggle('offline', !navigator.onLine);
        
        // Show notification when going offline
        if (!navigator.onLine) {
            showOfflineNotification();
        }
    }

    /**
     * Show offline notification
     */
    function showOfflineNotification() {
        const notification = document.createElement('div');
        notification.id = 'offline-notification';
        notification.innerHTML = `
            <style>
                #offline-notification {
                    position: fixed;
                    top: 20px;
                    left: 50%;
                    transform: translateX(-50%);
                    background: #ff6b6b;
                    color: white;
                    padding: 12px 24px;
                    border-radius: 8px;
                    font-size: 14px;
                    z-index: 10001;
                    box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                    animation: slideDown 0.3s ease-out;
                }
                @keyframes slideDown {
                    from { transform: translateX(-50%) translateY(-100px); opacity: 0; }
                    to { transform: translateX(-50%) translateY(0); opacity: 1; }
                }
            </style>
            📡 You're offline. Some features may be limited.
        `;
        
        document.body.appendChild(notification);
        
        // Remove when back online
        const removeOnOnline = () => {
            notification.remove();
            window.removeEventListener('online', removeOnOnline);
        };
        
        window.addEventListener('online', removeOnOnline);
    }

    /**
     * Initialize PWA features
     */
    function init() {
        // Register service worker
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', registerServiceWorker);
        } else {
            registerServiceWorker();
        }

        // Monitor online/offline status
        window.addEventListener('online', updateOnlineStatus);
        window.addEventListener('offline', updateOnlineStatus);
        updateOnlineStatus();

        // Log PWA status
        if (isStandalone()) {
            console.log('✅ Running as installed PWA');
        } else {
            console.log('🌐 Running in browser');
        }

        // Add PWA meta info to console
        console.log('%c🚀 Spirithub Cafe PWA', 
            'font-size: 20px; font-weight: bold; color: #715A5A;');
        console.log('%cOffline-capable • Fast • Reliable', 
            'font-size: 12px; color: #666;');
    }

    // Start initialization
    init();

    // Expose utility functions
    window.PWA = {
        isStandalone,
        showInstallPrompt: () => {
            if (deferredPrompt) {
                showInstallButton();
            } else {
                console.log('Install prompt not available');
            }
        },
        clearCache: async () => {
            if (swRegistration) {
                swRegistration.active.postMessage({ type: 'CLEAR_CACHE' });
                console.log('🗑️ Cache cleared');
            }
        },
        checkForUpdates: async () => {
            if (swRegistration) {
                await swRegistration.update();
                console.log('🔍 Checked for updates');
            }
        }
    };

})();
