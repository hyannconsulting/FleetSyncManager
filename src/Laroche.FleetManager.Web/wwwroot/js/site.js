// FleetSyncManager JavaScript functions

// Toast notification system
window.showToast = (type, message, title = '') => {
    // Create toast container if it doesn't exist
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    // Create toast element
    const toastId = 'toast-' + Date.now();
    const toastHtml = `
        <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header bg-${type} text-white">
                <i class="fas fa-${getToastIcon(type)} me-2"></i>
                <strong class="me-auto">${title || getToastTitle(type)}</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `;

    container.insertAdjacentHTML('beforeend', toastHtml);

    // Initialize and show toast
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: type === 'danger' ? 8000 : 5000
    });

    toast.show();

    // Remove toast element after it's hidden
    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    });
};

// Get toast icon based on type
function getToastIcon(type) {
    const icons = {
        'success': 'check-circle',
        'danger': 'exclamation-triangle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle',
        'primary': 'info-circle'
    };
    return icons[type] || 'info-circle';
}

// Get toast title based on type
function getToastTitle(type) {
    const titles = {
        'success': 'Succès',
        'danger': 'Erreur',
        'warning': 'Attention',
        'info': 'Information',
        'primary': 'Information'
    };
    return titles[type] || 'Notification';
}

// Confirmation dialog
window.confirmDialog = (message, title = 'Confirmation') => {
    return confirm(`${title}\n\n${message}`);
};

// Advanced confirmation with custom styling
window.showConfirmDialog = (message, title = 'Confirmation', confirmText = 'Confirmer', cancelText = 'Annuler') => {
    return new Promise((resolve) => {
        // Create modal HTML
        const modalId = 'confirm-modal-' + Date.now();
        const modalHtml = `
            <div class="modal fade" id="${modalId}" tabindex="-1" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">
                                <i class="fas fa-question-circle text-warning me-2"></i>
                                ${title}
                            </h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <p class="mb-0">${message}</p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">${cancelText}</button>
                            <button type="button" class="btn btn-danger confirm-btn">${confirmText}</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Add modal to DOM
        document.body.insertAdjacentHTML('beforeend', modalHtml);
        
        // Initialize modal
        const modalElement = document.getElementById(modalId);
        const modal = new bootstrap.Modal(modalElement);
        
        // Handle confirm button
        modalElement.querySelector('.confirm-btn').addEventListener('click', () => {
            resolve(true);
            modal.hide();
        });
        
        // Handle modal close (cancel)
        modalElement.addEventListener('hidden.bs.modal', () => {
            modalElement.remove();
            resolve(false);
        });
        
        // Show modal
        modal.show();
    });
};

// Initialize DataTables
window.initializeDataTable = (tableId, options = {}) => {
    const defaultOptions = {
        responsive: true,
        pageLength: 25,
        language: {
            url: '/js/datatables-fr.json',
            search: '_INPUT_',
            searchPlaceholder: 'Rechercher...'
        },
        dom: '<"row"<"col-sm-6"l><"col-sm-6"f>>rtip',
        columnDefs: [
            { className: 'text-center', targets: '_all' }
        ]
    };
    
    const finalOptions = { ...defaultOptions, ...options };
    
    if (typeof $ !== 'undefined' && $.fn.DataTable) {
        return $(`#${tableId}`).DataTable(finalOptions);
    }
};

// Format currency
window.formatCurrency = (amount, currency = 'EUR', locale = 'fr-FR') => {
    return new Intl.NumberFormat(locale, {
        style: 'currency',
        currency: currency
    }).format(amount);
};

// Format date
window.formatDate = (dateString, options = {}) => {
    const defaultOptions = {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        timeZone: 'Europe/Paris'
    };
    
    const finalOptions = { ...defaultOptions, ...options };
    
    try {
        const date = new Date(dateString);
        return date.toLocaleDateString('fr-FR', finalOptions);
    } catch (error) {
        console.error('Error formatting date:', error);
        return dateString;
    }
};

// Copy to clipboard
window.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        showToast('success', 'Copié dans le presse-papiers');
        return true;
    } catch (error) {
        console.error('Error copying to clipboard:', error);
        showToast('danger', 'Erreur lors de la copie');
        return false;
    }
};

// Smooth scroll to element
window.scrollToElement = (elementId, offset = 0) => {
    const element = document.getElementById(elementId);
    if (element) {
        const elementPosition = element.offsetTop - offset;
        window.scrollTo({
            top: elementPosition,
            behavior: 'smooth'
        });
    }
};

// Debounce function
window.debounce = (func, wait) => {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
};

// Local storage helpers
window.storage = {
    set: (key, value) => {
        try {
            localStorage.setItem(key, JSON.stringify(value));
            return true;
        } catch (error) {
            console.error('Error setting localStorage:', error);
            return false;
        }
    },
    
    get: (key, defaultValue = null) => {
        try {
            const item = localStorage.getItem(key);
            return item ? JSON.parse(item) : defaultValue;
        } catch (error) {
            console.error('Error getting localStorage:', error);
            return defaultValue;
        }
    },
    
    remove: (key) => {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (error) {
            console.error('Error removing localStorage:', error);
            return false;
        }
    }
};

// Initialize application
document.addEventListener('DOMContentLoaded', () => {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map((tooltipTriggerEl) => {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Initialize popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map((popoverTriggerEl) => {
        return new bootstrap.Popover(popoverTriggerEl);
    });
    
    console.log('FleetSyncManager JavaScript initialized');
});

// Handle Blazor reconnection
window.Blazor?.reconnect({
    onReconnecting: () => {
        showToast('warning', 'Tentative de reconnexion...', 'Connexion');
    },
    onReconnected: () => {
        showToast('success', 'Reconnecté avec succès', 'Connexion');
    },
    onReconnectionFailed: () => {
        showToast('danger', 'Échec de la reconnexion. Veuillez recharger la page.', 'Connexion');
    }
});
