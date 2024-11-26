<template>
  <main>
    <div class="simulador-linguagem">
      <h1>Simulador de Linguagem Regular</h1>

      <div class="card">
        <section>
          <h2>Definir Gramática Regular</h2>
          <form @submit.prevent="definirGramatica">
            <label>Terminais (separados por vírgula):</label>
            <input v-model="terminais" placeholder="Ex: a,b,c" />

            <label>Não terminais (separados por vírgula):</label>
            <input v-model="naoTerminais" placeholder="Ex: S,A,B" />

            <label>Símbolo Inicial:</label>
            <input v-model="simboloInicial" placeholder="Ex: S" />

            <label>Produções (uma por linha no formato 'De -> Símbolo | Para'):</label>
            <textarea v-model="producoes" placeholder="Ex: S -> a,A"></textarea>

            <button type="submit">Definir Gramática</button>
          </form>
        </section>

        <section v-if="gramaticaDefinida">
          <h2>Verificar String</h2>
          <form @submit.prevent="verificarString">
            <label>String para verificar:</label>
            <input v-model="stringEntrada" placeholder="Digite a string para verificar" />
            <button type="submit">Verificar String</button>
          </form>
        </section>

        <div v-if="mensagemResultado" class="resultado">
          <p>{{ mensagemResultado }}</p>
        </div>
      </div>
    </div>
  </main>
</template>

<script lang="js">
  import { defineComponent } from 'vue';
  import { Network } from 'vis-network/standalone';


  export default defineComponent({
    name: 'HomePage',
    data() {
      return {
        carregando: false,
        mensagemResultado: '',
        terminais: '',
        naoTerminais: '',
        simboloInicial: '',
        producoes: '',
        stringEntrada: '',
        gramaticaDefinida: false,
        gramatica: null,
        af: null,
      };
    },
    methods: {
      definirGramatica() {
        if (this.terminais === '' || this.naoTerminais === '' || this.simboloInicial === '' || this.producoes === '') {
          this.mensagemResultado = 'É necessário preencher todos os campos.';
          return;
        }

        const producoesPorNaoTerminal = {};

        const listaProducoes = this.producoes.split('\n').map(producao => {
          const [de, regra] = producao.split('->').map(p => p.trim());

          if (!de || !regra) {
            this.mensagemResultado = 'Formato de produção inválido. Verifique as produções no formato "De -> Regra".';
            return null;
          }

          // Adicionar as produções ao mapa de produções
          if (!producoesPorNaoTerminal[de]) {
            producoesPorNaoTerminal[de] = [];
          }

          // Lidar com produções alternativas (divididas por '|')
          const alternativas = regra.split('|').map(r => r.trim());
          alternativas.forEach(alt => {
            // Exemplo de produção "A -> aB", onde "a" é terminal e "B" é não-terminal
            producoesPorNaoTerminal[de].push(alt);
          });
        }).filter(producao => producao !== null); // Filtrar produções inválidas

        // Verificar se as produções foram processadas corretamente
        if (Object.keys(producoesPorNaoTerminal).length === 0) {
          this.mensagemResultado = 'Nenhuma produção válida encontrada.';
          return;
        }

        // Construir a gramática
        this.gramatica = {
          terminais: this.terminais.split(",").map(t => t.trim()),
          naoTerminais: this.naoTerminais.split(",").map(nt => nt.trim()),
          simboloInicial: this.simboloInicial.trim(),
          producoesPorNaoTerminal
        };

        // Gerar o autômato finito a partir da gramática
        this.af = this.criarAutomato();

        // Indicar que a gramática foi definida corretamente
        this.gramaticaDefinida = true;
        this.mensagemResultado = 'Gramática definida com sucesso.';
      },

      criarAutomato() {
        const estados = [];
        const transicoes = {};
        const estadosFinais = [];
        const estadoInicial = this.gramatica.simboloInicial;

        // Inicializar transições para cada não-terminal
        this.gramatica.naoTerminais.forEach(nt => {
          transicoes[nt] = [];
        });

        // Processar as produções para gerar as transições
        Object.keys(this.gramatica.producoesPorNaoTerminal).forEach(de => {
          const producoes = this.gramatica.producoesPorNaoTerminal[de];

          producoes.forEach(producao => {
            if (producao === 'ε') {
              // Se a produção é epsilon (ε), o estado é final
              estadosFinais.push(de);
            } else {
              // Verificar se a produção é um terminal simples
              if (this.gramatica.terminais.includes(producao)) {
                // Se a produção é apenas um terminal, trata-se de uma transição final
                transicoes[de].push({ simbolo: producao, destino: null }); // Transição para nenhum outro estado
                estadosFinais.push(de); // O estado é final
              } else {
                // Produção no formato "aB" onde "a" é um terminal e "B" é um não-terminal
                const simbolo = producao[0]; // O primeiro símbolo é o terminal
                const destino = producao.slice(1); // O restante é o não-terminal de destino

                // Adicionar transição de um terminal para um não-terminal
                transicoes[de].push({ simbolo, destino });
              }
            }
          });
        });

        // Definir o autômato com estados e transições
        return {
          estados: [...this.gramatica.naoTerminais, ...this.gramatica.terminais],
          estadoInicial,
          estadosFinais,
          transicoes
        };
      },



      verificarString() {
        if (!this.stringEntrada) {
          this.mensagemResultado = 'Por favor, insira uma string para verificar.';
          return;
        }

        let estadoAtual = this.af.estadoInicial;
        let i = 0; // Índice da string para iteração

        // Processar a string e verificar se é aceita pelo autômato
        while (i < this.stringEntrada.length) {
          const simbolo = this.stringEntrada[i];
          let transicaoEncontrada = false;

          // Verificar se o estado atual tem transições para o símbolo
          if (this.af.transicoes[estadoAtual]) {
            for (let transicao of this.af.transicoes[estadoAtual]) {
              if (transicao.simbolo === simbolo) {
                estadoAtual = transicao.destino;
                transicaoEncontrada = true;
                i++; // Avançar para o próximo símbolo da string
                break;
              }
            }
          }

          // Se não encontrar transição para o símbolo atual, rejeita a string
          if (!transicaoEncontrada) {
            this.mensagemResultado = `A string "${this.stringEntrada}" não é aceita pela linguagem.`;
            return;
          }
        }

        // Verificar se o estado final foi alcançado após consumir todos os símbolos
        if (this.af.estadosFinais.includes(estadoAtual) || estadoAtual == null) {
          this.mensagemResultado = `A string "${this.stringEntrada}" é aceita pela linguagem.`;
        } else {
          this.mensagemResultado = `A string "${this.stringEntrada}" não é aceita pela linguagem.`;
        }
      }


    }
  });
