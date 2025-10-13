// Dropdown Helper - Professional click outside handler for Blazor components
(function () {
    'use strict';

    let activeDropdowns = new Map();

    // Setup click outside handler for a Blazor component
    window.setupClickOutside = function (dotNetRef, methodName) {
        if (!dotNetRef || !methodName) {
            console.warn('setupClickOutside: Invalid parameters');
            return;
        }

        const handleClickOutside = function (event) {
            // Check if click is outside all dropdowns
            let clickedOutside = true;
            
            // Check if clicked element is part of any dropdown
            const clickedElement = event.target;
            if (clickedElement) {
                // Check if clicked on dropdown or its children
                const dropdown = clickedElement.closest('.relative[data-dropdown]') || 
                                clickedElement.closest('.relative');
                if (dropdown) {
                    clickedOutside = false;
                }
            }

            if (clickedOutside) {
                try {
                    dotNetRef.invokeMethodAsync(methodName);
                } catch (error) {
                    console.warn('Failed to invoke close method:', error);
                }
            }
        };

        // Store the handler for cleanup
        const dropdownId = Math.random().toString(36).substring(7);
        activeDropdowns.set(dropdownId, {
            dotNetRef,
            handler: handleClickOutside
        });

        // Add event listener
        document.addEventListener('click', handleClickOutside, true);
        document.addEventListener('touchstart', handleClickOutside, true);

        return dropdownId;
    };

    // Cleanup function
    window.cleanupClickOutside = function (dropdownId) {
        const dropdown = activeDropdowns.get(dropdownId);
        if (dropdown) {
            document.removeEventListener('click', dropdown.handler, true);
            document.removeEventListener('touchstart', dropdown.handler, true);
            activeDropdowns.delete(dropdownId);
        }
    };

    // Cleanup all dropdowns (useful for page navigation)
    window.cleanupAllDropdowns = function () {
        activeDropdowns.forEach((dropdown, id) => {
            window.cleanupClickOutside(id);
        });
        activeDropdowns.clear();
    };

    // Handle escape key for accessibility
    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' || event.keyCode === 27) {
            activeDropdowns.forEach((dropdown) => {
                try {
                    dropdown.dotNetRef.invokeMethodAsync('CloseDropdownFromJs');
                } catch (error) {
                    console.warn('Failed to close dropdown on escape:', error);
                }
            });
        }
    });

    // Prevent touch scrolling when dropdown is open on mobile
    window.lockBodyScroll = function () {
        document.body.style.overflow = 'hidden';
        document.body.style.touchAction = 'none';
    };

    window.unlockBodyScroll = function () {
        document.body.style.overflow = '';
        document.body.style.touchAction = '';
    };

    console.log('Dropdown Helper initialized successfully');
})();
