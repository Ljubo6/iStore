// Урок 11 Скрипт предпросмотра картинок
$(function () {

    /* Preview selected image */

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $("img#imgpreview")
                    .attr("src", e.target.result)
                    .width(200)
                    .height(200);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $("#imageUpload").change(function () {
        readURL(this);
    });

    /*-----------------------------------------------------------*/

    Dropzone.options.dropzoneFprm = {
        acceptedFiles: "images/*",
        init: function () {
            this.on("complete",
                function (file) {
                    if (this.getUploadingFiles().length === 0 && this.getUploadingFiles().length === 0) {
                        location.reload();
                    }
                });
            this.on("sending",
                function (file, xhr, formData) {
                    formData.append("id",@Model.Id);
                });
        }
    }
});