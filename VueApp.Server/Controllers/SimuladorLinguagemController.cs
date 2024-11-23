using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VueApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimuladorLinguagemController : ControllerBase
    {
        private readonly ILogger<SimuladorLinguagemController> _logger;

        public SimuladorLinguagemController(ILogger<SimuladorLinguagemController> logger)
        {
            _logger = logger;
        }

        // Estruturas para Gram�tica e Aut�mato Finito
        public class Producao
        {
            public string De { get; set; }
            public string Simbolo { get; set; }
            public string Para { get; set; }
        }

        public class Gramatica
        {
            public HashSet<string> Terminais { get; set; } = new HashSet<string>();
            public HashSet<string> NaoTerminais { get; set; } = new HashSet<string>();
            public string SimboloInicial { get; set; }
            public List<Producao> Producoes { get; set; } = new List<Producao>();
        }

        public class AutomatoFinito
        {
            public HashSet<string> Estados { get; set; } = new HashSet<string>();
            public HashSet<string> Alfabeto { get; set; } = new HashSet<string>();
            public string EstadoInicial { get; set; }
            public HashSet<string> EstadosDeAceitacao { get; set; } = new HashSet<string>();
            public Dictionary<(string, string), string> Transicoes { get; set; } = new Dictionary<(string, string), string>();

            public bool Aceita(string entrada)
            {
                string estadoAtual = EstadoInicial;

                foreach (var simbolo in entrada)
                {
                    var chave = (estadoAtual, simbolo.ToString());
                    if (Transicoes.TryGetValue(chave, out string proximoEstado))
                    {
                        estadoAtual = proximoEstado;
                    }
                    else
                    {
                        return false;
                    }
                }

                return EstadosDeAceitacao.Contains(estadoAtual);
            }
        }

        // M�todo para converter a Gram�tica Regular em Aut�mato Finito
        private static AutomatoFinito ConverterGramaticaEmAutomato(Gramatica gramatica)
        {
            var automato = new AutomatoFinito();
            automato.EstadoInicial = gramatica.SimboloInicial;
            automato.EstadosDeAceitacao.Add("F");

            foreach (var producao in gramatica.Producoes)
            {
                automato.Estados.Add(producao.De);
                if (producao.Para == null)
                {
                    automato.Transicoes[(producao.De, producao.Simbolo)] = "F";
                }
                else
                {
                    automato.Transicoes[(producao.De, producao.Simbolo)] = producao.Para;
                    automato.Estados.Add(producao.Para);
                }

                automato.Alfabeto.Add(producao.Simbolo);
            }

            return automato;
        }

        // Endpoint para definir a gram�tica
        [HttpPost("DefinirGramatica")]
        public IActionResult DefinirGramatica([FromBody] Gramatica gramatica)
        {
            try
            {
                if (gramatica == null)
                {
                    return BadRequest("Defini��o da gram�tica est� vazia.");
                }

                ConverterGramaticaEmAutomato(gramatica);


                return Ok("Gram�tica definida com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao definir a gram�tica.");
                return StatusCode(500, "Erro ao definir a gram�tica.");
            }
        }

        // Endpoint para verificar a string
        [HttpPost("VerificarString")]
        public IActionResult VerificarString([FromBody] string stringEntrada, [FromServices] Gramatica gramatica)
        {
            try
            {
                if (string.IsNullOrEmpty(stringEntrada))
                {
                    return BadRequest("String de entrada est� vazia.");
                }

                // Converter a gram�tica em um aut�mato
                var automato = ConverterGramaticaEmAutomato(gramatica);

                // Verificar se o aut�mato aceita a string
                bool eAceita = automato.Aceita(stringEntrada);
                return Ok(eAceita ? "A string � aceita pela gram�tica." : "A string n�o � aceita pela gram�tica.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar a string.");
                return StatusCode(500, "Erro ao verificar a string.");
            }
        }
    }
}
