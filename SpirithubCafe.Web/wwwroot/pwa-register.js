/**
 * PWA Registration - Lightweight & Fast
 * Silent registration without console noise
 */

(function() {
    'use strict';

    // Check if already initialized
    if (window.PWARegistered) return;
    window.PWARegistered = true;

    // Check if service workers are supported
    if (!('serviceWorker' in navigator)) return;

    // Configuration
    const CONFIG = {
        serviceWorkerUrl: '/service-worker.js',
        scope: '/',
        updateInterval: 60 * 60 * 1000,
        showInstallPrompt: false
    };

    let deferredPrompt = null;
    let swRegistration = null;

    /**
     * Register Service Worker (Silent)
     */
    async function registerServiceWorker() {
        try {
            const registration = await navigator.serviceWorker.register(
                CONFIG.serviceWorkerUrl,
                { scope: CONFIG.scope }
            );

            swRegistration = registration;

            // Check for updates periodically (silent)
            setInterval(() => registration.update(), CONFIG.updateInterval);

            // Handle updates silently
            registration.addEventListener('updatefound', () => {
                const newWorker = registration.installing;
                if (newWorker) {
                    newWorker.addEventListener('statechange', () => {
                        if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                            // Reload automatically on update
                            window.location.reload();
                        }
                    });
                }
            });

            return registration;
        } catch (error) {
            // Silent error handling
        }
    }

    /**
     * Handle beforeinstallprompt event (PWA install)
     */
    window.addEventListener('beforeinstallprompt', (event) => {
        event.preventDefault();
        deferredPrompt = event;
    });

    /**
     * Initialize PWA features (Silent)
     */
    function init() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', registerServiceWorker);
        } else {
            registerServiceWorker();
        }
    }

    // Start initialization
    init();

})();
