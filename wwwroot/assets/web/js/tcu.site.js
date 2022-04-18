function showNotify(message, type = "info") {
    var icon_before = "";
    if (type == "success") {
        icon_before = '<em class="fa fa-check"></em>';
    }
    if (type == "error") {
        icon_before = '<em class="fa fa-times-circle"></em>';
    }
    if (type == "warning") {
        icon_before = '<em class="icon-warning22"></em>';
    }
    if (type == "info") {
        icon_before = '<em class="fa fa-info"></em>';
    }
    new Noty({
        theme: "metroui",
        timeout: 5000,
        layout: "bottomRight",
        text: icon_before + "&nbsp;&nbsp;&nbsp;&nbsp;" + message,
        type: type,
    }).show();
}