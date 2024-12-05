<template>
  <main>
    <div class="simulador-estacionamento">
      <h1>Simulador de Sistema de Catraca para Estacionamento</h1>

      <div class="card">
        <section>
          <h2>Estado Atual da Catraca</h2>
          <p>Estado: <strong>{{ estadoAtual }}</strong></p>
        </section>

        <section>
          <h2>Simular Ação</h2>
          <form @submit.prevent="simularAcao">
            <label>Veículo Detectado! Informe o CPF:</label>
            <input type="text" v-model="cpf" id="cpf" placeholder="CPF (somente números)" />
            <button type="submit">Validar CPF</button>
          </form>
        </section>

        <div v-if="mensagemResultado" class="resultado">
          <p>{{ mensagemResultado }}</p>
        </div>

        <section>
          <h2>Visualização do Grafo</h2>
          <div id="network-container" class="grafo"></div>
        </section>
      </div>
    </div>
  </main>
</template>

<script>
  import { Network } from "vis-network/standalone";

  export default {
    name: "SimuladorEstacionamento",
    data() {
      return {
        estados: ["S0", "S1", "S2", "FechandoCatraca"],
        estadoInicial: "S0",
        estadoAtual: "S0",
        transicoes: {
          S0: {
            VeículoDetectado: "S1",
          },
          S1: {
            PagamentoValidado: "S2",
            ErroPagamento: "S0",
          },
          S2: {
            CatracaAberta: "FechandoCatraca",
          },
          FechandoCatraca: {
            CatracaFechada: "S0",
          },
        },

        cpf: "",
        mensagemResultado: "",
        grafo: null,
      };
    },
    methods: {
      simularAcao() {
        if (!this.cpf) {
          this.mensagemResultado = "Por favor, insira um CPF.";
          return;
        }

        // Validar o CPF
        if (!this.validarCPF(this.cpf)) {
          this.estadoAtual = "S0";
          this.mensagemResultado = "CPF inválido! Retornando para o estado inicial.";
          this.atualizarGrafo();
          return;
        }

        
        this.estadoAtual = "S1";
        this.mensagemResultado = "CPF válido! Passando para 'Pagamento Validado'.";

        this.atualizarGrafo();

        
        setTimeout(() => {
          this.estadoAtual = "S2";
          this.mensagemResultado = "Pagamento validado! Abrindo a catraca.";
          this.atualizarGrafo();

          setTimeout(() => {
            this.estadoAtual = "FechandoCatraca"; 
            this.mensagemResultado = "A catraca foi aberta! Agora fechando...";
            this.atualizarGrafo();

            
            setTimeout(() => {
              this.estadoAtual = "S0";
              this.mensagemResultado = "Catraca fechada, retornando ao estado inicial.";
              this.atualizarGrafo();
            }, 2000);
          }, 2000);
        }, 2000); 
      },

      validarCPF(cpf) {
        cpf = cpf.replace(/\D/g, '');
        if (cpf.length !== 11) return false;

        const digitos = cpf.split('').map(Number);
        let soma = 0;
        let peso = 10;
        for (let i = 0; i < 9; i++) {
          soma += digitos[i] * peso;
          peso--;
        }
        let primeiroDigito = 11 - (soma % 11);
        if (primeiroDigito >= 10) primeiroDigito = 0;

        soma = 0;
        peso = 11;
        for (let i = 0; i < 10; i++) {
          soma += digitos[i] * peso;
          peso--;
        }
        let segundoDigito = 11 - (soma % 11);
        if (segundoDigito >= 10) segundoDigito = 0;

        let valido = primeiroDigito === digitos[9] && segundoDigito === digitos[10]

        return valido;
      },

      criarGrafo() {
        const nodes = this.estados.map((estado) => ({
          id: estado,
          label: estado,
          color: estado === this.estadoInicial ? "#42b883" : "#ffffff",
        }));

        const edges = [];
        for (const [de, transicoes] of Object.entries(this.transicoes)) {
          for (const [acao, para] of Object.entries(transicoes)) {
            edges.push({
              from: de,
              to: para,
              label: acao,
              font: {
                align: "top",
              },
              smooth: {
                enabled: true,
                type: "curvedCCW",
              },
            });
          }
        }

        const container = document.getElementById("network-container");
        const data = { nodes, edges };
        const options = {
          nodes: {
            shape: "circle",
            font: {
              color: "#000",
              size: 16,
            },
            borderWidth: 2,
          },
          edges: {
            color: "#aaaaaa",
            font: {
              color: "#000",
              size: 12,
              align: "middle",
            },
            arrows: {
              to: {
                enabled: true,
              },
            },
          },
          physics: false,
        };

        this.grafo = new Network(container, data, options);
      },

      atualizarGrafo() {
        this.$nextTick(() => {
          if (this.grafo) {
            const nodes = this.estados.map((estado) => ({
              id: estado,
              label: estado,
              color: estado === this.estadoAtual ? "#ff5722" : "#ffffff",
            }));

            const edges = [];
            for (const [de, transicoes] of Object.entries(this.transicoes)) {
              for (const [acao, para] of Object.entries(transicoes)) {
                edges.push({
                  from: de,
                  to: para,
                  label: acao,
                });
              }
            }

            this.grafo.setData({ nodes, edges });
          }
        });
      },
    },
    mounted() {
      this.criarGrafo();
    },
  };
</script>

<style scoped>
  h1, h2, p {
    text-align: center;
    color: #ffffff;
  }

  body {
    background-color: #101010;
  }

  .simulador-estacionamento {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
    max-width: 800px;
    margin: auto;
    padding: 2rem;
    background-color: #1e1e1e;
    border-radius: 8px;
  }

  .card {
    background-color: #2c2c2c;
    padding: 2rem;
    border-radius: 8px;
  }

  form label, select, button {
    display: block;
    margin: 1rem 0;
    width: 100%;
  }

  select {
    padding: 0.75rem;
    background-color: #333;
    color: #e0e0e0;
    border: 1px solid #444;
    border-radius: 4px;
  }

  button {
    padding: 0.75rem;
    background-color: #42b883;
    color: white;
    border: none;
    cursor: pointer;
    border-radius: 4px;
  }

    button:hover {
      background-color: #357a65;
    }

  .resultado {
    margin-top: 1rem;
    color: #42b883;
    font-weight: bold;
    text-align: center;
  }

  .grafo {
    width: 100%;
    height: 400px;
    background-color: #ffffff;
    border: 1px solid #ddd;
    margin-top: 1rem;
    border-radius: 8px;
  }
</style>
