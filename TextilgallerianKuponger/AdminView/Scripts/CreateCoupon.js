﻿window.addEventListener('load', function () {
    var dropdown = document.querySelector('.type');
    var disposable = document.querySelector('.disposable');
    var specificFields = document.querySelectorAll('.specific-field');
    var campaignFields = document.querySelectorAll('.campaign-code');
    var disposableFields = document.querySelectorAll('.disposable-codes');

    function showCorrectFields() {
        var value = dropdown.options[dropdown.selectedIndex].value;
        var type = value.replace('Domain.Entities.', '');

        Array.prototype.forEach.call(specificFields, function (field) {
            if (!field.classList.contains(type)) {
                field.hidden = true;
            }
        });

        var currentFields = document.getElementsByClassName(type);

        Array.prototype.forEach.call(currentFields, function (field) {
            field.hidden = false;
        });

        Array.prototype.forEach.call(campaignFields, function (field) {
            field.hidden = disposable.checked;
        });
        Array.prototype.forEach.call(disposableFields, function (field) {
            field.hidden = !disposable.checked;
        });
    }

    dropdown.addEventListener('change', showCorrectFields);
    disposable.addEventListener('click', showCorrectFields);
    disposable.addEventListener('change', showCorrectFields);

    showCorrectFields();
});