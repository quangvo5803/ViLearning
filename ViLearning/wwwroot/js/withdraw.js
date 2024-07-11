var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/Admin/ManagerTeacherWithdraw/GetAll',
            type: 'GET',
            dataSrc: 'data'
        },
        "columns": [
            {
                "data": 'applicationUser.userName',
                "width": "25%"
            },
            {
                "data": 'requestDay',
                "render": function (data) {
                    var date = new Date(data);
                    var day = String(date.getDate()).padStart(2, '0');
                    var month = String(date.getMonth() + 1).padStart(2, '0'); // January is 0!
                    var year = date.getFullYear();
                    return `${day}/${month}/${year}`;
                },
                "width": "25%"
            },
            {
                "data": 'requestMoney',
                "render": function (data) {
                    var formattedMoney = parseInt(data).toLocaleString('vi-VN') + " VNĐ";
                    return formattedMoney;
                },
                "width": "25%"
            },
            {
                "data": 'withdrawRequestID',
                "render": function (data) {
                    return `<div class="w-100 btn-group" role="group"> 
                            <a href="/Admin/ManagerTeacherWithdraw/Payment?withdrawId=${data}" class="btn btn-primary mx-2">Thanh toán</a>
                            </div>`;
                },
                "width": "25%"
            }
        ]
    });
}
