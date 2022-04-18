var swalInit = swal.mixin({
    buttonsStyling: false,
    confirmButtonClass: 'btn btn-primary',
    cancelButtonClass: 'btn btn-light'
});

function display_timer_header() {
    var x = new Date();
    var month = x.getMonth() + 1;
    var day = x.getDate();
    var year = x.getFullYear();
    if (month < 10) {
        month = "0" + month;
    }
    if (day < 10) {
        day = "0" + day;
    }
    var x3 = day + "/" + month + "/" + year;

    // time part //
    var hour = x.getHours();
    var minute = x.getMinutes();
    var second = x.getSeconds();
    if (hour < 10) {
        hour = "0" + hour;
    }
    if (minute < 10) {
        minute = "0" + minute;
    }
    if (second < 10) {
        second = "0" + second;
    }
    var x3 = x3 + ", " + hour + ":" + minute + ":" + second;
    $("#timer_header").html(x3);
    setTimeout(function() {
        display_timer_header();
    }, 1000);
}

function askToLogout() {
    swalInit.fire({
        title: 'Đăng xuất khỏi hệ thống?',
        text: "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống",
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Thôi',
        confirmButtonClass: "btn btn-danger",
        cancelButtonClass: "btn btn-dark",
        buttonsStyling: false,
        position: 'top'
    }).then(function(result) {
        if (result.value) {
            window.location.href = '/Auth/Logout';
        }
    });
}

function askToDelete(element) {
    swalInit.fire({
            title: "Xóa bản ghi?",
            text: "Dữ liệu sẽ không thể khôi phục. Bạn có chắc chắn muốn xóa?",
            type: "warning",
            showCancelButton: true,
            confirmButtonText: "Xóa",
            cancelButtonText: "Hủy bỏ",
            confirmButtonClass: "btn btn-danger",
            cancelButtonClass: "btn btn-dark",
            buttonsStyling: false,
            position: "top",
        })
        .then(function(result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    url: $(element).attr("href"),
                    data: {
                        __RequestVerificationToken: _token()
                    },
                    dataType: "JSON",
                    success: function(res) {
                        if (res.status) {
                            if (typeof res.reload !== typeof undefined && res.reload) {
                                showNotify(res.message, "success");
                                setTimeout(() => {
                                    window.location.reload();
                                }, 1500);
                            } else {
                                $(element).parents("tr:first").remove();
                                showNotify(res.message, "success");
                            }
                        } else {
                            showNotify(res.message, "error");
                        }
                    },
                    error: function(res) {
                        showNotify("Lỗi không xác định. Không thể xóa!", "error");
                    }
                });
            }
        });
}

async function askToRefuse($id) {

    const { value: text } = await Swal.fire({
        title: "Từ chối duyệt bài viết?",
        input: 'textarea',
        inputValidator: (value) => {
            return !value && 'Chưa nhập lý do từ chối!'
        },
        inputAttributes: {
            rows: 5,
            width: '100%',
            style: 'width: 300px;'
        },
        inputPlaceholder: 'Nhập lý do từ chối',
        showCancelButton: true,
        confirmButtonText: "Lưu lại",
        cancelButtonText: "Hủy",
        confirmButtonClass: "btn btn-primary",
        cancelButtonClass: "btn btn-dark",
        position: "top",
    })

    if (text) {
        $.ajax({
            type: "POST",
            url: "/AdminCP/Posts/Refuse",
            data: {
                __RequestVerificationToken: _token(),
                Reason: text,
                Id: $id
            },
            dataType: "JSON",
            success: function(res) {
                if (res.status) {
                    showNotify(res.message, "success");
                    setTimeout(() => {
                        window.location.href = '/AdminCP/Posts?status=Refused';
                    }, 1500);

                } else {
                    showNotify(res.message, "error");
                }
            },
            error: function(res) {
                showNotify("Lỗi không xác định. Không thể từ chối bài viết!", "error");
            }
        });
    }
}

function removeAttachment(element) {
    var url = $(element).attr("href");
    var id = $(element).data("id");
    var curVal = $("#" + id).val();
    var jsonVal = jsonVal = curVal ? JSON.parse(curVal) : [];
    var valSet = new Set(jsonVal);
    if (valSet.size > 0) {
        valSet.delete(url);
        $(element).parents("tr:first").remove();
        if (valSet.size === 0) {
            jsonVal = '';
        } else {
            jsonVal = [...valSet];
        }
        $("#" + id).val(JSON.stringify(jsonVal)).trigger("change");
    }
}

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

