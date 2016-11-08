/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

Log = {
    container: null,
    load: function (container) {
        container = MasterPage.getContent(container);
        container.listarAjax();

        Aux.setarFoco(container);
        Log.container = container
    }
};