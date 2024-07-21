var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/feedback/getall',
            type: 'GET',
            dataSrc: 'data'
        },
        "columns": [
            {
                "data": 'feedBackStar',
                "width": "10%",
                "render": function (data) {
                    var stars = '';
                    for (var i = 1; i <= data; i++) {
                        stars += '<i class="bi bi-star-fill"></i>';
                    }
                    return stars;
                }
            },
            {
                "data": 'feedBackContent',
                "width": "30%"
            },
            {
                "data": 'courseName',
                "width": "30%",
            },
            {
                "data": 'createdAt',
                "width": "10%",
                "render": function (data, type, row) {
                    return moment(data).format('DD/MM/YYYY');
                }
            },
            {
                "data": 'feedBackId',
                "render": function (data) {
                    return `<div class="w-100 btn-group" role="group"> 
                            <a onclick="Delete('/Admin/Feedback/Delete?id=${data}')" class="btn btn-primary mx-2">Xoá</a>
                            </div>`;
                },
                "width": "20%"
            }
        ]
    });
}
function Delete(url) {
    Swal.fire({
        title: "Bạn có chắn chắn muốn xóa?",
        text: "Bạn không thể hoàn tác!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Vâng, xóa nó!",
        cancelButtonText: "Thoát"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    location.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}