function fmSetLink($url, id = "") {
    var $curVal = $("#" + id).val();
    if (typeof $("#" + id).data("attachments") !== typeof undefined && $("#" + id).data("attachments")) {
        var $jsonVal = $curVal ? JSON.parse($curVal) : [];
        var $valSet = new Set($jsonVal);
        $valSet.add($url);
        $jsonVal = [...$valSet];
        $("#" + id).val(JSON.stringify($jsonVal)).trigger("change");
        var $table = $('.previewAttachments[data-id=' + id + ']');
        $table.empty();
        var $tbody = $("<tbody>");
        $jsonVal.forEach(val => {
            var $fileName = decodeURIComponent(val.substring(val.lastIndexOf('/') + 1));
            $tbody.append('<tr><td>' + $fileName + '</td><td class="text-right"><a data-id="' + id + '" href="' + val + '" onclick="removeAttachment(this); return false;" class="list-icons-item text-danger"><i class="icon-cross2"></i></a></td></tr>');
        })
        $table.append($tbody);
    } else {
        $("#" + id).val($url).trigger("change");
    }
    if (typeof $("#" + id).data("preview") !== typeof undefined && $("#" + id).data("preview")) {
        $('.previewFileChoosed[data-id=' + id + ']').html('<img src="' + $url + '"><a href="' + $url + '" target="_blank" class="view-full" data-popup="tooltip" title="Xem ảnh gốc"><i class="fa fa-search-plus"></i></a>').show();
        $('[data-popup="tooltip"]').tooltip();
    }
    $("#modalSelectFile").modal("hide");
}

function initBrowseFile() {
    $(".btn-choose-file").click(e => {
        if (typeof $(e.target).data("id") !== typeof undefined) {
            $("#modalSelectFile .modal-body").html("");
            $("#modalSelectFile").modal("show");
            setTimeout(() => {
                $("#modalSelectFile .modal-body").html(
                    '<iframe src="/AdminCP/file-manager/standalone-browse?id=' +
                    $(e.target).data("id") +
                    '" frameborder="0" style="width:100%;height:100%"></iframe>'
                );
            }, 450);
        }
    });
    $(".btn-remove-file").click(e => {
        $(e.target)
            .parent()
            .parent()
            .parent()
            .find("input")
            .first()
            .val("").trigger("change");

        $(e.target)
            .parent()
            .parent()
            .parent()
            .parent()
            .find(".previewFileChoosed")
            .first()
            .hide()
            .html('');
    });
    $('[data-preview="true"]').each((i, e) => {
        if (typeof $("#" + $(e).attr('id')).data("preview") !== typeof undefined && $("#" + $(e).attr('id')).data("preview") && $(e).val() != '') {
            $('.previewFileChoosed[data-id=' + $(e).attr('id') + ']').html('<img src="' + $(e).val() + '"><a href="' + $(e).val() + '" target="_blank" class="view-full" data-popup="tooltip" title="Xem ảnh gốc"><i class="fa fa-search-plus"></i></a>').show();
            $('[data-popup="tooltip"]').tooltip();
        }
    });

    $('[data-attachments="true"]').each((i, e) => {
        var $id = $(e).attr('id');
        var $curVal = $(e).val();
        var $jsonVal = $curVal ? JSON.parse($curVal) : [];
        if (typeof $("#" + $id).data("attachments") !== typeof undefined && $("#" + $id).data("attachments") && $curVal !== '') {
            console.log($jsonVal);
            $("#" + $id).val(JSON.stringify($jsonVal)).trigger("change");
            var $table = $('.previewAttachments[data-id=' + $id + ']');
            $table.empty();
            var $tbody = $("<tbody>");
            $jsonVal.forEach($val => {
                var $fileName = decodeURIComponent($val.substring($val.lastIndexOf('/') + 1));;
                $tbody.append('<tr><td>' + $fileName + '</td><td class="text-right"><a data-id="' + $id + '" href="' + $val + '" onclick="removeAttachment(this); return false;" class="list-icons-item text-danger"><i class="icon-cross2"></i></a></td></tr>');
            })
            $table.append($tbody);
        }
    });
}

$(document).ready(function() {
    display_timer_header();
    initBrowseFile();
    $('.table-category tr').click(function(event) {
        if (event.target.type !== 'checkbox') {
            $(':checkbox', this).trigger('click');
        }
    });
});