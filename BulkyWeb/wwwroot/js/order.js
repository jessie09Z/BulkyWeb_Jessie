var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    console.log("Current URL search:", url); // 输出当前URL的查询部分

    if (url.includes("inprocess")) {
        console.log("Loading inprocess data");
        loadDataTable("inprocess");
    }
    else if (url.includes("pending")) {
        console.log("Loading pending data");
        loadDataTable("pending");
    }
    else if (url.includes("approved")) {
        console.log("Loading approved data");
        loadDataTable("approved");
    }
    else if (url.includes("completed")) {
        console.log("Loading completed data");
        loadDataTable("completed");
    }
    else {
        console.log("Loading all data");
        loadDataTable("all");
    }
});

function loadDataTable(status) {
   dataTable= $('#tblData').DataTable({
       "ajax": {
           "url": "/admin/order/getall?status=" + status,
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
            , "width": "10%" },
        ]
    });
}

