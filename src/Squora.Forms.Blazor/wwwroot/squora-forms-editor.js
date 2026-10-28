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

    // A component's own widget (a dropdown, a date picker, the spinner of a number, an upload button) handles its clicks itself and keeps them
    // from reaching the wrapper that selects the component, so the selection stayed on the layout around it. Blazor cannot listen while the event
    // goes down, this can: the click is passed on to the wrapper of the component that was hit, and then goes on to the widget as usual.
    document.addEventListener('click', e => {
        const hit = e.target;
        if (!(hit instanceof Element) || hit.closest('.bd-tag')) return;
        const wrapper = hit.closest('.bd-editable');
        // a layout is selected by Blazor's own click (the children inside it are what gets hit), and a click on the wrapper itself needs no help
        if (!wrapper || wrapper === hit || wrapper.classList.contains('bd-container')) return;
        wrapper.dispatchEvent(new MouseEvent('click', {
            bubbles: true, cancelable: true, view: window, button: 0, clientX: e.clientX, clientY: e.clientY,
        }));
    }, true);
}
