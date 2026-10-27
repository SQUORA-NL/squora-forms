// Edit mode: Firefox only starts a drag that carries data, and Blazor cannot set it from a dragstart handler.
let ready = false;

export function init() {
    if (ready) return;
    ready = true;
    document.addEventListener('dragstart', e => {
        const source = e.target;
        if (e.dataTransfer && source instanceof Element && source.closest('.bd-handle, .bd-toolbox-item')) {
            e.dataTransfer.setData('text/plain', 'squora-forms');
            e.dataTransfer.effectAllowed = 'move';
        }
    }, true);
}
