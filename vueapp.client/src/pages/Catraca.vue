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
            <label>Selecione a Ação:</label>
            <select v-model="acao">
              <option disabled value="">Escolha uma ação</option>
              <option value="VeículoDetectado">Veículo Detectado</option>
              <option value="PagamentoValidado">Pagamento Validado</option>
              <option value="CatracaAberta">Catraca Aberta</option>
              <option value="ErroPagamento">Erro no Pagamento</option>
            </select>
            <button type="submit">Executar Ação</button>
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
        // Estados e transições do autômato no formato S0, S1, ...
        estados: ["S0", "S1", "S2"],
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
            CatracaAberta: "S0",
          },
        },

        // Dados de interação
        acao: "",
        mensagemResultado: "",
        grafo: null,
      };
    },
    methods: {
      simularAcao() {
        if (!this.acao) {
          this.mensagemResultado = "Por favor, selecione uma ação.";
          return;
        }

        const proximoEstado = this.transicoes[this.estadoAtual]?.[this.acao];
        if (proximoEstado) {
          this.estadoAtual = proximoEstado;
          this.mensagemResultado = `Ação "${this.acao}" realizada com sucesso. Novo estado: "${this.estadoAtual}".`;
        } else {
          this.mensagemResultado = `Ação "${this.acao}" não é válida no estado "${this.estadoAtual}".`;
        }

        // Atualizar o grafo para destacar o estado atual
        this.atualizarGrafo();

        // Resetar a ação selecionada
        this.acao = "";
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
                align: "top", // Define a posição padrão das etiquetas
              },
              smooth: {
                enabled: true, // Habilita suavidade para evitar sobreposição
                type: "curvedCCW", // Aplica curvas às arestas
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
