// Simple Service Worker for SpirithubCafe
// Basic caching only - no updates, no popups

const CACHE_NAME = 'spirithubcafe-simple';

// Basic resources to cache
const CACHE_URLS = [
    '/',
    '/dist.css',
    '/favicon.ico',
    '/offline.html'
];

// Install - cache basic files
self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then((cache) => cache.addAll(CACHE_URLS))
            .then(() => self.skipWaiting())
    );
});

// Activate - take control
self.addEventListener('activate', (event) => {
    event.waitUntil(self.clients.claim());
});

// Fetch - network first, with offline fallback
self.addEventListener('fetch', (event) => {
    // Skip non-GET requests and special URLs
    if (event.request.method !== 'GET' ||
        event.request.url.includes('/_blazor') ||
        event.request.url.includes('/negotiate') ||
        event.request.url.startsWith('chrome-extension://')) {
        return;
    }
    
    // Handle navigation requests (HTML pages)
    if (event.request.mode === 'navigate') {
        event.respondWith(
            fetch(event.request)
                .catch(() => {
                    // If network fails for navigation, show offline page
                    return caches.match('/offline.html');
                })
        );
        return;
    }
    
    // Handle other requests
    event.respondWith(
        fetch(event.request)
            .catch(() => caches.match(event.request))
    );
});