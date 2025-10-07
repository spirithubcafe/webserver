// PWA Installation and Service Worker Registration
window.pwaHelper = {
    // Register service worker
    registerServiceWorker: function () {
        if ('serviceWorker' in navigator) {
            navigator.serviceWorker.register('/service-worker.js')
                .then(registration => {
                    console.log('✅ Service Worker registered successfully:', registration.scope);
                    
                    // Check for updates
                    registration.addEventListener('updatefound', () => {
                        const newWorker = registration.installing;
                        newWorker.addEventListener('statechange', () => {
                            if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                                // New service worker available
                                if (confirm('A new version of the app is available. Would you like to reload?')) {
                                    window.location.reload();
                                }
                            }
                        });
                    });
                })
                .catch(error => {
                    console.error('❌ Service Worker registration failed:', error);
                });
        }
    },

    // Show install prompt
    showInstallPrompt: function () {
        let deferredPrompt;
        
        window.addEventListener('beforeinstallprompt', (e) => {
            e.preventDefault();
            deferredPrompt = e;
            
            // Show install button (you can customize this)
            const installButton = document.querySelector('#install-pwa-button');
            if (installButton) {
                installButton.style.display = 'block';
                
                installButton.addEventListener('click', async () => {
                    if (deferredPrompt) {
                        deferredPrompt.prompt();
                        const { outcome } = await deferredPrompt.userChoice;
                        console.log(`User response to install prompt: ${outcome}`);
                        deferredPrompt = null;
                        installButton.style.display = 'none';
                    }
                });
            }
        });

        window.addEventListener('appinstalled', () => {
            console.log('✅ PWA installed successfully');
            deferredPrompt = null;
        });
    },

    // Check if PWA is installed
    isPWAInstalled: function () {
        return window.matchMedia('(display-mode: standalone)').matches ||
               window.navigator.standalone === true;
    },

    // Request notification permission
    requestNotificationPermission: async function () {
        if ('Notification' in window) {
            const permission = await Notification.requestPermission();
            return permission === 'granted';
        }
        return false;
    },

    // Enable performance optimizations
    enablePerformanceOptimizations: function () {
        // Lazy load images
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        const src = img.getAttribute('data-src');
                        if (src) {
                            img.src = src;
                            img.removeAttribute('data-src');
                            observer.unobserve(img);
                        }
                    }
                });
            });

            document.querySelectorAll('img[data-src]').forEach(img => {
                imageObserver.observe(img);
            });
        }

        // Preload critical resources
        const preloadLinks = document.querySelectorAll('link[rel="preload"]');
        console.log(`Preloading ${preloadLinks.length} critical resources`);

        // Connection optimization
        if ('connection' in navigator) {
            const connection = navigator.connection;
            if (connection.effectiveType === 'slow-2g' || connection.effectiveType === '2g') {
                console.log('⚠️ Slow connection detected, reducing quality...');
                document.body.classList.add('low-bandwidth');
            }
        }
    },

    // Prefetch next page
    prefetchPage: function (url) {
        const link = document.createElement('link');
        link.rel = 'prefetch';
        link.href = url;
        document.head.appendChild(link);
    },

    // Clear all caches
    clearCaches: async function () {
        if ('caches' in window) {
            const cacheNames = await caches.keys();
            await Promise.all(cacheNames.map(name => caches.delete(name)));
            console.log('✅ All caches cleared');
            return true;
        }
        return false;
    }
};

// Initialize PWA features
document.addEventListener('DOMContentLoaded', function () {
    window.pwaHelper.registerServiceWorker();
    window.pwaHelper.showInstallPrompt();
    window.pwaHelper.enablePerformanceOptimizations();
});
