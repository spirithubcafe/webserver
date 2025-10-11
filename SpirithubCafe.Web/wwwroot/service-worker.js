/**
 * SpirithubCafe Progressive Web App - Service Worker
 * Advanced Caching Strategy with Offline Support
 * Version: 1.0.0
 */

const CACHE_VERSION = 'spirithub-v1.0.0';
const STATIC_CACHE = `${CACHE_VERSION}-static`;
const DYNAMIC_CACHE = `${CACHE_VERSION}-dynamic`;
const IMAGE_CACHE = `${CACHE_VERSION}-images`;
const API_CACHE = `${CACHE_VERSION}-api`;

// Cache duration in milliseconds
const CACHE_DURATION = {
    static: 30 * 24 * 60 * 60 * 1000,    // 30 days
    dynamic: 7 * 24 * 60 * 60 * 1000,    // 7 days
    images: 14 * 24 * 60 * 60 * 1000,    // 14 days
    api: 5 * 60 * 1000                    // 5 minutes
};

// Static assets to cache on install
const STATIC_ASSETS = [
    '/',
    '/dist.css',
    '/app.css',
    '/manifest.json',
    '/favicon.ico',
    '/images/icon-192x192.png',
    '/images/icon-512x512.png'
];

// Resources to cache with Network First strategy
const DYNAMIC_PAGES = [
    '/products',
    '/cart',
    '/orders',
    '/about',
    '/contact'
];

// API endpoints to cache
const API_ENDPOINTS = [
    '/api/products',
    '/api/categories'
];

/**
 * Install Event - Cache static assets
 */
self.addEventListener('install', (event) => {
    console.log('[Service Worker] Installing...');
    
    event.waitUntil(
        caches.open(STATIC_CACHE)
            .then((cache) => {
                console.log('[Service Worker] Caching static assets');
                return cache.addAll(STATIC_ASSETS);
            })
            .then(() => {
                console.log('[Service Worker] Installation complete');
                return self.skipWaiting(); // Activate immediately
            })
            .catch((error) => {
                console.error('[Service Worker] Installation failed:', error);
            })
    );
});

/**
 * Activate Event - Clean up old caches
 */
self.addEventListener('activate', (event) => {
    console.log('[Service Worker] Activating...');
    
    event.waitUntil(
        caches.keys()
            .then((cacheNames) => {
                return Promise.all(
                    cacheNames
                        .filter((cacheName) => {
                            // Delete old caches
                            return cacheName.startsWith('spirithub-') && 
                                   !cacheName.startsWith(CACHE_VERSION);
                        })
                        .map((cacheName) => {
                            console.log('[Service Worker] Deleting old cache:', cacheName);
                            return caches.delete(cacheName);
                        })
                );
            })
            .then(() => {
                console.log('[Service Worker] Activation complete');
                return self.clients.claim(); // Take control immediately
            })
    );
});

/**
 * Fetch Event - Implement caching strategies
 */
self.addEventListener('fetch', (event) => {
    const { request } = event;
    const url = new URL(request.url);

    // Skip non-GET requests
    if (request.method !== 'GET') {
        return;
    }

    // Skip chrome extensions and cross-origin requests
    if (!url.origin.includes(self.location.origin)) {
        return;
    }

    // Choose caching strategy based on request type
    if (isStaticAsset(url)) {
        event.respondWith(cacheFirst(request, STATIC_CACHE));
    } else if (isImage(url)) {
        event.respondWith(cacheFirst(request, IMAGE_CACHE));
    } else if (isAPIRequest(url)) {
        event.respondWith(networkFirst(request, API_CACHE, CACHE_DURATION.api));
    } else if (isDynamicPage(url)) {
        event.respondWith(networkFirst(request, DYNAMIC_CACHE, CACHE_DURATION.dynamic));
    } else {
        event.respondWith(staleWhileRevalidate(request, DYNAMIC_CACHE));
    }
});

/**
 * Cache First Strategy - Good for static assets
 * Try cache first, fallback to network
 */
async function cacheFirst(request, cacheName) {
    try {
        const cache = await caches.open(cacheName);
        const cached = await cache.match(request);
        
        if (cached && !isExpired(cached)) {
            console.log('[Service Worker] Cache hit:', request.url);
            return cached;
        }

        console.log('[Service Worker] Cache miss, fetching:', request.url);
        const response = await fetch(request);
        
        if (response.ok) {
            await cache.put(request, response.clone());
        }
        
        return response;
    } catch (error) {
        console.error('[Service Worker] Cache first failed:', error);
        
        // Try to return cached version even if expired
        const cache = await caches.open(cacheName);
        const cached = await cache.match(request);
        if (cached) {
            return cached;
        }
        
        return getOfflinePage();
    }
}

/**
 * Network First Strategy - Good for dynamic content
 * Try network first, fallback to cache
 */
async function networkFirst(request, cacheName, maxAge = CACHE_DURATION.dynamic) {
    try {
        const response = await fetch(request);
        
        if (response.ok) {
            const cache = await caches.open(cacheName);
            await cache.put(request, response.clone());
            console.log('[Service Worker] Network response cached:', request.url);
        }
        
        return response;
    } catch (error) {
        console.log('[Service Worker] Network failed, trying cache:', request.url);
        
        const cache = await caches.open(cacheName);
        const cached = await cache.match(request);
        
        if (cached) {
            return cached;
        }
        
        return getOfflinePage();
    }
}

