﻿using MisMarcadores.Data.DataAccess;
using MisMarcadores.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MisMarcadores.Repository
{
    public interface IUsuariosRepository : IGenericRepository<Usuario>
    {
        Usuario ObtenerPorNombreUsuario(string nombreUsuario);
        void ModificarUsuario(Usuario usuario);
        void BorrarUsuario(Guid id);
    }
}
