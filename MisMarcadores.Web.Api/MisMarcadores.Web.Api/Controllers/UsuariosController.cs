﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MisMarcadores.Data.Entities;
using MisMarcadores.Logic;
using MisMarcadores.Web.Api.Filters;
using MisMarcadores.Web.Api.Models;

namespace MisMarcadores.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]

    [ServiceFilter(typeof(BaseFilter))]
    [ServiceFilter(typeof(AutenticacionFilter))]
    public class UsuariosController : Controller
    {
        private IUsuariosService _usuariosService { get; set; }

        public UsuariosController(IUsuariosService usuariosService)
        {
            _usuariosService = usuariosService;
        }

        // GET: api/Usuarios
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Usuario> usuarios = _usuariosService.ObtenerUsuarios();
            if (usuarios == null)
            {
                return NotFound();
            }
            return Ok(usuarios);
        }

        // GET: api/Usuarios/juanperez
        [HttpGet("{nombreUsuario}", Name = "GetUsuario")]
        public IActionResult Get(string nombreUsuario)
        {
            Usuario usuario = _usuariosService.ObtenerPorNombreUsuario(nombreUsuario);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        // POST: api/usuarios
        public IActionResult Post([FromBody]AgregarUsuario usuario)
        {
            if (!ModelState.IsValid) return BadRequest("Datos invalidos");
            try
            {
                this._usuariosService.AgregarUsuario(usuario.TransformarAUsuario());
                return CreatedAtRoute("GetUsuario", new { nombreUsuario = usuario.NombreUsuario }, usuario);
            }
            catch (UsuarioDataException)
            {
                return BadRequest("Datos invalidos");
            }
            catch (ExisteUsuarioException)
            {
                return StatusCode(409, "El nombre de usuario ya existe en la BD.");
            }
        }

        // PUT: api/Usuarios/juanperez
        [HttpPut("{nombreUsuario}")]
        public IActionResult Put(string nombreUsuario, [FromBody]ActualizarUsuario usuario)
        {
            if (!ModelState.IsValid) return BadRequest("Datos invalidos");
            try
            {
                this._usuariosService.ModificarUsuario(nombreUsuario, usuario.TransformarAUsuario());
                return Ok();
            }
            catch (UsuarioDataException)
            {
                return BadRequest("Datos invalidos");
            }
            catch (NoExisteUsuarioException)
            {
                return BadRequest("El usuario no existe en la BD.");
            }
        }

        // DELETE: api/Usuarios/juanperez
        [HttpDelete("{nombreUsuario}")]
        public IActionResult Delete(string nombreUsuario)
        {
            try
            {
                this._usuariosService.BorrarUsuario(nombreUsuario);
                return Ok();
            }
            catch (NoExisteUsuarioException)
            {
                return BadRequest("El usuario no existe en la BD.");
            }
        }
    }
}