/**
 * Stale While Revalidate Strategy - Good for frequently updated content
 * Return cache immediately, update in background
 */
async function staleWhileRevalidate(request, cacheName) {
    const cache = await caches.open(cacheName);
    const cached = await cache.match(request);
    
    const fetchPromise = fetch(request).then((response) => {
        if (response.ok) {
            cache.put(request, response.clone());
        }
        return response;
    });
    
    return cached || fetchPromise;
}

/**
 * Check if cached response is expired
 */
function isExpired(response) {
    const dateHeader = response.headers.get('date');
    if (!dateHeader) return false;
    
    const cacheDate = new Date(dateHeader);
    const now = new Date();
    const age = now - cacheDate;
    
    // Default to 7 days if no specific cache duration
    return age > CACHE_DURATION.dynamic;
}

/**
 * Check if request is for static asset
 */
function isStaticAsset(url) {
    const staticExtensions = ['.css', '.js', '.woff', '.woff2', '.ttf', '.eot'];
    return staticExtensions.some(ext => url.pathname.endsWith(ext));
}

/**
 * Check if request is for image
 */
function isImage(url) {
    const imageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp', '.svg', '.ico'];
    return imageExtensions.some(ext => url.pathname.endsWith(ext));
}

/**
 * Check if request is API call
 */
function isAPIRequest(url) {
    return url.pathname.startsWith('/api/');
}

/**
 * Check if request is for dynamic page
 */
function isDynamicPage(url) {
    return DYNAMIC_PAGES.some(page => url.pathname.startsWith(page));
}

/**
 * Get offline fallback page
 */
function getOfflinePage() {
    return new Response(
        `<!DOCTYPE html>
        <html lang="en" dir="ltr">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Offline - Spirithub Cafe</title>
            <style>
                * { margin: 0; padding: 0; box-sizing: border-box; }
                body {
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                    background: linear-gradient(135deg, #f5f5f5 0%, #ffffff 100%);
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    min-height: 100vh;
                    padding: 20px;
                }
                .offline-container {
                    text-align: center;
                    max-width: 500px;
                }
                .offline-icon {
                    width: 120px;
                    height: 120px;
                    margin: 0 auto 30px;
                    background: linear-gradient(135deg, #715A5A, #37353E);
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }
                .offline-icon svg {
                    width: 60px;
                    height: 60px;
                    fill: white;
                }
                h1 {
                    font-size: 32px;
                    color: #37353E;
                    margin-bottom: 16px;
                }
                p {
                    font-size: 16px;
                    color: #666;
                    line-height: 1.6;
                    margin-bottom: 30px;
                }
                button {
                    background: linear-gradient(135deg, #715A5A, #37353E);
                    color: white;
                    border: none;
                    padding: 14px 32px;
                    font-size: 16px;
                    border-radius: 8px;
                    cursor: pointer;
                    transition: transform 0.2s;
                }
                button:hover {
                    transform: translateY(-2px);
                }
            </style>
        </head>
        <body>
            <div class="offline-container">
                <div class="offline-icon">
                    <svg viewBox="0 0 24 24">
                        <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/>
                    </svg>
                </div>
                <h1>You're Offline</h1>
                <p>It looks like you've lost your internet connection. Don't worry, you can still browse cached pages.</p>
                <button onclick="window.location.reload()">Try Again</button>
            </div>
        </body>
        </html>`,
        {
            headers: {
                'Content-Type': 'text/html',
                'Cache-Control': 'no-cache'
            }
        }
    );
}

/**
 * Background Sync - For offline form submissions
 */
self.addEventListener('sync', (event) => {
    console.log('[Service Worker] Background sync:', event.tag);
    
    if (event.tag === 'sync-orders') {
        event.waitUntil(syncOrders());
    }
});

async function syncOrders() {
    // Implement order sync logic here
    console.log('[Service Worker] Syncing orders...');
}

/**
 * Push Notification Handler
 */
self.addEventListener('push', (event) => {
    if (!event.data) return;
    
    const data = event.data.json();
    const options = {
        body: data.body || 'New notification from Spirithub Cafe',
        icon: '/images/icon-192x192.png',
        badge: '/images/icon-72x72.png',
        vibrate: [200, 100, 200],
        data: data.url || '/',
        actions: [
            { action: 'open', title: 'View' },
            { action: 'close', title: 'Close' }
        ]
    };
    
    event.waitUntil(
        self.registration.showNotification(data.title || 'Spirithub Cafe', options)
    );
});

/**
 * Notification Click Handler
 */
self.addEventListener('notificationclick', (event) => {
    event.notification.close();
    
    if (event.action === 'open' || !event.action) {
        event.waitUntil(
            clients.openWindow(event.notification.data || '/')
        );
    }
});

/**
 * Message Handler - For communication with the app
 */
self.addEventListener('message', (event) => {
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting();
    }
    
    if (event.data && event.data.type === 'CLEAR_CACHE') {
        event.waitUntil(
            caches.keys().then((cacheNames) => {
                return Promise.all(
                    cacheNames.map((cacheName) => caches.delete(cacheName))
                );
            })
        );
    }
});

console.log('[Service Worker] Loaded and ready');
