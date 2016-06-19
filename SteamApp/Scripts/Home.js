    $("#new-prices-btn").on("click", function (e) {
        e.preventDefault();
        $("#loading").removeClass("hidden");
        $("#main-content").addClass("hidden");
        var url = "/Home/UpdatePrices";

        $.ajax({
            type: "POST",
            cache: false,
            url: url,
            data: { "items": data },
            success: function (result) {
                if (result.success === true) {
                    location.reload();
                    $("#main-content").removeClass("hidden");
                    $("#loading").addClass("hidden");
                } 
            }
        });
    });
