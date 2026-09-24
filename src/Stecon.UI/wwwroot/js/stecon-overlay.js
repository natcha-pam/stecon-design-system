// STECON UI - overlay lifecycle glue (M1D).
//
// The project's only JS module. It exists solely because native <dialog>
// showModal()/close() and the popover show/hide methods are imperative DOM
// APIs with no declarative Blazor/HTML-attribute equivalent - everything
// else (focus trap, focus restoration, background inertness, Escape) is
// handled natively by the browser and is NOT reimplemented here.
//
// Module-level state below is scoped to one browser connection (one Blazor
// Server circuit/tab gets its own JS execution context), so it never leaks
// across users/circuits.

const registry = new WeakMap();
let openOverlayCount = 0;
let modalActive = false;

function lockScroll() {
    if (openOverlayCount === 0) {
        document.body.classList.add('stecon-overlay-scroll-locked');
    }
    openOverlayCount++;
}

function unlockScroll() {
    openOverlayCount = Math.max(0, openOverlayCount - 1);
    if (openOverlayCount === 0) {
        document.body.classList.remove('stecon-overlay-scroll-locked');
    }
}

function release(dialog, entry) {
    if (!entry.locked) {
        return;
    }
    entry.locked = false;
    if (entry.exclusiveModal) {
        modalActive = false;
    }
    unlockScroll();
    dialog.classList.remove('stecon-overlay-open');
}

/**
 * Shows a <dialog> modally. Returns false (without opening) when exclusiveModal
 * is true and another exclusive modal is already active - the deterministic
 * "no second concurrent modal" rule; Drawer (exclusiveModal=false) is never
 * blocked and never blocks a Modal.
 */
export function showOverlay(dialog, dotNetRef, exclusiveModal, closeOnEscape, closeOnBackdrop) {
    if (!dialog) {
        return false;
    }
    if (dialog.open) {
        return true;
    }
    if (exclusiveModal && modalActive) {
        return false;
    }

    let entry = registry.get(dialog);
    if (!entry) {
        entry = {};
        entry.onCancel = (event) => {
            if (!entry.closeOnEscape) {
                event.preventDefault();
                return;
            }
            dotNetRef.invokeMethodAsync('OnNativeCloseAsync', 'Escape');
        };
        entry.onBackdropClick = (event) => {
            if (event.target === dialog && entry.closeOnBackdrop) {
                dotNetRef.invokeMethodAsync('OnNativeCloseAsync', 'Backdrop');
            }
        };
        entry.onClose = () => release(dialog, entry);

        dialog.addEventListener('cancel', entry.onCancel);
        dialog.addEventListener('click', entry.onBackdropClick);
        dialog.addEventListener('close', entry.onClose);
        registry.set(dialog, entry);
    }

    entry.exclusiveModal = exclusiveModal;
    entry.closeOnEscape = closeOnEscape;
    entry.closeOnBackdrop = closeOnBackdrop;
    entry.locked = true;

    if (exclusiveModal) {
        modalActive = true;
    }

    dialog.showModal();
    lockScroll();
    requestAnimationFrame(() => dialog.classList.add('stecon-overlay-open'));
    return true;
}

export function closeOverlay(dialog) {
    if (dialog && dialog.open) {
        dialog.close(); // fires the 'close' listener registered above, which releases bookkeeping
    }
}

export function disposeOverlay(dialog) {
    const entry = registry.get(dialog);
    if (!entry) {
        return;
    }

    if (dialog.open) {
        dialog.close();
    }

    dialog.removeEventListener('cancel', entry.onCancel);
    dialog.removeEventListener('click', entry.onBackdropClick);
    dialog.removeEventListener('close', entry.onClose);
    registry.delete(dialog);
}

// Toast host: a "manual" popover so it participates in the top layer (renders
// above an open <dialog>) without ever behaving like a modal - popovers never
// steal focus or make the background inert, by native specification.
export function showPopoverHost(element) {
    if (element && !element.matches(':popover-open')) {
        element.showPopover();
    }
}

export function hidePopoverHost(element) {
    if (element && element.matches(':popover-open')) {
        element.hidePopover();
    }
}
