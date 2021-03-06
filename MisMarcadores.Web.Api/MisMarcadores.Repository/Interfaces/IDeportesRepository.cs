﻿using MisMarcadores.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MisMarcadores.Repository
{
    public interface IDeportesRepository : IGenericRepository<Deporte>
    {
        Deporte ObtenerDeportePorNombre(String nombre);
        void ModificarDeporte(Deporte deporte);
        void BorrarDeporte(String nombre);
    }
}
