var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
   dataTable= $('#tblData').DataTable({
        "ajax": {
            "url": "/admin/order/getall",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                // 如果返回的数据没有data属性，可以在这里添加
                if (!json.hasOwnProperty('data')) {
                    return json;
                }
                console.log(json.data);
                return json.data;
            }
        },
        "columns": [
            { "data": "id", "width": "6%" },
            { "data": "name", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
          
            { "data": "orderStatus", "width": "10%" },
            { "data": "orderTotal", "width": "15%" },
            { "data": "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> </a>               
                    
                    </div>`

                }
            , "width": "25%" },
        ]
    });
}

