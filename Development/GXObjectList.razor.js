// --------------------------------------------------------------------------
// Gurux Ltd
//
// Copyright (c) Gurux Ltd.
//
// This file is part of Gurux Device Framework.
//
// Commit: $Format:%H$
// Date:   $Format:%cI$
//
// This software is available under either of the following licenses:
//
// 1. GNU General Public License, version 2 only (GPL-2.0-only).
//    See the LICENSE file included with this distribution.
//
// 2. A commercial license granted by Gurux Ltd under a separate written
//    agreement. Use under this option requires a valid commercial license
//    and is governed by the terms of that agreement.
//
// If you have not obtained a commercial license from Gurux Ltd,
// use of this software is governed by GPL-2.0-only.
//
// Commercial licensing: https://www.gurux.fi/
//
// SPDX-License-Identifier: GPL-2.0-only OR LicenseRef-Gurux-Commercial
// --------------------------------------------------------------------------

export function initialize(id, component) {
    const table = document.getElementById(id);
    const events = new AbortController();
    const options = { signal: events.signal };
    let drag = null;
    const clamp = value => Math.max(64, Math.min(4096, value));
    const column = key => [...table.querySelectorAll('col[data-column]')].find(col => col.dataset.column === key);
    function apply(key, value) {
        column(key).style.width = `${value}px`;
        table.style.width = `${[...table.querySelectorAll('col')].reduce((sum, col) => sum + parseFloat(col.style.width), 0)}px`;
        table.querySelectorAll('.column-resizer').forEach(handle => {
            if (handle.dataset.column === key) handle.setAttribute('aria-valuenow', value);
        });
    }
    function save(key, width) {
        component.invokeMethodAsync('SaveColumnWidthAsync', key, width)
            .catch(error => console.error('Could not save object column width.', error));
    }
    table.addEventListener('pointerdown', event => {
        const handle = event.target.closest('.column-resizer');
        if (!handle || event.button !== 0 || drag) return;
        event.preventDefault();
        const key = handle.dataset.column;
        drag = { handle, key, pointer: event.pointerId, start: event.clientX, width: parseFloat(column(key).style.width) };
        handle.setPointerCapture(event.pointerId);
    }, options);
    table.addEventListener('pointermove', event => {
        if (drag?.pointer !== event.pointerId) return;
        apply(drag.key, clamp(drag.width + event.clientX - drag.start));
    }, options);
    function finish(event) {
        if (drag?.pointer !== event.pointerId) return;
        const current = drag;
        drag = null;
        const width = event.type === 'pointerup' ? clamp(current.width + event.clientX - current.start) : current.width;
        apply(current.key, width);
        if (current.handle.hasPointerCapture(current.pointer)) current.handle.releasePointerCapture(current.pointer);
        if (event.type === 'pointerup') save(current.key, width);
    }
    table.addEventListener('pointerup', finish, options);
    table.addEventListener('pointercancel', finish, options);
    table.addEventListener('lostpointercapture', finish, options);
    table.addEventListener('keydown', event => {
        const handle = event.target.closest('.column-resizer');
        if (!handle || !['ArrowLeft', 'ArrowRight'].includes(event.key)) return;
        event.preventDefault();
        const key = handle.dataset.column;
        const width = clamp(parseFloat(column(key).style.width) + (event.key === 'ArrowRight' ? 1 : -1) * (event.shiftKey ? 50 : 10));
        apply(key, width);
        save(key, width);
    }, options);
    return { dispose() {
        events.abort();
        if (drag?.handle.hasPointerCapture(drag.pointer)) drag.handle.releasePointerCapture(drag.pointer);
        drag = null;
    } };
}
