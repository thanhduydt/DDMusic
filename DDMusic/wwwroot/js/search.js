var common = {
    init: function () {
        common.registerEvent();

    },
    registerEvent: function () {
        $("#txtKeyword").autocomplete({
            minLength: 0,
            source: function (request, response) {
                $.ajax({
                    url: "/Home/ListName",
                    dataType: "jsonp",
                    data: {
                        term: request.term
                    },
                    success: function (data) {
                        response(data.data);
                    }
                });
            },
            focus: function (event, ui) {
                $("#txtKeyword").val(ui.item.label);
                return false;
            },
            select: function (event, ui) {
                $("#txtKeyword").val(ui.item.label);
                //$("#project-id").val(ui.item.value);
                //$("#project-description").html(ui.item.desc);
                //$("#project-icon").attr("src", "images/" + ui.item.icon);

                return false;
            }
        })
            .autocomplete("instance")._renderItem = function (ul, item) {
                return $("<li>")
                    .append("<div>" + item.label + "<br>" + item.desc + "</div>")
                    .appendTo(ul);
            };
    }
}
common.init();