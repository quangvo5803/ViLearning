var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { "data": 'userName', "width": "20%" },
            {
                "data": 'fullName',
                "width": "20%",
                "render": function (data, type, row) {
                    return data ? data : "Chưa điền thông tin";
                }
            },
            {
                "data": 'dob',
                "width": "20%",
                "render": function (data, type, row) {
                    return data ? moment(data).format('DD/MM/YYYY') : "Chưa điền thông tin";
                }
            },
            { "data": 'role', "width": "20%" },
            {
                "data": 'teacherCertificateImgUrl',
                "width": "20%",
                "render": function (data, type, row) {
                    return data ? "Đang chờ xét duyệt" : "Không";
                }
            },
        ]
    });
}