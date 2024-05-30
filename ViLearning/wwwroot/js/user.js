var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/user/getall',
            type: 'GET',
            dataSrc: 'data'
        },
        "columns": [
            {
                "data": 'userName',
                "width": "15%"
            },
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
                    return data ? moment(data).format('DD/MM/YYYY') : "Chưa điền thông tin";
                }
            },
            {
                "data": 'role',
                "width": "10%"
            },
            {
                "data": 'teacherCertificateImgUrl',
                "width": "15%",
                "render": function (data, type, row) {
                    if (data != null && row.teacherCertificate == true)
                    {
                        return "Đã xét duyệt";
                    }
                    else if (data != null && row.teacherCertificate != true)
                    {
                        return "Đang chờ xét duyệt";
                    } else {
                        return "Không";
                    }
                }
            },
            {
                "data": 'id',
                "render": function (data, type, row) {
                    var btnDisabled = row.teacherCertificateImgUrl ? "" : "return false;";
                    if (row.teacherCertificate == true) {
                        btnDisabled = "return false";
                    }
                    return `<div class="w-100 btn-group" role="group"> 
                            <a href="user/submit?id=${data}" class="btn btn-primary mx-2" onclick="${btnDisabled}">Duyệt</a>
                            </div>`;
                },
                "width": "15%"
            }
        ]
    });
}
