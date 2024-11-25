import { createRouter, createWebHistory } from 'vue-router';
import Home from '@/pages/Home.vue';  // Importando o componente Home
import Catraca from '@/pages/Catraca.vue';  // Importando o componente Catraca

const routes = [
  { path: '/home', component: Home },    // Rota para Home
  { path: '/catraca', component: Catraca }, // Rota para Catraca
];

const router = createRouter({
  history: createWebHistory(), // Usando o modo histórico (URL limpa)
  routes, // Registrando as rotas
});

export default router;
