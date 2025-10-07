const CACHE_NAME = 'spirithub-v3';
const STATIC_CACHE = 'spirithub-static-v3';
const DYNAMIC_CACHE = 'spirithub-dynamic-v3';
const IMAGE_CACHE = 'spirithub-images-v3';

// Files to cache immediately (only truly static assets)
const STATIC_FILES = [
  '/css/dist.css',
  '/manifest.json'
];

// Install event - cache static files
self.addEventListener('install', event => {
  console.log('[SW] Installing Service Worker...');
  event.waitUntil(
    caches.open(STATIC_CACHE)
      .then(cache => {
        console.log('[SW] Caching static files');
        return cache.addAll(STATIC_FILES);
      })
      .catch(err => console.log('[SW] Error caching static files:', err))
  );
  self.skipWaiting();
});

// Activate event - clean up old caches
self.addEventListener('activate', event => {
  console.log('[SW] Activating Service Worker...');
  event.waitUntil(
    caches.keys().then(cacheNames => {
      return Promise.all(
        cacheNames.map(cacheName => {
          if (cacheName !== STATIC_CACHE && 
              cacheName !== DYNAMIC_CACHE && 
              cacheName !== IMAGE_CACHE) {
            console.log('[SW] Deleting old cache:', cacheName);
            return caches.delete(cacheName);
          }
        })
      );
    })
  );
  return self.clients.claim();
});

// Fetch event - serve from cache, fallback to network
self.addEventListener('fetch', event => {
  const { request } = event;
  const url = new URL(request.url);

  // Skip non-http(s) schemes (chrome-extension, blob, data, etc.)
  if (!url.protocol.startsWith('http')) {
    return;
  }

  // Skip non-GET requests
  if (request.method !== 'GET') {
    return;
  }

  // Skip Blazor SignalR connections, initializers, and framework files
  if (url.pathname.includes('/_blazor') || url.pathname.includes('/_framework')) {
    return;
  }

  // Skip API calls (you may want to cache some API responses)
  if (url.pathname.includes('/api/')) {
    return;
  }
  
  // Skip admin pages and dynamic Blazor pages (to prevent component mismatch)
  if (url.pathname.includes('/admin') || url.pathname.includes('/account')) {
    return;
  }

  // Handle image requests
  if (request.destination === 'image') {
    event.respondWith(
      caches.match(request).then(response => {
        return response || fetch(request).then(fetchResponse => {
          // Only cache successful responses
          if (fetchResponse.ok) {
            return caches.open(IMAGE_CACHE).then(cache => {
              cache.put(request, fetchResponse.clone());
              return fetchResponse;
            });
          }
          return fetchResponse;
        });
      }).catch(() => {
        // Return placeholder image if offline
        return new Response('<svg>...</svg>', {
          headers: { 'Content-Type': 'image/svg+xml' }
        });
      })
    );
    return;
  }

  // Network ONLY for HTML documents (no caching to prevent Blazor component mismatch)
  if (request.destination === 'document') {
    event.respondWith(
      fetch(request).catch(() => {
        // Only use cache as last resort when completely offline
        return caches.match(request).then(cachedResponse => {
          return cachedResponse || caches.match('/offline.html');
        });
      })
    );
    return;
  }

  // Cache first, fallback to network for other resources
  event.respondWith(
    caches.match(request).then(response => {
      return response || fetch(request).then(fetchResponse => {
        // Only cache successful responses
        if (fetchResponse.ok) {
          return caches.open(DYNAMIC_CACHE).then(cache => {
            cache.put(request, fetchResponse.clone());
            return fetchResponse;
          });
        }
        return fetchResponse;
      });
    }).catch(() => {
      // Offline fallback
      if (request.destination === 'document') {
        return caches.match('/offline.html');
      }
    })
  );
});

// Background sync for form submissions
self.addEventListener('sync', event => {
  if (event.tag === 'sync-forms') {
    event.waitUntil(syncForms());
  }
});

async function syncForms() {
  // Implement form sync logic here
  console.log('[SW] Syncing forms...');
}

// Push notifications
self.addEventListener('push', event => {
  const data = event.data ? event.data.json() : {};
  const title = data.title || 'Spirithub Cafe';
  const options = {
    body: data.body || 'You have a new notification',
    icon: '/images/icon-192x192.png',
    badge: '/images/icon-72x72.png',
    vibrate: [200, 100, 200],
    data: data.url || '/'
  };

  event.waitUntil(
    self.registration.showNotification(title, options)
  );
});

self.addEventListener('notificationclick', event => {
  event.notification.close();
  event.waitUntil(
    clients.openWindow(event.notification.data)
  );
});
