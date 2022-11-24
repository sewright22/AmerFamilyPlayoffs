// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var Common = Common || {};

Common = function () {
    this.initialize = function (parentElement) {
        document.addEventListener("DOMContentLoaded", function () {
            //let buttonList = parentElement.querySelectorAll("button");
            //console.log(buttonList);
        });
    }
}

var common = new Common();
common.initialize(document.documentElement);