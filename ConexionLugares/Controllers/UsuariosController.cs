using ConexionLugares.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ConexionLugares.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly string cadenaSQL;

        public UsuariosController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }


        [HttpGet]
        [Route("Asientos")]
        public IActionResult Lista()
        {
            List<Asientos> lista = new List<Asientos>();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_asientos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Asientos
                            {
                                IdAsiento = Convert.ToInt32(rd["IdAsiento"]),
                                NumeroAsiento = rd["NumeroAsiento"].ToString(),
                                Seccion = rd["Seccion"].ToString(),
                                Fila = rd["Fila"].ToString(),
                                Columna = Convert.ToInt32(rd["Columna"]),
                                Disponible = Convert.ToBoolean(rd["Disponible"]),
                                IdEscenario = Convert.ToInt32(rd["IdEscenario"]),
                                NombreReserva = rd["NombreReserva"].ToString()
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = lista });
            }

        }



        [HttpPost]
        [Route("ComprarBoleto")]
        public IActionResult ComprarBoleto([FromBody] CompraBoletoRequest request)
        {
            try
            {
                // Validar si el request es nulo, no contiene asientos seleccionados o el nombre de la reserva es nulo o vacío
                if (request == null || request.IdAsientos == null || !request.IdAsientos.Any() || string.IsNullOrEmpty(request.NombreReserva))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Los asientos seleccionados y el nombre de la reserva no pueden ser nulos o vacíos." });
                }

                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    // Verificar si el usuario existe en la base de datos
                    var cmdVerificarUsuario = new SqlCommand("SELECT COUNT(1) FROM Usuarios WHERE IdUsuario = @IdUsuario", conexion);
                    cmdVerificarUsuario.Parameters.AddWithValue("@IdUsuario", request.IdUsuario);
                    int usuarioExiste = (int)cmdVerificarUsuario.ExecuteScalar();

                    if (usuarioExiste == 0)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "El usuario no existe en la base de datos." });
                    }

                    // Llamar al procedimiento almacenado para comprar boletos
                    var cmd = new SqlCommand("sp_ComprarBoletos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pasar parámetros al procedimiento almacenado
                    cmd.Parameters.AddWithValue("@IdUsuario", request.IdUsuario);
                    cmd.Parameters.AddWithValue("@AsientosIds", string.Join(",", request.IdAsientos)); // Convertir lista a cadena separada por comas
                    cmd.Parameters.AddWithValue("@FechaReserva", request.FechaReserva);
                    cmd.Parameters.AddWithValue("@NombreReserva", request.NombreReserva); // Nuevo parámetro para el nombre de la reserva

                    int resultado = cmd.ExecuteNonQuery();

                    if (resultado > 0)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "Boletos comprados con éxito." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Uno o más asientos no están disponibles." });
                    }
                }
            }
            catch (Exception error)
            {
                // Registrar el error para depuración
                Console.WriteLine($"Error al comprar boletos: {error.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }

      

        [HttpPut]
        [Route("EditarReserva")]
        public IActionResult EditarReserva([FromBody] EditarBoletoRequest request)
        {
            try
            {
                // Validar si el request es nulo, no contiene asientos seleccionados o el nombre de la reserva es nulo o vacío
                if (request == null || request.NombresAsientos == null || !request.NombresAsientos.Any() || string.IsNullOrEmpty(request.NombreReserva))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Los asientos seleccionados y el nombre de la reserva no pueden ser nulos o vacíos." });
                }

                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    // Verificar si la reserva existe en la base de datos
                    var cmdVerificarReserva = new SqlCommand("SELECT COUNT(1) FROM Reservas WHERE IdReserva = @IdReserva", conexion);
                    cmdVerificarReserva.Parameters.AddWithValue("@IdReserva", request.IdReserva);
                    int reservaExiste = (int)cmdVerificarReserva.ExecuteScalar();

                    if (reservaExiste == 0)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "La reserva no existe en la base de datos." });
                    }

                    // Llamar al procedimiento almacenado para editar la reserva
                    var cmd = new SqlCommand("sp_EditarReserva", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pasar parámetros al procedimiento almacenado
                    cmd.Parameters.AddWithValue("@IdReserva", request.IdReserva);
                    cmd.Parameters.AddWithValue("@NombreReserva", request.NombreReserva);
                    cmd.Parameters.AddWithValue("@NombresAsientos", string.Join(",", request.NombresAsientos)); // Convertir lista a cadena separada por comas
                    cmd.Parameters.AddWithValue("@FechaReserva", request.FechaReserva);

                    int resultado = cmd.ExecuteNonQuery();

                    if (resultado > 0)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "Reserva editada con éxito." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Uno o más asientos no están disponibles." });
                    }
                }
            }
            catch (Exception error)
            {
                // Registrar el error para depuración
                Console.WriteLine($"Error al editar la reserva: {error.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }




    }

}