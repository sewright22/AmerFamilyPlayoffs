// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var Common = Common || {};

class HTML_IDENTIFIERS {
    static get MODAL() { return "#site-modal "; }
    static get MODAL_LAYOUT() { return "staticBackdrop"; }
}

Common = function () {
    this.initialize = function (parentElement) {
        // Use parentElement if provided, otherwise use document
        var parent = parentElement || document.documentElement;

        $(parent).find("button.modal-button").each(function (index, element) {
            element.addEventListener("click", function (event) {
                // Get url from data-url attribute
                var url = element.getAttribute("data-url");

                loadModal(url);
            });
        });

        // Find each submit button not in a form and add a click event to submit the form
        $(parent).find("button[type=submit]").not('form button').each(function (index, element) {
            element.addEventListener("click", function (event) {
                // Get form from data-form attribute
                var formId = element.getAttribute("data-form-id");
                var form = document.getElementById(formId);

                // Submit form
                form.submit();
            });
        });

        var timezoneOffsetField = $(parent).find("input[name='TimeZoneOffset']");
        if (timezoneOffsetField.length > 0) {
            var timezoneOffset = new Date().getTimezoneOffset();
            timezoneOffsetField.val(timezoneOffset);
        }
    };
}

document.addEventListener("DOMContentLoaded", function () {
    var common = new Common();
    common.initialize(document.documentElement);
});

function loadModal(url) {
    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            // Set modal body html
            $(HTML_IDENTIFIERS.MODAL).html(data);
            var common = new Common();
            common.initialize($(HTML_IDENTIFIERS.MODAL));
            var myModalEl = document.getElementById('staticBackdrop');
            myModalEl.addEventListener('hidden.bs.modal', function (event) {
                $("#site-modal").html('');
            });

            // Show modal
            var myModal = new bootstrap.Modal(myModalEl, {
                keyboard: false
            });

            myModal.show();
        },
        error: function (xhr, status, error) {
            // Show error
            $("#site-modal .modal-body").html(xhr.responseText);
            console.error(xhr.responseText);

            // Show modal
            var modalDiv = document.getElementById("site-modal");
            modal.style.display = "block";
        }
    });
}
