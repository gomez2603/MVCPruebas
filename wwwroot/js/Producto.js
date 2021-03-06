var datatable
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "ajax": {
            "url":"/Admin/Productos/obtenerTodos"
        },
        "columns": [
            { "data": "numeroSerie", "width": "15%" },
            { "data": "descripcion", "width": "15%" },
            { "data": "categoria.nombre", "width": "15%" },
            { "data": "marca.nombre", "width": "15%" },
            { "data": "precio", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return`
                    <div class="text-center">
                     <a href="/Admin/Productos/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer;"> <i class="fa-solid fa-pen-to-square"></i> </a>
                <a onclick=Delete("/Admin/Productos/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer;"> <i class="fa-solid fa-trash"></i> </a>
                </div>`;
                }, "width":"15%"
                
            }
        ]
    });
}

function Delete(url) {

    swal({
        title: "¿Estas seguro que desea eliminar el Prodcucto?",
        text: "Este Registro no se podra recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    });
}