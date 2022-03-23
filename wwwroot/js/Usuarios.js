var datatable
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url": "/Admin/Usuarios/obtenerTodos"
        },
        "columns": [
            { "data": "userName", "width": "10%" },
            { "data": "nombres", "width": "10%" },
            { "data": "apellidos", "width": "10%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "role", "width": "15%" },
            {
                "data": {
                    id: "id",
                    lockoutEnd: "locoutkEnd",
                },
                "render": function (data) {
                    var hoy = new Date().getTime();
                    var bloqueo = new Date(data.lockoutEnd).getTime()
                    if (bloqueo > hoy) {
                        //Usuario Bloqueado
                        return `
                    <div class="text-center">
                <a onclick=Bloquear('${data.id}') class="btn btn-danger text-white w-100" style="cursor:pointer;"> <i class="fa-solid fa-lock-open"></i> 
                    Desbloquear</a>
                </div>`;
                    }
                    else {
                        return `
                    <div class="text-center">
                <a onclick=Bloquear('${data.id}') class="btn btn-success text-white w-100" style="cursor:pointer;"> <i class="fa-solid fa-lock"></i> 
                    Bloquear</a>
                </div>`;
                    }

                }, "width": "20%"
            }
        ]
    });

}

function Bloquear(id) {

   
    $.ajax({
        type: "POST",
        url: '/Admin/Usuarios/BloquearDesbloquear',
        data: JSON.stringify(id),
        contentType:"application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                datatable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    });
        

}