var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall', type:'GET', dataSrc: 'data' },
        "columns": [
            { "data": 'userName', "width": "15%" },
            {
                "data": 'fullName',
                "width": "20%",
                "render": function (data, type, row) {
                    return data ? data : "Chưa điền thông tin";
                }
            },
            {
                "data": 'dob',
                "width": "15%",
                "render": function (data, type, row) {
                    return data ? moment(data).format('MM/DD/YYYY') : "Chưa điền thông tin";
                }
            },
            { "data": 'role', "width": "10%" },
            {
                "data": 'teacherCertificateImgUrl',
                "data1": 'teacherCerfiticate',
                "width": "15%",
                "render": function (data, type, row) {
                    if (!row.teacherCerfiticateUrl) {
                        return "Không";
                    } else if (row.teacherCerfiticateUrl && row.teacherCerfiticate) {
                        return "Đã duyệt";
                    } else {
                        return "Đang xét duyệt";
                    }
                }
            },
            {
                "data": 'id',
                "render": function (data, type, row) {
                    var btnDisable = row.teacherCertificateImgUrl ? "" : "disabled";
                    return `<div class="w-100 btn-group" role="group"> 
                            <a href="user/submit?id=${data}" class="btn btn-primary mx-2"" ${btnDisable}>Duyệt</a>
                            </div >`
                }
                , "width": "15%"
            }
        ]
    });
}