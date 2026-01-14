/**
 * Admin Panel - JavaScript
 * Sidebar, Modals, Charts, Tables, etc.
 */

(function () {
    'use strict';

    // ============================================
    // SIDEBAR MANAGEMENT
    // ============================================

    window.toggleSidebar = function () {
        const sidebar = document.getElementById('adminSidebar');
        sidebar.classList.toggle('show');
        sidebar.classList.toggle('collapsed');

        // Save state to localStorage
        if (sidebar.classList.contains('collapsed')) {
            localStorage.setItem('sidebarCollapsed', 'true');
        } else {
            localStorage.removeItem('sidebarCollapsed');
        }
    };

    // Restore sidebar state on load
    document.addEventListener('DOMContentLoaded', function () {
        const isCollapsed = localStorage.getItem('sidebarCollapsed');
        if (isCollapsed === 'true') {
            const sidebar = document.getElementById('adminSidebar');
            sidebar.classList.add('collapsed');
        }
    });

    // ============================================
    // MODAL MANAGEMENT
    // ============================================

    window.showModal = function (modalId) {
        const modal = document.getElementById(modalId);
        const backdrop = document.getElementById('modalBackdrop');

        if (modal) {
            modal.classList.add('show');
            if (backdrop) {
                backdrop.classList.add('show');
            }
            document.body.style.overflow = 'hidden';
        }
    };

    window.hideModal = function (modalId) {
        const modal = document.getElementById(modalId);
        const backdrop = document.getElementById('modalBackdrop');

        if (modal) {
            modal.classList.remove('show');
            if (backdrop) {
                backdrop.classList.remove('show');
            }
            document.body.style.overflow = '';
        }
    };

    // Close modal on backdrop click
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('modal-backdrop')) {
            const modals = document.querySelectorAll('.modal.show');
            modals.forEach(modal => {
                modal.classList.remove('show');
            });
            e.target.classList.remove('show');
            document.body.style.overflow = '';
        }
    });

    // ============================================
    // CONFIRMATION DIALOGS
    // ============================================

    window.confirmDelete = function (message, url) {
        if (confirm(message || 'Bạn có chắc chắn muốn xóa?')) {
            window.location.href = url;
        }
    };

    window.confirmAction = function (message, callback) {
        if (confirm(message || 'Bạn có chắc chắn?')) {
            callback();
        }
    };

    // ============================================
    // TOAST NOTIFICATIONS
    // ============================================

    window.showToast = function (message, type = 'info') {
        const toastContainer = getOrCreateToastContainer();

        const toast = document.createElement('div');
        toast.className = `alert alert-${type}`;
        toast.style.cssText = 'margin-bottom: 10px; animation: slideIn 0.3s ease-out;';

        const icons = {
            success: 'fa-check-circle',
            danger: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };

        toast.innerHTML = `
            <i class="fas ${icons[type]} alert-icon"></i>
            <div class="alert-content">${message}</div>
        `;

        toastContainer.appendChild(toast);

        // Auto dismiss after 5 seconds
        setTimeout(() => {
            toast.style.animation = 'slideOut 0.3s ease-out';
            setTimeout(() => toast.remove(), 300);
        }, 5000);
    };

    function getOrCreateToastContainer() {
        let container = document.getElementById('toastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toastContainer';
            container.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 10000;
                max-width: 400px;
            `;
            document.body.appendChild(container);
        }
        return container;
    }

    // Add animations
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from {
                transform: translateX(400px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(400px);
                opacity: 0;
            }
        }
    `;
    document.head.appendChild(style);

    // ============================================
    // TABLE UTILITIES
    // ============================================

    window.sortTable = function (table, columnIndex) {
        const tbody = table.querySelector('tbody');
        const rows = Array.from(tbody.querySelectorAll('tr'));
        const isAscending = table.dataset.sortOrder !== 'asc';

        rows.sort((a, b) => {
            const aValue = a.cells[columnIndex].textContent.trim();
            const bValue = b.cells[columnIndex].textContent.trim();

            // Try to parse as number
            const aNum = parseFloat(aValue.replace(/[^0-9.-]/g, ''));
            const bNum = parseFloat(bValue.replace(/[^0-9.-]/g, ''));

            if (!isNaN(aNum) && !isNaN(bNum)) {
                return isAscending ? aNum - bNum : bNum - aNum;
            }

            return isAscending
                ? aValue.localeCompare(bValue)
                : bValue.localeCompare(aValue);
        });

        rows.forEach(row => tbody.appendChild(row));
        table.dataset.sortOrder = isAscending ? 'asc' : 'desc';
    };

    // ============================================
    // SEARCH/FILTER UTILITIES
    // ============================================

    window.filterTable = function (inputId, tableId) {
        const input = document.getElementById(inputId);
        const table = document.getElementById(tableId);
        const filter = input.value.toLowerCase();
        const rows = table.querySelectorAll('tbody tr');

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(filter) ? '' : 'none';
        });
    };

    // ============================================
    // FORM VALIDATION
    // ============================================

    window.validateForm = function (formId) {
        const form = document.getElementById(formId);
        const inputs = form.querySelectorAll('[required]');
        let isValid = true;

        inputs.forEach(input => {
            const errorDiv = input.nextElementSibling;

            if (!input.value.trim()) {
                input.classList.add('error');
                if (errorDiv && errorDiv.classList.contains('form-error')) {
                    errorDiv.textContent = 'Trường này là bắt buộc';
                }
                isValid = false;
            } else {
                input.classList.remove('error');
                if (errorDiv && errorDiv.classList.contains('form-error')) {
                    errorDiv.textContent = '';
                }
            }
        });

        return isValid;
    };

    // Real-time validation
    document.addEventListener('DOMContentLoaded', function () {
        const requiredInputs = document.querySelectorAll('[required]');

        requiredInputs.forEach(input => {
            input.addEventListener('blur', function () {
                if (!this.value.trim()) {
                    this.classList.add('error');
                } else {
                    this.classList.remove('error');
                }
            });

            input.addEventListener('input', function () {
                if (this.value.trim()) {
                    this.classList.remove('error');
                }
            });
        });
    });

    // ============================================
    // IMAGE PREVIEW
    // ============================================

    window.previewImage = function (input, previewId) {
        const preview = document.getElementById(previewId);

        if (input.files && input.files[0]) {
            const reader = new FileReader();

            reader.onload = function (e) {
                preview.src = e.target.result;
                preview.style.display = 'block';
            };

            reader.readAsDataURL(input.files[0]);
        }
    };

    // ============================================
    // LOADING OVERLAY
    // ============================================

    window.showLoading = function () {
        let overlay = document.getElementById('loadingOverlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'loadingOverlay';
            overlay.className = 'loading-overlay';
            overlay.innerHTML = '<div class="spinner spinner-lg"></div>';
            document.body.appendChild(overlay);
        }
        overlay.classList.add('show');
    };

    window.hideLoading = function () {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.classList.remove('show');
        }
    };

    // ============================================
    // NUMBER FORMATTING
    // ============================================

    window.formatCurrency = function (amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    };

    window.formatNumber = function (number) {
        return new Intl.NumberFormat('vi-VN').format(number);
    };

    // ============================================
    // DATE UTILITIES
    // ============================================

    window.formatDate = function (date, format = 'dd/MM/yyyy') {
        const d = new Date(date);
        const day = String(d.getDate()).padStart(2, '0');
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const year = d.getFullYear();
        const hours = String(d.getHours()).padStart(2, '0');
        const minutes = String(d.getMinutes()).padStart(2, '0');

        return format
            .replace('dd', day)
            .replace('MM', month)
            .replace('yyyy', year)
            .replace('HH', hours)
            .replace('mm', minutes);
    };

    // ============================================
    // COPY TO CLIPBOARD
    // ============================================

    window.copyToClipboard = function (text) {
        navigator.clipboard.writeText(text).then(() => {
            showToast('Đã sao chép vào clipboard', 'success');
        }).catch(() => {
            showToast('Không thể sao chép', 'danger');
        });
    };

    // ============================================
    // PRINT UTILITIES
    // ============================================

    window.printElement = function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            const printWindow = window.open('', '', 'height=600,width=800');
            printWindow.document.write('<html><head><title>Print</title>');
            printWindow.document.write('<link rel="stylesheet" href="/css/admin-variables.css">');
            printWindow.document.write('<link rel="stylesheet" href="/css/admin-base.css">');
            printWindow.document.write('<style>body { padding: 20px; }</style>');
            printWindow.document.write('</head><body>');
            printWindow.document.write(element.innerHTML);
            printWindow.document.write('</body></html>');
            printWindow.document.close();
            printWindow.print();
        }
    };

})();
