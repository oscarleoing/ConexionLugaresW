
    using ConexionLugares.Models;
    namespace ConexionLugares.Models
    {
        using System;
        using System.Collections.Generic;

        public class Usuarios
        {
            public int IdUsuario { get; set; }
            public string Nombre { get; set; }
            public string CorreoElectronico { get; set; }
            public string Contraseña { get; set; }
            public string Telefono { get; set; }

            public ICollection<Reservas> Reservas { get; set; }
        }

        public class Eventos
        {
            public int IdEvento { get; set; }
            public string NombreEvento { get; set; }
            public DateTime Fecha { get; set; }
            public string Descripcion { get; set; }
            public string Ubicacion { get; set; }

            public ICollection<Escenarios> Escenarios { get; set; }
        }

        public class Escenarios
        {
            public int IdEscenario { get; set; }
            public string NombreEscenario { get; set; }
            public int Capacidad { get; set; }
            public int IdEvento { get; set; }
            public Eventos Eventos { get; set; }

            public ICollection<Asientos> Asientos { get; set; }
        }

        public class Asientos
        {
            public int IdAsiento { get; set; }
            public string NumeroAsiento { get; set; } // Número del asiento
            public string Seccion { get; set; } // Sección del asiento
            public string Fila { get; set; } // Fila del asiento
            public int Columna { get; set; } // Columna del asiento
            public bool Disponible { get; set; } // Estado de disponibilidad del asiento
            public int IdEscenario { get; set; } // Clave foránea a la tabla Escenarios
            public Escenarios Escenarios { get; set; } // Relación con la tabla Escenarios
            public string NombreReserva { get; set; } // Nombre del usuario que reservó el asiento

            public ICollection<Reservas> Reservas { get; set; } // Relación con la tabla Reservas
        }


        public class Reservas
        {
            public int IdReserva { get; set; }
            public string NombreReserva { get; set; } // Nombre del usuario que realizó la reserva
            public int CantidadAsientos { get; set; } // Número de asientos seleccionados
            public string NombreAsientosSeleccionados { get; set; } // Nombres de los asientos seleccionados
            public string IdAsientosSeleccionados { get; set; } // IDs de los asientos seleccionados (como cadena)
            public DateTime FechaReserva { get; set; } // Fecha de la reserva
            public int IdUsuario { get; set; }
            public Usuarios Usuario { get; set; } // Relación con la tabla Usuarios
        }

    }

    public class CompraBoletoRequest
{
    public int IdUsuario { get; set; }
    public List<int> IdAsientos { get; set; }
    public string NombreReserva { get; set; } // Asegúrate de tener este campo
    public DateTime FechaReserva { get; set; }
}

public class EditarBoletoRequest
{
    public int IdReserva { get; set; } // ID de la reserva a editar
    public string NombreReserva { get; set; } // Nombre de la reserva
    public List<string> NombresAsientos { get; set; } // Lista de nombres de los asientos seleccionados
    public DateTime FechaReserva { get; set; } // Fecha de la reserva
}