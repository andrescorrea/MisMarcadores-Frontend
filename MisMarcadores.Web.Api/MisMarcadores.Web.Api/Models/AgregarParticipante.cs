﻿using MisMarcadores.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace MisMarcadores.Web.Api.Models
{
    public class AgregarParticipante
    {
        [Required]
        public string Nombre { get; set; }

        public string Foto { get; set; }

        [Required]
        public string NombreDeporte { get; set; }

        public Participante TransformarAParticipante()
        {
            return new Participante
            {
                Nombre = this.Nombre,
                Foto = this.Foto,
                Deporte = new Deporte { Nombre = this.NombreDeporte }
            };
        }
    }
}
