﻿using MisMarcadores.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace MisMarcadores.Web.Api.Models
{
    public class AgregarParticipante
    {
        [Required]
        public string Nombre { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Foto { get; set; }

        [Required]
        public string NombreDeporte { get; set; }

        [Required]
        public bool EsEquipo { get; set; }

        public Participante TransformarAParticipante()
        {
            return new Participante
            {
                Nombre = this.Nombre,
                Foto = this.Foto,
                Deporte = new Deporte { Nombre = this.NombreDeporte },
                EsEquipo = this.EsEquipo
               
            };
        }
    }
}