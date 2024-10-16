function toggleCheckbox(item) {
    let id = item.id.replace('ftCheckbox_', '');
    let hidden = document.getElementById(id);
    item.checked === false ? hidden.value = 'off' : hidden.value = 'on';
}