</script>

<style scoped>
  h1 {
    color: #ffffff;
    margin-bottom: 1rem;
    text-align: center;
  }

  h2 {
    color: #ffffff;
    margin-bottom: 1rem;
    text-align: center;
  }

  p {
    text-align: center;
    margin-bottom: 2rem;
    color: #ddd;
  }

  html, body {
    height: 100%;
    margin: 0;
    background-color: #101010;
    color: #e0e0e0;
  }

  #app {
    display: flex;
    height: 100%;
    justify-content: center;
    align-items: center;
  }

  .simulador-linguagem {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 100%;
    max-width: 800px;
    height: 100%;
    background-color: #1e1e1e;
    padding: 2rem;
    border-radius: 8px;
  }

  .card {
    background-color: #2c2c2c;
    border-radius: 8px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.5);
    padding: 2rem;
    width: 100%;
    text-align: left;
    display: flex;
    flex-direction: column;
    align-items: flex-start;
  }

  section {
    margin-bottom: 1.5rem;
    width: 100%;
  }

  .loading {
    color: #ff5722;
    font-weight: bold;
    margin: 1rem 0;
  }

  label {
    display: block;
    margin-top: 0.5rem;
    color: #bbb;
  }

  input, textarea {
    width: 100%;
    padding: 0.75rem;
    margin-top: 0.5rem;
    border: 1px solid #444;
    border-radius: 4px;
    background-color: #333;
    color: #e0e0e0;
  }

  textarea {
    resize: vertical;
  }

  button {
    width: 100%;
    padding: 0.75rem;
    margin-top: 1rem;
    border: none;
    background-color: #42b883;
    color: white;
    font-weight: bold;
    cursor: pointer;
    border-radius: 4px;
    transition: background-color 0.3s;
  }

    button:hover {
      background-color: #357a65;
    }

  .resultado {
    color: #ddd;
    font-weight: bold;
    margin-top: 1rem;
  }
</style>
