/**
 * User-Facing JavaScript Utilities
 * Restaurant Customer Experience
 */

(function () {
    'use strict';

    // ============================================
    // CART MANAGEMENT
    // ============================================

    window.addToCart = function (dishId, quantity = 1) {
        // Show loading
        showLoading();

        fetch('/Cart/Add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ dishId: dishId, quantity: quantity })
        })
            .then(response => response.json())
            .then(data => {
                hideLoading();
                if (data.success) {
                    showToast('Đã thêm món vào giỏ hàng!', 'success');
                    updateCartCount(data.cartCount || 0);
                    showStickyCart();

                    // Haptic feedback on mobile
                    if (navigator.vibrate) {
                        navigator.vibrate(50);
                    }
                } else {
                    showToast(data.message || 'Có lỗi xảy ra', 'error');
                }
            })
            .catch(error => {
                hideLoading();
                console.error('Error:', error);
                showToast('Không thể thêm món. Vui lòng thử lại', 'error');
            });
    };

    window.updateCartCount = function (count) {
        const badges = document.querySelectorAll('.bottom-nav-badge, #cartCount, .sticky-cart-count');
        badges.forEach(badge => {
            if (badge) {
                badge.textContent = count;
                badge.style.display = count > 0 ? 'flex' : 'none';
            }
        });
    };

    window.showStickyCart = function () {
        const stickyCart = document.querySelector('.sticky-cart');
        if (stickyCart) {
            stickyCart.classList.add('show');
        }
    };

    // ============================================
    // TOAST NOTIFICATIONS
    // ============================================

    window.showToast = function (message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `toast-user toast-${type}`;

        const icons = {
            success: 'fa-check-circle',
            error: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };

        toast.innerHTML = `
            <i class="fas ${icons[type]}"></i>
            <span>${message}</span>
        `;

        toast.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: ${getToastColor(type)};
            color: white;
            padding: var(--user-space-4) var(--user-space-6);
            border-radius: var(--user-radius-lg);
            box-shadow: var(--user-shadow-xl);
            z-index: 10000;
            display: flex;
            align-items: center;
            gap: var(--user-space-3);
            font-size: var(--user-font-base);
            font-weight: var(--user-font-medium);
            animation: slideInRight 0.3s ease-out;
            max-width: 90%;
        `;

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    };

    function getToastColor(type) {
        const colors = {
            success: 'var(--user-success)',
            error: 'var(--user-danger)',
            warning: 'var(--user-warning)',
            info: 'var(--user-info)'
        };
        return colors[type] || colors.info;
    }

    // Add animations
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideInRight {
            from { transform: translateX(400px); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        @keyframes slideOutRight {
            from { transform: translateX(0); opacity: 1; }
            to { transform: translateX(400px); opacity: 0; }
        }
    `;
    document.head.appendChild(style);

    // ============================================
    // LOADING OVERLAY
    // ============================================

    window.showLoading = function () {
        let overlay = document.getElementById('loadingOverlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'loadingOverlay';
            overlay.innerHTML = `
                <div style="
                    width: 60px;
                    height: 60px;
                    border: 4px solid rgba(255,255,255,0.3);
                    border-top-color: var(--user-primary);
                    border-radius: 50%;
                    animation: spin 0.8s linear infinite;
                "></div>
            `;
            overlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(255, 255, 255, 0.9);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 99999;
                opacity: 0;
                transition: opacity 0.2s;
            `;
            document.body.appendChild(overlay);
        }
        setTimeout(() => overlay.style.opacity = '1', 10);
    };

    window.hideLoading = function () {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => overlay.remove(), 200);
        }
    };

    // ============================================
    // SMOOTH SCROLL
    // ============================================

    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            if (href !== '#') {
                e.preventDefault();
                const target = document.querySelector(href);
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });

    // ============================================
    // IMAGE LAZY LOADING
    // ============================================

    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    if (img.dataset.src) {
                        img.src = img.dataset.src;
                        img.removeAttribute('data-src');
                    }
                    observer.unobserve(img);
                }
            });
        });

        document.querySelectorAll('img[data-src]').forEach(img => {
            imageObserver.observe(img);
        });
    }

    // ============================================
    // BOTTOM NAV ACTIVE STATE
    // ============================================

    document.addEventListener('DOMContentLoaded', function () {
        const path = window.location.pathname;
        const navItems = document.querySelectorAll('.bottom-nav-item');

        navItems.forEach(item => {
            const href = item.getAttribute('href');
            if (href && path.includes(href) && href !== '/') {
                item.classList.add('active');
            }
        });

        // Load cart count on page load
        fetch('/Cart/GetCartCount')
            .then(response => response.json())
            .then(data => {
                if (data.count !== undefined) {
                    updateCartCount(data.count);
                }
            })
            .catch(error => console.error('Error loading cart count:', error));
    });

    // ============================================
    // WISHLIST
    // ============================================

    window.toggleWishlist = function (dishId) {
        const wishlistBtn = event.target.closest('.dish-card-wishlist');
        const icon = wishlistBtn.querySelector('i');

        // Toggle icon
        if (icon.classList.contains('far')) {
            icon.classList.remove('far');
            icon.classList.add('fas');
            wishlistBtn.style.color = 'var(--user-primary)';
            showToast('Đã thêm vào yêu thích', 'success');
        } else {
            icon.classList.remove('fas');
            icon.classList.add('far');
            wishlistBtn.style.color = '';
            showToast('Đã xóa khỏi yêu thích', 'info');
        }

        // Haptic feedback
        if (navigator.vibrate) {
            navigator.vibrate(30);
        }
    };

    // ============================================
    // QUANTITY STEPPER
    // ============================================

    window.updateQuantity = function (dishId, delta) {
        const input = document.querySelector(`#quantity-${dishId}`);
        if (input) {
            let value = parseInt(input.value) || 1;
            value = Math.max(1, value + delta);
            input.value = value;
        }
    };

    // ============================================
    // FORMAT CURRENCY
    // ============================================

    window.formatCurrency = function (amount) {
        return new Intl.NumberFormat('vi-VN').format(amount) + '₫';
    };

})();
