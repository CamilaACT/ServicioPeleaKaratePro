﻿using Api_Karate_Pro.Custom;
using Api_Karate_Pro.model.proc;
using Api_Karate_Pro.model.Request;
using Api_Karate_Pro.model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Karate_Pro.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Utilidades _utilidades;

        public LoginController(Utilidades utilidades)
        {
            _utilidades = utilidades;
        }
        [HttpPost]
        [Route("registrarse")]
        public Respuesta m_1_Login_1_1([FromBody] usuario_A_usuario usaurio1)
        {
            var contrasenia = _utilidades.encriptaSHA256(usaurio1.usu_clave);
            usaurio1.usu_clave = contrasenia;

            Respuesta res = p_Usuario.grabaUsuario(usaurio1);
            return res;
        }

        [HttpPost]
        [Route("inicioSesion")]
        public Respuesta m_1_Login_1_2([FromBody] Login log)
        {
            var contrasenia = _utilidades.encriptaSHA256(log.clave);
            log.clave = contrasenia;



            Respuesta res = p_Login.iniciarSesion(log);

            if (res.CodigoError == -1)
            {
                usuario_E_usuario id_usuario = new usuario_E_usuario();
                var respusta = res.Message.Split('|');
                id_usuario.us_id = int.Parse(respusta[0].ToString());

                Respuesta resUsuario = p_Login.buscaUsuario(id_usuario);
                Usuario usuarioEncontrado = new Usuario();

                if (resUsuario.Result != null)
                {

                    var dataAsList = resUsuario.Result as List<Dictionary<string, object>>;

                    if (dataAsList != null && dataAsList.Count > 0)
                    {
                        var item = dataAsList[0];


                        usuarioEncontrado = new Usuario
                        {
                            idUsuario = Convert.ToInt32(item["idUsuario"]),
                            nombre = item["nombre"].ToString(),
                            correo = item["correo"].ToString(),
                            idRol = Convert.ToInt32(item["idRol"]),
                            rolDescripcion = item["rolDescripcion"].ToString(),
                            clave = item["clave"].ToString(),
                            status = Convert.ToInt32(item["status"])
                        };


                    }



                    res.Message = res.Message + "|" + _utilidades.generarJWT(usuarioEncontrado);
                }

            }

            return res;

        }

        [HttpGet]
        [Route("validarToken")]
        public Respuesta validarToken([FromQuery] string token)
        {
            Respuesta respuesta1 = new Respuesta();
            bool respuesta = _utilidades.validarToken(token);
            if (respuesta)
            {
                respuesta1.CodigoError = -1;
                respuesta1.Message = "ok";

                return respuesta1;
            }
            else
            {
                respuesta1.CodigoError = 1;
                respuesta1.Message = "bad";

                return respuesta1;
            }


        }
    }
}
