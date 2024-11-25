<template>
  <div class="simulador">
    <h1>Sistema de Catraca</h1>

    <!-- Gráfico Mermaid -->
    <div class="mermaid">
      stateDiagram-v2
      [*] --> S0 : Iniciar
      S0 --> S1 : CPF inserido
      S1 --> S2 : CPF válido
      S1 --> S3 : CPF inválido
      S2 --> S4 : Catraca liberada
      S2 --> S6 : Estacionamento lotado
      S4 --> S5 : Passagem liberada
      S5 --> [*] : Catraca fechada
      S3 --> [*] : Fim
      S6 --> [*] : Fim
    </div>

    <!-- Simulação do sistema de catraca -->
    <div v-if="estado === 'S0'">
      <label for="cpf">Digite o CPF:</label>
      <input v-model="cpf" id="cpf" placeholder="CPF (somente números)" />
      <button @click="validarCpf">Validar CPF</button>
    </div>

    <div v-if="estado === 'S3'">
      <p>CPF Inválido! Tente novamente.</p>
    </div>

    <div v-if="estado === 'S2'">
      <button @click="liberarCatraca">Liberar Catraca</button>
    </div>

    <div v-if="estado === 'S4'">
      <p>Catraca liberada! Você pode passar.</p>
      <button @click="fecharCatraca">Fechar Catraca</button>
    </div>

    <div v-if="estado === 'S5'">
      <p>Catraca fechada. Aguardando novo CPF.</p>
    </div>

    <div v-if="estado === 'S6'">
      <p>Estacionamento lotado! Não há vagas disponíveis.</p>
    </div>
  </div>
</template>

<script>
  export default {
    data() {
      return {
        cpf: '',
        estado: 'S0', // Estado inicial
        estacionamentoLotado: false,
        capacidadeEstacionamento: 5,
        carrosDentro: 0,
      };
    },
    methods: {
      validarCpf() {
        if (this.isCpfValido(this.cpf)) {
          if (this.carrosDentro < this.capacidadeEstacionamento) {
            this.estado = 'S2'; // CPF válido, verificar vagas
          } else {
            this.estado = 'S6'; // Estacionamento lotado
          }
        } else {
          this.estado = 'S3'; // CPF inválido
        }
      },
      isCpfValido(cpf) {
        // Lógica simples para validar o CPF
        return cpf.length === 11 && !/^\d{11}$/.test(cpf);
      },
      liberarCatraca() {
        this.estado = 'S4'; // Catraca liberada
      },
      fecharCatraca() {
        this.estado = 'S5'; // Catraca fechada
        this.carrosDentro++;
        setTimeout(() => {
          this.carrosDentro--;
          this.estado = 'S0'; // Voltar ao estado inicial
        }, 3000);
      }
    }
  };
</script>

<style scoped>
  .simulador {
    text-align: center;
    padding: 20px;
    background-color: #f0f0f0;
  }

  button {
    padding: 10px;
    margin-top: 10px;
  }

  input {
    padding: 10px;
    margin-top: 10px;
  }

  .mermaid {
    margin: 20px auto;
    width: 80%;
    height: 300px;
    background-color: #e0e0e0;
    padding: 10px;
    border-radius: 8px;
  }
</style>
