// Chat Widget JavaScript Functions

window.scrollToBottom = (element) => {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

window.getClientIpAddress = async () => {
    try {
        // This is a simple approach - in production you might want to use a service
        const response = await fetch('https://api.ipify.org?format=json');
        const data = await response.json();
        return data.ip;
    } catch (error) {
        console.log('Could not get IP address:', error);
        return 'Unknown';
    }
};

// Notification functions
window.requestNotificationPermission = async () => {
    if ('Notification' in window && Notification.permission === 'default') {
        return await Notification.requestPermission();
    }
    return Notification.permission;
};

window.showNotification = (title, body, icon) => {
    if ('Notification' in window && Notification.permission === 'granted') {
        new Notification(title, {
            body: body,
            icon: icon || '/favicon.ico'
        });
    }
};

// Audio notification
window.playNotificationSound = () => {
    try {
        // Create a simple beep sound using Web Audio API
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();
        
        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);
        
        oscillator.frequency.setValueAtTime(800, audioContext.currentTime);
        oscillator.type = 'sine';
        
        gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);
        
        oscillator.start();
        oscillator.stop(audioContext.currentTime + 0.5);
    } catch (e) {
        console.log('Audio not available:', e);
    }
};

// Focus and visibility detection
window.isPageVisible = () => {
    return !document.hidden;
};

window.focusElement = (element) => {
    if (element) {
        element.focus();
    }
};

// Local storage helpers
window.chatStorage = {
    get: (key) => {
        try {
            return localStorage.getItem(key);
        } catch (e) {
            return null;
        }
    },
    set: (key, value) => {
        try {
            localStorage.setItem(key, value);
            return true;
        } catch (e) {
            return false;
        }
    },
    remove: (key) => {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (e) {
            return false;
        }
    }
};