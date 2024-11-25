<template>
  <div id="app">
    <h1 v-if="showText"><router-link to="/catraca">Catraca</router-link></h1>
    <h1 v-if="showText"><router-link to="/home">Simulador de Linguagem</router-link></h1>
    <router-view />
  </div>
</template>

<script>
  export default {
    name: 'App',
    computed: {
      // Verifica se a navbar deve ser exibida com base na rota
      showText() {
        // Defina as rotas em que a navbar deve aparecer
        return this.$router.path !== '/catraca' || this.$router.path !== '/home'; // Exemplo: a navbar não aparece na rota '/catraca'
      }
    }
  }
</script>

<style>
/* Estilos globais do seu App */
</style>
