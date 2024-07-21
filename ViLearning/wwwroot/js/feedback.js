var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/teacher/feedback/getall',
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
                "width": "40%"
            },
            {
                "data": 'courseName',
                "width": "40%",
            },
            {
                "data": 'createdAt',
                "width": "10%",
                "render": function (data, type, row) {
                    return moment(data).format('DD/MM/YYYY');
                }
            }
        ]
    });
}
