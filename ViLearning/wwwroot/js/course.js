var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/Admin/ManageCourse/getall',
            type: 'GET',
            dataSrc: 'data',
            error: function (xhr, error, thrown) {
                console.log('Ajax error:', error);
                console.log('Thrown error:', thrown);
            }
        },
        "columns": [
            {
                "data": 'courseName',
                "width": "25%"
            },
            {
                "data": 'description',
                "width": "50%"
            },
            {
                "data": 'price',
                "render": function (data) {
                    var formattedMoney = parseInt(data).toLocaleString('vi-VN') + " VNĐ";
                    return formattedMoney;
                },
                "width": "25%"
            }
        ]
    });
}
